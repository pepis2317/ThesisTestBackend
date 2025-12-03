using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.OrderRequests
{
    public class RespondOrderRequest : IRequest<(ProblemDetails?, OrderRequestResponse?)>
    {
        public Guid RequestId {  get; set; }
        public string Status {  get; set; } = string.Empty;
        public string? DeclineReason { get; set; }
    }
}
