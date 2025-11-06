using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Review
{
    public class CreateSellerReviewRequest:IRequest<(ProblemDetails?, string?)>
    {
        public string Review {  get; set; } = string.Empty;
        public int Rating { get; set; }
        public Guid SellerId { get; set; }
        public Guid AuthorId { get; set; }
    }
}
