using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Process;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Process
{
    public class CreateProcessHandler : IRequestHandler<CreateProcessRequest, (ProblemDetails?, ProcessResponse?)>
    {
        private readonly ProcessService _service;
        private readonly ThesisDbContext _db;
        private readonly NotificationService _notificationService;
        public CreateProcessHandler(ProcessService service, NotificationService notificationService, ThesisDbContext db)
        {
            _service = service;
            _notificationService = notificationService;
            _db = db;
        }

        public async Task<(ProblemDetails?, ProcessResponse?)> Handle(CreateProcessRequest request, CancellationToken cancellationToken)
        {
            var processResponse = await _service.CreateProcess(request);
            var orderRequest = await _db.Requests.Include(q=>q.RequestNavigation).Where(q=>q.RequestId == request.RequestId).FirstOrDefaultAsync();
            await _db.SaveChangesAsync();
            var receiverId = orderRequest.RequestNavigation.AuthorId;
            await _notificationService.SendNotification("New Process has been created by seller", receiverId);
            return (null, processResponse);
        }
    }
}
