namespace ThesisTestAPI.Models.Producer
{
    public class PaginatedProducersResponse
    {
        public int? total { get; set; }
        public List<ProducerResponse>? producers { get; set; }  
    }
}
