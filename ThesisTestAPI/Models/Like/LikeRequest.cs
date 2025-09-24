using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Like
{
    public class LikeRequest :IRequest<(ProblemDetails?, LikeResponse?)>
    {
        public Guid AuthorId { get; set; }
        public Guid ContentId { get; set; }
    }
}
