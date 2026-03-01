using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class CreateCompleteProcessHandler : IRequestHandler<CreateCompleteProcessRequest, (ProblemDetails?, CompleteProcessResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly ProcessService _processService;
        private readonly NotificationService _notifService;
        public CreateCompleteProcessHandler (ThesisDbContext db, ProcessService processService, NotificationService notifService)
        {
            _db = db;
            _processService = processService;
            _notifService = notifService;
        }
        public async Task<(ProblemDetails?, CompleteProcessResponse?)> Handle(CreateCompleteProcessRequest request, CancellationToken cancellationToken)
        {
            var response = await _processService.CreateCompleteRequest(request);
            return (null, response);
        }
    }
}
