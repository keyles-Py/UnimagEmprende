namespace EventManager.Domain.Exceptions;

public sealed class EmailAlreadyExistsException : Exception
{
    public string Email { get; }

    public EmailAlreadyExistsException(string email)
        : base($"El email '{email}' ya está registrado.")
    {
        Email = email;
    }
}
