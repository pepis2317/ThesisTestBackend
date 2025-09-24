using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Review
{
    public class DeleteProducerReviewRequest:IRequest<(ProblemDetails?, string?)>
    {
        public Guid ReviewId { get; set; }
    }
}
