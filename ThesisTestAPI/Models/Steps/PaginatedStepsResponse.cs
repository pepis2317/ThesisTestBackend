namespace ThesisTestAPI.Models.Steps
{
    public class PaginatedStepsResponse
    {
        public int? Total {  get; set; }
        public List<StepResponse>? Steps {  get; set; }
    }
}
