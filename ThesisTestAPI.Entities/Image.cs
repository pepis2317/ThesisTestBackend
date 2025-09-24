using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Image
{
    public Guid ImageId { get; set; }

    public string ImageName { get; set; } = null!;

    public Guid ContentId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual Content Content { get; set; } = null!;
}
