namespace ThesisTestAPI.Models.Biteship
{
    public class BiteshipOrderBody
    {
        public string origin_contact_name { get; set; } = string.Empty;
        public string origin_contact_phone { get; set; } = string.Empty;
        public string origin_contact_email { get; set; } = string.Empty;
        public string origin_address { get; set; } = string.Empty;
        public string origin_note {  get; set; } = string.Empty;
        public int origin_postal_code { get; set; }
        public string destination_contact_name { get; set; } = string.Empty;
        public string destination_contact_phone { get; set; } = string.Empty;
        public string destination_contact_email { get; set; } = string.Empty;
        public string destination_address { get; set; } = string.Empty;
        public int destination_postal_code { get; set; }
        public string destination_note { get; set; } = string.Empty;
        public string delivery_type { get; set; } = string.Empty;
        public string courier_company { get; set; } = string.Empty;
        public string courier_type { get; set; } = string.Empty;
        public string order_note { get;set; } = string.Empty;
        public List<BiteshipItem> items { get; set; } = new();
    }
    public class BiteshipItem
    {
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public double value { get; set; }
        public int quantity { get; set; }
        public double height {  get; set; }
        public double width { get; set; }
        public double length { get; set; }
        public double weight { get; set; }
    }
}
