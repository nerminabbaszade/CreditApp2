using System.Runtime.Serialization;

namespace CreditApp.DAL.Exceptions;

public class EntityNotFoundException : Exception
{
    private const string DefaultMessage = "Entity not found";
    public EntityNotFoundException() : base(DefaultMessage)
    {
    }

    protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public EntityNotFoundException(string? message) : base(message)
    {
    }

    public EntityNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}