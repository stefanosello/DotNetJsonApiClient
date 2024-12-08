using System.Linq.Expressions;
using JsonApiClient.Interfaces;
using JsonApiClient.Statements.ExpressionVisitors;

namespace JsonApiClient.Statements;

internal class IncludeStatement<TEntity>(Expression<Func<TEntity,object>> expression) : IStatement
    where TEntity : class, IJsonApiResource
{
    public KeyValuePair<string,string> Translate()
    {
        var includedSubresource = SubresourceSelectorExpressionVisitor.VisitExpression(expression.Body);
        return new KeyValuePair<string, string>($"include",includedSubresource);
    }
}