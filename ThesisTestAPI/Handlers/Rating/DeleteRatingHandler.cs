using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Rating;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Rating
{
    public class DeleteRatingHandler : IRequestHandler<DeleteRatingRequest, (ProblemDetails?, string?)>
    {
        private readonly RatingService _ratingService;
        public DeleteRatingHandler(RatingService ratingService)
        {
            _ratingService = ratingService;
        }

        public async Task<(ProblemDetails?, string?)> Handle(DeleteRatingRequest request, CancellationToken cancellationToken)
        {
            var deleteResponse = await _ratingService.DeleteRating(request);
            return (null, "Successfully deleted rating");
        }
    }
}
