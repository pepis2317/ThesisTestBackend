namespace ThesisTestAPI.Models.Producer
{
    public class ProducerResponse
    {
        public Guid ProducerId { get; set; }
        public string? ProducerName { get; set; }
        public Guid OwnerId { get; set; }
        public int? Rating { get; set; }
        public int? Clients { get; set; }
        public string? Banner { get; set; }
    }
}
