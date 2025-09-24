using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Post;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Post
{
    public class CreatePostMetadataHandler : IRequestHandler<CreatePostRequest, (ProblemDetails?, Guid?)>
    {
        private readonly PostService _postService;
        public CreatePostMetadataHandler(PostService postService)
        {
            _postService = postService;
        }
        public async Task<(ProblemDetails?, Guid?)> Handle(CreatePostRequest request, CancellationToken cancellationToken)
        {
            var postId = await _postService.CreatePostMetadata(request);
            return (null, postId);
        }
    }
}
