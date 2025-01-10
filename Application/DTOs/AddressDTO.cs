namespace Application.DTOs
{
    public class AddressDTO
    {
        public Guid Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public Guid CustomerId { get; set; }
    }
}
