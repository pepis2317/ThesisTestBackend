using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Conversation
{
    public class CreateConversationRequest :IRequest<(ProblemDetails?, string?)>
    {
        public string ConversationName { get; set; } = string.Empty;
    }
}
