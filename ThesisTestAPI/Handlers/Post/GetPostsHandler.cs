using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Post;
using ThesisTestAPI.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ThesisTestAPI.Handlers.Post
{
    public class GetPostsHandler : IRequestHandler<PostQuery, (ProblemDetails?, PaginatedPostResponse?)>
    {
        private readonly PostService _postService;
        public GetPostsHandler(PostService postService)
        {
            _postService = postService;
        }
        public async Task<(ProblemDetails?, PaginatedPostResponse?)> Handle(PostQuery request, CancellationToken cancellationToken)
        {
            var paginatedPostResponse = await _postService.GetPosts(request);
            return (null, paginatedPostResponse);
        }
    }
}
