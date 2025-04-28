using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ThesisTestAPI.Models.User
{
    public class UserLoginRequest :IRequest<(ProblemDetails?, LoginResponse?)>
    {
        public required string Email {  get; set; }
        public required string Password {  get; set; }
    }
}
