using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Telemetry.Probing;
using NuClear.ValidationRules.SingleCheck.Store;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.SingleCheck
{
    public sealed class Pipeline
    {
        private readonly IReadOnlyCollection<Type> _factAccessorTypes;
        private readonly IReadOnlyCollection<Type> _aggregateAccessorTypes;
        private readonly IReadOnlyCollection<Type> _messageAccessorTypes;
        private readonly SchemaManager _schemaManager; // todo: убрать, некрасиво
        private readonly LockManager _lockManager;

        public Pipeline(IReadOnlyCollection<Type> factAccessorTypes, IReadOnlyCollection<Type> aggregateAccessorTypes, IReadOnlyCollection<Type> messageAccessorTypes, SchemaManager schemaManager)
        {
            _factAccessorTypes = factAccessorTypes;
            _aggregateAccessorTypes = aggregateAccessorTypes;
            _messageAccessorTypes = messageAccessorTypes;
            _schemaManager = schemaManager;
            _lockManager = new LockManager();
        }

        public IReadOnlyCollection<Version.ValidationResult> Execute(long orderId)
        {
            using (Probe.Create("Execute"))
            using (var erm = new ErmStoreFactory(orderId))
            using (var store = new PersistentTableStoreFactory(_lockManager, _schemaManager))
            using (var messages = new HashSetStoreFactory())
            {
                var ermQuery = erm.CreateQuery();
                using (Probe.Create("Facts"))
                    Replicate(ermQuery, store.CreateStore(), _factAccessorTypes);
                using (Probe.Create("Aggregates"))
                    Replicate(store.CreateQuery(), store.CreateStore(), _aggregateAccessorTypes);
                using (Probe.Create("Messages"))
                    Replicate(store.CreateQuery(), messages.CreateStore(), _messageAccessorTypes);

                return messages.CreateQuery().For<Version.ValidationResult>().Where(x => x.OrderId == orderId).ToArray();
            }
        }

        private void Replicate(IQuery source, IStore target, IReadOnlyCollection<Type> accessorTypes)
        {
            foreach (var type in accessorTypes)
            {
                using (Probe.Create(type.FullName))
                {
                    var proxy = AccessorProxy.Create(type, source, target);
                    proxy.Process();
                }
            }
        }

        private abstract class AccessorProxy
        {
            public static AccessorProxy Create(Type accessorType, IQuery source, IStore target)
            {
                var dataObjectType = GetAccessorDataObject(accessorType);
                var helperType = typeof(AccessorProxyImpl<>).MakeGenericType(dataObjectType);
                var accessor = Activator.CreateInstance(accessorType, source);
                return (AccessorProxy)Activator.CreateInstance(helperType, accessor, target);
            }

            private static Type GetAccessorDataObject(Type type)
                => type.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStorageBasedDataObjectAccessor<>)).GetGenericArguments().Single();

            public abstract void Process();

            private class AccessorProxyImpl<TDataType> : AccessorProxy
                where TDataType : class
            {

                private readonly IStorageBasedDataObjectAccessor<TDataType> _accessor;
                private readonly IStore _target;

                public AccessorProxyImpl(IStorageBasedDataObjectAccessor<TDataType> accessor, IStore target)
                {
                    _accessor = accessor;
                    _target = target;
                }

                public override void Process()
                {
                    var query = _accessor.GetSource();

                    var data = query.Execute(_accessor.GetType().Name);

                    if (data.Length > 1)
                    {
                        _target.AddRange(data);
                    }
                    else if (data.Length > 0)
                    {
                        _target.Add(data[0]);
                    }
                }
            }
        }
    }
}