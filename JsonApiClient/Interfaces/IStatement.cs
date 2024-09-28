namespace JsonApiClient.Interfaces;

internal interface IStatement
{
    KeyValuePair<string,string> Translate();

    void Validate();
}