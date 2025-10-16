using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Midtrans
{
    public class MidtransNotification : IRequest<(ProblemDetails?, string?)>
    {
        public string order_id { get; set; } = default!;
        public string status_code { get; set; } = default!;
        public string gross_amount { get; set; } = default!;
        public string transaction_status { get; set; } = default!;
        public string fraud_status { get; set; } = default!;
        public string signature_key { get; set; } = default!;
    }
}
