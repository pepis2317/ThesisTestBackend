using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Reaction
{
    public Guid ReactionId { get; set; }

    public Guid AuthorId { get; set; }

    public Guid ContentId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual Content Content { get; set; } = null!;

    public virtual Like? Like { get; set; }

    public virtual Rating? Rating { get; set; }
}
