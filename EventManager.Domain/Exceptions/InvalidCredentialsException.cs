namespace EventManager.Domain.Exceptions;

public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Las credenciales proporcionadas son inválidas.")
    {
    }
}
