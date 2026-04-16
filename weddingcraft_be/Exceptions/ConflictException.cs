namespace weddingcraft_be.Exceptions;

/// <summary>Thrown when a resource already exists or a unique constraint would be violated (HTTP 409).</summary>
public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, 409) { }
}
