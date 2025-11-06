namespace ThesisTestAPI.Models.Shipment
{
    public class PaginatedShipmentResponse
    {
        public int? Total { get; set; }
        public List<ShipmentResponse>? Shipments { get; set; }
    }
}
