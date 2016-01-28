using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;
using NuClear.CustomerIntelligence.Domain.EntityTypes;
using NuClear.CustomerIntelligence.OperationsProcessing.Contexts;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Identities.Telemetry;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary
{
    /// <summary>
    /// Стратегия выполняет фильтрацию операций, приехавших в TUC, и преобразование этих операций в операции над фактами.
    /// </summary>
    public sealed class ImportFactsFromErmAccumulator : MessageProcessingContextAccumulatorBase<ImportFactsFromErmFlow, TrackedUseCase, OperationAggregatableMessage<FactOperation>>
    {
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly IEntityTypeMappingRegistry<FactsSubDomain> _registry;

        public ImportFactsFromErmAccumulator(ITracer tracer, ITelemetryPublisher telemetryPublisher, IEntityTypeMappingRegistry<FactsSubDomain> registry)
        {
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
            _registry = registry;
        }

        protected override OperationAggregatableMessage<FactOperation> Process(TrackedUseCase message)
        {
            _tracer.DebugFormat("Processing TUC {0}", message.Id);

            var receivedOperationCount = message.Operations.Sum(x => x.AffectedEntities.Changes.Sum(y => y.Value.Sum(z => z.Value.Count)));
            _telemetryPublisher.Publish<ErmReceivedOperationCountIdentity>(receivedOperationCount);

            var filteredOperations = Filter(message);
            var factOperations = Convert(filteredOperations).ToArray();

            _telemetryPublisher.Publish<ErmEnqueuedOperationCountIdentity>(factOperations.Length);

            return new OperationAggregatableMessage<FactOperation>
            {
                TargetFlow = MessageFlow,
                Operations = factOperations,
                OperationTime = message.Context.Finished.UtcDateTime,
            };
        }

        private static IEnumerable<OperationDescriptor> Filter(TrackedUseCase message)
        {
            var operations = (IEnumerable<OperationDescriptor>)message.Operations;

            var disallowedIds = new HashSet<Guid>();
            var disallowedOperations = operations.Where(x => OperationIdentityMetadata.DisallowedOperationIdentities.Contains(x.OperationIdentity));
            foreach (var disallowedOperation in disallowedOperations)
            {
                disallowedIds.Add(disallowedOperation.Id);
                disallowedIds.UnionWith(message.GetNestedOperations(disallowedOperation.Id).Select(x => x.Id));
            }

            if (disallowedIds.Any())
            {
                operations = operations.Where(x => !disallowedIds.Contains(x.Id));
            }

            return operations;
        }

        private IEnumerable<FactOperation> Convert(IEnumerable<OperationDescriptor> operations)
        {
            var factOperations = operations
                .SelectMany(x =>
                            {

                                var tuples = x.AffectedEntities.Changes
                                              .Select(y =>
                                                      {
                                                          var mappedKey = ErmToFactsTypeMap.MapErmToFacts(y.Key);

                                                          Type entityType;
                                                          var parsed = _registry.TryGetEntityType(mappedKey, out entityType);
                                                          return Tuple.Create(parsed, entityType, y);
                                                      })
                                              .Where(y => y.Item1).ToArray();

                                if (tuples.Any())
                                {
                                    if (!OperationIdentityMetadata.AllowedOperationIdentities.Contains(x.OperationIdentity))
                                    {
                                        var entitySet = new EntitySet(tuples.Select(y => y.Item3.Key).Distinct().ToArray());
                                        _tracer.WarnFormat("Received well-known entities '{0}' frow unknown ERM operation '{1}'", entitySet, x.OperationIdentity);
                                    }
                                }

                                return tuples;
                            })
                .GroupBy(x => x.Item2, x => x.Item3.Value.Keys)
                .SelectMany(x => x.SelectMany(y => y).Distinct().Select(y => new FactOperation(x.Key, y)));

            return factOperations;
        }

        private static class ErmToFactsTypeMap
        {
            private static readonly Dictionary<IEntityType, IEntityType> ErmToFactsTypeMapping
                = new[]
                  {
                      CreateTypeMapping<EntityTypeAppointment, EntityTypeActivity>(),
                      CreateTypeMapping<EntityTypePhonecall, EntityTypeActivity>(),
                      CreateTypeMapping<EntityTypeTask, EntityTypeActivity>(),
                      CreateTypeMapping<EntityTypeLetter, EntityTypeActivity>(),
                  }.ToDictionary(pair => pair.Key, pair => pair.Value);

            public static IEntityType MapErmToFacts(IEntityType entityType)
            {
                IEntityType mappedEntityType;
                if (ErmToFactsTypeMapping.TryGetValue(entityType, out mappedEntityType))
                {
                    return mappedEntityType;
                }

                return entityType;
            }

            private static KeyValuePair<IEntityType, IEntityType> CreateTypeMapping<TFrom, TTo>()
                where TFrom : IdentityBase<TFrom>, IEntityType, new()
                where TTo : IdentityBase<TTo>, IEntityType, new()
            {
                return new KeyValuePair<IEntityType, IEntityType>(IdentityBase<TFrom>.Instance, IdentityBase<TTo>.Instance);
            }
        }
    }
}
