using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Models.Reaction
{
    public class CommentResponse
    {
        public string Comment {  get; set; } = string.Empty;
        public UserResponse Author {  get; set; }
        public int Likes { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
