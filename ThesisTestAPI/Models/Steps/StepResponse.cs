namespace ThesisTestAPI.Models.Steps
{
    public class StepResponse
    {
        public Guid StepId {  get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public string MinCompleteEstimate { get; set; } = string.Empty;
        public string MaxCompleteEstimate {  get; set; } = string.Empty;
        public string? Status {  get; set; } = string.Empty;
        public long Price { get; set; }
    }
}
