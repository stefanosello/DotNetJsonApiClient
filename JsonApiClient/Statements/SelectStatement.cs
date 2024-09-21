using System.Linq.Expressions;
using JsonApiClient.Attributes;
using JsonApiClient.Exceptions;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;

namespace JsonApiClient.Statements;

public class SelectStatement<TEntity>(Expression<Func<TEntity,object>> expression) : IStatement where TEntity : class
{
    public KeyValuePair<string,string> Translate(string? targetResourceName = null)
    {
        ValidateExpression();
        
        var newExpression = (expression.Body as NewExpression)!;

        var fields =
            newExpression.Arguments.Select(arg => (arg as MemberExpression)!.Member.Name.Uncapitalize());

        var resourceName = targetResourceName ?? typeof(TEntity).Name.Uncapitalize();
        
        return new KeyValuePair<string, string>($"fields[{resourceName}]",$"{string.Join(',', fields)}");
    }

    private void ValidateExpression()
    {
        if (expression.Body is not NewExpression newExpression)
            throw new StatementTranslationException($"Invalid expression body type. Expected: {nameof(NewExpression)}. Found: {expression.Body.NodeType}");

        var firstWrongTypeArgument = newExpression.Arguments.FirstOrDefault(arg => arg is not MemberExpression);
        if (firstWrongTypeArgument != null)
            throw new StatementTranslationException($"Invalid expression body argument type. Expected: {nameof(MemberExpression)}. Found: {firstWrongTypeArgument.NodeType}");
        
        var firstInvalidMemberName = newExpression.Arguments.FirstOrDefault(arg => arg is MemberExpression member && !IsValidMemberName(member.Member.Name));
        if (firstInvalidMemberName != null)
            throw new StatementTranslationException($"Invalid member name: member {(firstInvalidMemberName as MemberExpression)!.Member.Name} is not a valid property name for {typeof(TEntity).Name}");
    }

    private static bool IsValidMemberName(string memberName)
    {
        var validPropertyNames = typeof(TEntity)
            .GetProperties()
            .Where(p =>
                Attribute.GetCustomAttribute(p, typeof(JAttrAttribute)) is not null ||
                Attribute.GetCustomAttribute(p, typeof(JRelAttribute)) is not null)
            .Select(p => p.Name);
        
        return validPropertyNames.Contains(memberName);
    }
}