using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Process;

namespace ThesisTestAPI.Services
{
    public class ProcessService
    {
        private readonly ThesisDbContext _db;
        public ProcessService(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<ProcessResponse> CreateProcess(CreateProcessRequest request)
        {
            var process = new ThesisTestAPI.Entities.Process
            {
                ProcessId = Guid.NewGuid(),
                RequestId = request.RequestId,
                Description = request.Description,
                Title = request.Title,
                CreatedAt = DateTimeOffset.Now,
                Status = ProcessStatuses.CREATED
            };
            _db.Processes.Add(process);
            await _db.SaveChangesAsync();
            return new ProcessResponse
            {
                ProcessId = process.ProcessId
            };
        }
    }
}
