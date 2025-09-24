using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Post
{
    public class GetPostRequest : IRequest<(ProblemDetails?, PostResponse?)>
    {
        public Guid PostId { get; set; }
    }
}
