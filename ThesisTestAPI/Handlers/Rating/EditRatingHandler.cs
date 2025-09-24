using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Rating;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Rating
{
    public class EditRatingHandler : IRequestHandler<EditRatingRequest, (ProblemDetails?, RatingResponse?)>
    {
        private readonly RatingService _ratingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EditRatingHandler(RatingService ratingService, IHttpContextAccessor httpContextAccessor)
        {
            _ratingService = ratingService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, RatingResponse?)> Handle(EditRatingRequest request, CancellationToken cancellationToken)
        {
            var ratingResponse = await _ratingService.EditRating(request);
            if (ratingResponse != null)
            {
                return (null,  ratingResponse);
            }
            var problemDetails = new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Invalid Request Data",
                Detail = "No rating with such id was found",
                Instance = _httpContextAccessor.HttpContext?.Request.Path
            };
            return (problemDetails, null);
        }
    }
}
