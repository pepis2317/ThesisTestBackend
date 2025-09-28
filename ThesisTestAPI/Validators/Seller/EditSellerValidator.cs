using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;

namespace ThesisTestAPI.Validators.Producer
{
    public class EditSellerValidator:AbstractValidator<EditSellerRequest>
    {
        private readonly ThesisDbContext _db;
        public EditSellerValidator(ThesisDbContext db)
        {
            _db = db;
            RuleFor(x => x.SellerId).NotNull().NotEmpty().WithMessage("Seller id must be provided");
            RuleFor(x => x).MustAsync(CheckProducerName).When(x => !string.IsNullOrEmpty(x.SellerName)).WithMessage("Seller name already in use");
            RuleFor(x => x).Must(x => x.Latitude.HasValue && x.Longitude.HasValue).When(x => x.Latitude != null || x.Longitude != null).WithMessage("Incomplete latitude / longitude provided");
        }
        private async Task<bool> CheckProducerName(EditSellerRequest request, CancellationToken token)
        {
            var data = await _db.Sellers.FirstOrDefaultAsync(q => q.SellerName == request.SellerName);
            if (data != null && data.SellerId!=request.SellerId)
            {
                return false;
            }
            return true;
        }
    }
}
