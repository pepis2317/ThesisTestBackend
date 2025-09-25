using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Review
{
    public class DeleteSellerReviewRequest:IRequest<(ProblemDetails?, string?)>
    {
        public Guid ReviewId { get; set; }
    }
}
