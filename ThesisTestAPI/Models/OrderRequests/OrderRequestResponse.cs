namespace ThesisTestAPI.Models.OrderRequests
{
    public class OrderRequestResponse
    {
        public Guid RequestId {  get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? PictureUrl {  get; set; } = string.Empty;
    }
}
