using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Rating
{
    public class CreateRatingRequest : IRequest<(ProblemDetails?, RatingResponse?)>
    {
        public Guid AuthorId { get; set; }
        public Guid ContentId { get; set; }
        public int Rating { get; set; }

    }
}
