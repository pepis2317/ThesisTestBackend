using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Steps
{
    public class EditStepRequest:IRequest<(ProblemDetails?, StepResponse?)>
    {
        public Guid StepId {  get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? MinCompleteEstimate {  get; set; }
        public DateTimeOffset? MaxCompleteEstimate { get; set; }
        public string? Status {  get; set; }

    }
}
