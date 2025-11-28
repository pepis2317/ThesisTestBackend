using ThesisTestAPI.Models.Biteship;

namespace ThesisTestAPI.Models.Shipment
{
    public class BiteshipRatesBody
    {
        public double origin_latitude { get; set; }
        public double origin_longitude { get; set; }
        public double destination_latitude { get; set; }
        public double destination_longitude { get; set; }
        public string couriers { get; set; } = string.Empty;
        public List<BiteshipItem> items { get; set; } = new();
    }
}
