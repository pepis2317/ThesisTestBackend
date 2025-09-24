using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Rating
{
    public class DeleteRatingRequest: IRequest<(ProblemDetails?,string?)>
    {
        public Guid RatingId {  get; set; }
    }
}
