using ThesisTestAPI.Models.Producer;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Models.Refunds
{
    public class RefundResponse
    {
        public Guid RefundId { get; set; }
        public Guid ProcessId {  get; set; }
        public string? Message {  get; set; }
        public string? Status { get; set; }
        public SellerResponse? Seller { get; set; }
        public UserResponse? User { get; set; }
    }
}
