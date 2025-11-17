using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Models.Process
{
    public class ProcessResponse
    {
        public Guid ProcessId { get; set; }
        public string? Title {  get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status {  get; set; } = string.Empty;
        public string? Picture { get; set; } = string.Empty;
        public SellerResponse? Seller { get; set; }
        public UserResponse? User { get; set; }
    }
}
