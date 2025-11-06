using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Steps
{
    public class GetStepRequest :IRequest<(ProblemDetails?,StepResponse?)>
    {
        public Guid StepId {  get; set; }
    }
}
