using FluentValidation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Validators.User
{
    public class UserLoginValidator:AbstractValidator<UserLoginRequest>
    {
        private readonly ThesisDbContext _db;
        private readonly IDataProtector _protector;
        public UserLoginValidator(ThesisDbContext db, IDataProtectionProvider provider)
        {
            _db = db;
            _protector = provider.CreateProtector("CredentialsProtector");
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email must be filled");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password must be filled");
            RuleFor(x => x.Email).Must(IsValidEmail).WithMessage("Invalid email format");
            RuleFor(x => x).MustAsync(ValidCredentials).WithMessage("Invalid credentials");
        }
        private async Task<bool> ValidCredentials(UserLoginRequest request, CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q => q.Email == request.Email, token);
            if (user == null)
            {
                return false;
            }
            return _protector.Unprotect(user.Password) == request.Password;
        }
        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
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
    }
}
