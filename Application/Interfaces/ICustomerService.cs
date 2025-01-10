using Application.DTOs;

namespace Application.Interfaces
{
    public interface ICustomerService
    {
        Task<Guid> AddCustomerAsync(CustomerDTO customerDto);
        Task UpdateCustomerAsync(CustomerDTO customerDto);
        Task<CustomerDTO> GetCustomerByIdAsync(Guid id);
        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync();
        Task RemoveCustomerAsync(Guid id);
    }
}
