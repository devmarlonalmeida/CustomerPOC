using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Address address)
        {
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
        }

        public async Task<Address> GetByIdAsync(Guid id)
        {
            return await _context.Addresses
                                 .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            return await _context.Addresses.ToListAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var address = await GetByIdAsync(id);
            if (address != null)
            {
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
            }
        }
    }
}
