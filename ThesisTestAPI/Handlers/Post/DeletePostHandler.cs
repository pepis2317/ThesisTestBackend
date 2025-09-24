using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Post;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Post
{
    public class DeletePostHandler : IRequestHandler<DeletePostRequest, (ProblemDetails?, string?)>
    {
        private readonly PostService _postService;
        public DeletePostHandler(PostService postService)
        {
            _postService = postService;
        }

        public async Task<(ProblemDetails?, string?)> Handle(DeletePostRequest request, CancellationToken cancellationToken)
        {
            var deleteConfirmation = await _postService.DeletePost(request);
            return (null, deleteConfirmation);
        }
    }
}
