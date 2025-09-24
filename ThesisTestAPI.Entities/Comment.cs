using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Comment
{
    public Guid CommentId { get; set; }

    public Guid TargetContentId { get; set; }

    public string? Comment1 { get; set; }

    public virtual Content CommentNavigation { get; set; } = null!;

    public virtual Content TargetContent { get; set; } = null!;
}
