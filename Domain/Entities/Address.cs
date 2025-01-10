namespace Domain.Entities
{
    public class Address
    {
        public Guid Id { get; private set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public Address(string street, string city, string state)
        {
            Id = Guid.NewGuid();
            Street = street;
            City = city;
            State = state;
        }

        public Address(Guid id, string street, string city, string state)
        {
            Id = id;
            Street = street;
            City = city;
            State = state;
        }
    }
}
