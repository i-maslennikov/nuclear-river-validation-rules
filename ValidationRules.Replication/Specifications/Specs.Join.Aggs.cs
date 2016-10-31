using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NuClear.ValidationRules.Replication.PriceRules.Validation.Dto;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Join
        {
            public static class Aggs
            {
                const int NoDependency = 2;
                const int Match = 1;
                const int Different = 3;

                /// <summary>
                /// Возвращает выражение для сопоставления основных и сопутствующих позиций.
                /// Выражение учитывает все типы сравнения объектов привязки.
                /// Но не учитывает "области видимости" позиций.
                /// </summary>
                public static Expression<Func<Dto<OrderAssociatedPosition>, IEnumerable<Dto<OrderPosition>>>> AvailablePrincipalPosition(IQueryable<Dto<OrderPosition>> principals)
                {
                    Expression<Func<Dto<OrderAssociatedPosition>, IEnumerable<Dto<OrderPosition>>>> expression =
                        associated => principals.Where(principal => MatchedPeriod<OrderAssociatedPosition>().Compile().Invoke(principal, associated))
                                                .Where(principal => associated.Position.BindingType == NoDependency ||
                                                                    associated.Position.BindingType == Match && MatchedBindingObjects<OrderAssociatedPosition>().Compile().Invoke(principal.Position, associated.Position) ||
                                                                    associated.Position.BindingType == Different && !MatchedBindingObjects<OrderAssociatedPosition>().Compile().Invoke(principal.Position, associated.Position))
                                                .Where(principal => principal.Position.ItemPositionId == associated.Position.PrincipalPositionId &&
                                                                    principal.Position.OrderPositionId != associated.Position.CauseOrderPositionId);
                    return (Expression<Func<Dto<OrderAssociatedPosition>, IEnumerable<Dto<OrderPosition>>>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                /// <summary>
                /// Возвращает выражение выборки основных позиций заказа по принципу совпадения обектов привязки.
                /// </summary>
                public static Expression<Func<Dto<OrderAssociatedPosition>, IEnumerable<Dto<OrderPosition>>>> WithMatchedBindingObject(IQueryable<Dto<OrderPosition>> principals)
                {
                    Expression<Func<Dto<OrderAssociatedPosition>, IEnumerable<Dto<OrderPosition>>>> expression =
                        associated => principals.Where(principal => MatchedPeriod<OrderAssociatedPosition>().Compile().Invoke(principal, associated))
                                                .Where(principal => Scope.CanSee(associated.Scope, principal.Scope))
                                                .Where(principal => MatchedBindingObjects<OrderAssociatedPosition>().Compile().Invoke(principal.Position, associated.Position))
                                                .Where(principal => principal.Position.ItemPositionId == associated.Position.PrincipalPositionId &&
                                                                    principal.Position.OrderPositionId != associated.Position.CauseOrderPositionId);
                    return (Expression<Func<Dto<OrderAssociatedPosition>, IEnumerable<Dto<OrderPosition>>>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                /// <summary>
                /// Возвращает выражение выборки запрещённых позиций заказа по принципу совпадения обектов привязки.
                /// </summary>
                public static Expression<Func<Dto<OrderDeniedPosition>, IEnumerable<Dto<OrderPosition>>>> DeniedWithMatchedBindingObject(IQueryable<Dto<OrderPosition>> principals)
                {
                    Expression<Func<Dto<OrderDeniedPosition>, IEnumerable<Dto<OrderPosition>>>> expression =
                        denied => principals.Where(principal => MatchedPeriod<OrderDeniedPosition>().Compile().Invoke(principal, denied))
                                            .Where(principal => Scope.CanSee(denied.Scope, principal.Scope))
                                            .Where(principal => MatchedBindingObjects<OrderDeniedPosition>().Compile().Invoke(principal.Position, denied.Position))
                                            .Where(principal => principal.Position.ItemPositionId == denied.Position.DeniedPositionId &&
                                                                principal.Position.OrderPositionId != denied.Position.CauseOrderPositionId);
                    return (Expression<Func<Dto<OrderDeniedPosition>, IEnumerable<Dto<OrderPosition>>>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                /// <summary>
                /// Возвращает выражение выборки запрещённых позиций заказа по принципу различия обектов привязки.
                /// </summary>
                public static Expression<Func<Dto<OrderDeniedPosition>, IEnumerable<Dto<OrderPosition>>>> DeniedWithDifferentBindingObject(IQueryable<Dto<OrderPosition>> principals)
                {
                    Expression<Func<Dto<OrderDeniedPosition>, IEnumerable<Dto<OrderPosition>>>> expression =
                        denied => principals.Where(principal => MatchedPeriod<OrderDeniedPosition>().Compile().Invoke(principal, denied))
                                            .Where(principal => Scope.CanSee(denied.Scope, principal.Scope))
                                            .Where(principal => !MatchedBindingObjects<OrderDeniedPosition>().Compile().Invoke(principal.Position, denied.Position))
                                            .Where(principal => principal.Position.ItemPositionId == denied.Position.DeniedPositionId &&
                                                                principal.Position.OrderPositionId != denied.Position.CauseOrderPositionId);
                    return (Expression<Func<Dto<OrderDeniedPosition>, IEnumerable<Dto<OrderPosition>>>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                /// <summary>
                /// Возвращает выражение выборки запрещённых позиций заказа без учёта обектов привязки.
                /// </summary>
                public static Expression<Func<Dto<OrderDeniedPosition>, IEnumerable<Dto<OrderPosition>>>> DeniedWithoutConsideringBindingObject(IQueryable<Dto<OrderPosition>> principals)
                {
                    Expression<Func<Dto<OrderDeniedPosition>, IEnumerable<Dto<OrderPosition>>>> expression =
                        denied => principals.Where(principal => MatchedPeriod<OrderDeniedPosition>().Compile().Invoke(principal, denied))
                                            .Where(principal => Scope.CanSee(denied.Scope, principal.Scope))
                                            .Where(principal => principal.Position.ItemPositionId == denied.Position.DeniedPositionId &&
                                                                principal.Position.OrderPositionId != denied.Position.CauseOrderPositionId);
                    return (Expression<Func<Dto<OrderDeniedPosition>, IEnumerable<Dto<OrderPosition>>>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                /// <summary>
                /// Возвращает выражение для пересечения влияющих друг на друга позиций (сопутствующих или запрещённых)
                /// </summary>
                public static Expression<Func<Dto<OrderPosition>, Dto<T>, bool>> MatchedPeriod<T>()
                {
                    return (principal, dto) => principal.FirmId == dto.FirmId &&
                                               principal.Start == dto.Start &&
                                               principal.OrganizationUnitId == dto.OrganizationUnitId;
                }

                /// <summary>
                /// Возвращает выражение для сравнения объектов привязки.
                /// Выражение пытается реализивать таблицу соответствий, описанную в документации:
                /// https://github.com/2gis/nuclear-river/blob/feature/validation-rules/docs/ru/validation-rules/compare-linking-objects.md
                /// Выражение достаточно не тривиальное и используется многократно, поэтому и создан <see cref="ExpandMethodCallVisitor"/>
                /// </summary>
                public static Expression<Func<OrderPosition, T, bool>> MatchedBindingObjects<T>()
                    where T: IBindingObject
                {
                    return (position, binding) => (binding.HasNoBinding == position.HasNoBinding) &&
                                                  ((binding.Category1Id == position.Category1Id) &&
                                                   (binding.Category3Id == position.Category3Id || binding.Category3Id == null || position.Category3Id == null) &&
                                                   (binding.FirmAddressId == position.FirmAddressId || binding.FirmAddressId == null || position.FirmAddressId == null) ||
                                                   (binding.Category1Id == position.Category1Id || binding.Category1Id == null || position.Category1Id == null) &&
                                                   (binding.Category3Id == null || position.Category3Id == null) &&
                                                   (binding.FirmAddressId == position.FirmAddressId));
                }

                /// <summary>
                /// Позволяет заменить вызов метода, возвращающего выражение (с последующей компиляцией и вызовом, как того требует синтаксис)
                /// на собственно это результат вызова, что позволяет переиспользовать куски выражений при построении запросов.
                /// </summary>
                private class ExpandMethodCallVisitor : ExpressionVisitor
                {
                    protected override Expression VisitMethodCall(MethodCallExpression node)
                    {
                        if (node.Method.Name == nameof(Func<object>.Invoke))
                        {
                            var compileInvocation = (MethodCallExpression)node.Object;
                            var expressionFactoryInvocation = (MethodCallExpression)compileInvocation.Object;
                            return ExpandExpression(expressionFactoryInvocation.Method, node.Arguments);
                        }

                        return base.VisitMethodCall(node);
                    }

                    private Expression ExpandExpression(MethodInfo method, ReadOnlyCollection<Expression> arguments)
                    {
                        if (!method.IsStatic || method.GetParameters().Any())
                        {
                            throw new ArgumentException($"Method {method.Name} has to be static parameterless to be expanded", nameof(method));
                        }

                        var constructedExpression = (LambdaExpression)method.Invoke(null, new object[0]);
                        var replacementDictionary = constructedExpression.Parameters.Zip(arguments, (parameter, argument) => new { parameter, argument })
                            .ToDictionary(x => x.parameter, x => x.argument);
                        return new ReplaceParameterVisitor(replacementDictionary).Visit(constructedExpression.Body);
                    }

                    /// <summary>
                    /// Позволяет заменить обращения к параметрам в выражении на некие другие обращения.
                    /// </summary>
                    private class ReplaceParameterVisitor : ExpressionVisitor
                    {
                        private readonly IDictionary<ParameterExpression, Expression> _dictionary;

                        public ReplaceParameterVisitor(IDictionary<ParameterExpression, Expression> dictionary)
                        {
                            _dictionary = dictionary;
                        }

                        protected override Expression VisitParameter(ParameterExpression node)
                        {
                            Expression replcement;
                            return _dictionary.TryGetValue(node, out replcement) ? replcement : base.Visit(node);
                        }
                    }
                }
            }
        }
    }
}