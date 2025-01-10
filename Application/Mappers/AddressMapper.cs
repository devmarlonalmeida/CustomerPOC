using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers
{
    public static class AddressMapper
    {
        public static Address ToEntity(AddressDTO dto)
        {
            return new Address(dto.Street, dto.City, dto.State);
        }

        public static AddressDTO ToDTO(Address entity)
        {
            return new AddressDTO
            {
                Id = entity.Id,
                Street = entity.Street,
                City = entity.City,
                State = entity.State
            };
        }
    }
}
