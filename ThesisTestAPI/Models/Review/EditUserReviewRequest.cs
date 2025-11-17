using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Review
{
    public class EditUserReviewRequest : IRequest<(ProblemDetails?, string?)>
    {
        public Guid ReviewId { get; set; }
        public string Review { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}
