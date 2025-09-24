using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Rating
{
    public class EditRatingRequest:IRequest<(ProblemDetails?, RatingResponse?)>
    {
        public Guid RatingId { get; set; }
        public int Rating { get; set; }
    }
}
