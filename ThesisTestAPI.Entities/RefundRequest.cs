using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class RefundRequest
{
    public Guid RefundRequestId { get; set; }

    public string Message { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? ExternalRef { get; set; }

    public Guid ProcessId { get; set; }

    public Guid SellerUserId { get; set; }

    public virtual Process Process { get; set; } = null!;

    public virtual Content RefundRequestNavigation { get; set; } = null!;

    public virtual User SellerUser { get; set; } = null!;
}
