using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NuClear.Storage.API.Readings;

namespace NuClear.ValidationRules.SingleCheck.Optimization
{
    internal sealed class SkipCheckVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _check;
        private readonly HashSet<LambdaExpression> _dependencyPredicates;
        private Expression _predicate;

        public SkipCheckVisitor()
        {
            _check = Expression.Parameter(typeof(Func<Type, LambdaExpression, bool>), "func");
            _dependencyPredicates = new HashSet<LambdaExpression>();
        }

        public ParameterExpression Parameter => _check;

        public IReadOnlyCollection<LambdaExpression> DependencyPredicates => _dependencyPredicates;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            IQuery query;
            if (node.Method.Name == nameof(query.For) && node.Method.DeclaringType == typeof(IQuery))
            {
                var type = node.Method.GetGenericArguments().Single();
                return GenerateEmptyCheckCall(type, _predicate);
            }

            if (node.Method.Name == nameof(Queryable.DefaultIfEmpty) && node.Method.DeclaringType == typeof(Queryable))
            {
                return Expression.Constant(false);
            }

            if (node.Method.Name == nameof(Queryable.SelectMany) && node.Method.DeclaringType == typeof(Queryable))
            {
                return Expression.Or(base.Visit(node.Arguments[0]), base.Visit(node.Arguments[1]));
            }

            if (node.Method.Name == nameof(Queryable.Union) && node.Method.DeclaringType == typeof(Queryable))
            {
                return Expression.And(base.Visit(node.Arguments[0]), base.Visit(node.Arguments[1]));
            }

            if (node.Method.Name == nameof(Queryable.Concat) && node.Method.DeclaringType == typeof(Queryable))
            {
                return Expression.And(base.Visit(node.Arguments[0]), base.Visit(node.Arguments[1]));
            }

            if (node.Method.Name == nameof(Queryable.Where))
            {
                _predicate = node.Arguments[1];
                var result = base.Visit(node.Arguments[0]);
                _predicate = null;
                return result;
            }

            if (node.Method.Name == nameof(Queryable.Select) && node.Method.DeclaringType == typeof(Queryable))
            {
                return base.Visit(node.Arguments[0]);
            }

            if (node.Method.Name == nameof(Queryable.GroupBy) && node.Method.DeclaringType == typeof(Queryable))
            {
                return base.Visit(node.Arguments[0]);
            }

            if (node.Method.Name == nameof(Queryable.Distinct) && node.Method.DeclaringType == typeof(Queryable))
            {
                return base.Visit(node.Arguments[0]);
            }

            if (node.Method.Name == nameof(Queryable.OrderBy) && node.Method.DeclaringType == typeof(Queryable))
            {
                return base.Visit(node.Arguments[0]);
            }

            if (node.Method.Name == nameof(Queryable.Join) && node.Method.DeclaringType == typeof(Queryable))
            {
                return Expression.Or(base.Visit(node.Arguments[0]), base.Visit(node.Arguments[1]));
            }

            if (node.Method.Name == nameof(Queryable.Take) && node.Method.DeclaringType == typeof(Queryable))
            {
                return base.Visit(node.Arguments[0]);
            }

            throw new ArgumentException($"unsupported method {node.Method.Name}");
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var q = Expression.Lambda<Func<object>>(node).Compile().Invoke() as IQueryable;
            return base.Visit(q.Expression);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (IsQueryable(node.Type))
            {
                var type = node.Type.GetGenericArguments().Single();
                return GenerateEmptyCheckCall(type, _predicate);
            }

            return Expression.Constant(false);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return base.Visit(node.Body);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return base.Visit(node.Operand);
        }

        private Expression GenerateEmptyCheckCall(Type type, Expression predicate)
        {
            var filterPredicate = UnwrapSimpleFilterPredicate(type, predicate) ?? Expression.Lambda(Expression.Constant(true), Expression.Parameter(type));
            _dependencyPredicates.Add(filterPredicate);
            return Expression.Call(_check, "Invoke", new Type[0], Expression.Constant(type), filterPredicate);
        }

        private LambdaExpression UnwrapSimpleFilterPredicate(Type type, Expression predicate)
        {
            var quote = predicate as UnaryExpression;
            var lambda = quote?.Operand as LambdaExpression;
            if (lambda == null || lambda.Parameters.Single().Type != type)
            {
                return null;
            }

            var v = new PredicateVisitor(lambda.Parameters.Single());
            v.Visit(lambda.Body);
            if (!v.IsSimple)
            {
                return null;
            }

            return lambda;
        }

        private static bool IsQueryable(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            var args = type.GetGenericArguments();
            if (args.Length != 1)
            {
                return false;
            }

            var qt = typeof(IQueryable<>).MakeGenericType(args.Single());
            return qt.IsAssignableFrom(type);
        }
    }
}