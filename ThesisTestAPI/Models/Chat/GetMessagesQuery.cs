using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Chat
{
    public class GetMessagesQuery:IRequest<(ProblemDetails?, CursorPage<MessageResponse>?)>
    {
        public Guid ConversationId {  get; set; }
        public int Limit { get; set; }
        public string? After { get; set; }   // fetch messages strictly newer than this cursor
        public string? Before { get; set; }
    }
    public sealed class CursorPage<T>
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
        public string? NextCursor { get; init; }   // pass as 'after' to get the next page (newer)
        public string? PrevCursor { get; init; }   // pass as 'before' to get the previous page (older)
        public bool HasNext { get; init; }
        public bool HasPrev { get; init; }
    }
    public class MessageResponse
    {
        public Guid MessageId {  get; set; }
        public string? Message { get; set; } = string.Empty;
        public Guid SenderId { get; set; }
        public bool? HasAttachments { get;set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
