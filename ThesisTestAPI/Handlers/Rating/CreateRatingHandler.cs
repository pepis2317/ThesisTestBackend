using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Rating;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Rating
{
    public class CreateRatingHandler : IRequestHandler<CreateRatingRequest, (ProblemDetails?, RatingResponse?)>
    {
        private readonly RatingService _ratingService;
        public CreateRatingHandler(RatingService ratingService)
        {
            _ratingService = ratingService;
        }

        public async Task<(ProblemDetails?, RatingResponse?)> Handle(CreateRatingRequest request, CancellationToken cancellationToken)
        {
            var ratingResponse = await _ratingService.CreateRating(request);
            return (null, ratingResponse);
        }
    }
}
