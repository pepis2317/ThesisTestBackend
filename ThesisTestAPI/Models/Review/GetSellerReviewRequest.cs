using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Review
{
    public class GetSellerReviewRequest : IRequest<(ProblemDetails?, List<ReviewResponse>?)>
    {
        public Guid AuthorId { get; set; }
        public Guid SellerId { get; set; }
    }
}
