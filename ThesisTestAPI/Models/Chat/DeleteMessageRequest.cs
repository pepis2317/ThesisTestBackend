using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Chat
{
    public class DeleteMessageRequest: IRequest<(ProblemDetails?, MessageResponse?)>
    {
        public Guid MessageId { get; set; }
    }
}
