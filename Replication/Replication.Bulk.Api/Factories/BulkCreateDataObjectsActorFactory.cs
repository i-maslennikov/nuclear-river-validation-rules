using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.Replication.Bulk.API.Actors;
using NuClear.Replication.Bulk.API.Storage;
using NuClear.Replication.Core.API;

namespace NuClear.Replication.Bulk.API.Factories
{
    public class BulkCreateDataObjectsActorFactory : IDataObjectsActorFactory, IDisposable
    {
        private readonly DataConnection _sourceDataConnection;
        private readonly DataConnection _targetDataConnection;

        private static readonly IReadOnlyDictionary<Type, Type> AccessorTypes =
            (from type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.ExportedTypes)
             from @interface in type.GetInterfaces()
             where @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IStorageBasedDataObjectAccessor<>)
             select new { GenericArgument = @interface.GetGenericArguments()[0], Type = type })
                .ToDictionary(x => x.GenericArgument, x => x.Type);

        public BulkCreateDataObjectsActorFactory(DataConnection sourceDataConnection, DataConnection targetDataConnection)
        {
            _sourceDataConnection = sourceDataConnection;
            _targetDataConnection = targetDataConnection;
        }

        public IActor Create(Type dataObjectType)
        {
            var accessorType = AccessorTypes[dataObjectType];
            var accessorInstance = Activator.CreateInstance(accessorType, new LinqToDbQuery(_sourceDataConnection));
            var actorType = typeof(BulkCreateDataObjectsActor<>).MakeGenericType(dataObjectType);
            return (IActor)Activator.CreateInstance(actorType, accessorInstance, _targetDataConnection);
        }

        public void Dispose()
        {
            _sourceDataConnection.Dispose();
            _targetDataConnection.Dispose();
        }
    }
}