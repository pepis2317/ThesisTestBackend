using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Like;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Like
{
    public class LikeHandler : IRequestHandler<LikeRequest, (ProblemDetails?, LikeResponse?)>
    {
        private readonly LikesService _likesService;
        public LikeHandler(LikesService likesService)
        {
            _likesService = likesService;
        }
        public async Task<(ProblemDetails?, LikeResponse?)> Handle(LikeRequest request, CancellationToken cancellationToken)
        {
            var likeResponse = await _likesService.Like(request);
            return (null, likeResponse);
        }
    }
}
