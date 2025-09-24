using FluentValidation;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;

namespace ThesisTestAPI.Validators.Producer
{
    public class ProducerQueryValidator:AbstractValidator<ProducerQuery>
    {
        public ProducerQueryValidator() { 
            RuleFor(x=>x.pageNumber).NotNull().NotEmpty().WithMessage("Page number must be included");
            RuleFor(x=>x.pageSize).NotNull().NotEmpty().WithMessage("Page size must be included");
        }
    }
}
