using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.Replication.Bulk.API.Actors;
using NuClear.Replication.Bulk.API.Commands;
using NuClear.Replication.Bulk.API.Storage;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Actors;
using NuClear.Replication.Core.API.DataObjects;

namespace NuClear.Replication.Bulk.API.Factories
{
    public sealed class ReplaceDataObjectsInBulkActorFactory : IDataObjectsActorFactory
    {
        private static readonly IReadOnlyDictionary<Type, Type> AccessorTypes =
            (from type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.ExportedTypes)
             from @interface in type.GetInterfaces()
             where @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IStorageBasedDataObjectAccessor<>)
             select new { GenericArgument = @interface.GetGenericArguments()[0], Type = type })
                .ToDictionary(x => x.GenericArgument, x => x.Type);

        private readonly IDataObjectTypesProvider _dataObjectTypesProvider;
        private readonly DataConnection _sourceDataConnection;
        private readonly DataConnection _targetDataConnection;

        public ReplaceDataObjectsInBulkActorFactory(
            IDataObjectTypesProvider dataObjectTypesProvider,
            DataConnection sourceDataConnection,
            DataConnection targetDataConnection)
        {
            _dataObjectTypesProvider = dataObjectTypesProvider;
            _sourceDataConnection = sourceDataConnection;
            _targetDataConnection = targetDataConnection;
        }

        public IReadOnlyCollection<IActor> Create()
        {
            var actors = new List<IActor>();

            foreach (var dataObjectType in _dataObjectTypesProvider.Get<ReplaceDataObjectsInBulkCommand>())
            {
                var accessorType = AccessorTypes[dataObjectType];
                var accessorInstance = Activator.CreateInstance(accessorType, new LinqToDbQuery(_sourceDataConnection));
                var actorType = typeof(ReplaceDataObjectsInBulkActor<>).MakeGenericType(dataObjectType);
                var actor = (IActor)Activator.CreateInstance(actorType, accessorInstance, _targetDataConnection);
                actors.Add(actor);
            }

            return actors;
        }
    }
}