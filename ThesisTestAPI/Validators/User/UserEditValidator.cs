using Azure.Core;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Validators.User
{
    public class UserEditValidator : AbstractValidator<UserEditRequest>
    {
        private readonly ThesisDbContext _db;
        public UserEditValidator(ThesisDbContext db)
        {
            _db = db;
            RuleFor(x => x.UserId).NotNull().NotEmpty().WithMessage("User id must be provided");
            RuleFor(x => x.UserId).MustAsync(ValidUserId).WithMessage("Invalid user id");
            RuleFor(x => x).MustAsync(IsValidUserName).When(x => !string.IsNullOrEmpty(x.UserName)).WithMessage("Username already in use");
            RuleFor(x => x.Email).Must(IsValidEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Invalid email format");
            RuleFor(x => x).MustAsync(CheckEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Email already in use by another user");
            RuleFor(x => x).MustAsync(CheckPhone).When(x => !string.IsNullOrEmpty(x.Phone)).WithMessage("Phone already in use");
        }
        private async Task<bool> CheckPhone(UserEditRequest request, CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Phone == request.Phone, token);
            if (user != null && (request.UserId != user.UserId))
            {
                return false;
            }
            return true;
        }

        private async Task<bool> CheckEmail(UserEditRequest request, CancellationToken token)
        {
            var user = await _db.Users.Where(q => q.Email == request.Email).FirstOrDefaultAsync(token);
            if (user != null && (request.UserId != user.UserId))
            {
                return false;
            }
            return true;
        }
        private bool IsValidEmail(string? email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> IsValidUserName(UserEditRequest request, CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName, token);
            if (user != null && (request.UserId != user.UserId))
            {
                return false;
            }
            return true;
        }
        private async Task<bool> ValidUserId(Guid UserId, CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == UserId, token);
            if (user == null)
            {
                return false;
            }
            return true;
        }
    }
}
