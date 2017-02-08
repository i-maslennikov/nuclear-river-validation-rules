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
                const int BindingObjectMatch = 1;
                const int Different = 3;

                public static Expression<Func<Dto<Order.OrderAssociatedPosition>, IEnumerable<Dto<Order.OrderPosition>>>> RegardlessBindingObject(IQueryable<Dto<Order.OrderPosition>> principals)
                {
                    Expression<Func<Dto<Order.OrderAssociatedPosition>, IEnumerable<Dto<Order.OrderPosition>>>> expression =
                        associated => principals.Where(principal => MatchedPeriod<Order.OrderAssociatedPosition>().Compile().Invoke(principal, associated))
                                                .Where(principal => Scope.CanSee(associated.Scope, principal.Scope))
                                                .Where(principal => principal.Position.ItemPositionId == associated.Position.PrincipalPositionId);
                    return (Expression<Func<Dto<Order.OrderAssociatedPosition>, IEnumerable<Dto<Order.OrderPosition>>>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                public static Expression<Func<Dto<Order.OrderAssociatedPosition>, Dto<Order.OrderPosition>, Dto<Order.OrderPosition, Order.OrderPosition>>> RegardlessBindingObject()
                {
                    Expression<Func<Dto<Order.OrderAssociatedPosition>, Dto<Order.OrderPosition>, Dto<Order.OrderPosition, Order.OrderPosition>>> expression =
                        (associated, principal) => new Dto<Order.OrderPosition, Order.OrderPosition>
                            {
                                FirmId = associated.FirmId,
                                OrganizationUnitId = associated.OrganizationUnitId,
                                Start = associated.Start,
                                Match = principal == null ? Match.NoPosition : MatchedBindingObjects<Order.OrderAssociatedPosition>().Compile().Invoke(principal.Position, associated.Position) ? Match.MatchedBindingObject : Match.DifferentBindingObject,
                                CausePosition = new Order.OrderPosition
                                    {
                                        OrderId = associated.Position.OrderId,
                                        OrderPositionId = associated.Position.CauseOrderPositionId,
                                        PackagePositionId = associated.Position.CausePackagePositionId,
                                        ItemPositionId = associated.Position.CauseItemPositionId,

                                        HasNoBinding = associated.Position.HasNoBinding,
                                        Category1Id = associated.Position.Category1Id,
                                        Category3Id = associated.Position.Category3Id,
                                        FirmAddressId = associated.Position.FirmAddressId,
                                    },
                                RelatedPosition = principal.Position,
                            };
                    return (Expression<Func<Dto<Order.OrderAssociatedPosition>, Dto<Order.OrderPosition>, Dto<Order.OrderPosition, Order.OrderPosition>>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                /// <summary>
                /// Возвращает выражение для сопоставления основных и сопутствующих позиций.
                /// Выражение учитывает все типы сравнения объектов привязки.
                /// Но не учитывает "области видимости" позиций.
                /// </summary>
                public static Expression<Func<Dto<Order.OrderAssociatedPosition>, IEnumerable<Dto<Order.OrderPosition>>>> AvailablePrincipalPosition(IQueryable<Dto<Order.OrderPosition>> principals)
                {
                    Expression<Func<Dto<Order.OrderAssociatedPosition>, IEnumerable<Dto<Order.OrderPosition>>>> expression =
                        associated => principals.Where(principal => MatchedPeriod<Order.OrderAssociatedPosition>().Compile().Invoke(principal, associated))
                                                .Where(principal => associated.Position.BindingType == NoDependency ||
                                                                    associated.Position.BindingType == BindingObjectMatch && MatchedBindingObjects<Order.OrderAssociatedPosition>().Compile().Invoke(principal.Position, associated.Position) ||
                                                                    associated.Position.BindingType == Different && !MatchedBindingObjects<Order.OrderAssociatedPosition>().Compile().Invoke(principal.Position, associated.Position))
                                                .Where(principal => principal.Position.ItemPositionId == associated.Position.PrincipalPositionId &&
                                                                    principal.Position.OrderPositionId != associated.Position.CauseOrderPositionId);
                    return (Expression<Func<Dto<Order.OrderAssociatedPosition>, IEnumerable<Dto<Order.OrderPosition>>>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                /// <summary>
                /// Возвращает выражение выборки запрещённых позиций заказа по принципу совпадения обектов привязки.
                /// </summary>
                public static Expression<Func<Dto<Order.OrderDeniedPosition>, IEnumerable<Dto<Order.OrderPosition>>>> DeniedWithMatchedBindingObject(IQueryable<Dto<Order.OrderPosition>> principals)
                {
                    Expression<Func<Dto<Order.OrderDeniedPosition>, IEnumerable<Dto<Order.OrderPosition>>>> expression =
                        denied => principals.Where(principal => MatchedPeriod<Order.OrderDeniedPosition>().Compile().Invoke(principal, denied))
                                            .Where(principal => Scope.CanSee(denied.Scope, principal.Scope))
                                            .Where(principal => MatchedBindingObjects<Order.OrderDeniedPosition>().Compile().Invoke(principal.Position, denied.Position))
                                            .Where(principal => principal.Position.ItemPositionId == denied.Position.DeniedPositionId &&
                                                                principal.Position.OrderPositionId != denied.Position.CauseOrderPositionId);
                    return (Expression<Func<Dto<Order.OrderDeniedPosition>, IEnumerable<Dto<Order.OrderPosition>>>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                /// <summary>
                /// Возвращает выражение выборки запрещённых позиций заказа по принципу различия обектов привязки.
                /// </summary>
                public static Expression<Func<Dto<Order.OrderDeniedPosition>, IEnumerable<Dto<Order.OrderPosition>>>> DeniedWithDifferentBindingObject(IQueryable<Dto<Order.OrderPosition>> principals)
                {
                    Expression<Func<Dto<Order.OrderDeniedPosition>, IEnumerable<Dto<Order.OrderPosition>>>> expression =
                        denied => principals.Where(principal => MatchedPeriod<Order.OrderDeniedPosition>().Compile().Invoke(principal, denied))
                                            .Where(principal => Scope.CanSee(denied.Scope, principal.Scope))
                                            .Where(principal => !MatchedBindingObjects<Order.OrderDeniedPosition>().Compile().Invoke(principal.Position, denied.Position))
                                            .Where(principal => principal.Position.ItemPositionId == denied.Position.DeniedPositionId &&
                                                                principal.Position.OrderPositionId != denied.Position.CauseOrderPositionId);
                    return (Expression<Func<Dto<Order.OrderDeniedPosition>, IEnumerable<Dto<Order.OrderPosition>>>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                /// <summary>
                /// Возвращает выражение выборки запрещённых позиций заказа без учёта обектов привязки.
                /// </summary>
                public static Expression<Func<Dto<Order.OrderDeniedPosition>, IEnumerable<Dto<Order.OrderPosition>>>> DeniedWithoutConsideringBindingObject(IQueryable<Dto<Order.OrderPosition>> principals)
                {
                    Expression<Func<Dto<Order.OrderDeniedPosition>, IEnumerable<Dto<Order.OrderPosition>>>> expression =
                        denied => principals.Where(principal => MatchedPeriod<Order.OrderDeniedPosition>().Compile().Invoke(principal, denied))
                                            .Where(principal => Scope.CanSee(denied.Scope, principal.Scope))
                                            .Where(principal => principal.Position.ItemPositionId == denied.Position.DeniedPositionId &&
                                                                principal.Position.OrderPositionId != denied.Position.CauseOrderPositionId);
                    return (Expression<Func<Dto<Order.OrderDeniedPosition>, IEnumerable<Dto<Order.OrderPosition>>>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                /// <summary>
                /// Возвращает выражение для пересечения влияющих друг на друга позиций (сопутствующих или запрещённых)
                /// </summary>
                private static Expression<Func<Dto<Order.OrderPosition>, Dto<T>, bool>> MatchedPeriod<T>()
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
                private static Expression<Func<Order.OrderPosition, T, bool>> MatchedBindingObjects<T>()
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