namespace JsonApiClient.Exceptions;

/// <summary>
/// Exception thrown when an HTTP request fails.
/// </summary>
public class JsonApiHttpException : JsonApiException
{
    /// <summary>
    /// The HTTP status code of the failed request.
    /// </summary>
    public int StatusCode { get; }
    
    /// <summary>
    /// The response content from the failed request.
    /// </summary>
    public string? ResponseContent { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonApiHttpException"/> class.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="message">The error message.</param>
    /// <param name="responseContent">The response content.</param>
    public JsonApiHttpException(int statusCode, string message, string? responseContent = null) 
        : base(message)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonApiHttpException"/> class.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="responseContent">The response content.</param>
    public JsonApiHttpException(int statusCode, string message, Exception innerException, string? responseContent = null) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }
} 