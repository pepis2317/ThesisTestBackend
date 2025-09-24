using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Wallet
{
    public Guid WalletId { get; set; }

    public Guid UserId { get; set; }

    public string Currency { get; set; } = null!;

    public long BalanceMinor { get; set; }

    public string Status { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
}
