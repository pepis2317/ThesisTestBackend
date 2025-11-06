using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Chat;

namespace ThesisTestAPI.Services
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ThesisDbContext _db;
        public ChatHub(ThesisDbContext db) => _db = db;

        public static string GroupName(Guid conversationId) => $"conv:{conversationId}";
        public override async Task OnConnectedAsync()
        {
            try
            {
                var userIdStr =
                    Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? Context.User?.FindFirst("UserId")?.Value;

                if (!Guid.TryParse(userIdStr, out var userId))
                {
                    Context.Abort();
                    return;
                }
                var conversationIds = await _db.ConversationMembers
                    .Where(q => q.UserId == userId)
                    .Select(q => q.ConversationId)
                    .ToListAsync();

                foreach (var id in conversationIds)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(id));
                }
            }
            catch (Exception ex)
            {
                Context.Abort();
                return;
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            // (optional) presence cleanup; don’t throw
            await base.OnDisconnectedAsync(ex);
        }

        // Add these to match what your client invokes
        public Task JoinConversationGroup(string groupName)
            => Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        public Task LeaveConversationGroup(string groupName)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        public Task Typing(Guid conversationId) =>
            Clients.Group(GroupName(conversationId))
                   .SendAsync("Typing", new { conversationId, userId = Context.UserIdentifier });

        public Task Read(Guid conversationId, Guid messageId) =>
            Clients.Group(GroupName(conversationId))
                   .SendAsync("Read", new { conversationId, messageId, userId = Context.UserIdentifier });
    }

}
