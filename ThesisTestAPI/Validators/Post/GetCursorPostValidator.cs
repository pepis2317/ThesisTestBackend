using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Post;

namespace ThesisTestAPI.Validators.Post
{
    public class GetCursorPostValidator : AbstractValidator<GetCursorPostRequest>
    {
        private readonly ThesisDbContext _db;
        public GetCursorPostValidator( ThesisDbContext db )
        {
            _db = db;
            RuleFor(x => x.AuthorId).NotNull().NotEmpty().WithMessage("Author id must be specified");
            RuleFor(x => x).Must(CheckValidQuery).WithMessage("Either query for previous or next post");
            RuleFor(x => x).MustAsync(CheckPrevPost).When(x => x.GetPrevPostId!=null).WithMessage("This is the last post");
            RuleFor(x => x).MustAsync(CheckNextPost).When(x => x.GetNextPostId!=null).WithMessage("This is the latest post");
        }
        private async Task<bool> CheckPrevPost(GetCursorPostRequest request, CancellationToken token)
        {
            var current = await _db.Posts.Include(q => q.PostNavigation).Where(q => q.PostId == request.GetPrevPostId && q.PostNavigation.AuthorId == request.AuthorId).FirstOrDefaultAsync();
            var prev = await _db.Posts.Include(q => q.PostNavigation)
                .Where(p => p.PostNavigation.CreatedAt < current.PostNavigation.CreatedAt && p.PostNavigation.AuthorId == request.AuthorId)
                .OrderByDescending(p => p.PostNavigation.CreatedAt)
                .FirstOrDefaultAsync();
            if (prev != null)
            {
                return true;
            }
            return false;
        }
        private async Task<bool> CheckNextPost(GetCursorPostRequest request, CancellationToken token)
        {
            var current = await _db.Posts.Include(q => q.PostNavigation).Where(q => q.PostId == request.GetNextPostId && q.PostNavigation.AuthorId == request.AuthorId).FirstOrDefaultAsync();
            var next = await _db.Posts.Include(q => q.PostNavigation)
                .Where(p => p.PostNavigation.CreatedAt > current.PostNavigation.CreatedAt && p.PostNavigation.AuthorId == request.AuthorId)
                .OrderBy(p => p.PostNavigation.CreatedAt)
                .FirstOrDefaultAsync();
            if (next != null)
            {
                return true;
            }
            return false;
        }
        private bool CheckValidQuery( GetCursorPostRequest request )
        {
            if((request.GetPrevPostId != null && request.GetNextPostId!=null)||(request.GetPrevPostId == null && request.GetNextPostId == null))
            {
                return false;
            }
            return true;
        }
    }
}
