namespace ThesisTestAPI.Models.Conversation
{
    public class ConversationResponse
    {
        public string Name { get; set; } = string.Empty;
        public string? Picture {  get; set; } = string.Empty;
        public string? LatestMessage {  get; set; } = string.Empty;
        public string? SellerName { get; set; }
        public Guid ConversationId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set;}
    }
}
