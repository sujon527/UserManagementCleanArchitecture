namespace UserManagement.Domain.Exceptions;

public class UserDomainException : Exception
{
    public UserDomainException(string message) : base(message) { }
}
