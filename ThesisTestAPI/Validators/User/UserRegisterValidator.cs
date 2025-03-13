using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Validators.User
{
    public class UserRegisterValidator:AbstractValidator<UserRegisterModel>
    {
        private readonly ThesisDbContext _db;
        public UserRegisterValidator(ThesisDbContext db)
        {
            _db = db;
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email must be filled");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password must be filled");
            RuleFor(x => x.Phone).NotNull().NotEmpty().WithMessage("Phone must be filled");
            RuleFor(x => x.Phone).MustAsync(CheckUniquePhone).WithMessage("Phone already in use");
            RuleFor(x => x.Email).Must(IsValidEmail).WithMessage("Invalid email format");
            RuleFor(x => x).MustAsync(CheckUniqueEmail).WithMessage("Email already in use");
        }
        private async Task<bool> CheckUniquePhone(string phone, CancellationToken token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(q => q.Phone == phone, token);
            if(user != null)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> CheckUniqueEmail(UserRegisterModel request, CancellationToken token)
        {
            var user = await _db.Users.Where(q => q.Email == request.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                return false;
            }
            return true;
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
