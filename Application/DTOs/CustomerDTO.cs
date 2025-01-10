using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class CustomerDTO
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        public IFormFile Logo { get; set; }

        public byte[]? LogoBytes { get; set; }

        public string? LogoFileName { get; set; }

        public string? LogoContentType { get; set; }

        public List<AddressDTO> Addresses { get; set; }
    }
}
