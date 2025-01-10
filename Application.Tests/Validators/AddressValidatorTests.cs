using Application.DTOs;
using Application.Validators;
using FluentAssertions;

namespace Application.Tests.Validators
{
    public class AddressValidatorTests
    {
        [Fact]
        public void Validate_ShouldReturnValidForCorrectData()
        {
            // Arrange
            var validator = new AddressValidator();
            var addressDto = new AddressDTO
            {
                Street = "Street",
                City = "City",
                State = "State"
            };

            // Act
            var result = validator.Validate(addressDto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldReturnInvalidForMissingName()
        {
            // Arrange
            var validator = new AddressValidator();
            var addressDto = new AddressDTO
            {
                Street = "Street",
                City = "City",
            };

            // Act
            var result = validator.Validate(addressDto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "State");
        }
    }
}
