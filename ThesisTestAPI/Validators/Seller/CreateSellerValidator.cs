using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;

namespace ThesisTestAPI.Validators.Producer
{
    public class CreateSellerValidator:AbstractValidator<CreateSellerRequest>
    {
        private readonly ThesisDbContext _db;
        public CreateSellerValidator(ThesisDbContext db)
        {
            _db = db;
            RuleFor(x => x.OwnerId).NotNull().NotEmpty().WithMessage("Owner id must be provided");
            RuleFor(x => x.SellerName).NotNull().NotEmpty().WithMessage("Seller name must be provided");
            RuleFor(x => x.Latitude).NotNull().NotEmpty().WithMessage("Latitude must be provided");
            RuleFor(x => x.Longitude).NotNull().NotEmpty().WithMessage("Longitude must be provided");
            RuleFor(x => x.SellerName).MustAsync(CheckProducerName).WithMessage("Seller name already in use");

        }
        private async Task<bool> CheckProducerName(string name, CancellationToken token)
        {
            var data = await _db.Sellers.FirstOrDefaultAsync(q => q.SellerName == name);
            if (data != null)
            {
                return false;
            }
            return true;
        }
    }
}
