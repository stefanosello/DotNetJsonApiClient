namespace JsonApiClient.Exceptions;

public class MissingAttributeException: InvalidOperationException
{
    public MissingAttributeException(string message) : base(message)
    { }

    public MissingAttributeException(string message, Exception innerException) : base(message, innerException)
    { }
}