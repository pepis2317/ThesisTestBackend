using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Enum;
using ThesisTestAPI.Models.Refunds;

namespace ThesisTestAPI.Handlers.Refund
{
    public class CreateRefundRequestHandler : IRequestHandler<CreateRefundRequest, (ProblemDetails?, RefundResponse?)>
    {
        private readonly ThesisDbContext _db;
        public CreateRefundRequestHandler(ThesisDbContext db)
        {
            _db = db;
        }
        public async Task<(ProblemDetails?, RefundResponse?)> Handle(CreateRefundRequest request, CancellationToken cancellationToken)
        {
            var sellerId = await _db.Processes.Include(q => q.Request).ThenInclude(q => q.Seller).Where(q => q.ProcessId == request.ProcessId).Select(q=>q.Request.Seller.OwnerId).FirstOrDefaultAsync();
            var contentId = Guid.NewGuid();
            var content = new Content
            {
                ContentId = contentId,
                AuthorId = request.AuthorId,
                CreatedAt = DateTimeOffset.Now
            };
            var refundRequest = new ThesisTestAPI.Entities.RefundRequest
            {
                RefundRequestId = contentId,
                Message = request.Message,
                Status = RequestStatuses.PENDING,
                ProcessId = request.ProcessId,
                SellerUserId = sellerId,
                RefundRequestNavigation = content,
                CreatedAt = DateTimeOffset.Now
            };
            _db.RefundRequests.Add(refundRequest);
            await _db.SaveChangesAsync();
            return (null, new RefundResponse { RefundId = contentId});
        }
    }
}
