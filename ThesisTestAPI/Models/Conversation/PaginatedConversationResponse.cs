using ThesisTestAPI.Models.OrderRequests;

namespace ThesisTestAPI.Models.Conversation
{
    public class PaginatedConversationResponse
    {
        public int? Total { get; set; }
        public List<ConversationResponse>? Conversations { get; set; }
    }
}
