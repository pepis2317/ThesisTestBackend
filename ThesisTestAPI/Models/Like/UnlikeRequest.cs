using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Like
{
    public class UnlikeRequest : IRequest<(ProblemDetails?, string?)>
    {
        public Guid ContentId { get; set; }
        public Guid UserId {  get; set; }
    }
}
