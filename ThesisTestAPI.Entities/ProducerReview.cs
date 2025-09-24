using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class ProducerReview
{
    public Guid ProducerReviewId { get; set; }

    public Guid ProducerId { get; set; }

    public string Review { get; set; } = null!;

    public virtual Producer Producer { get; set; } = null!;

    public virtual Content ProducerReviewNavigation { get; set; } = null!;
}
