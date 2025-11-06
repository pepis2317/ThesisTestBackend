using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ThesisTestAPI.Services
{
    public class NotificationService
    {
        private readonly ThesisDbContext _db;
        public NotificationService(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task SendNotification(string message, Guid receiverId)
        {
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = receiverId,
                Message = message,
                CreatedAt = DateTimeOffset.Now
            };
            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();
            var receiver = await _db.Users.Where(q => q.UserId == receiverId).FirstOrDefaultAsync();
            if(receiver != null && receiver.ExpoPushToken != null)
            {
                using var http = new HttpClient();
                var payload = new
                {
                    to = receiver.ExpoPushToken,
                    sound = "default",
                    body = message
                };
                await http.PostAsJsonAsync("https://exp.host/--/api/v2/push/send", payload);
            }
        }
    }
}
