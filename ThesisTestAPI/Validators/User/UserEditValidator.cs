using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Validators.User
{
    public class UserEditValidator : AbstractValidator<UserEditModel>
    {
        private readonly ThesisDbContext _db;
        public UserEditValidator(ThesisDbContext db)
        {
            _db = db;
            RuleFor(x => x.UserId).NotNull().NotEmpty().WithMessage("User id must be provided");
            RuleFor(x => x.UserId).MustAsync(ValidUserId).WithMessage("Invalid user id");
            RuleFor(x => x.UserName).MustAsync(IsValidUserName).When(x => !string.IsNullOrEmpty(x.UserName)).WithMessage("Username already in use");
            RuleFor(x => x.Email).Must(IsValidEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Invalid email format");
            RuleFor(x => x.Email).MustAsync(CheckEmail).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Email already in use by another user");
            RuleFor(x => x.Phone).Must(IsValidPhone).When(x => !string.IsNullOrEmpty(x.Phone)).WithMessage("Invalid phone format");
            RuleFor(x => x.Phone).MustAsync(CheckPhone).When(x => !string.IsNullOrEmpty(x.Phone)).WithMessage("Phone already in use");
        }
        private async Task<bool> CheckPhone(string? phone, CancellationToken token)
        {
            var data = await _db.Users.FirstOrDefaultAsync(q => q.Phone == phone, token);
            if(data != null)
            {
                return false;
            }
            return true;
        }
        private bool IsValidPhone(string? phone)
        {
            if(phone == null)
            {
                return false;
            }
            string pattern = @"^0\d{9,10}$"; // Ensures it starts with 0 and has 10 or 11 digits
            return Regex.IsMatch(phone, pattern);
        }
        private async Task<bool> CheckEmail(string? email, CancellationToken token)
        {
            var user = await _db.Users.Where(q => q.Email == email).FirstOrDefaultAsync();
            if (user != null)
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

        private async Task<bool> IsValidUserName(string? username, CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName == username);
            if (user != null)
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
