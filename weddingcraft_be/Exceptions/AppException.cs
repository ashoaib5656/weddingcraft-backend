namespace weddingcraft_be.Exceptions;

/// <summary>Base class for all application-level exceptions.</summary>
public abstract class AppException : Exception
{
    public int StatusCode { get; }

    protected AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
