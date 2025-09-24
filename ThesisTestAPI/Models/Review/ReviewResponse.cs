namespace ThesisTestAPI.Models.Review
{
    public class ReviewResponse
    {
        public Guid? ReviewId { get; set; }
        public Guid? AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorPfp { get; set; } = string.Empty;
        public string Review { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public int Comments { get; set; }
    }
}
