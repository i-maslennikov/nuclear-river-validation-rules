using System.Linq.Expressions;
using System.Text;

namespace NuClear.ValidationRules.SingleCheck.Optimization
{
    internal sealed class DebuggingVisitor : ExpressionVisitor
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private int _indent = 0;

        public string Text => _sb.ToString();

        public override Expression Visit(Expression node)
        {
            if (node != null)
            {
                _sb.AppendLine();
                _sb.Append(new string(' ', _indent * 2));
                _sb.Append($"{{ {node.NodeType} {node.GetType().Name}");
                _indent++;
                var result = base.Visit(node);
                _sb.Append($"}}");
                _indent--;
                return result;
            }

            return base.Visit(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            _sb.Append($" {node.Method.Name}");
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _sb.Append($" {node.Type.Name} {node.Value}");
            return base.VisitConstant(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _sb.Append($" {node.Member.Name}");
            return base.VisitMember(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            _sb.Append($" {node.Name}");
            return base.VisitParameter(node);
        }
    }
}