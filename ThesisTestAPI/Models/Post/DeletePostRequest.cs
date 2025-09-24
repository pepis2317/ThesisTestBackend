using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Post
{
    public class DeletePostRequest : IRequest<(ProblemDetails?, string?)>
    {
        public Guid PostId { get;set; }
    }
}
