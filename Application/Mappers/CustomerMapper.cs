using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers
{
    public static class CustomerMapper
    {
        public static Customer ToEntity(CustomerDTO dto, byte[]? imageBytes = null)
        {
            var customer = new Customer(dto.Name, dto.Email, imageBytes, dto.Logo.FileName, dto.Logo.ContentType);

            if (dto.Addresses != null)
            {
                foreach (var addressDto in dto.Addresses)
                {
                    customer.AddAddress(AddressMapper.ToEntity(addressDto));
                }
            }

            return customer;
        }

        public static CustomerDTO ToDTO(Customer entity)
        {
            return new CustomerDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Addresses = entity.Addresses.Select(AddressMapper.ToDTO).ToList(),
                LogoBytes = entity?.Logo,
                LogoFileName = entity?.LogoFileName,
                LogoContentType = entity?.LogoContentType
            };
        }
    }
}
