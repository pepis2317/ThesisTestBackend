using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Like
{
    public Guid LikeId { get; set; }

    public virtual Reaction LikeNavigation { get; set; } = null!;
}
