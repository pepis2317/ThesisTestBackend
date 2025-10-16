using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class CompleteProcessRequest
{
    public Guid RequestId { get; set; }

    public Guid ProcessId { get; set; }

    public string Status { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual Process Process { get; set; } = null!;
}
