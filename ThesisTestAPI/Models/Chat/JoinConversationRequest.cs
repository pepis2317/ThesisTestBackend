namespace ThesisTestAPI.Models.Chat
{
    public class JoinConversationRequest
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
    }
}
