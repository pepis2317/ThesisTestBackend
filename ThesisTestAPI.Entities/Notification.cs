using System;
using System.Collections.Generic;

namespace ThesisTestAPI.Entities;

public partial class Notification
{
    public Guid NotificationId { get; set; }

    public Guid UserId { get; set; }

    public string? Message { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? SeenAt { get; set; }

    public virtual User User { get; set; } = null!;
}
