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
    public sealed class ReplaceDataObjectsInBulkActorFactory : IDataObjectsActorFactory, IDisposable
    {
        private static readonly IReadOnlyDictionary<Type, Type> AccessorTypes =
            (from type in AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic).SelectMany(x => x.ExportedTypes)
             from @interface in type.GetInterfaces()
             where @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IStorageBasedDataObjectAccessor<>)
             select new { GenericArgument = @interface.GetGenericArguments()[0], Type = type })
                .ToDictionary(x => x.GenericArgument, x => x.Type);

        private readonly IDataObjectTypesProvider _dataObjectTypesProvider;
        private readonly Func<DataConnection> _sourceDataConnectionFactory;
        private readonly Func<DataConnection> _targetDataConnectionFactory;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public ReplaceDataObjectsInBulkActorFactory(
            IDataObjectTypesProvider dataObjectTypesProvider,
            Func<DataConnection> sourceDataConnectionFactory,
            Func<DataConnection> targetDataConnectionFactory)
        {
            _dataObjectTypesProvider = dataObjectTypesProvider;
            _sourceDataConnectionFactory = sourceDataConnectionFactory;
            _targetDataConnectionFactory = targetDataConnectionFactory;
        }

        public IReadOnlyCollection<IActor> Create()
        {
            var actors = new List<IActor>();

            foreach (var dataObjectType in _dataObjectTypesProvider.Get<ReplaceDataObjectsInBulkCommand>())
            {
                var sourceDataConnection = _sourceDataConnectionFactory();
                var targetDataConnection = _targetDataConnectionFactory();
                _disposables.Add(sourceDataConnection);
                _disposables.Add(targetDataConnection);

                var accessorType = AccessorTypes[dataObjectType];
                var accessorInstance = Activator.CreateInstance(accessorType, new LinqToDbQuery(sourceDataConnection));
                var actorType = typeof(ReplaceDataObjectsInBulkActor<>).MakeGenericType(dataObjectType);
                var actor = (IActor)Activator.CreateInstance(actorType, accessorInstance, targetDataConnection);
                actors.Add(actor);
            }

            return actors;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}