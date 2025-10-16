using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.Midtrans
{
    public class IrisPayoutNotification : IRequest<(ProblemDetails?, string?)>
    {
        public string reference_no { get; init; } = "";
        public string amount { get; init; } = "";                  // string number, e.g. "10000"
        public string status { get; init; } = "";                  // approved|processed|completed|failed|rejected
        public DateTimeOffset updated_at { get; init; }            // ISO8601
        public string? partner_reference_no { get; init; }         // your own id (if sent on create)
        public string? error_code { get; init; }
        public string? error_message { get; init; }
    }
}
