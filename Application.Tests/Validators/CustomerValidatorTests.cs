using Application.DTOs;
using Application.Validators;
using FluentAssertions;

namespace Application.Tests.Validators
{
    public class CustomerValidatorTests
    {
        [Fact]
        public void Validate_ShouldReturnValidForCorrectData()
        {
            // Arrange
            var validator = new CustomerValidator();
            var customerDto = new CustomerDTO
            {
                Name = "John Doe",
                Email = "john@example.com"
            };

            // Act
            var result = validator.Validate(customerDto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldReturnInvalidForMissingName()
        {
            // Arrange
            var validator = new CustomerValidator();
            var customerDto = new CustomerDTO
            {
                Email = "john@example.com"
            };

            // Act
            var result = validator.Validate(customerDto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }
    }
}
