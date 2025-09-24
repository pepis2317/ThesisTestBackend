using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Comment;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Comment
{
    public class GetCommentsHandler : IRequestHandler<GetCommentsRequest, (ProblemDetails?, PaginatedCommentsResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetCommentsHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, PaginatedCommentsResponse)> Handle(GetCommentsRequest request, CancellationToken cancellationToken)
        {

            var comments = await _db.Comments.Include(q=>q.CommentNavigation).Skip((request.pageNumber-1)*request.pageSize).Take(request.pageSize).OrderByDescending(q=>q.CommentNavigation.CreatedAt).Where(q => q.TargetContentId == request.ContentId).ToListAsync();
            var commentsList = new List<CommentResponse>();
            foreach (var comment in comments)
            {
                var user = await _db.Users.Where(q => q.UserId == comment.CommentNavigation.AuthorId).FirstOrDefaultAsync();
                var authorPfp = await _blobStorageService.GetTemporaryImageUrl(user.Pfp, Enum.BlobContainers.PFP);
                var replies = await _db.Comments.Where(q=>q.TargetContentId == comment.CommentId).CountAsync();
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
                commentsList.Add(commentResponse);
            }
            var total = await _db.Comments.Where(q => q.TargetContentId == request.ContentId).CountAsync();
            return(null,new PaginatedCommentsResponse
            {
                Total = total,
                Comments = commentsList
            });
        }
    }
}
