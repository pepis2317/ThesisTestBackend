using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Producer;

namespace ThesisTestAPI.Models.Post
{
    public class PostQuery : IRequest<(ProblemDetails?, PaginatedPostResponse?)>
    {
        public Guid AuthorId{ get; set; }
        public int pageSize { get; set; }
        public Guid? LastPostId{ get; set; }
        public DateTimeOffset? LastCreatedAt { get; set; }
    }
}
