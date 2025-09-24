using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Chat;

namespace ThesisTestAPI.Handlers.Chat
{
    public class GetMessagesHandler : IRequestHandler<GetMessagesQuery, (ProblemDetails?, List<MessageResponse>?)>
    {
        private readonly ThesisDbContext _db;
        public GetMessagesHandler(ThesisDbContext db)
        {
            _db = db;
        }

        public async Task<(ProblemDetails?, List<MessageResponse>?)> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var messages = await _db.Messages.Where(q => q.ConversationId == request.ConversationId).OrderByDescending(q => q.CreatedAt).Skip((request.pageNumber - 1)*request.pageSize).Take(request.pageSize).ToListAsync();
            messages.Reverse();
            var messageResponses = new List<MessageResponse>();
            foreach(var message in messages)
            {
                var messageResponse = new MessageResponse
                {
                    SenderId = message.SenderId,
                    Message = message.Message1,
                    CreatedAt = message.CreatedAt,
                    UpdatedAt = message.UpdatedAt,
                    DeletedAt = message.DeletedAt,
                };
                messageResponses.Add(messageResponse);
            }
            return(null, messageResponses);
        }
    }
}
