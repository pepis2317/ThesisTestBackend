using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Step
{
    public Guid StepId { get; set; }

    public Guid ProcessId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid? TransactionId { get; set; }

    public DateTimeOffset MinCompleteEstimate { get; set; }

    public DateTimeOffset MaxCompleteEstimate { get; set; }

    public DateTimeOffset? CompletedDate { get; set; }

    public Guid? NextStepId { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Step> InverseNextStep { get; set; } = new List<Step>();

    public virtual Step? NextStep { get; set; }

    public virtual Process Process { get; set; } = null!;

    public virtual Content StepNavigation { get; set; } = null!;

    public virtual WalletTransaction? Transaction { get; set; }
}
