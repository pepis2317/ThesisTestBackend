using ThesisTestAPI.Models.Biteship;

namespace ThesisTestAPI.Models.Shipment
{
    public class BiteshipRatesBody
    {
        public int origin_postal_code { get; set; }
        public int destination_postal_code { get; set; }
        public string couriers { get; set; } = string.Empty;
        public List<BiteshipItem> items { get; set; } = new();
    }
}
