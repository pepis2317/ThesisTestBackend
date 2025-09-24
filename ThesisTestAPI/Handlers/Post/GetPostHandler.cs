using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Post;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Post
{
    public class GetPostHandler: IRequestHandler<GetPostRequest, (ProblemDetails?, PostResponse?)>
    {
        private readonly PostService _postService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetPostHandler(PostService postService, IHttpContextAccessor httpContextAccessor)
        {
            _postService = postService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(ProblemDetails?, PostResponse?)> Handle(GetPostRequest request, CancellationToken cancellationToken)
        {
            var postResponse = await _postService.GetPost(request);
            if (postResponse == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = "No post with such id was found",
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                return (problemDetails, null);
            }
            return (null, postResponse);
        }
    }
}
