namespace Domain.Exceptions
{
    public class AddressNotFoundException : DomainException
    {
        public AddressNotFoundException() : base("Endereço não encontrado.")
        {
        }
    }
}
