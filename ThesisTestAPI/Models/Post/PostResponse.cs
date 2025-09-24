using ThesisTestAPI.Models.Reaction;

namespace ThesisTestAPI.Models.Post
{
    public class PostResponse
    {
        public Guid PostId { get; set; }
        public string Caption { get; set; } = string.Empty;
        public string? Thumbnail { get; set; } = string.Empty;
        public bool? isMultipleImages { get; set; }
        public bool? hasMore { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

    }
    public class PaginatedPostResponse
    {
        public bool HasMore { get; set; }
        public List<PostResponse>? Posts { get; set; }
        public Guid? LastId {  get; set; }
        public DateTimeOffset? LastCreatedAt { get; set; }
    }
}
