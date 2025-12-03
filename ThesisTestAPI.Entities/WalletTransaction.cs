using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class WalletTransaction
{
    public Guid TransactionId { get; set; }

    public Guid WalletId { get; set; }

    public long AmountMinor { get; set; }

    public string Direction { get; set; } = null!;

    public long? SignedAmount { get; set; }

    public string Type { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTime? PostedAt { get; set; }

    public DateTime? VoidedAt { get; set; }

    public string? IdempotencyKey { get; set; }

    public string? ExternalRef { get; set; }

    public Guid? TransferGroupId { get; set; }

    public Guid? ParentTransactionId { get; set; }

    public string? ReferenceType { get; set; }

    public Guid? ReferenceId { get; set; }

    public string? Memo { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<WalletTransaction> InverseParentTransaction { get; set; } = new List<WalletTransaction>();

    public virtual WalletTransaction? ParentTransaction { get; set; }

    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();

    public virtual ICollection<Step> Steps { get; set; } = new List<Step>();

    public virtual Wallet Wallet { get; set; } = null!;
}
