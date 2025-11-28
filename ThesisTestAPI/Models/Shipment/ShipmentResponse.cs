namespace ThesisTestAPI.Models.Shipment
{
    public class ShipmentResponse
    {
        public Guid ShipmentId {  get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? OrderId { get; set; }
        public string? Status { get; set; }
        public int Quantity { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Weight { get;set; }
        public string? Category { get; set; }
        public string? CourierCompany { get; set; }
        public string? CourierType { get; set; }
        public string? OrderNote { get; set; }
        public string? OriginNote { get; set; }
        public string? DestinationNote { get; set; }
        public string token { get; set; } = string.Empty;
        public string redirectUrl { get; set; } = string.Empty;
    }
}
