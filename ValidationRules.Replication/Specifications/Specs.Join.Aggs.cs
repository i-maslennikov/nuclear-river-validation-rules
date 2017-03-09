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

                public static Expression<Func<Firm.FirmPosition, AssociatedPositionDto>> WithPrincipalPositions(IQueryable<Firm.FirmAssociatedPosition> requirements, IQueryable<Firm.FirmPosition> principals)
                {
                    Expression<Func<Firm.FirmPosition, AssociatedPositionDto>> expression =
                        associated => new AssociatedPositionDto
                            {
                                Associated = associated,
                                RequirePrincipal = requirements.Any(requirement => requirement.OrderPositionId == associated.OrderPositionId && requirement.ItemPositionId == associated.ItemPositionId),
                                Principals = PrincipalPositions().Compile().Invoke(associated, requirements, principals)
                            };
                    return (Expression<Func<Firm.FirmPosition, AssociatedPositionDto>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                public static Expression<Func<Firm.FirmPosition, IEnumerable<PrincipalPositionDto>>> PrincipalPositions(IQueryable<Firm.FirmAssociatedPosition> requirements, IQueryable<Firm.FirmPosition> principals)
                {
                    Expression<Func<Firm.FirmPosition, IEnumerable<PrincipalPositionDto>>> expression =
                        associated => PrincipalPositions().Compile().Invoke(associated, requirements, principals);
                    return (Expression<Func<Firm.FirmPosition, IEnumerable<PrincipalPositionDto>>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                private static Expression<Func<Firm.FirmPosition, IQueryable<Firm.FirmAssociatedPosition>, IQueryable<Firm.FirmPosition>, IEnumerable<PrincipalPositionDto>>> PrincipalPositions()
                {
                    Expression<Func<Firm.FirmPosition, IQueryable<Firm.FirmAssociatedPosition>, IQueryable<Firm.FirmPosition>, IEnumerable<PrincipalPositionDto>>> expression =
                        (associated, requirements, principals) =>
                            requirements
                                .Where(requirement => requirement.OrderPositionId == associated.OrderPositionId && requirement.ItemPositionId == associated.ItemPositionId)
                                .SelectMany(requirement => principals
                                                .Where(principal => principal.Begin == associated.Begin && principal.FirmId == associated.FirmId)
                                                .Where(principal => principal.ItemPositionId == requirement.PrincipalPositionId && principal.FirmId == requirement.FirmId)
                                                .Where(principal => Scope.CanSee(associated.Scope, principal.Scope))
                                                .Select(principal => new PrincipalPositionDto
                                                {
                                                    Position = principal,
                                                    RequiredMatch = requirement.BindingType == BindingObjectMatch,
                                                    RequiredDifferent = requirement.BindingType == Different,
                                                    IsBindingObjectConditionSatisfied = requirement.BindingType == NoDependency || (MatchedBindingObjects().Compile().Invoke(principal, associated) ? requirement.BindingType == BindingObjectMatch : requirement.BindingType == Different)
                                                }));
                    return (Expression<Func<Firm.FirmPosition, IQueryable<Firm.FirmAssociatedPosition>, IQueryable<Firm.FirmPosition>, IEnumerable<PrincipalPositionDto>>>)new ExpandMethodCallVisitor().Visit(expression);
                }

                /// <summary>
                /// Возвращает выражение выборки запрещённых позиций заказа по принципу совпадения обектов привязки.
                /// </summary>
                public static Expression<Func<Dto<Order.OrderDeniedPosition>, IEnumerable<Dto<Order.OrderPosition>>>> DeniedWithMatchedBindingObject(IQueryable<Dto<Order.OrderPosition>> principals)
                {
                    Expression<Func<Dto<Order.OrderDeniedPosition>, IEnumerable<Dto<Order.OrderPosition>>>> expression =
                        denied => principals.Where(principal => MatchedPeriod<Order.OrderDeniedPosition>().Compile().Invoke(principal, denied))
                                            .Where(principal => Scope.CanSee(denied.Scope, principal.Scope))
                                            .Where(principal => MatchedBindingObjects().Compile().Invoke(principal.Position, denied.Position))
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
                                            .Where(principal => !MatchedBindingObjects().Compile().Invoke(principal.Position, denied.Position))
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
                public static Expression<Func<IBindingObject, IBindingObject, bool>> MatchedBindingObjects()
                {
                    return (position, binding) => (binding.HasNoBinding == position.HasNoBinding) &&
                                                  ((position.Category3Id != null &&
                                                    position.Category3Id == binding.Category3Id &&
                                                    (position.FirmAddressId == null || binding.FirmAddressId == null)) ||
                                                   ((position.Category1Id == null ||
                                                     binding.Category1Id == null ||
                                                     position.Category3Id == binding.Category3Id ||
                                                     position.Category1Id == binding.Category1Id && position.Category3Id == null && binding.Category3Id == null) &&
                                                    position.FirmAddressId == binding.FirmAddressId));
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
                            return _dictionary.TryGetValue(node, out replcement) ? replcement : base.VisitParameter(node);
                        }
                    }
                }

                public class AssociatedPositionDto
                {
                    public Firm.FirmPosition Associated { get; set; }
                    public IEnumerable<PrincipalPositionDto> Principals { get; set; }
                    public bool RequirePrincipal { get; set; }
                }

                public class PrincipalPositionDto
                {
                    public Firm.FirmPosition Position { get; set; }
                    public bool RequiredMatch { get; set; }
                    public bool RequiredDifferent { get; set; }
                    public bool IsBindingObjectConditionSatisfied { get; set; }
                }
            }
        }
    }
}