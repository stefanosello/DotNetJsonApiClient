namespace JsonApiClient.Extensions;

internal static class StringExtensions
{
    internal static string Uncapitalize(this string arg)
    {
        if (string.IsNullOrEmpty(arg)) return arg;
        IEnumerable<string> strings = arg.Split(".");
        var uncapitalizedStrings = strings.Select(s => char.ToLower(s[0]) + s[1..]);
        return string.Join(".", uncapitalizedStrings);
    }
}