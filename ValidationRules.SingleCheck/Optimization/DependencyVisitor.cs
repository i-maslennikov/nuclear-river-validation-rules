using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Storage.API.Readings;

namespace NuClear.ValidationRules.SingleCheck.Optimization
{
    internal sealed class DependencyVisitor : ExpressionVisitor
    {
        private readonly HashSet<Type> _dependencies = new HashSet<Type>();

        public IReadOnlyCollection<Type> Dependencies => _dependencies;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "For" && node.Object != null && typeof(IQuery).IsAssignableFrom(node.Object.Type))
            {
                var type = node.Method.GetGenericArguments().Single();
                _dependencies.Add(type);
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (typeof(IQueryable).IsAssignableFrom(node.Type))
            {
                var q = Expression.Lambda<Func<object>>(node).Compile().Invoke() as IQueryable;
                base.Visit(q.Expression);
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (typeof(IQueryable).IsAssignableFrom(node.Type))
            {
                var type = node.Type.GetGenericArguments().Single();
                _dependencies.Add(type);
            }

            return base.VisitConstant(node);
        }
    }
}