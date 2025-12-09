using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Post;

namespace ThesisTestAPI.Services
{
    public class PostService
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public PostService(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }
        public async Task<Guid> CreatePostMetadata(CreatePostRequest request)
        {
            var contentId = Guid.NewGuid();
            var content = new Content
            {
                ContentId = contentId,
                AuthorId = request.AuthorId,
                CreatedAt = DateTimeOffset.Now
            };
            var post = new ThesisTestAPI.Entities.Post
            {
                PostId = contentId,
                Caption = request.Caption,
                PostNavigation = content
            };
            _db.Posts.Add(post);
            var created = new List<Image>();
            foreach(var file in request.Files.Where(q => q.Length > 0))
            {
                var contentType = file.ContentType;
                using var stream = file.OpenReadStream();
                created.Add(new Image {
                    ImageId = Guid.NewGuid(),
                    ContentId = contentId,
                    ImageName = file.FileName,
                    CreatedAt = DateTimeOffset.Now,
                });
                await _blobStorageService.UploadImageAsync(stream, file.FileName, contentType, Enum.BlobContainers.IMAGES, 200);
            }
            _db.Images.AddRange(created);
            await _db.SaveChangesAsync();
            return contentId;
        }
        public async Task<string> DeletePost(DeletePostRequest request)
        {
            var images = await _db.Images.Where(q => q.ContentId == request.PostId).ToListAsync();
            foreach (var image in images)
            {
                await _blobStorageService.DeleteFileAsync(image.ImageName, Enum.BlobContainers.IMAGES);
            }

            await _db.Posts.Where((q => q.PostId == request.PostId)).ExecuteDeleteAsync();
            await _db.Contents.Where(c => c.ContentId == request.PostId).ExecuteDeleteAsync();
            return "Successfully deleted post";
        }
        public async Task<string?> EditPost(EditPostRequest request)
        {
            var post = await _db.Posts.Include(q=>q.PostNavigation).Where(q => q.PostId == request.PostId).FirstOrDefaultAsync();
            if (post == null)
            {
                return null;
            }
            post.Caption = request.Caption;
            post.PostNavigation.UpdatedAt = DateTimeOffset.Now;
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
            return $@"Successfully Updated {post.PostId}";
        }
        public async Task<PostResponse?> GetPost(GetPostRequest request)
        {
            var post = await _db.Posts.Include(q => q.PostNavigation).Where(q => q.PostId == request.PostId).FirstOrDefaultAsync();
            if (post == null)
            {
                return null;
            }
            return new PostResponse
            {
                PostId = post.PostId,
                Caption = post.Caption,
                CreatedAt = post.PostNavigation.CreatedAt,
                UpdatedAt = post.PostNavigation.UpdatedAt
            };
        }
        public async Task<PaginatedPostResponse> GetPosts(PostQuery request)
        {
            var baseQuery = _db.Posts.AsNoTracking().Include(p => p.PostNavigation).Where(p => p.PostNavigation.AuthorId == request.AuthorId);
            if (request.LastCreatedAt is not null && request.LastPostId is not null)
            {
                baseQuery = baseQuery.Where(p =>
                    p.PostNavigation.CreatedAt < request.LastCreatedAt
                    || (p.PostNavigation.CreatedAt == request.LastCreatedAt && p.PostId < request.LastPostId));
            }
            var ordered = baseQuery.OrderByDescending(p => p.PostNavigation.CreatedAt).ThenByDescending(p => p.PostId);
            var posts = await ordered.Take(request.pageSize + 1).ToListAsync();
            var hasMore = posts.Count > request.pageSize;
            if (hasMore)
            {
                posts.RemoveAt(posts.Count - 1);
            }
            var lastPost = posts.LastOrDefault();
            var postIds = posts.Select(q => q.PostId).ToList();
            var commentCounts = await _db.Comments
                .Where(q => postIds.Contains(q.TargetContentId))
                .GroupBy(q => q.TargetContentId)
                .Select(g => new
                {
                    TargetContentId = g.Key,
                    CommentCount = g.Count()
                })
                .ToListAsync();
            var likes = await _db.Likes.Include(q => q.LikeNavigation).Where(q => postIds.Contains(q.LikeNavigation.ContentId))
                .GroupBy(q => q.LikeNavigation.ContentId).Select(q => new
                {
                    ContentId = q.Key,
                    Likes = q.Count()
                }).ToListAsync();
            var liked = await _db.Likes.Include(q => q.LikeNavigation).Where(q => postIds.Contains(q.LikeNavigation.ContentId) && q.LikeNavigation.AuthorId == request.UserId).ToListAsync();
            var thumbnails = await _db.Images.Where(q => postIds.Contains(q.ContentId)).GroupBy(q => q.ContentId).Select(g => g.OrderBy(img => img.CreatedAt).FirstOrDefault()).ToListAsync();
            var postsList = new List<PostResponse>();
            foreach (var post in posts)
            {
                var isLiked = liked.Where(q => q.LikeNavigation.ContentId == post.PostId).Any();
                var likeCount = likes.Where(q => q.ContentId == post.PostId).Select(q => q.Likes).FirstOrDefault();
                var commentCount = commentCounts.Where(q => q.TargetContentId == post.PostId).FirstOrDefault();
                var thumbnail = thumbnails.Where(q => q.ContentId == post.PostId).FirstOrDefault();
                var images = await _db.Images.Where(q => q.ContentId == post.PostId).CountAsync();
                var thumbnailUrl = "";
                if (thumbnail != null)
                {
                    thumbnailUrl = await _blobStorageService.GetTemporaryImageUrl(thumbnail.ImageName, Enum.BlobContainers.IMAGES);

                }
                postsList.Add(new PostResponse
                {
                    PostId = post.PostId,
                    Caption = post.Caption,
                    isMultipleImages = images > 1,
                    Liked = isLiked,
                    Likes = likeCount,
                    Comments = commentCount != null ? commentCount.CommentCount : 0,
                    Thumbnail = thumbnailUrl,
                    CreatedAt = post.PostNavigation.CreatedAt,
                    UpdatedAt = post.PostNavigation.UpdatedAt,
                });
            }
            return new PaginatedPostResponse
            {
                HasMore = hasMore,
                LastId = lastPost?.PostId,
                LastCreatedAt = lastPost?.PostNavigation.CreatedAt,
                Posts = postsList

            };
        }
    }
}
