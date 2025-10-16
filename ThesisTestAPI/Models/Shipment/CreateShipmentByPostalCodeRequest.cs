using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Biteship
{
    public class CreateShipmentByPostalCodeRequest :IRequest<(ProblemDetails?, OrderCreatedResponse?)>
    {
        public Guid OriginUserId { get; set; }
        public Guid DestinationUserId {  get; set; }
        public string CourierCompany { get; set; } = string.Empty;
        public string CourierType { get; set; } = string.Empty;
        public string DeliveryType { get; set; } = string.Empty;
        public string OrderNote { get; set; } = string.Empty;
        public string OriginNote { get; set; } = string.Empty;
        public string DestinationNote { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new();

    }
    public class OrderItem
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public long Value { get; set; }
        public int Quantity { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Weight { get; set; }
        public double Length { get;set; }
    }
}
