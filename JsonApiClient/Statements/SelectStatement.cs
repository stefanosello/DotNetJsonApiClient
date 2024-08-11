using System.Linq.Expressions;
using JsonApiClient.Extensions;
using JsonApiDotNetCore.Resources;
using NotSupportedException = System.NotSupportedException;

namespace JsonApiClient.Statements;

public class SelectStatement<TEntity>(Expression<Func<TEntity,object>> expression) : IStatement where TEntity : class, IIdentifiable
{
    public KeyValuePair<string,string> Parse()
    {
        List<string> fields = [];
        if (expression.Body is not NewExpression newExpression)
            throw new NotSupportedException($"Can not define a Select statement without a {typeof(NewExpression)} expression body.");

        foreach (var field in newExpression.Arguments)
        {
            if (field is MemberExpression member)
                fields.Add(member.Member.Name.Uncapitalize());
            else
                throw new NotSupportedException(
                    $"Found {typeof(NewExpression)} argument which is not a {typeof(MemberExpression)}.");
        }
        
        return new KeyValuePair<string, string>($"fields[{typeof(TEntity).Name.Uncapitalize()}]",$"{string.Join(',', fields)}");
    }
}