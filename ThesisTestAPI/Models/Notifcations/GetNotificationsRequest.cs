using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Notifcations
{
    public class GetNotificationsRequest:IRequest<(ProblemDetails?, PaginatedNotificationResponse?)>
    {
        public Guid UserId { get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
