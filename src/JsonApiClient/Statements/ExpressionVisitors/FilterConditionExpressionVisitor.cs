using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using JsonApiClient.Extensions;

namespace JsonApiClient.Statements.ExpressionVisitors;

internal class FilterConditionExpressionVisitor : ExpressionVisitor
{
    private readonly string? _memberPrefix;
    private readonly StringBuilder _sb = new();
    private static readonly IEnumerable<string> StringComparisonMethodNames = ["Contains", "StartsWith", "EndsWith"];
    
    private FilterConditionExpressionVisitor(string? memberPrefix = null)
    {
        _memberPrefix = memberPrefix;
    }

    public static string VisitExpression(Expression expression, string? memberPrefix = null)
    {
        var visitor = new FilterConditionExpressionVisitor(memberPrefix);
        visitor.Visit(expression);
        return visitor._sb.ToString();
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        _sb.Append(node.NodeType switch
        {
            ExpressionType.Equal => "equals(",
            ExpressionType.NotEqual => "not(equals(",
            ExpressionType.GreaterThan => "greaterThan(",
            ExpressionType.GreaterThanOrEqual => "greaterOrEqual(",
            ExpressionType.LessThan => "lessThan(",
            ExpressionType.LessThanOrEqual => "lessOrEqual(",
            ExpressionType.AndAlso => "and(",
            ExpressionType.OrElse => "or(",
            _ => throw new NotSupportedException($"The binary operator '{node.NodeType}' is not supported")
        });
        Visit(node.Left);
        _sb.Append(',');
        Visit(node.Right);
        _sb.Append(node.NodeType == ExpressionType.NotEqual ? "))" : ")");
        return node;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var methodName = node.Method.Name;
        var isStringMethod = node.Method.DeclaringType == typeof(string);

        switch (isStringMethod)
        {
            case true when StringComparisonMethodNames.Contains(methodName):
                _sb.Append($"{methodName.Uncapitalize()}(");
                VisitStringOrCollectionMethodCall(node, true);
                _sb.Append(')');
                break;
            case false when methodName == "Contains" && node.Arguments.Count > 0:
                _sb.Append("any(");
                VisitStringOrCollectionMethodCall(node, false);
                _sb.Append(')');
                break;
            case false when methodName == "Any" && node.Arguments is [MemberExpression propertyExpression]:
                _sb.Append($"has({SubresourceSelectorExpressionVisitor.VisitExpression(propertyExpression)})");
                break;
            case false when methodName == "Any" && node.Arguments is [MemberExpression propertyExpression, LambdaExpression lambdaExpression]:
                _sb.Append(VisitExpression(lambdaExpression,GetFullMemberName(SubresourceSelectorExpressionVisitor.VisitExpression(propertyExpression))));
                break;
            default:
                throw new NotSupportedException($"The method '{methodName}' is not supported");
        }
        
        return node;
    }
    
    private void VisitStringOrCollectionMethodCall(MethodCallExpression node, bool isStringMethod)
    {
        var isExtensionMethod = node.Arguments.Count > 1;
        Visit(isExtensionMethod ? node.Arguments[1] : isStringMethod ? node.Object : node.Arguments[0]);
        _sb.Append(',');
        Visit(isExtensionMethod || isStringMethod ? node.Arguments[0] : node.Object);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is ConstantExpression)
            AppendValue(Expression.Lambda(node).Compile().DynamicInvoke());
        else if (node.Type == typeof(bool))
            _sb.Append($"equals({GetFullMemberName(AttributeSelectorExpressionVisitor.VisitExpression(node))},'true')");
        else
        {
            var attributeName = GetFullMemberName(AttributeSelectorExpressionVisitor.VisitExpression(node));
            _sb.Append(attributeName);
        }   
        
        return node;
    }

    protected override Expression VisitNewArray(NewArrayExpression node)
    {
        VisitCollection(node.Expressions);
        return node;
    }

    protected override Expression VisitListInit(ListInitExpression node)
    {
        VisitCollection(node.Initializers.SelectMany(init => init.Arguments));
        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        AppendValue(node.Value);
        return node;
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        if (node.NodeType == ExpressionType.Not)
        {
            _sb.Append("not(");
            Visit(node.Operand);
            _sb.Append(')');
        }
        else base.VisitUnary(node);
        return node;
    }

    private void VisitCollection(IEnumerable<Expression> expressions)
    {
        var first = true;
        foreach (var expression in expressions)
        {
            if (!first) _sb.Append(',');
            Visit(expression);
            first = false;
        }
    }

    private void AppendValue(object? value)
    {
        _sb.Append(value switch
        {
            null => "null",
            string strValue => $"'{strValue}'",
            bool boolValue => $"'{boolValue.ToString().ToLower()}'",
            DateTime dateTime => $"'{dateTime:yyyy-MM-dd HH:mm:ss}'",
            DateTimeOffset dateTimeOffset => $"'{dateTimeOffset:yyyy-MM-dd HH:mm:ss}'",
            TimeSpan timeSpan => $"'{timeSpan}'",
            IEnumerable<object> enumerable => string.Join(",", enumerable.Select(v => $"'{v}'")),
            _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
        });
    }

    private string? GetFullMemberName(string? memberName) => memberName is null ? null : _memberPrefix is null ? memberName : $"{_memberPrefix}.{memberName}";
}