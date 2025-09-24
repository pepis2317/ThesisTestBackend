using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Comment
{
    public class CommentResponse
    {
        public Guid CommentId { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorPfp { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt {  get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public int Replies { get; set; }
    }
}
