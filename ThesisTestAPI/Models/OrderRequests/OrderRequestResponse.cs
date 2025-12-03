namespace ThesisTestAPI.Models.OrderRequests
{
    public class OrderRequestResponse
    {
        public Guid RequestId {  get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public Guid BuyerUserId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public Guid SellerId { get; set; }
        public string? DeclineReason { get; set; }
        public string? BuyerPictureUrl {  get; set; } = string.Empty;
        public string? SellerPictureUrl {  get; set; } = string.Empty;
    }
}
