namespace ThesisTestAPI.Models.Shipment
{
    public class ShipmentResponse
    {
        public Guid ShipmentId {  get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? OrderId { get; set; }
        public string? Status { get; set; }
        public string token { get; set; } = string.Empty;
        public string redirectUrl { get; set; } = string.Empty;
    }
}
