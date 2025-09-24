using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Chat
{
    public class SendMessageRequest : IRequest<(ProblemDetails?, string?)>
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
