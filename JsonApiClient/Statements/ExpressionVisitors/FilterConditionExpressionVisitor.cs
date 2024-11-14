using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using JsonApiClient.Extensions;

namespace JsonApiClient.Statements.ExpressionVisitors;

internal class FilterConditionExpressionVisitor : ExpressionVisitor
{
    private readonly StringBuilder _sb = new();
    private static readonly IEnumerable<string> StringComparisonMethodNames = ["Contains", "StartsWith", "EndsWith"];

    public static string VisitExpression(Expression expression)
    {
        var visitor = new FilterConditionExpressionVisitor();
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
                break;
            case false when methodName == "Contains" && node.Arguments.Count > 0:
                _sb.Append("any(");
                break;
            default:
                throw new NotSupportedException($"The method '{methodName}' is not supported");
        }
        
        var isExtensionMethod = node.Arguments.Count > 1;
        Visit(isExtensionMethod ? node.Arguments[1] : isStringMethod ? node.Object : node.Arguments[0]);
        _sb.Append(',');
        Visit(isExtensionMethod || isStringMethod ? node.Arguments[0] : node.Object);
        _sb.Append(')');
        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is ConstantExpression)
            AppendValue(Expression.Lambda(node).Compile().DynamicInvoke());
        else if (node.Type == typeof(bool))
            _sb.Append($"equals({node.Member.Name.Uncapitalize()},'true')");
        else
            _sb.Append(node.Member.Name.Uncapitalize());

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
}