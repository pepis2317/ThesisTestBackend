using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Notifcations;

namespace ThesisTestAPI.Handlers.Notifications
{
    public class GetNotificationsHandler : IRequestHandler<GetNotificationsRequest, (ProblemDetails?, PaginatedNotificationResponse?)>
    {
        private readonly ThesisDbContext _db;
        public GetNotificationsHandler(ThesisDbContext db)
        {
            _db = db;
        }

        public async Task<(ProblemDetails?, PaginatedNotificationResponse?)> Handle(GetNotificationsRequest request, CancellationToken cancellationToken)
        {
            var query = _db.Notifications
                .Where(q => q.UserId == request.UserId)
                .OrderByDescending(q => q.CreatedAt);
            var notifications = await query
                .Skip((request.pageNumber - 1) * request.pageSize)
                .Take(request.pageSize)
                .ToListAsync();

            var now = DateTimeOffset.UtcNow;
            foreach (var n in notifications)
            {
                n.SeenAt = now;
            }
            await _db.SaveChangesAsync();

            var result = notifications.Select(q => new NotificationResponse
            {
                NotificationId = q.NotificationId,
                UserId = q.UserId,
                Message = q.Message,
                CreatedAt = q.CreatedAt,
                SeenAt = q.SeenAt
            }).ToList();

            var total = await _db.Notifications.Where(q => q.UserId == request.UserId).CountAsync();
            return (null, new PaginatedNotificationResponse
            {
                Total = total,
                Notifications = result
            });

        }
    }
}
