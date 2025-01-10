using System.ComponentModel.DataAnnotations;

namespace ClientesPOCUI.Models
{
    public class CustomerViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public IFormFile Logo { get; set; }

        public byte[]? LogoBytes { get; set; }

        public string? LogoFileName { get; set; }

        public string? LogoContentType { get; set; }

        [Required]
        public List<AddressViewModel> Addresses { get; set; } = new();
    }


}
