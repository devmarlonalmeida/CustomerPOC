using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMemoryCache _memoryCache;

        private const string CustomerCacheKey = "CustomerCache";
        private const string CustomerByIdCacheKey = "CustomerCache_";

        public CustomerService(ICustomerRepository customerRepository, IMemoryCache memoryCache)
        {
            _customerRepository = customerRepository;
            _memoryCache = memoryCache;
        }

        public async Task<Guid> AddCustomerAsync(CustomerDTO customerDto)
        {

            var existingCustomerWithEmail = await _customerRepository.GetByEmailAsync(customerDto.Email);

            if(existingCustomerWithEmail == null)
            {
                byte[]? imageBytes = null;

                if (customerDto.Logo != null && customerDto.Logo.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await customerDto.Logo.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }

                var customer = CustomerMapper.ToEntity(customerDto, imageBytes);
                await _customerRepository.AddAsync(customer);

                _memoryCache.Remove(CustomerCacheKey);

                return customer.Id;
            }
            else
                throw new CustomerWithSameEmailAlreadyExistsException();
            
        }

        public async Task UpdateCustomerAsync(CustomerDTO customerDto)
        {
            var existingCustomerWithEmail = await _customerRepository.GetByEmailAsync(customerDto.Email);

            if (existingCustomerWithEmail == null || (existingCustomerWithEmail is not null && existingCustomerWithEmail.Id == customerDto.Id))
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(customerDto.Id) ?? throw new CustomerNotFoundException();

                // Handle addresses
                var existingAddresses = existingCustomer.Addresses.ToList();

                // Remove addresses not present in the DTO
                var addressesToRemove = existingAddresses
                    .Where(a => !customerDto.Addresses.Any(dto => dto.Id == a.Id))
                    .ToList();

                foreach (var address in addressesToRemove)
                {
                    existingCustomer.Addresses.Remove(address);
                }

                // Add or update addresses from DTO
                foreach (var dtoAddress in customerDto.Addresses)
                {
                    var existingAddress = existingCustomer.Addresses.FirstOrDefault(a => a.Id == dtoAddress.Id);

                    if (existingAddress == null)
                    {
                        // Add new address
                        existingCustomer.Addresses.Add(new Address(dtoAddress.Street, dtoAddress.City, dtoAddress.State));
                    }
                    else
                    {
                        // Update existing address
                        existingAddress.Street = dtoAddress.Street;
                        existingAddress.City = dtoAddress.City;
                        existingAddress.State = dtoAddress.State;
                    }
                }

                await _customerRepository.UpdateAsync(existingCustomer);

                _memoryCache.Remove(CustomerCacheKey);
                _memoryCache.Remove(CustomerByIdCacheKey + existingCustomer.Id);
            }
            else
                throw new CustomerWithSameEmailAlreadyExistsException();

        }

        public async Task<CustomerDTO> GetCustomerByIdAsync(Guid id)
        {
            var cacheKey = CustomerByIdCacheKey + id;

            if (_memoryCache.TryGetValue(cacheKey, out CustomerDTO cachedCustomer))
            {
                return cachedCustomer;
            }

            var customer = await _customerRepository.GetByIdAsync(id) ?? throw new CustomerNotFoundException();
            var customerDTO = CustomerMapper.ToDTO(customer);
            _memoryCache.Set(cacheKey, customerDTO, TimeSpan.FromMinutes(10));

            return customerDTO;
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync()
        {
            if (_memoryCache.TryGetValue(CustomerCacheKey, out IEnumerable<CustomerDTO> cachedCustomers))
            {
                return cachedCustomers;
            }

            var customers = await _customerRepository.GetAllAsync();
            var customerDtos = customers.Select(CustomerMapper.ToDTO);

            _memoryCache.Set(CustomerCacheKey, customerDtos, TimeSpan.FromMinutes(10));

            return customerDtos;
        }

        public async Task RemoveCustomerAsync(Guid id)
        {
            _ = await _customerRepository.GetByIdAsync(id) ?? throw new CustomerNotFoundException();

            await _customerRepository.RemoveAsync(id);

            _memoryCache.Remove(CustomerCacheKey);
            _memoryCache.Remove(CustomerByIdCacheKey + id);
        }
    }
}
