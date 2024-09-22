using System.Linq.Expressions;
using System.Reflection;
using JsonApiClient.Attributes;
using JsonApiClient.Exceptions;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;

namespace JsonApiClient.Statements;

public class IncludeStatement<TEntity>(Expression<Func<TEntity,object>> expression) : IStatement where TEntity : class
{
    public KeyValuePair<string,string> Translate(string? targetResourceName = null)
    {
        var member = (expression.Body as MemberExpression)!;
        return new KeyValuePair<string, string>($"include",$"{member.GetRelationshipName()}");
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