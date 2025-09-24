using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Review
{
    public class GetProducerReviewRequest : IRequest<(ProblemDetails?, List<ReviewResponse>?)>
    {
        public Guid AuthorId { get; set; }
        public Guid ProducerId { get; set; }
    }
}
