namespace EventManager.Domain.Exceptions;

public class EventFullException : Exception
{
    public EventFullException(Guid eventId)
        : base($"El evento con identificador '{eventId}' ha alcanzado su capacidad máxima de asistentes.")
    {
    }
}
