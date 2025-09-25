using FluentValidation;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Producer;

namespace ThesisTestAPI.Validators.Producer
{
    public class SellerQueryValidator:AbstractValidator<SellerQuery>
    {
        public SellerQueryValidator() { 
            RuleFor(x=>x.pageNumber).NotNull().NotEmpty().WithMessage("Page number must be included");
            RuleFor(x=>x.pageSize).NotNull().NotEmpty().WithMessage("Page size must be included");
        }
    }
}
