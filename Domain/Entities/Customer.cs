namespace Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public List<Address> Addresses { get; private set; }
        public byte[] Logo { get; private set; }
        public string LogoFileName { get; private set; }
        public string LogoContentType { get; private set; }

        public Customer(string name, string email, byte[] logo, string fileName, string fileContentType)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Addresses = new List<Address>();
            Logo = logo;
            LogoFileName = fileName;
            LogoContentType = fileContentType;
        }

        public Customer(string name, string email)
        {
            Name = name;
            Email = email;
            Addresses = new List<Address>();
        }

        public Customer(Guid id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
            Addresses = new List<Address>();
        }

        public void AddAddress(Address address)
        {
            Addresses.Add(address);
        }
        public void UpdateLogo(byte[] logo)
        {
            Logo = logo;
        }
    }
}
