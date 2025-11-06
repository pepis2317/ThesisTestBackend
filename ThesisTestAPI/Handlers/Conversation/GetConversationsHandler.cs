using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Conversation;
using ThesisTestAPI.Models.OrderRequests;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Conversation
{
    public class GetConversationsHandler : IRequestHandler<GetConversationsRequest, (ProblemDetails?, PaginatedConversationResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        public GetConversationsHandler(ThesisDbContext db, BlobStorageService blobStorageService)
        {
            _db = db;
            _blobStorageService = blobStorageService;
        }

        public async Task<(ProblemDetails?, PaginatedConversationResponse?)> Handle(GetConversationsRequest request, CancellationToken cancellationToken)
        {
            var conversationIds = await _db.ConversationMembers.Where(q => q.UserId == request.UserId).Select(q => q.ConversationId).ToListAsync();
            var conversations = await _db.Conversations.Skip((request.pageNumber - 1) * request.pageSize)
              .Include(q => q.ConversationMembers).ThenInclude(q => q.User)
              .Where(q => conversationIds.Contains(q.ConversationId))
              .OrderByDescending(q => q.UpdatedAt ?? q.CreatedAt)
              .ToListAsync();

            var list = new List<ConversationResponse>();
            foreach (var conversation in conversations)
            {
                var response = new ConversationResponse
                {
                    ConversationId = conversation.ConversationId,
                    Name = conversation.ConversationName,
                    CreatedAt = conversation.CreatedAt,
                    UpdatedAt = conversation.UpdatedAt,
                };
                var otherMember = conversation.ConversationMembers.Where(q => q.UserId != request.UserId).Select(q=>q.User).FirstOrDefault();
                if (otherMember != null && !string.IsNullOrEmpty(otherMember.Pfp))
                {
                    response.Picture = await _blobStorageService.GetTemporaryImageUrl(otherMember.Pfp, Enum.BlobContainers.PFP);
                }
                list.Add(response);
            }
            var total = await _db.Conversations.Where(q => conversationIds.Contains(q.ConversationId)).CountAsync();
            return (null, new PaginatedConversationResponse
            {
                Total = total,
                Conversations = list,
            });
        }
    }
}
