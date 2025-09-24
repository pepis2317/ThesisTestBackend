namespace ThesisTestAPI.Models.Producer
{
    public class PaginatedProducersResponse
    {
        public int? Total { get; set; }
        public List<ProducerResponse>? Producers { get; set; }  
    }
}
