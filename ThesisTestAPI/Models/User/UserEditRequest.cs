using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.User
{
    public class UserEditRequest :IRequest<(ProblemDetails?, UserResponse?)>
    {
        public required Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email {  get; set; }
        public string? Password { get; set; }
        public string? Phone {  get; set; }
        public string? Role { get; set; }
    }
}
