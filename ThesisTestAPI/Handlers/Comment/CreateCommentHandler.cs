using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Comment;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Comment
{
    public class CreateCommentHandler : IRequestHandler<CreateCommentRequest, (ProblemDetails?, CommentResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public CreateCommentHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<(ProblemDetails?, CommentResponse?)> Handle(CreateCommentRequest request, CancellationToken cancellationToken)
        {
            var contentId = Guid.NewGuid();
            var content = new Content
            {
                ContentId = contentId,
                AuthorId = request.AuthorId,
                CreatedAt = DateTimeOffset.Now
            };
            var comment = new ThesisTestAPI.Entities.Comment
            {
                CommentId = contentId,
                Comment1 = request.Comment,
                CommentNavigation = content,
                TargetContentId = request.TargetContentId
            };
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();
            var user = await _db.Users.Where(q => q.UserId == comment.CommentNavigation.AuthorId).FirstOrDefaultAsync();
            var authorPfp = await _blobStorageService.GetTemporaryImageUrl(user.Pfp, Enum.BlobContainers.PFP);
            var replies = await _db.Comments.Where(q => q.CommentNavigation.ContentId == comment.CommentId).CountAsync();
            var commentResponse = new CommentResponse
            {
                AuthorId = user.UserId,
                AuthorName = user.UserName,
                CommentId = comment.CommentId,
                AuthorPfp = authorPfp,
                Comment = comment.Comment1,
                CreatedAt = comment.CommentNavigation.CreatedAt,
                UpdatedAt = comment.CommentNavigation.UpdatedAt,
                Replies = replies
            };
            return (null, commentResponse);
        }
    }
}
