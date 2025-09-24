using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Chat
{
    public class GetMessagesQuery:IRequest<(ProblemDetails?, List<MessageResponse>?)>
    {
        public Guid ConversationId {  get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
    public class MessageResponse
    {
        public string Message { get; set; } = string.Empty;
        public Guid SenderId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
