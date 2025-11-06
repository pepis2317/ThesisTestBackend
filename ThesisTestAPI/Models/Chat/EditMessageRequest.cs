using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Chat
{
    public class EditMessageRequest: IRequest<(ProblemDetails?, MessageResponse?)>
    {
        public Guid MessageId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
