using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer?> GetByEmailAsync(string email);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task AddAsync(Customer client);
        Task UpdateAsync(Customer client);
        Task RemoveAsync(Guid id);
    }
}
