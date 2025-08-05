namespace JsonApiClient.Exceptions;

/// <summary>
/// Base exception for JSON:API client errors.
/// </summary>
public class JsonApiException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonApiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public JsonApiException(string message) : base(message) { }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonApiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public JsonApiException(string message, Exception innerException) : base(message, innerException) { }
} 