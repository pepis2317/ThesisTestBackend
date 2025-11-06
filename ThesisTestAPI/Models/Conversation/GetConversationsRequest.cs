using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Conversation
{
    public class GetConversationsRequest:IRequest<(ProblemDetails?, PaginatedConversationResponse?)>
    {
        public Guid UserId { get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
