using System;
using System.Collections.Generic;

using LinqToDB;
using LinqToDB.Data;

using NuClear.Replication.Bulk.API.Storage;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Bulk.API.Replicators
{
	public sealed class InsertsBulkReplicator<T> : IBulkReplicator where T : class
    {
        private readonly IQuery _source;
        private readonly DataConnection _target;
	    private readonly MapSpecification<IQuery, IEnumerable<T>> _mapSpecification;

	    public InsertsBulkReplicator(IQuery source, DataConnection target, MapSpecification<IQuery, IEnumerable<T>> mapSpecification)
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
                _target.GetTable<T>().Delete();
                _target.BulkCopy(options, sourceQueryable);
            }
            catch (Exception ex)
            {
                var linq2DBSource = _source as LinqToDbQuery;
                throw new Exception($"Can not process entity type {typeof(T).Name}\n{linq2DBSource?.LastQuery}", ex);
            }
        }
    }
}