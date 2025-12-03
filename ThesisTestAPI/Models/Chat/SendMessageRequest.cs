using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Chat
{
    public class SendMessageRequest : IRequest<(ProblemDetails?, MessageResponse?)>
    {
        public Guid? MessageId { get; set; }
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public required List<IFormFile>? Files { get; set; } = new();
        public string? Text { get; set; } = string.Empty;
        //public bool HasAttachments { get; set; }
    }
}
