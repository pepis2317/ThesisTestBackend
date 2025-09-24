using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Chat
{
    public class EditMessageRequest: IRequest<(ProblemDetails?, string?)>
    {
        public Guid MessageId { get; set; }
        public Guid ConversationId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
