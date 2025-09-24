using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Like;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Like
{
    public class GetLikesHandler : IRequestHandler<GetLikesRequest, (ProblemDetails?, LikesResponse?)>
    {
        private readonly LikesService _likesService;
        public GetLikesHandler(LikesService likesService)
        {
            _likesService = likesService;
        }
        public async Task<(ProblemDetails?, LikesResponse?)> Handle(GetLikesRequest request, CancellationToken cancellationToken)
        {
            var likesResponse = await _likesService.GetLikes(request);
            return (null, likesResponse);
        }
    }
}
