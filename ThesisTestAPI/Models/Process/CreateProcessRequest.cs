using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Process
{
    public class CreateProcessRequest : IRequest<(ProblemDetails? , ProcessResponse?)>
    {
        public Guid RequestId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}
