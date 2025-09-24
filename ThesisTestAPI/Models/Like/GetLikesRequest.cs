using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Like
{
    public class GetLikesRequest : IRequest<(ProblemDetails?, LikesResponse?)>
    {
        public Guid AuthorId { get; set; }
        public Guid ContentId { get; set; }
    }
}
