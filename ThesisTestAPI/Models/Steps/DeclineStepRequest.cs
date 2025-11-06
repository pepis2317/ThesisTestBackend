using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Steps
{
    public class DeclineStepRequest:IRequest<(ProblemDetails?, StepResponse?)>
    {
        public Guid StepId {  get; set; }
    }
}
