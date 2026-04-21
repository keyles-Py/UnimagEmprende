namespace EventManager.Domain.Exceptions;

public class DuplicateRegistrationException : Exception
{
    public DuplicateRegistrationException(Guid eventId, Guid userId)
        : base($"El usuario con identificador '{userId}' ya se encuentra inscrito en el evento '{eventId}'.")
    {
    }
}
