using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Message
{
    public Guid MessageId { get; set; }

    public Guid ConversationId { get; set; }

    public Guid SenderId { get; set; }

    public string? Message1 { get; set; }

    public string? Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public bool? HasAttachments { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual ICollection<MessageAttachment> MessageAttachments { get; set; } = new List<MessageAttachment>();

    public virtual User Sender { get; set; } = null!;
}
