using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Shipment;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Shipment
{
    public class CreateShipmentHandler : IRequestHandler<CreateShipmentRequest, (ProblemDetails?, ShipmentResponse?)>
    {
        private readonly ThesisDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NotificationService _notificationService;
        public CreateShipmentHandler(ThesisDbContext db, IHttpContextAccessor httpContextAccessor, NotificationService notificationService)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
        }
        private ProblemDetails ProblemDetailTemplate(string detail)
        {
            return new ProblemDetails
            {
                Type = "http://veryCoolAPI.com/errors/invalid-data",
                Title = "Create shipment error",
                Detail = detail,
                Instance = _httpContextAccessor.HttpContext?.Request.Path
            };
        }
        public async Task<(ProblemDetails?, ShipmentResponse?)> Handle(CreateShipmentRequest request, CancellationToken cancellationToken)
        {
            var process = await _db.Processes.Include(q=>q.Request).ThenInclude(q=>q.RequestNavigation).Include(q=>q.Steps).Where(q => q.ProcessId == request.ProcessId).FirstOrDefaultAsync();
            if (process == null)
            {
                return (ProblemDetailTemplate("Invalid process"), null);
            }
            long amount = 0;
            foreach(var step in process.Steps)
            {
                if(step.Status == StepStatuses.COMPLETED || step.Status == StepStatuses.WORKING)
                {
                    amount += step.Amount;
                }
            }
            var shipment = new ThesisTestAPI.Entities.Shipment
            {
                ShipmentId = Guid.NewGuid(),
                ProcessId = request.ProcessId,
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Quantity = request.Quantity,
                Height = request.Height,
                Width = request.Width,
                Weight = request.Weight,
                Length = request.Length,
                Status = ShipmentStatuses.PENDING,
                Value = amount,
                CreatedAt = DateTimeOffset.Now,
            };
            _db.Shipments.Add(shipment);
            await _db.SaveChangesAsync();
            await _notificationService.SendNotification($"Shipment request created for process {process.Title}", process.Request.RequestNavigation.AuthorId);
            return (null, new ShipmentResponse
            {
                ShipmentId = shipment.ShipmentId
            });
        }
    }
}
