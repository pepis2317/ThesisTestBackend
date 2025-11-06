using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Process
{
    public class RespondCompleteProcess:IRequest<(ProblemDetails?,CompleteProcessResponse?)>
    {
        public Guid CompleteProcessRequestId { get; set; }
        public string Response {  get; set; } = string.Empty;
    }
}
