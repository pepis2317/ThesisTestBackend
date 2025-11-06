namespace ThesisTestAPI.Models.OrderRequests
{
    public class PaginatedOrderRequestsResponse
    {
        public int? Total { get; set; }
        public List<OrderRequestResponse>? Requests { get; set; }
    }
}
