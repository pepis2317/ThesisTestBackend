using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Post;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace ThesisTestAPI.Handlers.Post
{
    public class GetCursorPostHandler : IRequestHandler<GetCursorPostRequest, (ProblemDetails?, PostResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidator<GetCursorPostRequest> _validator;
        public GetCursorPostHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor, IValidator<GetCursorPostRequest> validator)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _validator = validator;
        }

        public async Task<(ProblemDetails?, PostResponse?)> Handle(GetCursorPostRequest request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "http://veryCoolAPI.com/errors/invalid-data",
                    Title = "Invalid Request Data",
                    Detail = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)),
                    Instance = _httpContextAccessor.HttpContext?.Request.Path
                };
                return (problemDetails, null);
            }
            var posts = await _db.Posts.Include(q => q.PostNavigation).Where(q => q.PostNavigation.AuthorId == request.AuthorId).ToListAsync();
            var current = CurrentHelper(posts, request);
            if (request.GetPrevPostId != null)
            {
                var prev = await _db.Posts.Include(q => q.PostNavigation)
                    .Where(p => p.PostNavigation.CreatedAt < current.PostNavigation.CreatedAt && p.PostNavigation.AuthorId == request.AuthorId)
                    .OrderByDescending(p => p.PostNavigation.CreatedAt)
                    .FirstOrDefaultAsync();
                var isLastPost = await _db.Posts.Include(q => q.PostNavigation)
                    .Where(p => p.PostNavigation.CreatedAt < prev.PostNavigation.CreatedAt && p.PostNavigation.AuthorId == request.AuthorId)
                    .OrderByDescending(p => p.PostNavigation.CreatedAt)
                    .FirstOrDefaultAsync();
                return (null, new PostResponse
                {
                    PostId = prev.PostId,
                    Caption = prev.Caption,
                    CreatedAt = prev.PostNavigation.CreatedAt,
                    UpdatedAt = prev.PostNavigation.UpdatedAt,
                    hasMore = isLastPost==null
                });
            }
            var next = await _db.Posts.Include(q => q.PostNavigation)
                .Where(p => p.PostNavigation.CreatedAt > current.PostNavigation.CreatedAt && p.PostNavigation.AuthorId == request.AuthorId)
                .OrderBy(p => p.PostNavigation.CreatedAt)
                .FirstOrDefaultAsync();
            var isFirstPost = await _db.Posts.Include(q => q.PostNavigation)
                    .Where(p => p.PostNavigation.CreatedAt < next.PostNavigation.CreatedAt && p.PostNavigation.AuthorId == request.AuthorId)
                    .OrderByDescending(p => p.PostNavigation.CreatedAt)
                    .FirstOrDefaultAsync();
            return (null, new PostResponse
            {
                PostId = next.PostId,
                Caption = next.Caption,
                CreatedAt = next.PostNavigation.CreatedAt,
                UpdatedAt = next.PostNavigation.UpdatedAt,
                hasMore = isFirstPost==null
            });
        }
        private Entities.Post? CurrentHelper(List<Entities.Post> posts,GetCursorPostRequest request)
        {
            if(request.GetPrevPostId != null)
            {
                return posts.Where(q => q.PostId == request.GetPrevPostId).FirstOrDefault();
            }
            return posts.Where(q => q.PostId == request.GetNextPostId).FirstOrDefault();
        }
    }
}
