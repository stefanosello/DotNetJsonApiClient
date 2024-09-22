namespace JsonApiClient.Extensions;

public static class StringExtension
{
    public static string Uncapitalize(this string arg)
    {
        if (string.IsNullOrEmpty(arg)) return arg;
        IEnumerable<string> strings = arg.Split(".");
        var uncapitalizedStrings = strings.Select(s => char.ToLower(s[0]) + s[1..]);
        return string.Join(".", uncapitalizedStrings);
    }
}