namespace ThesisTestAPI.Models.Midtrans
{
    public class IrisPayoutStatusResponse
    {
        public string Status { get; set; } = "";          // "queued","processing","completed","failed", etc.
        public string FailureReason { get; set; } = "";   // if failed
        public string ReferenceNo { get; set; } = "";
        public string Id { get; set; } = "";
    }
}
