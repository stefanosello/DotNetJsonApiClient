using System.Linq.Expressions;
using JsonApiClient.Exceptions;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;

namespace JsonApiClient.Statements;

public class SelectStatement<TEntity>(Expression<Func<TEntity,object>> expression) : IStatement where TEntity : class
{
    public KeyValuePair<string,string> Translate(string? targetResourceName = null)
    {
        ValidateExpression();
        
        NewExpression newExpression = (expression.Body as NewExpression)!;

        IEnumerable<string> fields =
            newExpression.Arguments.Select(arg => (arg as MemberExpression)!.Member.Name.Uncapitalize());

        string resourceName = targetResourceName ?? typeof(TEntity).Name.Uncapitalize();
        
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

    private bool IsValidMemberName(string memberName)
    {
        IEnumerable<string> validMemberNames = typeof(TEntity).GetMembers().Select(m => m.Name);
        return validMemberNames.Contains(memberName);
    }
}