using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Process
{
    public class GetProcessRequest: IRequest<(ProblemDetails?, ProcessResponse?)>
    {
        public Guid ProcessId { get; set; }
    }
}
