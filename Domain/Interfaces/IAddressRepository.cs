using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAddressRepository
    {
        Task<Address> GetByIdAsync(Guid id);
        Task<IEnumerable<Address>> GetAllAsync();
        Task AddAsync(Address address);
        Task UpdateAsync(Address address);
        Task RemoveAsync(Guid id);
    }
}
