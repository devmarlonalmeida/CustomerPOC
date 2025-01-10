using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class AddressValidator : AbstractValidator<AddressDTO>
    {
        public AddressValidator()
        {
            RuleFor(a => a.Street)
                .NotEmpty().WithMessage("Street is required.");

            RuleFor(a => a.City)
                .NotEmpty().WithMessage("City is required.");

            RuleFor(a => a.State)
                .NotEmpty().WithMessage("State is required.");
        }
    }
}
