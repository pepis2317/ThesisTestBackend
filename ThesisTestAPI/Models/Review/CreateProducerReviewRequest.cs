using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Review
{
    public class CreateProducerReviewRequest:IRequest<(ProblemDetails?, string?)>
    {
        public string Review {  get; set; } = string.Empty;
        public Guid ProducerId { get; set; }
        public Guid AuthorId { get; set; }
    }
}
