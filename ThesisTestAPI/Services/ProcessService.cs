using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Process;

namespace ThesisTestAPI.Services
{
    public class ProcessService
    {
        private readonly ThesisDbContext _db;
        public ProcessService(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<ProcessResponse> CreateProcess(CreateProcessRequest request)
        {
            var orderRequest = await _db.Requests.Include(q => q.Seller).Include(q => q.RequestNavigation).Where(q => q.RequestId == request.RequestId).FirstOrDefaultAsync();
            var sellerId = orderRequest.Seller.OwnerId;
            var userId = orderRequest.RequestNavigation.AuthorId;
            var process = new ThesisTestAPI.Entities.Process
            {
                ProcessId = Guid.NewGuid(),
                RequestId = request.RequestId,
                Description = request.Description,
                Title = request.Title,
                CreatedAt = DateTimeOffset.Now,
                Status = ProcessStatuses.CREATED
            };
            var existingConvo = await _db.Conversations.Include(q => q.ConversationMembers)
                .Where(c =>
                    c.ConversationMembers.Any(m => m.UserId == sellerId) &&
                    c.ConversationMembers.Any(m => m.UserId == userId)
                ).FirstOrDefaultAsync();
            if(existingConvo == null)
            {
                var conversation = new ThesisTestAPI.Entities.Conversation
                {
                    ConversationId = Guid.NewGuid(),
                    ConversationName = process.Title,
                    IsGroup = 0,
                    CreatedAt = DateTimeOffset.Now
                };
                var sellerMember = new ConversationMember
                {
                    MemberId = Guid.NewGuid(),
                    ConversationId = conversation.ConversationId,
                    UserId = sellerId,
                    JoinedAt = DateTimeOffset.Now,
                };
                var userMember = new ConversationMember
                {
                    MemberId = Guid.NewGuid(),
                    ConversationId = conversation.ConversationId,
                    UserId = userId,
                    JoinedAt = DateTimeOffset.Now,
                };
                _db.Conversations.Add(conversation);
                _db.ConversationMembers.Add(userMember);
                _db.ConversationMembers.Add(sellerMember);
            }

            _db.Processes.Add(process);
            await _db.SaveChangesAsync();
            return new ProcessResponse
            {
                ProcessId = process.ProcessId
            };
        }
    }
}
