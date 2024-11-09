using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using JsonApiClient.Attributes;
using JsonApiClient.Exceptions;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;

namespace JsonApiClient.Statements;

internal class IncludeStatement<TEntity>(Expression<Func<TEntity,object>> expression) : IStatement
    where TEntity : class, IJsonApiResource
{
    public KeyValuePair<string,string> Translate()
    {
        var value = expression.Body switch
        {
            MemberExpression member => member.GetRelationshipName(),
            MethodCallExpression methodCall => methodCall.GetRelationshipsChain(),
            _ => throw new InvalidExpressionException($"Expression of type {typeof(MemberExpression)} or {typeof(MethodCallExpression)} expected, but #{expression.GetType().Name} found: {expression}.")
        };
        return new KeyValuePair<string, string>($"include",value);
    }

    public void Validate()
    {
        if (typeof(TEntity).GetCustomAttribute(typeof(JResAttribute)) is null)
            throw new StatementTranslationException($"Invalid entity type. Entity {nameof(TEntity)} is not decorated with the {nameof(JResAttribute)} attribute.");
        
        if (expression.Body is not MemberExpression memberExpression)
            throw new StatementTranslationException($"Invalid expression body type. Expected: {nameof(MemberExpression)}. Found: {expression.Body.NodeType}");
        
        if (!IsValidMemberName(memberExpression.Member.Name))
            throw new StatementTranslationException($"Invalid relationship name: member {memberExpression.Member.Name} is not a valid property name for {typeof(TEntity).Name}");
    }

    private static bool IsValidMemberName(string memberName)
    {
        var validPropertyNames = typeof(TEntity)
            .GetProperties()
            .Where(p => Attribute.GetCustomAttribute(p, typeof(JRelAttribute)) is not null)
            .Select(p => p.Name);
        
        return validPropertyNames.Contains(memberName);
    }
}