using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.SingleCheck.Store;

namespace NuClear.ValidationRules.SingleCheck
{
    public class Pipeline
    {
        private readonly IReadOnlyCollection<Type> _factAccessorTypes;
        private readonly IReadOnlyCollection<Type> _aggregateAccessorTypes;
        private readonly IReadOnlyCollection<Type> _messageAccessorTypes;

        public Pipeline(
            IReadOnlyCollection<Type> factAccessorTypes,
            IReadOnlyCollection<Type> aggregateAccessorTypes,
            IReadOnlyCollection<Type> messageAccessorTypes)
        {
            _factAccessorTypes = factAccessorTypes;
            _aggregateAccessorTypes = aggregateAccessorTypes;
            _messageAccessorTypes = messageAccessorTypes;
        }

        public void Execute(IQuery source, IStore pipelineStore, IQuery pipelineQuery, IStore target)
        {
            foreach (var type in _factAccessorTypes)
            {
                var proxy = AccessorProxy.Create(type, source, pipelineStore);
                proxy.Process();
            }

            foreach (var type in _aggregateAccessorTypes)
            {
                var proxy = AccessorProxy.Create(type, pipelineQuery, pipelineStore);
                proxy.Process();
            }

            foreach (var type in _messageAccessorTypes)
            {
                var proxy = AccessorProxy.Create(type, pipelineQuery, target);
                proxy.Process();
            }
        }

        private abstract class AccessorProxy
        {
            public static AccessorProxy Create(Type accessorType, IQuery query, IStore dataConnection)
            {
                var dataObjectType = GetAccessorDataObject(accessorType);
                var helperType = typeof(AccessorProxyImpl<>).MakeGenericType(dataObjectType);
                var accessor = Activator.CreateInstance(accessorType, query);
                return (AccessorProxy)Activator.CreateInstance(helperType, accessor, dataConnection);
            }

            private static Type GetAccessorDataObject(Type type)
                => type.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStorageBasedDataObjectAccessor<>)).GetGenericArguments().Single();

            public abstract void Process();

            private class AccessorProxyImpl<TDataType> : AccessorProxy
                where TDataType : class
            {
                private readonly IStorageBasedDataObjectAccessor<TDataType> _accessor;
                private readonly IStore _repository;

                public AccessorProxyImpl(IStorageBasedDataObjectAccessor<TDataType> accessor, IStore repository)
                {
                    _accessor = accessor;
                    _repository = repository;
                }

                public override void Process()
                {
                    var data = _accessor.GetSource().Execute();
                    try
                    {
                        if (data.Any())
                        {
                            _repository.AddRange(data);
                        }
                    }
                    catch (Exception)
                    {
                        Debugger.Break();
                    }
                }
            }
        }
    }
}