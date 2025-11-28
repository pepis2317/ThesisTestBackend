namespace ThesisTestAPI.Models.Review
{
    public class PaginatedReviewsResponse
    {
        public int? Total {  get; set; }
        public List<ReviewResponse> Reviews { get; set; }
    }
}
