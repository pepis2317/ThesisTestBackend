using Microsoft.Extensions.Options;
using System.Buffers.Text;
using System.Drawing;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using ThesisTestAPI.Models.Midtrans;

namespace ThesisTestAPI.Services
{
    public class MidtransService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly MidtransOptions _opt;
        private readonly IrisOptions _iris;
        public MidtransService(IHttpClientFactory httpFactory, IOptions<MidtransOptions> opt, IOptions<IrisOptions> iris)
        {
            _httpFactory = httpFactory;
            _opt = opt.Value;
            _iris = iris.Value;
        }
        public async Task<SnapCreateResponse?> CreateSnapTransactionAsync(string orderId, long amountMinor, string? email = null, string? firstName = null)
        {
            var amountMajor = amountMinor / 100m;
            var body = new
            {
                transaction_details = new { order_id = orderId, gross_amount = amountMajor },
                customer_details = new { email = email ?? "demo@example.com", first_name = firstName ?? "Demo" }
            };

            var http = _httpFactory.CreateClient("midtrans");
            using var resp = await http.PostAsJsonAsync(_opt.SnapUrl, body);
            if (!resp.IsSuccessStatusCode)
            {
                return null;
            }
            var data = await resp.Content.ReadFromJsonAsync<SnapCreateResponse>();
            if (data is null) throw new InvalidOperationException("Invalid Snap response");
            return data;
        }
        public async Task<bool> CreateMidtransRefundAsync(string transactionId, long amountMinor, string reason)
        {
            var amountMajor = amountMinor / 100m;
            var body = new
            {
                refund_key = Guid.NewGuid().ToString(),
                amount = amountMajor,
                reason = reason
            };
            var http = _httpFactory.CreateClient("midtrans");
            var url = $"{_opt.CoreStatusUrl}/{transactionId}/refund";
            using var resp = await http.PostAsJsonAsync(url, body);
            var ass = await resp.Content.ReadAsStringAsync();
            return resp.IsSuccessStatusCode;
        }
        public async Task<IrisPayoutCreateResponse?> CreateWithdrawalAsync(
            string referenceNo,
            long amountMinor,
            string beneficiaryBankCode,
            string beneficiaryAccount,
            string beneficiaryName,
            string beneficiaryEmail
            )
        {
            var amountMajor = amountMinor / 100m;
            var payload = new
            {
                payouts = new[]
            {
                new
                {
                    beneficiary_name = beneficiaryName,
                    beneficiary_account = beneficiaryAccount,
                    beneficiary_bank = beneficiaryBankCode.ToLowerInvariant(),
                    beneficiary_email = beneficiaryEmail,
                    amount = amountMajor.ToString(), // IRIS expects string integer
                    notes = $"Withdrawal {referenceNo}",
                    reference_no = referenceNo,     // your unique reference
                    sender_name = _iris.SenderName  // appears in bank statement (where supported)
                }
            }
            };
            var http = _httpFactory.CreateClient("midtrans-iris");
            using var req = new HttpRequestMessage(HttpMethod.Post, $"{_iris.BaseUrl}/payouts")
            {
                Content = JsonContent.Create(payload)
            };
            var basic = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_iris.ApiKey}:"));
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);
            req.Headers.TryAddWithoutValidation("X-Idempotency-Key", referenceNo);

            using var resp = await http.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"IRIS payout failed ({(int)resp.StatusCode}): {err}");
            }

            return await resp.Content.ReadFromJsonAsync<IrisPayoutCreateResponse>();
        }
        public async Task<IrisPayoutStatusResponse?> GetWithdrawalStatusAsync(string referenceNo)
        {
            var http = _httpFactory.CreateClient("midtrans-iris");
            using var req = new HttpRequestMessage(HttpMethod.Get, $"{_iris.BaseUrl}/payouts/{referenceNo}");

            var basic = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_iris.ApiKey}:"));
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);

            using var resp = await http.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"IRIS get status failed ({(int)resp.StatusCode}): {err}");
            }

            return await resp.Content.ReadFromJsonAsync<IrisPayoutStatusResponse>();
        }
        public async Task<MidtransStatus?> GetStatusAsync(string orderId)
        {
            var http = _httpFactory.CreateClient("midtrans");
            using var resp = await http.GetAsync($"{_opt.CoreStatusUrl}/{orderId}/status");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<MidtransStatus>();
        }
        public bool VerifySignature(MidtransNotification n)
        {
            var raw = n.order_id + n.status_code + n.gross_amount + _opt.ServerKey;
            using var sha = SHA512.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
            var hex = BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            return hex == n.signature_key;
        }
    }
}
