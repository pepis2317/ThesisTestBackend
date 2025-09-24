using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Like
{
    public class UnlikeRequest : IRequest<(ProblemDetails?, string?)>
    {
        public Guid LikeId { get; set; }
    }
}
