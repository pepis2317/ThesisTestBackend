using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Post;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Post
{
    public class EditPostHandler : IRequestHandler<EditPostRequest, (ProblemDetails?, string?)>
    {
        private readonly PostService _postService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EditPostHandler(PostService postService, IHttpContextAccessor httpContextAccessor)
        {
            _postService = postService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<(ProblemDetails?, string?)> Handle(EditPostRequest request, CancellationToken cancellationToken)
        {
            var editPostResponse = await _postService.EditPost(request);
            if (editPostResponse == null)
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
            return(null, editPostResponse);
        }
    }
}
