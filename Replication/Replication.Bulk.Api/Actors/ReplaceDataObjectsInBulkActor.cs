using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;
using LinqToDB.Data;

using NuClear.Replication.Bulk.API.Commands;
using NuClear.Replication.Core.API.Actors;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Bulk.API.Actors
{
    public sealed class ReplaceDataObjectsInBulkActor<TDataObject> : IActor where TDataObject : class
    {
        private readonly IQueryable<TDataObject> _source;
        private readonly DataConnection _target;

        public ReplaceDataObjectsInBulkActor(IStorageBasedDataObjectAccessor<TDataObject> dataObjectAccessor, DataConnection target)
        {
            _source = dataObjectAccessor.GetSource();
            _target = target;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var casted = commands.OfType<ReplaceDataObjectsInBulkCommand>();
            if (casted.Count() != 1)
            {
                return Array.Empty<IEvent>();
            }

            try
            {
                var options = new BulkCopyOptions { BulkCopyTimeout = 1800 };
                _target.GetTable<TDataObject>().Delete();
                _target.BulkCopy(options, _source);

                return Array.Empty<IEvent>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Can not process entity type {typeof(TDataObject).Name}\n{_target.LastQuery}", ex);
            }
        }
    }
}