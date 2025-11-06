namespace ThesisTestAPI.Models.Notifcations
{
    public class PaginatedNotificationResponse
    {
        public int? Total { get; set; }
        public List<NotificationResponse>? Notifications { get; set; }
    }
    public class NotificationResponse
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public string? Message { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? SeenAt { get; set; }
    }
}
