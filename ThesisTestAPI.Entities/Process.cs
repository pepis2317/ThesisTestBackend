using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Process
{
    public Guid ProcessId { get; set; }

    public Guid RequestId { get; set; }

    public string? Description { get; set; }

    public string? Title { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<CompleteProcessRequest> CompleteProcessRequests { get; set; } = new List<CompleteProcessRequest>();

    public virtual ICollection<RefundRequest> RefundRequests { get; set; } = new List<RefundRequest>();

    public virtual Request Request { get; set; } = null!;

    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();

    public virtual ICollection<Step> Steps { get; set; } = new List<Step>();
}
