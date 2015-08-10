using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal static class AggregateInfoBuilder
    {
        public static AggregateInfoBuilder<TAggregate> OfType<TAggregate>() where TAggregate : class, ICustomerIntelligenceObject, IIdentifiable
        {
            return new AggregateInfoBuilder<TAggregate>();
        }
    }

    internal class AggregateInfoBuilder<TAggregate> where TAggregate : class, ICustomerIntelligenceObject, IIdentifiable
    {
        private readonly List<IValueObjectInfo> _valueObjects;
        private readonly Func<IEnumerable<long>, MapSpecification<IQuery, IQueryable<TAggregate>>> _mapToTargetSpecProvider =
            ids => new MapSpecification<IQuery, IQueryable<TAggregate>>(q => q.For(new FindSpecification<TAggregate>(x => ids.Contains(x.Id))));
        
        private Func<IEnumerable<long>, MapSpecification<IQuery, IQueryable<TAggregate>>> _mapToSourceSpecProvider;

        public AggregateInfoBuilder()
        {
            _valueObjects = new List<IValueObjectInfo>();
        }

        public IAggregateInfo Build()
        {
            return new AggregateInfo<TAggregate>(_mapToSourceSpecProvider, _mapToTargetSpecProvider, _valueObjects);
        }

        public AggregateInfoBuilder<TAggregate> HasSource(Func<IEnumerable<long>, MapSpecification<IQuery, IQueryable<TAggregate>>> mapToSourceSpecProvider)
        {
            _mapToSourceSpecProvider = mapToSourceSpecProvider;
            return this;
        }

        public AggregateInfoBuilder<TAggregate> HasValueObject<TValueObject>(MapSpecification<IQuery, IQueryable<TValueObject>> queryProvider, Expression<Func<TValueObject, long>> parentIdSelector)
        {
            var queryByParentIds = CreateFilteredQueryProvider(queryProvider, parentIdSelector);
            _valueObjects.Add(new ValueObjectInfo<TValueObject>(queryByParentIds));
            return this;
        }

        private static Func<IQuery, IEnumerable<long>, IQueryable<T>> CreateFilteredQueryProvider<T>(Func<IQuery, IQueryable<T>> queryProvider, Expression<Func<T, long>> idSelector)
        {
            return (query, ids) =>
            {
                var queryable = queryProvider(query);
                var filterExpression = CreateFilterExpression(ids, idSelector);
                return queryable.Where(filterExpression);
            };
        }

        private static Expression<Func<T, long>> CreateKeyAccessor<T>()
        {
            // ���� �������� (TAggregate x) => x.Id, �� � ���� ��������� �������� Id ����� �������� �� � ���� TAggregate, � � ���� IIdentifiable
            // � ����������� ������� ��� ��� �����, �� ��� ����� ����� ���� � linq2db - ��� ���������� ������, ��� ��������� ������ �������� 
            // ������ ���� TAggregate � �� ������ �� �����, ��� ����� IIdentifiable.Id
            var param = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, long>>(Expression.Property(param, "Id"), param);
        }

        private static Expression<Func<T, bool>> CreateFilterExpression<T>(IEnumerable<long> ids, Expression<Func<T, long>> idSelector)
        {
            Expression<Func<T, bool>> example = foo => ids.Contains(0);
            var exampleMethodCall = (MethodCallExpression)example.Body;
            var methodCall = exampleMethodCall.Update(null, new[] { exampleMethodCall.Arguments[0], idSelector.Body });
            return Expression.Lambda<Func<T, bool>>(methodCall, idSelector.Parameters);
        }
    }
}