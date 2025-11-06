namespace ThesisTestAPI.Models.Process
{
    public class PaginatedProcessesResponse
    {
        public int? Total { get; set; }
        public List<ProcessResponse>? Processes {  get; set; }
    }
}
