using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerDTO>
    {
        public CustomerValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Customer name is required.")
                .MaximumLength(100).WithMessage("Customer name must not exceed 100 characters.");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleForEach(c => c.Addresses).SetValidator(new AddressValidator());
        }
    }
}
