using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using ThesisTestAPI.Entities;

namespace ThesisTestAPI.Services
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ThesisDbContext _db;
        public ChatHub(ThesisDbContext db)
        {
            _db = db;
        }
        private static string GroupName(Guid conversationId) => $"conv:{conversationId}";
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Context.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("User not authenticated");
            }
            var conversationIds = await _db.ConversationMembers.Where(q=>q.UserId == Guid.Parse(userId)).Select(q => q.ConversationId).ToListAsync();
            foreach(var id in conversationIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(id));
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            // Optional: cleanup, presence tracking, etc.
            await base.OnDisconnectedAsync(ex);
        }
        public Task Typing(Guid conversationId) =>
        Clients.Group(GroupName(conversationId))
               .SendAsync("Typing", new { conversationId, userId = Context.UserIdentifier });
        public Task Read(Guid conversationId, Guid messageId) =>
        Clients.Group(GroupName(conversationId))
               .SendAsync("Read", new { conversationId, messageId, userId = Context.UserIdentifier });

    }
}
