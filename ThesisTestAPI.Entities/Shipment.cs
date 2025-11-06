using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Shipment
{
    public Guid ShipmentId { get; set; }

    public Guid ProcessId { get; set; }

    public Guid? TransactionId { get; set; }

    public string? OrderId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public long Value { get; set; }

    public int Quantity { get; set; }

    public double Height { get; set; }

    public double Length { get; set; }

    public double Weight { get; set; }

    public double Width { get; set; }

    public string Category { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? CourierCompany { get; set; }

    public string? CourierType { get; set; }

    public string? OrderNote { get; set; }

    public string? OriginNote { get; set; }

    public string? DestinationNote { get; set; }

    public virtual Process Process { get; set; } = null!;

    public virtual WalletTransaction? Transaction { get; set; }
}
