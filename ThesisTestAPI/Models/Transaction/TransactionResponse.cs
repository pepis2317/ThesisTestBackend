namespace ThesisTestAPI.Models.Transaction
{
    public class TransactionResponse
    {
        public string orderId { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string redirectUrl { get; set; } = string.Empty;
        public string paymentStatus { get; set; } = string.Empty;
    }
}
