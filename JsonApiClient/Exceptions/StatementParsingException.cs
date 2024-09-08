namespace JsonApiClient.Exceptions;

public class StatementParsingException : InvalidOperationException
{
    public StatementParsingException(string message) : base(message)
    { }

    public StatementParsingException(string message, Exception innerException) : base(message, innerException)
    { }
}