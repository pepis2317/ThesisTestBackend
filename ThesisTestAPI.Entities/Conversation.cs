using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Conversation
{
    public Guid ConversationId { get; set; }

    public string ConversationName { get; set; } = null!;

    public int IsGroup { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
