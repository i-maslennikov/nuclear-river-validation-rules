using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB;
using LinqToDB.Data;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka
{
    /// <summary>
    /// Аналог BulkInsertDataObjectsActor для IMemoryBasedDataObjectAccessor
    /// </summary>
    public sealed class BulkInsertInMemoryDataObjectsActor<TDataObject> : IActor
        where TDataObject : class
    {
        private static readonly Type DataObjectType = typeof(TDataObject);

        // из-за закрытого дизайна FindSpecification, заточенного на использование в Nuclear.DAL,
        // приходится использовать reflection чтобы добраться до _приватного_ свойства Predicate
        private static readonly PropertyInfo PredicateInfo = typeof(FindSpecification<TDataObject>).GetProperty("Predicate", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly IMemoryBasedDataObjectAccessor<TDataObject> _dataObjectAccessor;
        private readonly BulkCopyOptions _bulkCopyOptions;
        private readonly ITable<TDataObject> _table;

        public BulkInsertInMemoryDataObjectsActor(IMemoryBasedDataObjectAccessor<TDataObject> dataObjectAccessor,
                                                  DataConnection dataConnection,
                                                  BulkCopyOptions bulkCopyOptions)
        {
            _dataObjectAccessor = dataObjectAccessor;
            _bulkCopyOptions = bulkCopyOptions;
            _table = dataConnection.GetTable<TDataObject>();
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var command = commands.OfType<BulkInsertInMemoryDataObjectsCommand>()
                                  .SingleOrDefault(x => x.DataObjectType == DataObjectType);
            if (command != null)
            {
                ExecuteBulkReplace(command.Dtos);
            }

            return Array.Empty<IEvent>();
        }

        private void ExecuteBulkReplace(IEnumerable<object> sourceDtos)
        {
            var replaceDataObjectCommand = new ReplaceDataObjectCommand(DataObjectType, sourceDtos);

            var findSpecification = _dataObjectAccessor.GetFindSpecification(replaceDataObjectCommand);
            var predicate = (Expression<Func<TDataObject, bool>>)PredicateInfo.GetValue(findSpecification);
            _table.Delete<TDataObject>(predicate);

            var dataObjects = _dataObjectAccessor.GetDataObjects(replaceDataObjectCommand);

            try
            {
                _table.BulkCopy<TDataObject>(_bulkCopyOptions, dataObjects);
            }
            catch (Exception ex)
            {
                throw new DataException($"Error occurred while bulk replacing data for data object of type {DataObjectType.Name} using {_dataObjectAccessor.GetType().Name}{Environment.NewLine}", ex);
            }
        }
    }
}