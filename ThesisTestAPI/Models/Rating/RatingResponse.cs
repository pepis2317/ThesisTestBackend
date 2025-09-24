namespace ThesisTestAPI.Models.Rating
{
    public class RatingResponse
    {
        public Guid RatingId { get; set; }
        public Guid AuthorId { get; set; }
        public double Rating { get; set; }
    }
}
