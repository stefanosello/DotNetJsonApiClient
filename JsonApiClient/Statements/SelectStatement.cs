using System.Linq.Expressions;
using System.Reflection;
using JsonApiClient.Attributes;
using JsonApiClient.Exceptions;
using JsonApiClient.Extensions;
using JsonApiClient.Interfaces;

namespace JsonApiClient.Statements;

internal class SelectStatement<TEntity>(Expression<Func<TEntity,object>> expression) : IStatement
    where TEntity : class, IJsonApiResource
{
    public KeyValuePair<string,string> Translate()
    {
        var newExpression = (expression.Body as NewExpression)!;

        var fields =
            newExpression.Arguments.Select(arg => (arg as MemberExpression)!.Member.Name.Uncapitalize());

        var resourceName = typeof(TEntity).GetResourceName();
        
        return new KeyValuePair<string, string>($"fields[{resourceName}]",$"{string.Join(',', fields)}");
    }
}