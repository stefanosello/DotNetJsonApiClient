namespace JsonApiClient.Extensions;

public static class StringExtension
{
    public static string Uncapitalize(this string arg)
    {
        if (string.IsNullOrEmpty(arg)) return arg;
        return char.ToLower(arg[0]) + arg[1..];
    }
}