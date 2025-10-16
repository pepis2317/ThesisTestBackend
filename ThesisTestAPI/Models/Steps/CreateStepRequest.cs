using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Steps
{
    //note sub step itu pas seller create step ketika current step mereka blm kelar
    public class CreateStepRequest : IRequest<(ProblemDetails?, StepResponse?)>
    {
        public Guid AuthorId { get; set; }
        public Guid ProcessId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long Amount { get; set; }
        public DateTimeOffset MinCompleteEstimate { get; set; }
        public DateTimeOffset MaxCompleteEstimate { get; set; }
        public Guid? PreviousStepId { get; set; }
    }
}
