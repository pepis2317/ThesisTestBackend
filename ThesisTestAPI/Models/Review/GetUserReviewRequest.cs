using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Review
{
    public class GetUserReviewRequest : IRequest<(ProblemDetails?, List<ReviewResponse>?)>
    {
        public Guid AuthorId { get; set; }
        public Guid UserId { get; set; }
    }
}
