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
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Address { get; set; }
        public int? PostalCode { get; set; }
    }
}
