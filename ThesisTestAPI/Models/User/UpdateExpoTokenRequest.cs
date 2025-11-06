using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.User
{
    public class UpdateExpoTokenRequest:IRequest<(ProblemDetails?, string?)>
    {
        public Guid UserId {  get; set; }
        public string? ExpoPushToken { get; set; } = string.Empty;
    }
}
