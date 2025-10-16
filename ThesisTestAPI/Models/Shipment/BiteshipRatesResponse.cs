namespace ThesisTestAPI.Models.Shipment
{
    public class BiteshipRatesResponse
    {
        public bool Success { get; set; }
        public string Object { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Code { get; set; }
        public RatesResponseOrigin Origin { get; set; } = new();
        public RatesResponseOrigin Destination { get; set; } = new();
        public List<RatesPricing>Pricing { get; set; } = new();
    }
    public class RatesPricing
    {
        public List<string> available_collection_metod {  get; set; } = new();
        public bool available_for_cash_on_delivery { get; set; }
        public bool available_for_proof_of_delivery { get; set; }
        public bool available_for_instant_waybill_id { get; set; }
        public bool available_for_insurance {  get; set; }
        public string company { get; set; } = string.Empty;
        public string courier_name { get;set; } = string.Empty;
        public string courier_code { get; set; } = string.Empty;
        public string courier_service_name { get; set; } = string.Empty;
        public string courier_service_code { get;set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public string description { get;set; } = string.Empty;
        public string duration {  get; set; } = string.Empty;
        public string shipment_duration_range { get;set; } = string.Empty;
        public string shipment_duration_uni { get; set; } = string.Empty;
        public string service_type { get; set;} = string.Empty;
        public string shipping_type { get; set; } = string.Empty;
        public int price { get; set; } = 0;
        public int shipping_fee { get; set; } = 0;
        public string type { get; set; } = string.Empty;
    }
    public class RatesResponseOrigin
    {
        public string? location_id { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public int postal_code { get; set; }
        public string country_name { get; set; }
        public string country_code { get; set; }
        public string administrative_division_level_1_name { get; set; }
        public string administrative_division_level_1_type { get; set; }
        public string administrative_division_level_2_name { get; set; }
        public string administrative_division_level_2_type { get; set; }
        public string administrative_division_level_3_name { get; set; }
        public string administrative_division_level_3_type { get; set; }
        public string administrative_division_level_4_name { get; set; }
        public string administrative_division_level_4_type { get; set; }
        public string? address { get; set; }
    }
}
