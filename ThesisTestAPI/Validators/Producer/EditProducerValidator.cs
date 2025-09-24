using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;

namespace ThesisTestAPI.Validators.Producer
{
    public class EditProducerValidator:AbstractValidator<EditProducerRequest>
    {
        private readonly ThesisDbContext _db;
        public EditProducerValidator(ThesisDbContext db)
        {
            _db = db;
            RuleFor(x => x.ProducerId).NotNull().NotEmpty().WithMessage("Producer id must be provided");
            RuleFor(x => x).MustAsync(CheckProducerName).When(x => !string.IsNullOrEmpty(x.ProducerName)).WithMessage("Producer name already in use");
            RuleFor(x => x).Must(x => x.Latitude.HasValue && x.Longitude.HasValue).When(x => x.Latitude != null || x.Longitude != null).WithMessage("Incomplete latitude / longitude provided");
        }
        private async Task<bool> CheckProducerName(EditProducerRequest request, CancellationToken token)
        {
            var data = await _db.Producers.FirstOrDefaultAsync(q => q.ProducerName == request.ProducerName);
            if (data != null && data.ProducerId!=request.ProducerId)
            {
                return false;
            }
            return true;
        }
    }
}
