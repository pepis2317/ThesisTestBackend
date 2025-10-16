namespace ThesisTestAPI.Models.OrderRequests
{
    public class OrderRequestResponse
    {
        public Guid RequestId {  get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
