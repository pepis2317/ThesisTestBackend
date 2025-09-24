using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;

namespace ThesisTestAPI.Models.Review
{
    public class EditProducerReviewRequest : IRequest<(ProblemDetails?, string?)>
    {
        public Guid ReviewId { get; set; }
        public string Review { get; set; } = string.Empty;
    }
}
