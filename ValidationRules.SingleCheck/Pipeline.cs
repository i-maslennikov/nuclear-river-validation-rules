using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

using LinqToDB.Data;

using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Telemetry.Probing;
using NuClear.ValidationRules.SingleCheck.Store;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Model.Messages;

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
        private readonly IPipelineStrategy _strategy;

        public Pipeline(IReadOnlyCollection<Type> factAccessorTypes, IReadOnlyCollection<Type> aggregateAccessorTypes, IReadOnlyCollection<Type> messageAccessorTypes, SchemaManager schemaManager)
        {
            _factAccessorTypes = factAccessorTypes;
            _aggregateAccessorTypes = aggregateAccessorTypes;
            _messageAccessorTypes = messageAccessorTypes;
            _schemaManager = schemaManager;
            _lockManager = new LockManager();
            _strategy = new OverOptimizedPipelineStrategy(); // new OptimizedPipelineStrategy();
        }

        public IReadOnlyCollection<Version.ValidationResult> Execute(long orderId, ICheckModeDescriptor checkModeDescriptor)
        {
            // todo: можно использовать checkModeDescriptor для дальнейшей оптимизации
            var optimization = new Optimizer();
            Func<IStore, IStore> wrap = store => new OptimizerStore(optimization, store);

            using (Probe.Create("Execute"))
            using (var erm = new HashSetStoreFactory())
            using (var store = new PersistentTableStoreFactory(_lockManager, _schemaManager))
            using (var messages = new HashSetStoreFactory())
            {
                IReadOnlyCollection<Replicator> factReplicators;
                IReadOnlyCollection<Replicator> aggregateReplicators;
                IReadOnlyCollection<Replicator> messageReplicators;

                using (Probe.Create("Initialization"))
                {
                    factReplicators = CreateReplicators(_factAccessorTypes, erm.CreateQuery(), wrap(store.CreateStore()));
                    aggregateReplicators = CreateReplicators(_aggregateAccessorTypes, store.CreateQuery(), wrap(store.CreateStore()));
                    messageReplicators = CreateReplicators(_messageAccessorTypes, store.CreateQuery(), wrap(messages.CreateStore()))
                        .Where(x => x.DataObjectType == typeof(Version.ValidationResult) && checkModeDescriptor.Rules.Contains(x.Rule)).ToArray();

                    var predicates = factReplicators.Concat(aggregateReplicators).Concat(messageReplicators).SelectMany(x => x.DependencyPredicates);
                    optimization.PrepareToUse(predicates.Distinct());
                }

                using (Probe.Create("Erm -> Erm slice"))
                    ReadErmSlice(orderId, wrap(erm.CreateStore()));

                using (Probe.Create("Erm slice -> Facts"))
                    _strategy.ProcessFacts(factReplicators, aggregateReplicators, messageReplicators, optimization);

                using (Probe.Create("Facts -> Aggregates"))
                    _strategy.ProcessAggregates(aggregateReplicators, messageReplicators, optimization);

                using (Probe.Create("Aggregates -> Messages"))
                    _strategy.ProcessMessages(messageReplicators, optimization);

                using (Probe.Create("Query result"))
                    return messages.CreateQuery().For<Version.ValidationResult>().Where(x => x.OrderId == orderId && checkModeDescriptor.Rules.Contains((MessageTypeCode)x.MessageType)).ToArray();
            }
        }

        private static void ReadErmSlice(long orderId, IStore store)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            using (var connection = new DataConnection("Erm").AddMappingSchema(Schema.Erm))
            {
                ErmDataLoader.Load(orderId, connection, store);
            }
        }

        private static IReadOnlyCollection<Replicator> CreateReplicators(IReadOnlyCollection<Type> accessorTypes, IQuery source, IStore target)
            => accessorTypes
                .Select(x => new { Accessor = x, DataObject = GetAccessorDataObject(x) })
                .Select(x => Replicator.Create(x.Accessor, x.DataObject, source, target))
                .ToArray();

        private static Type GetAccessorDataObject(Type type)
            => type.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStorageBasedDataObjectAccessor<>)).GetGenericArguments().Single();

        private sealed class Optimizer
        {
            private readonly IDictionary<LambdaExpression, int> _processedTypes = new Dictionary<LambdaExpression, int>();
            private Dictionary<Type, HashSet<Tuple<LambdaExpression, Delegate>>> _predicates;

            public void Processed<T>(IReadOnlyCollection<T> entities)
            {
                HashSet<Tuple<LambdaExpression, Delegate>> predicates;
                if (!_predicates.TryGetValue(typeof(T), out predicates))
                {
                    predicates = new HashSet<Tuple<LambdaExpression, Delegate>>();
                }

                foreach (var p in predicates)
                {
                    var count = 0;
                    _processedTypes.TryGetValue(p.Item1, out count);
                    _processedTypes[p.Item1] = count + entities.Where((Func<T, bool>)p.Item2).Count();
                }
            }

            public bool IsKnownEmpty(Type type, LambdaExpression expression)
            {
                int count;
                return _processedTypes.TryGetValue(expression, out count) && count == 0;
            }

            public void PrepareToUse(IEnumerable<LambdaExpression> predicates)
            {
                _predicates = predicates
                    .GroupBy(x => x.Parameters.Single().Type, x => Tuple.Create(x, x.Compile()))
                    .ToDictionary(x => x.Key, x => new HashSet<Tuple<LambdaExpression, Delegate>>(x));
            }
        }

        private sealed class OptimizerStore : IStore
        {
            private readonly Optimizer _optimizer;
            private readonly IStore _store;

            public OptimizerStore(Optimizer optimizer, IStore store)
            {
                _optimizer = optimizer;
                _store = store;
            }

            public void Add<T>(T entity) where T : class
            {
                AddRange(new[] { entity });
            }

            public void AddRange<T>(IReadOnlyCollection<T> entities) where T : class
            {
                _optimizer.Processed(entities);

                if (entities.Count > 1)
                {
                    _store.AddRange(entities);
                }
                else if (entities.Count > 0)
                {
                    _store.Add(entities.First());
                }
            }
        }

        private interface IPipelineStrategy
        {
            void ProcessMessages(IReadOnlyCollection<Replicator> messageReplicators, Optimizer optimizer);
            void ProcessAggregates(IReadOnlyCollection<Replicator> aggregateReplicators, IReadOnlyCollection<Replicator> messageReplicators, Optimizer optimizer);
            void ProcessFacts(IReadOnlyCollection<Replicator> factReplicators, IReadOnlyCollection<Replicator> aggregateReplicators, IReadOnlyCollection<Replicator> messageReplicators, Optimizer optimizer);
        }

        private sealed class NotOptimizedPipelineStrategy : IPipelineStrategy
        {
            public void ProcessMessages(IReadOnlyCollection<Replicator> messageReplicators, Optimizer optimizer)
            {
                foreach (var r in messageReplicators)
                    r.Process(false);
            }

            public void ProcessAggregates(IReadOnlyCollection<Replicator> aggregateReplicators, IReadOnlyCollection<Replicator> messageReplicators, Optimizer optimizer)
            {
                foreach (var r in aggregateReplicators)
                    r.Process(false);
            }

            public void ProcessFacts(IReadOnlyCollection<Replicator> factReplicators, IReadOnlyCollection<Replicator> aggregateReplicators, IReadOnlyCollection<Replicator> messageReplicators, Optimizer optimizer)
            {
                foreach (var r in factReplicators)
                    r.Process(false);
            }
        }

        private sealed class OptimizedPipelineStrategy : IPipelineStrategy
        {
            public void ProcessMessages(IReadOnlyCollection<Replicator> messageReplicators, Optimizer optimizer)
            {
                foreach (var r in messageReplicators)
                    r.Process(r.SkipCheckFunction.Invoke(optimizer.IsKnownEmpty));
            }

            public void ProcessAggregates(IReadOnlyCollection<Replicator> aggregateReplicators, IReadOnlyCollection<Replicator> messageReplicators, Optimizer optimizer)
            {
                foreach (var r in aggregateReplicators)
                    r.Process(r.SkipCheckFunction.Invoke(optimizer.IsKnownEmpty));
            }

            public void ProcessFacts(IReadOnlyCollection<Replicator> factReplicators, IReadOnlyCollection<Replicator> aggregateReplicators, IReadOnlyCollection<Replicator> messageReplicators, Optimizer optimizer)
            {
                foreach (var r in factReplicators)
                    r.Process(r.SkipCheckFunction.Invoke(optimizer.IsKnownEmpty));
            }
        }

        private sealed class OverOptimizedPipelineStrategy : IPipelineStrategy
        {
            public void ProcessMessages(IReadOnlyCollection<Replicator> messageReplicators, Optimizer optimizer)
            {
                var skipMessageReplicators = messageReplicators.Where(x => x.SkipCheckFunction.Invoke(optimizer.IsKnownEmpty)).ToList();

                foreach (var r in messageReplicators.Except(skipMessageReplicators))
                {
                    r.Process(false);
                }
            }

            public void ProcessAggregates(IReadOnlyCollection<Replicator> aggregateReplicators, IReadOnlyCollection<Replicator> messageReplicators, Optimizer optimizer)
            {
                var skipAggregateReplicators = aggregateReplicators.Where(x => x.SkipCheckFunction.Invoke(optimizer.IsKnownEmpty)).ToList();
                foreach (var r in skipAggregateReplicators)
                    r.Process(true);

                var skipMessageReplicators = messageReplicators.Where(x => x.SkipCheckFunction.Invoke(optimizer.IsKnownEmpty)).ToList();

                var actualAggregateTypes = new HashSet<Type>(messageReplicators.Except(skipMessageReplicators).SelectMany(x => x.Dependencies));
                var inactualAggregateReplicators = aggregateReplicators.Where(x => !actualAggregateTypes.Contains(x.DataObjectType)).ToList();

                foreach (var r in aggregateReplicators.Except(skipAggregateReplicators).Except(inactualAggregateReplicators))
                {
                    r.Process(false);
                }
            }

            public void ProcessFacts(IReadOnlyCollection<Replicator> factReplicators, IReadOnlyCollection<Replicator> aggregateReplicators, IReadOnlyCollection<Replicator> messageReplicators, Optimizer optimizer)
            {
                var skipFactReplicators = factReplicators.Where(x => x.SkipCheckFunction.Invoke(optimizer.IsKnownEmpty)).ToList();
                foreach (var r in skipFactReplicators)
                    r.Process(true);

                var skipAggregateReplicators = aggregateReplicators.Where(x => x.SkipCheckFunction.Invoke(optimizer.IsKnownEmpty)).ToList();
                foreach (var r in skipAggregateReplicators)
                    r.Process(true);

                var skipMessageReplicators = messageReplicators.Where(x => x.SkipCheckFunction.Invoke(optimizer.IsKnownEmpty)).ToList();

                var actualAggregateTypes = new HashSet<Type>(messageReplicators.Except(skipMessageReplicators).SelectMany(x => x.Dependencies));
                var inactualAggregateReplicators = aggregateReplicators.Where(x => !actualAggregateTypes.Contains(x.DataObjectType)).ToList();

                var actualFactTypes = new HashSet<Type>(aggregateReplicators.Except(skipAggregateReplicators).Except(inactualAggregateReplicators).SelectMany(x => x.Dependencies));
                var inactualFactReplicators = factReplicators.Where(x => !actualFactTypes.Contains(x.DataObjectType)).ToList();

                foreach (var r in factReplicators.Except(skipFactReplicators).Except(inactualFactReplicators))
                {
                    r.Process(false);
                }
            }
        }
    }
}