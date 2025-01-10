using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Application.Tests.Repositories
{
    public class CustomerRepositoryTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;

        public CustomerRepositoryTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer ("John Doe", "john@example.com"),
                new Customer ("Jane Doe", "jane@example.com")
            };

            _customerRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(customers);

            // Act
            var result = await _customerRepositoryMock.Object.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().Contain(c => c.Name == "John Doe");
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ShouldReturnCustomer()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer(customerId, "Customer", "customer@customer.com");

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync(customer);

            // Act
            var result = await _customerRepositoryMock.Object.GetByIdAsync(customerId);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Contain("Customer");
        }

        [Fact]
        public async Task CreateAsync_ShouldAddNewCustomer()
        {
            // Arrange
            var customer = new Customer("New Customer", "new@example.com");

            _customerRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Customer>()))
                .Returns(Task.FromResult(customer));

            // Act
            await _customerRepositoryMock.Object.AddAsync(customer);

            // Assert
            _customerRepositoryMock.Verify(repo => repo.AddAsync(customer), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCustomer()
        {
            // Arrange
            var customer = new Customer("New Customer", "new@example.com");

            _customerRepositoryMock.Setup(repo => repo.UpdateAsync(customer))
                .Returns(Task.CompletedTask);

            // Act
            await _customerRepositoryMock.Object.UpdateAsync(customer);

            // Assert
            _customerRepositoryMock.Verify(repo => repo.UpdateAsync(customer), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveCustomer()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer("Customer", "customer@customer.com");

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId))
            .Returns(Task.FromResult(customer));

            _customerRepositoryMock.Setup(repo => repo.RemoveAsync(customerId))
                .Returns(Task.CompletedTask);

            // Act
            await _customerRepositoryMock.Object.RemoveAsync(customerId);

            // Assert
            _customerRepositoryMock.Verify(repo => repo.RemoveAsync(customerId), Times.Once);
        }
    }
}
