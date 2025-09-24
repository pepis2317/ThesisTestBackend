using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Post
{
    public class GetCursorPostRequest: IRequest<(ProblemDetails?, PostResponse?)>
    {
        public Guid AuthorId {get; set;}
        public Guid? GetPrevPostId { get; set; }
        public Guid? GetNextPostId { get; set; }
    }
}
