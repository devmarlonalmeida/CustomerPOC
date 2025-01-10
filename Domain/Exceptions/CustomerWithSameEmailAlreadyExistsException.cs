namespace Domain.Exceptions
{
    public class CustomerWithSameEmailAlreadyExistsException : DomainException
    {
        public CustomerWithSameEmailAlreadyExistsException() : base("Já existe um cliente registrado com esse email.")
        {
        }
    }
}
