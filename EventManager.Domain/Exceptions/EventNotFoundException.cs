namespace EventManager.Domain.Exceptions;

public class EventNotFoundException : Exception
{
    public EventNotFoundException(Guid eventId)
        : base($"El evento con identificador '{eventId}' no fue encontrado.")
    {
    }
}
