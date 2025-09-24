using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;

namespace ThesisTestAPI.Validators.Producer
{
    public class CreateProducerValidator:AbstractValidator<CreateProducerRequest>
    {
        private readonly ThesisDbContext _db;
        public CreateProducerValidator(ThesisDbContext db)
        {
            _db = db;
            RuleFor(x => x.OwnerId).NotNull().NotEmpty().WithMessage("Owner id must be provided");
            RuleFor(x => x.ProducerName).NotNull().NotEmpty().WithMessage("Producer name must be provided");
            RuleFor(x => x.Latitude).NotNull().NotEmpty().WithMessage("Latitude must be provided");
            RuleFor(x => x.Longitude).NotNull().NotEmpty().WithMessage("Longitude must be provided");
            RuleFor(x => x.ProducerName).MustAsync(CheckProducerName).WithMessage("Producer name already in use");

        }
        private async Task<bool> CheckProducerName(string name, CancellationToken token)
        {
            var data = await _db.Producers.FirstOrDefaultAsync(q => q.ProducerName == name);
            if (data != null)
            {
                return false;
            }
            return true;
        }
    }
}
