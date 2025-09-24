namespace ThesisTestAPI.Models.Comment
{
    public class CreateCommentAuthorizedRequest
    {
        public Guid TargetContentId { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
