using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class UserReview
{
    public Guid UserReviewId { get; set; }

    public Guid UserId { get; set; }

    public string Review { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual Content UserReviewNavigation { get; set; } = null!;
}
