namespace JsonApiClient.Statements;

public interface IStatement
{
    KeyValuePair<string,string> Translate(string? targetResourceName = null);
}