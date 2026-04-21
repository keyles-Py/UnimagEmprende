namespace EventManager.Domain.Exceptions;

public class InvalidEventCapacityException : Exception
{
    public InvalidEventCapacityException(int capacity)
        : base($"La capacidad máxima de asistentes debe ser mayor a cero. Valor recibido: {capacity}.")
    {
    }
}
