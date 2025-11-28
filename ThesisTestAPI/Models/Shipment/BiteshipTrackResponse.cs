using System.Net.Mail;
using System.Text.Json.Serialization;
using ThesisTestAPI.Models.Biteship;

namespace ThesisTestAPI.Models.Shipment
{
    public class BiteshipTrackResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Object { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string ShortId { get; set; } = string.Empty;
        public Destination Destination { get; set; } = new();
        public Shipper Shipper { get; set; } = new();
        public Origin Origin { get; set; } = new();
        public Delivery Delivery { get; set; } = new();
        public Voucher Voucher { get; set; } = new();
        public Courier Courier { get; set; } = new();
        public string? reference_id { get; set; }
        public string? invoice_id { get; set; }
        public List<BiteshipItem> items { get; set; } = new();
        public string? note { get; set; }
        public string currency { get; set; } = string.Empty;
        public int price { get; set; }
        public string status { get; set; } = string.Empty;
        public string? ticket_status { get; set; }
        public string? draft_order_id { get; set; }
        public string? return_id { get; set; }

    }
    public class Courier
    {
        public string tracking_id { get; set; } = string.Empty;
        public string waybill_id { get; set; } = string.Empty;
        public string company { get; set; } = string.Empty;
        public List<History> history { get; set; } = new();
        public string link { get; set; } = string.Empty;
        public string? name { get; set; }
        public string? phone { get; set; }
        public string? driver_name { get; set; }
        public string? driver_phone { get; set; }
        public string? driver_plate_number { get; set; }
        public string? driver_photo_url { get; set; }
        public string type { get; set; } = string.Empty;
        public string? routing_code { get; set; }
        public int shipment_fee { get; set; }
        public Insurance insurance { get; set; } = new();
    }
    public class History
    {
        public string service_type { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string note { get; set; } = string.Empty;
        public DateTimeOffset updated_at { get; set; }
    }
    public class Insurance
    {
        public int amount { get; set; }
        public string amount_currency { get; set; } = string.Empty;
        public int fee { get; set; }
        public string fee_currency { get; set; } = string.Empty;
        public string? note { get; set; }
    }
    public class Voucher
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public int? value { get; set; }
        public string? type { get; set; }
    }
    public class Delivery
    {
        public DateTimeOffset datetime { get; set; }
        public string? note { get; set; }
        public string type { get; set; } = string.Empty;
        public double? distance { get; set; }
        public string? distance_unit { get; set; }
    }
    public class Shipper
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
    }
    public class Origin
    {
        public string contact_name { get; set; } = string.Empty;
        public string contact_phone { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string note { get; set; } = string.Empty;
        public int postal_code { get; set; }
        public BiteshipCoordinate coordinate { get; set; } = new();
        public string collection_method { get; set; } = string.Empty;
        public string administrative_division_level_1_name { get; set; } = string.Empty;
        public string administrative_division_level_2_name { get; set; } = string.Empty;
        public string administrative_division_level_3_name { get; set; } = string.Empty;
        public string administrative_division_level_4_name { get; set; } = string.Empty;

    }
    public class BiteshipCoordinate
    {
        public double? latitude { get; set; }
        public double? longitude { get; set; }
    }
    public class Destination
    {
        public string contact_name { get; set; } = string.Empty;
        public string contact_phone { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string note { get; set; } = string.Empty;
        public Proof proof_of_delivery { get; set; } = new();
        public int postal_code { get; set; }
        public BiteshipCoordinate coordinate { get; set; } = new();
        public Cod cash_on_delivery { get; set; } = new();
        public string administrative_division_level_1_name { get; set; } = string.Empty;
        public string administrative_division_level_2_name { get; set; } = string.Empty;
        public string administrative_division_level_3_name { get; set; } = string.Empty;
        public string administrative_division_level_4_name { get; set; } = string.Empty;
    }
    public class Proof
    {
        public bool use { get; set; }
        public int fee { get; set; }
        public string? note { get; set; }
        public string? link { get; set; }
    }
    public class Cod
    {
        public string? id { get; set; }
        public int amount { get; set; }
        public int fee { get; set; }
        public string amountCurrency { get; set; } = string.Empty;
        public string feeCurrency { get; set; } = string.Empty;
        public string? note { get; set; }
        public string? type { get; set; }
        public string? status { get; set; }
        public string? payment_status { get; set; }
        public string? payment_method { get; set; }
    }

}
