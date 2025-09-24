using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Chat
{
    public class DeleteMessageRequest: IRequest<(ProblemDetails?, string?)>
    {
        public Guid MessageId { get; set; }
        public Guid ConversationId { get; set; }
    }
}
