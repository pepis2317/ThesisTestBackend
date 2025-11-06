using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Process
{
    public class GetCompleteRequest:IRequest<(ProblemDetails?, CompleteProcessResponse?)>
    {
        public Guid ProcessId {  get; set; }
    }
}
