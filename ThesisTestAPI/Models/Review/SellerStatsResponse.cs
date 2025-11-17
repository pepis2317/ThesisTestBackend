namespace ThesisTestAPI.Models.Review
{
    public class SellerStatsResponse
    {
        public double Rating { get; set; }
        public int Clients { get; set; }
        public int Reviews { get; set; }
        public double CompletionRate { get; set; }
    }
}
