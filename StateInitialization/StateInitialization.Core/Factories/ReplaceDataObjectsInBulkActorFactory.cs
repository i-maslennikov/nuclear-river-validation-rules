using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.StateInitialization.Core.Actors;
using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Storage;

namespace NuClear.StateInitialization.Core.Factories
{
    public sealed class ReplaceDataObjectsInBulkActorFactory : IDataObjectsActorFactory
    {
        private static readonly IReadOnlyDictionary<Type, Type> AccessorTypes =
            (from type in AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic).SelectMany(x => x.ExportedTypes)
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