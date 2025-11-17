namespace ThesisTestAPI.Models.Refunds
{
    public class PaginatedRefundRequestResponse
    {
        public int? Total { get; set; }
        public List<RefundResponse>? RefundRequests { get; set; }
    }
}
