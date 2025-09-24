using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Rating
{
    public Guid RatingId { get; set; }

    public int? Rating1 { get; set; }

    public virtual Reaction RatingNavigation { get; set; } = null!;
}
