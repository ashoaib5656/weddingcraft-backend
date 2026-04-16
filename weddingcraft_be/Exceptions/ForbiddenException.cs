namespace weddingcraft_be.Exceptions;

/// <summary>Thrown when an action is not permitted for the current caller (HTTP 403).</summary>
public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "You do not have permission to perform this action.")
        : base(message, 403) { }
}
