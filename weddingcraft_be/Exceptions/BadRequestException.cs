namespace weddingcraft_be.Exceptions;

/// <summary>Thrown when a client sends an invalid request (HTTP 400).</summary>
public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message, 400) { }
}
