using ThesisTestAPI.Models.Producer;

namespace ThesisTestAPI.Models.Comment
{
    public class PaginatedCommentsResponse
    {
        public int? Total { get; set; }
        public List<CommentResponse>? Comments { get; set; }
    }
}
