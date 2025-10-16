namespace ThesisTestAPI.Models.Midtrans
{
    public class IrisPayoutCreateResponse
    {
        public string Status { get; set; } = "";          // e.g., "pending"
        public string ReferenceNo { get; set; } = "";     // your reference_no
        public string Id { get; set; } = "";              // payout id
        public string BeneficiaryBank { get; set; } = "";
        public string BeneficiaryAccount { get; set; } = "";
        public string Amount { get; set; } = "";          // string integer (IDR)
        public string Notes { get; set; } = "";
    }
}
