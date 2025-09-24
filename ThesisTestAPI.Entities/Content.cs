using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Content
{
    public Guid ContentId { get; set; }

    public Guid AuthorId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual Comment? CommentCommentNavigation { get; set; }

    public virtual ICollection<Comment> CommentTargetContents { get; set; } = new List<Comment>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual Post? Post { get; set; }

    public virtual ProducerReview? ProducerReview { get; set; }

    public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

    public virtual Request? Request { get; set; }

    public virtual Step? Step { get; set; }

    public virtual UserReview? UserReview { get; set; }
}
