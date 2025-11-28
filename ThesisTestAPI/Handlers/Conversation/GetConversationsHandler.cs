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
            var latestMessages = await _db.Messages
                .Where(m => conversationIds.Contains(m.ConversationId))
                .GroupBy(m => m.ConversationId)
                .Select(g => g.OrderByDescending(m => m.CreatedAt).Where(q=>q.Message1!= null && q.Message1.Length >1).First())
                .ToListAsync();
            var memberIds = conversations.SelectMany(c => c.ConversationMembers).Select(cm => cm.User.UserId).Distinct().ToList();
            var sellers = await _db.Sellers.Where(q => memberIds.Contains(q.OwnerId)).ToListAsync();


            var list = new List<ConversationResponse>();
            foreach (var conversation in conversations)
            {
                var response = new ConversationResponse
                {
                    ConversationId = conversation.ConversationId,
                    CreatedAt = conversation.CreatedAt,
                    UpdatedAt = conversation.UpdatedAt,
                };
                var latestMessage = latestMessages.Where(q=>q.ConversationId == conversation.ConversationId).FirstOrDefault();
                var otherMember = conversation.ConversationMembers.Where(q => q.UserId != request.UserId).Select(q => q.User).FirstOrDefault();
                if (otherMember != null)
                {
                    response.Name = otherMember.UserName;
                    if (!string.IsNullOrEmpty(otherMember.Pfp))
                    {
                        response.Picture = await _blobStorageService.GetTemporaryImageUrl(otherMember.Pfp, Enum.BlobContainers.PFP);
                    }
                    if (otherMember.Role == "Seller")
                    {
                        var name = sellers.Where(q => q.OwnerId == otherMember.UserId).Select(q => q.SellerName).FirstOrDefault();
                        response.SellerName = name;
                    }
                    if (latestMessage != null)
                    {
                        response.LatestMessage = latestMessage.SenderId == otherMember.UserId ? latestMessage.Message1 : "(You) " + latestMessage.Message1;
                    }
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
