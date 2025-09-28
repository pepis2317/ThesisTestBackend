namespace ThesisTestAPI.Models.Producer
{
    public class PaginatedSellersResponse
    {
        public int? Total { get; set; }
        public List<SellerResponse>? Sellers { get; set; }  
    }
}
