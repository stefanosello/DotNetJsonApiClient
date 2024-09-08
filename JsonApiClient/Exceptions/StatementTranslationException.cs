namespace JsonApiClient.Exceptions;

public class StatementTranslationException : InvalidOperationException
{
    public StatementTranslationException(string message) : base(message)
    { }

    public StatementTranslationException(string message, Exception innerException) : base(message, innerException)
    { }
}