namespace JsonApiClient.Interfaces;

internal interface IStatement
{
    KeyValuePair<string,string> Translate(string? targetResourceName = null);

    void Validate();
}