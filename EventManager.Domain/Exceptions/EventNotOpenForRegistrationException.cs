using EventManager.Domain.Enums;

namespace EventManager.Domain.Exceptions;

public class EventNotOpenForRegistrationException : Exception
{
    public EventNotOpenForRegistrationException(Guid eventId, EventStatus currentStatus)
        : base($"El evento con identificador '{eventId}' no permite inscripciones porque se encuentra en estado '{currentStatus}'.")
    {
    }
}
