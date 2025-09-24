using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Like;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Like
{
    public class UnlikeHandler : IRequestHandler<UnlikeRequest, (ProblemDetails?, string?)>
    {
        private readonly LikesService _likesService;
        public UnlikeHandler(LikesService likesService)
        {
            _likesService = likesService;
        }

        public async Task<(ProblemDetails?, string?)> Handle(UnlikeRequest request, CancellationToken cancellationToken)
        {
            var unlikeResponse = await _likesService.Unlike(request);
            return (null, unlikeResponse);
        }
    }
}
