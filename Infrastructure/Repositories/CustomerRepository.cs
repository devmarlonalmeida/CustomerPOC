using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Customer customer)
        {
            var addressesTable = new DataTable();
            addressesTable.Columns.Add("Street", typeof(string));
            addressesTable.Columns.Add("City", typeof(string));
            addressesTable.Columns.Add("State", typeof(string));
            addressesTable.Columns.Add("CustomerId", typeof(Guid));

            foreach (var address in customer.Addresses)
            {
                addressesTable.Rows.Add(
                    address.Street,
                    address.City,
                    address.State,
                    customer.Id
                );
            }

            var addressesParam = new SqlParameter("@Addresses", addressesTable)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "AddressTableType"
            };

            await _context.Database.ExecuteSqlRawAsync(@"
            EXEC AddCustomer 
                @Id = {0}, 
                @Name = {1}, 
                @Email = {2},
                @Logo = {3},
	            @LogoContentType = {4},
	            @LogoFileName = {5},
                @Addresses = {6}",
                    customer.Id,
                    customer.Name,
                    customer.Email,
                    customer.Logo ?? (object)DBNull.Value,
                    customer.LogoContentType,
                    customer.LogoFileName,
                    addressesParam);
        }

        public async Task UpdateAsync(Customer customer)
        {
            var addressesTable = new DataTable();
            addressesTable.Columns.Add("Street", typeof(string));
            addressesTable.Columns.Add("City", typeof(string));
            addressesTable.Columns.Add("State", typeof(string));
            addressesTable.Columns.Add("CustomerId", typeof(Guid));

            foreach (var address in customer.Addresses)
            {
                addressesTable.Rows.Add(
                    address.Street,
                    address.City,
                    address.State,
                    customer.Id
                );
            }

            var addressesParam = new SqlParameter("@Addresses", addressesTable)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "AddressTableType"
            };

            await _context.Database.ExecuteSqlRawAsync(@"
            EXEC UpdateCustomer 
                @Id = {0}, 
                @Name = {1}, 
                @Email = {2}, 
                @Logo = {3}, 
	            @LogoContentType = {4},
	            @LogoFileName = {5},
                @Addresses = {6}",
                customer.Id,
                customer.Name,
                customer.Email,
                customer.Logo ?? (object)DBNull.Value,
                customer.LogoContentType,
                customer.LogoFileName,
                addressesParam);
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            return await _context.Customers
                                 .Include(c => c.Addresses)
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customers
                                 .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                                    .Include(c => c.Addresses)
                                    .ToListAsync();
            
        }

        public async Task RemoveAsync(Guid id)
        {
            var customer = await GetByIdAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
    }
}
