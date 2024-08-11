using JsonApiDotNetCore.Resources;

namespace JsonApiClient.Statements;

public interface IStatement
{
    KeyValuePair<string,string> Parse();
}