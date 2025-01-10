using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Collections;
using System.IO;
using System.Net.Mime;

namespace Application.Tests.Service
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly ICustomerService _customerService;

        public CustomerServiceTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _cacheMock = new Mock<IMemoryCache>();

            _customerService = new CustomerService(_customerRepositoryMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
            new Customer ("John Doe", "john@example.com"),
            new Customer ("Jane Doe", "jane@example.com")
        };

            _customerRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(customers);

            _cacheMock
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>);

            // Act
            var result = await _customerService.GetAllCustomersAsync();

            // Assert
            result.Should().HaveCount(2);
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

            _cacheMock
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(Mock.Of<ICacheEntry>);

            // Act
            var result = await _customerService.GetCustomerByIdAsync(customerId);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Contain("Customer");
        }

        [Fact]
        public async Task CreateAsync_ShouldAddNewCustomer()
        {
            // Arrange
            var byteArray = new byte[510];
            var stream = new MemoryStream(byteArray);
            var contentType = "test/Type";

            var formFile = new FormFile(stream, 0, byteArray.Length, "file", "fileMock")
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            var customerDto = new CustomerDTO
            {
                Name = "New Customer",
                Email = "new@example.com",
                Logo = formFile
            };

            var customer = new Customer(customerDto.Name, customerDto.Email);

            _customerRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Customer>()))
                .Returns(Task.FromResult(customer));

            // Act
            var result = await _customerService.AddCustomerAsync(customerDto);

            // Assert
            result.GetType().Should().Be(typeof(Guid));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCustomer()
        {
            // Arrange
            var customerDto = new CustomerDTO
            {
                Name = "New Customer",
                Email = "new@example.com",
                Addresses = new List<AddressDTO>
                {
                    new AddressDTO
                    {
                        State = "State",
                        City = "City",
                        Street = "Street"
                    }
                }
            };

            var customer = new Customer(customerDto.Name, customerDto.Email);

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customer.Id))
                .Returns(Task.FromResult(customer));

            _customerRepositoryMock.Setup(repo => repo.UpdateAsync(customer))
                .Returns(Task.CompletedTask);

            // Act
            await _customerService.UpdateCustomerAsync(customerDto);

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
            await _customerService.RemoveCustomerAsync(customerId);

            // Assert
            _customerRepositoryMock.Verify(repo => repo.RemoveAsync(customerId), Times.Once);
        }
    }
}
