namespace Domain.Exceptions
{
    public class UserNotFoundException : DomainException
    {
        public UserNotFoundException() : base("Usuário não encontrado.")
        {
        }
    }
}
