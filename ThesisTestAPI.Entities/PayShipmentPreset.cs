using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class PayShipmentPreset
{
    public Guid PresetId { get; set; }

    public Guid TransactionId { get; set; }

    public string Method { get; set; } = null!;

    public string CourierCompany { get; set; } = null!;

    public string CourierType { get; set; } = null!;

    public string DeliveryType { get; set; } = null!;

    public string OrderNote { get; set; } = null!;

    public string OriginNote { get; set; } = null!;

    public string DestinationNote { get; set; } = null!;

    public virtual WalletTransaction Transaction { get; set; } = null!;
}
