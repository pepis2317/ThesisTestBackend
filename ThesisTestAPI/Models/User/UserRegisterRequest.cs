using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.User
{
    public class UserRegisterRequest:IRequest<(ProblemDetails?, UserResponse?)>
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Role { get; set; }

    }
}
