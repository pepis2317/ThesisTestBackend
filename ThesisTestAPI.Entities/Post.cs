using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Post
{
    public Guid PostId { get; set; }

    public string? Caption { get; set; }

    public virtual Content PostNavigation { get; set; } = null!;
}
