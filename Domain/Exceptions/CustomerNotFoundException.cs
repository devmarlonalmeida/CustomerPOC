namespace Domain.Exceptions
{
    public class CustomerNotFoundException : DomainException
    {
        public CustomerNotFoundException() : base("Cliente não encontrado.")
        {
        }
    }
}
