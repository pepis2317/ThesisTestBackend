using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Conversation;

namespace ThesisTestAPI.Handlers.Conversation
{
    public class CreateConversationHandler : IRequestHandler<CreateConversationRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        public CreateConversationHandler(ThesisDbContext db)
        {
            _db = db;
        }

        public async Task<(ProblemDetails?, string?)> Handle(CreateConversationRequest request, CancellationToken cancellationToken)
        {
            var conversation = new ThesisTestAPI.Entities.Conversation
            {
                ConversationId = Guid.NewGuid(),
                ConversationName = request.ConversationName,
                IsGroup = 0,
                CreatedAt = DateTimeOffset.Now
            };
            _db.Conversations.Add(conversation);
            await _db.SaveChangesAsync();
            return (null, "created conversation");
        }
    }
}
