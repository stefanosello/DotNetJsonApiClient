using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using JsonApiClient.Extensions;

namespace JsonApiClient.ExpressionVisitors;

internal class JsonApiFilterExpressionVisitor: ExpressionVisitor
{
    private readonly StringBuilder _sb = new();

    public override string ToString() => _sb.ToString();

    protected override Expression VisitBinary(BinaryExpression node)
    {
        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                HandleEquality(node);
                break;
            case ExpressionType.NotEqual:
                HandleInequality(node);
                break;
            case ExpressionType.GreaterThan:
                HandleComparison(node, "greaterThan");
                break;
            case ExpressionType.GreaterThanOrEqual:
                HandleComparison(node, "greaterOrEqual");
                break;
            case ExpressionType.LessThan:
                HandleComparison(node, "lessThan");
                break;
            case ExpressionType.LessThanOrEqual:
                HandleComparison(node, "lessOrEqual");
                break;
            case ExpressionType.AndAlso:
                HandleLogical(node, "and");
                break;
            case ExpressionType.OrElse:
                HandleLogical(node, "or");
                break;
            default:
                throw new NotSupportedException($"The binary operator '{node.NodeType}' is not supported");
        }

        return node;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType == typeof(string))
        {
            switch (node.Method.Name)
            {
                case "Contains":
                    HandleStringMethod(node, "contains");
                    break;
                case "StartsWith":
                    HandleStringMethod(node, "startsWith");
                    break;
                case "EndsWith":
                    HandleStringMethod(node, "endsWith");
                    break;
                default:
                    throw new NotSupportedException($"The method '{node.Method.Name}' is not supported");
            }
        }
        else if (node.Method.Name == "Contains" && node.Arguments.Count > 0)
        {
            HandleCollectionContains(node);
        }
        else
        {
            throw new NotSupportedException($"The method '{node.Method.Name}' is not supported");
        }

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is ConstantExpression)
        {
            var value = Expression.Lambda(node).Compile().DynamicInvoke();
            AppendValue(value);
            return node;
        }
        
        if (node.Type == typeof(bool))
        {
            _sb.Append("equals(");
            _sb.Append(node.Member.Name.Uncapitalize());
            _sb.Append($",'true')");
        }
        else
        {
            _sb.Append(node.Member.Name.Uncapitalize());
        }
        
        return node;
    }

    protected override Expression VisitNewArray(NewArrayExpression node)
    {
        foreach (Expression exp in node.Expressions)
        {
            Visit(exp);
            if (node.Expressions.Last() != exp)
                _sb.Append(',');
        }

        return node;
    }
    
    protected override Expression VisitListInit(ListInitExpression node)
    {
        IReadOnlyCollection<Expression> arguments = node.Initializers.SelectMany(init => init.Arguments).ToArray();
        foreach (Expression exp in arguments)
        {
            Visit(exp);
            if (arguments.Last() != exp)
                _sb.Append(','); 
        }
        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        AppendValue(node.Value);
        return node;
    }
    
    protected override Expression VisitUnary(UnaryExpression node)
    {
        if (node.NodeType != ExpressionType.Not) return base.VisitUnary(node);
        HandleNegation(node);
        return node;
    }

    private void AppendValue(object? value)
    {
        switch (value)
        {
            case null:
                _sb.Append("null");
                break;
            case string strValue:
                _sb.Append($"'{strValue}'");
                break;
            case bool boolValue:
                _sb.Append($"'{boolValue.ToString().ToLower()}'");
                break;
            case DateTime dateTime:
                _sb.Append($"'{dateTime:yyyy-MM-dd HH:mm:ss}'");
                break;
            case DateTimeOffset dateTimeOffset:
                _sb.Append($"'{dateTimeOffset:yyyy-MM-dd HH:mm:ss}'");
                break;
            case TimeSpan timeSpan:
                _sb.Append($"'{timeSpan.ToString()}'");
                break;
            case IEnumerable<object> enumerable:
                _sb.Append(string.Join(",", enumerable.Select(v => $"'{v}'")));
                break;
            default:
                _sb.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
                break;
        }
    }
    
    private void HandleEquality(BinaryExpression node)
    {
        _sb.Append("equals(");
        Visit(node.Left);
        _sb.Append(',');
        Visit(node.Right);
        _sb.Append(')');
    }

    private void HandleInequality(BinaryExpression node)
    {
        _sb.Append("not(equals(");
        Visit(node.Left);
        _sb.Append(',');
        Visit(node.Right);
        _sb.Append("))");
    }

    private void HandleNegation(UnaryExpression node)
    {
        _sb.Append("not(");
        Visit(node.Operand);
        _sb.Append(')');
    }

    private void HandleComparison(BinaryExpression node, string operatorName)
    {
        _sb.Append($"{operatorName}(");
        Visit(node.Left);
        _sb.Append(',');
        Visit(node.Right);
        _sb.Append(')');
    }

    private void HandleLogical(BinaryExpression node, string operatorName)
    {
        _sb.Append($"{operatorName}(");
        Visit(node.Left);
        _sb.Append(',');
        Visit(node.Right);
        _sb.Append(')');
    }

    private void HandleStringMethod(MethodCallExpression node, string operatorName)
    {
        _sb.Append($"{operatorName}(");
        Visit(node.Object);
        _sb.Append(',');
        Visit(node.Arguments[0]);
        _sb.Append(')');
    }

    private void HandleCollectionContains(MethodCallExpression node)
    {
        _sb.Append("any(");
        switch (node.Arguments.Count)
        {
            case 2:
                Visit(node.Arguments[1]);
                _sb.Append(',');
                Visit(node.Arguments[0]);
                break;
            case 1:
                Visit(node.Arguments[0]);
                _sb.Append(',');
                Visit(node.Object);
                break;
            default:
                throw new NotSupportedException("Collection 'Contains' method is not supported when called with more then 2 arguments.");
        }
        _sb.Append(')');
    }
}