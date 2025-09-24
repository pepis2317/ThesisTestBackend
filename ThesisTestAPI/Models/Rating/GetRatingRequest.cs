using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Rating
{
    public class GetRatingRequest : IRequest<(ProblemDetails?, double?)>
    {
        public Guid ContentId {  get; set; }
    }
}
