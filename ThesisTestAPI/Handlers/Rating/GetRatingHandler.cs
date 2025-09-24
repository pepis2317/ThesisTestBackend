using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Models.Rating;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Rating
{
    public class GetRatingHandler : IRequestHandler<GetRatingRequest, (ProblemDetails?, double?)>
    {
        private readonly RatingService _ratingService;
        public GetRatingHandler(RatingService ratingService)
        {
            _ratingService = ratingService;
        }

        public async Task<(ProblemDetails?, double?)> Handle(GetRatingRequest request, CancellationToken cancellationToken)
        {
            var average = await _ratingService.GetRating(request);
            return(null, average);
        }
    }
}
