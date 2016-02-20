using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB;
using LinqToDB.Data;

using NuClear.Replication.Bulk.API.Storage;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Bulk.API.Replicators
{
    public class UpdatesBulkReplicator<T> : IBulkReplicator where T : class
    {
        private readonly IQuery _source;
        private readonly DataConnection _target;
        private readonly MapSpecification<IQuery, IEnumerable<T>> _mapSpecification;

        public UpdatesBulkReplicator(IQuery source, DataConnection target, MapSpecification<IQuery, IEnumerable<T>> mapSpecification)
        {
            _source = source;
            _target = target;
            _mapSpecification = mapSpecification;
        }

        public void Replicate()
        {
            try
            {
                var sourceQueryable = _mapSpecification.Map(_source);
                var options = new BulkCopyOptions { BulkCopyTimeout = 1800 };

                var temptable = _target.CreateTable<T>($"#{Guid.NewGuid():N}");
                temptable.BulkCopy(options, sourceQueryable);

                var primaryKeys = _target.MappingSchema.GetEntityDescriptor(typeof(T)).Columns.Where(x => x.IsPrimaryKey);
                var keyExpression = CreateMappingExpressionForProperties(primaryKeys.Select(x => x.MemberInfo));

                var datatable = _target.GetTable<T>();
                temptable.Join(datatable, keyExpression, keyExpression, (x, y) => x).Update(datatable, x => x);
            }
            catch (Exception ex)
            {
                var linq2DBSource = _source as LinqToDbQuery;
                throw new Exception($"Can not process entity type {typeof(T).Name}\n{linq2DBSource?.LastQuery}", ex);
            }
        }

        public Expression<Func<T, T>> CreateMappingExpressionForProperties(IEnumerable<MemberInfo> primaryKey)
        {
            // TOuter x => new TOuter { primaryKey = x.primaryKey }

            var parameter = Expression.Parameter(typeof(T));
            var newExpression = Expression.New(typeof(T).GetConstructor(new Type[0]));
            var bindings = primaryKey.Select(property => Expression.Bind(property, Expression.MakeMemberAccess(parameter, property)));
            var init = Expression.MemberInit(newExpression, bindings);

            return Expression.Lambda<Func<T, T>>(init, parameter);
        }
    }
}