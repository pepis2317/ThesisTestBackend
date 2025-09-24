using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class MessageAttachment
{
    public Guid AttachmentId { get; set; }

    public Guid MessageId { get; set; }

    public string? FileType { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public string BlobFileName { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public virtual Message Message { get; set; } = null!;
}
