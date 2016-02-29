using System;
using System.Collections.Generic;

using Microsoft.Practices.Unity;

using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.Facts;
using NuClear.River.Common.Metadata.Features;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    internal class UnityFactDependencyProcessorFactory : IFactDependencyProcessorFactory
    {
        private static readonly IDictionary<Type, Type> KnownFeatureProcessors = new Dictionary<Type, Type>
            {
                { typeof(DirectlyDependentEntityFeature<>), typeof(DirectlyDependentEntityFeatureProcessor<>) },
                { typeof(IndirectlyDependentEntityFeature<,>), typeof(IndirectlyDependentEntityFeatureProcessor<,>) },
            };

        private readonly IUnityContainer _unityContainer;

        public UnityFactDependencyProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IFactDependencyProcessor Create(IFactDependencyFeature metadata)
        {
            Type processorType;
            if (!KnownFeatureProcessors.TryGetValue(metadata.GetType().GetGenericTypeDefinition(), out processorType))
            {
                throw new ArgumentException($"Feature of type {metadata.GetType().Name} has no known processor", nameof(metadata));
            }

            processorType = processorType.MakeGenericType(metadata.GetType().GetGenericArguments());
            var metadataOverride = new DependencyOverride(metadata.GetType(), metadata);
            var processor = _unityContainer.Resolve(processorType, metadataOverride);
            return (IFactDependencyProcessor)processor;
        }
    }
}