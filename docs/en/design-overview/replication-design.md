# Details on Replication component

As it was mentioned in [high-level overview article](README.md), **Replication** component's control flow consists at least of two parts/stages - _primary_ and _final_, so it's a pipeline-based engine. 

### Primary stage

On the _primary_ stage you have a set of events from the [source system][terms] that needed to be processed. Based on your knowledge about business cases that executed in a source system, you can distinguish events that you're interested in for particular bounded context. All other events can be safely skipped.

The main goal at _primary_ stage is to sync [facts storage][terms] with storage of the source system. So, processing of input events leads to changes in facts storage, but not only. At this stage we also need to generate commands to be executed on the next stage. 

Now it's time to deep dive into implementation detais. Let's see what we need to do to customize the _primary_ stage to support a new bounded context. We assume that the domain analysis is already carried out.

First, we need to describe facts storage with [metadata descriptions][terms]:

```csharp
HierarchyMetadata factsReplicationMetadataRoot =
    HierarchyMetadata
        .Config
        .Id.Is(Id.For<ReplicationMetadataIdentity>(ReplicationMetadataName.Facts))
        .Childs(
            FactMetadata<Territory>
                .Config
                .HasSource(Specs.Map.Erm.ToFacts.Territories)
                .HasMatchedAggregate<CI::Territory>(),

            FactMetadata<Category>
                .Config
                .HasSource(Specs.Map.Erm.ToFacts.Categories)
                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByCategory),

            FactMetadata<Client>
                .Config
                .HasSource(Specs.Map.Erm.ToFacts.Clients)
                .HasMatchedAggregate<CI::Client>()
                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByClient),

            FactMetadata<Firm>
                .Config
                .HasSource(Specs.Map.Erm.ToFacts.Firms)
                .LeadsToStatisticsCalculation(Specs.Map.Facts.ToStatistics.ByFirm)
                .HasMatchedAggregate<CI::Firm>()
                .HasDependentAggregate<CI::Client>(Specs.Map.Facts.ToClientAggregate.ByFirm));
```

Here we have hierarchical descriptions of objects in the facts storage. For every such object we define [specification][terms] (e.g. `HasSource(Specs.Map.Erm.ToFacts.Accounts)` method call) that should be used to get a data from the source system storage, and one or more [specifications][terms] (e.g. `HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByAccount)` method call) that should be used to determine types and identities of [aggregates][terms] that we need to deal with on the next stage. Using this information and the information about changes in the facts storage, we generate commands to initialize/recalculate/destroy a corresponding aggregate.

Next, we need to configure an interaction with source system. **NuClear River** uses very pluggable library from the [NuClear.Operations*][nuclear-operations-libraries] set for that purpose. 

So, create a class derived from `MessageFlowBase<T>`. It needed to configure the flow which will be used for events consuming.

```csharp
public sealed class InputEventFlow : MessageFlowBase<InputEventFlow>
{
    public override Guid Id
    {
        get { return new Guid("8a4238d6-1c94-47f6-a83c-b8ed01f309a4"); }
    }

    public override string Description
    {
        get { return "Events flow"; }
    }
}
```

Create a class derived from `MessageProcessingContextAccumulatorBase<TMessageFlow, TMessage, TProcessingResult>`:

```csharp
public sealed class InputEventAccumulator : 
    MessageProcessingContextAccumulatorBase<InputEventFlow, 
                                            TrackedUseCase, 
                                            OperationAggregatableMessage<FactOperation>>
{
    protected override OperationAggregatableMessage<FactOperation> Process(TrackedUseCase message)
    {
        // Process event from source system here
        return new OperationAggregatableMessage<FactOperation> 
        {
            TargetFlow = MessageFlow,
            // Replace empty sequence with descriptors of events that should be processed further
            Operations = Enumerable.Empty<OperationDescriptor>(),
            OperationTime = message.Context.Finished.UtcDateTime,
        };
    }
}
```

Responsibility of this class is to get events from a transport level (e.g. deserialize), analyze input events and make decisions about them. It is the place where inessential events can be skipped.

Next, an implementation of `IMessageProcessingHandler` should be created to handle accumulated events. But as end users we don't need to implement this interface by our own, **NuClear River** already have an appropriate implementation. Let's take a look at the following code, just to understand the whole idea: 

```csharp
public sealed class InputEventHandler : IMessageProcessingHandler
{
    public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
    {
        try
        {
            // Handle input event's descriptors here
            // Typical scenario is to access to source system's data (throught the event context or 
            // by grab it directly from the storage), sync a state of facts storage and generate commands
            // to be executed on the next stage
            return processingResultsMap.Keys.Select(
                bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded());
        }
        catch (Exception ex)
        {
            return processingResultsMap.Keys.Select(
                bucketId => MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex));
        }
    }
}
```

That's almost everything you need to customize the _primary_ stage of **Replication** component. Now, you'll be able to consume events from source system and sync the stage of facts storage of **NuClear River**.

Here is more details on how everything works under the hood. We have a simple abstraction `IFactsReplicator` in `NuClear.Replication.Core.API.Facts` namespace:

```csharp
public interface IFactsReplicator
{
    IReadOnlyCollection<IOperation> Replicate(IEnumerable<FactOperation> operations);
}
```

It takes source system's event descriptors (of type `IEnumerable<FactOperation>`) as input and have a set of commands (of type `IReadOnlyCollection<IOperation>`) as output. It is used in `IMessageProcessingHandler` implementation to perform actual handling. 

Implementation of `IFactsReplicator` is in `NuClear.Replication.Core.Facts` namespace and looks like this:

```csharp
public class FactsReplicator : IFactsReplicator
{
    private readonly IMetadataProvider _metadataProvider;
    private readonly IFactProcessorFactory _factProcessorFactory;

    public FactsReplicator(
        IMetadataProvider metadataProvider,
        IFactProcessorFactory factProcessorFactory)
    {
        _metadataProvider = metadataProvider;
        _factProcessorFactory = factProcessorFactory;
    }

    public IReadOnlyCollection<IOperation> Replicate(IEnumerable<FactOperation> operations)
    {
        var result = Enumerable.Empty<IOperation>();

        var slices = operations.GroupBy(operation => new { operation.FactType });
        foreach (var slice in slices)
        {   
            var factType = slice.Key.FactType;

            // Take metadata descriptions for a particular fact type
            IMetadataElement factMetadata;
            var metadataId = ReplicationMetadataIdentity.Instance.Id.WithRelative(new Uri(string.Format("Facts/{0}", factType.Name), UriKind.Relative));
            if (!_metadataProvider.TryGetMetadata(metadataId, out factMetadata))
            {
                throw new NotSupportedException(string.Format("The fact of type '{0}' is not supported.", factType));
            }

            // Create facts processor based on metadata descriptions
            var processor = _factProcessorFactory.Create(factMetadata);

            var factIds = slice.Select(x => x.FactId);
            foreach (var batch in factIds.CreateBatches())
            {
                // Sync the state of facts storage
                var aggregateOperations = processor.ApplyChanges(batch);

                result = result.Concat(aggregateOperations);
            }
        }

        // Pass commands to the next stage
        return result.ToArray();
    }
}
```

The actual impementation of `IFactsReplicator` is [here](https://github.com/2gis/nuclear-river/blob/master/Replication/Replication.Core/Facts/FactsReplicator.cs).

### Final stage

The design of the _final_ stage is very similar. But the key difference here - we have a set of **commands** as the input on that stage. Commands cannot be skipped, we need to process them in any case - change an aggregate state or throw an exception.

To configure _final_ stage, we need to start from metadata descriptions to define aggregates:

```csharp
HierarchyMetadata aggregateConstructionMetadataRoot =
    HierarchyMetadata
        .Config
        .Id.Is(Id.For<ReplicationMetadataIdentity>(ReplicationMetadataName.Aggregates))
        .Childs(AggregateMetadata<Firm>
                    .Config
                    .HasSource(Specs.Map.Facts.ToCI.Firms)
                    .HasValueObject(Specs.Map.Facts.ToCI.FirmTerritories, Specs.Find.CI.FirmTerritories),

                AggregateMetadata<Client>
                    .Config
                    .HasSource(Specs.Map.Facts.ToCI.Clients)
                    .HasValueObject(Specs.Map.Facts.ToCI.ClientContacts, Specs.Find.CI.ClientContacts),

                AggregateMetadata<Territory>
                    .Config
                    .HasSource(Specs.Map.Facts.ToCI.Territories);
```

Here again we have hierarchical descriptions, but at this time, descriptions of aggregates. For every such aggregate we define [specification][terms] (e.g. `HasSource(Specs.Map.Facts.ToCI.Firms)` method call) that should be used to get a data from the facts storage, and one or more [specifications][terms] (e.g. `HasValueObject(Specs.Map.Facts.ToCI.FirmTerritories, Specs.Find.CI.FirmTerritories)` method call) that should be used to rebuild aggregate structure, it's value objects.

Next, we need to do exactly the same we did at the _primary_ stage.

Create a class derived from `MessageFlowBase<T>`. It needed to configure the flow which will be used for _commands_ consuming.

```csharp
public sealed class AggregatesFlow : MessageFlowBase<AggregatesFlow>
{
    public override Guid Id
    {
        get { return new Guid("96F17B1A-4CC8-40CC-9A92-16D87733C39F"); }
    }

    public override string Description
    {
        get { return "Commands flow"; }
    }
}
```

Create a class derived from `MessageProcessingContextAccumulatorBase<TMessageFlow, TMessage, TProcessingResult>`:

```csharp
public sealed class AggregateOperationAccumulator<TMessageFlow> :
    MessageProcessingContextAccumulatorBase<TMessageFlow, 
                                            PerformedOperationsFinalProcessingMessage, 
                                            OperationAggregatableMessage<AggregateOperation>>
    where TMessageFlow : class, IMessageFlow, new()
{
    private readonly AggregateOperationSerializer _serializer;

    public AggregateOperationAccumulator(AggregateOperationSerializer serializer)
    {
        _serializer = serializer;
    }

    protected override OperationAggregatableMessage<AggregateOperation> Process(
        PerformedOperationsFinalProcessingMessage message)
    {
        var operations = message.FinalProcessings.Select(x => _serializer.Deserialize(x)).ToArray();
        var oldestOperation = message.FinalProcessings.Min(x => x.CreatedOn);

        return new OperationAggregatableMessage<AggregateOperation>
        {
            TargetFlow = MessageFlow,
            Operations = operations,
            OperationTime = oldestOperation,
        };
    }
}
```

Again, we don't have to create an implemention of `IMessageProcessingHandler`, because **NuClear River** already have it:

```csharp
public sealed class AggregateOperationAggregatableMessageHandler : IMessageProcessingHandler
{
    private readonly IAggregatesConstructor _aggregatesConstructor;
    
    public AggregateOperationAggregatableMessageHandler(IAggregatesConstructor aggregatesConstructor)
    {
        _aggregatesConstructor = aggregatesConstructor;
    }

    public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
    {
        return processingResultsMap.Select(pair => Handle(pair.Key, pair.Value));
    }

    private StageResult Handle(Guid bucketId, IEnumerable<IAggregatableMessage> messages)
    {
        try
        {
            foreach (var message in messages.OfType<OperationAggregatableMessage<AggregateOperation>>())
            {
                _aggregatesConstructor.Construct(message.Operations);
            }

            return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
        }
        catch (Exception ex)
        {
            _tracer.Error(ex, "Error when calculating aggregates");
            return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
        }
    }
}
```

At the end, create a class derived from `MetadataSourceBase<MetadataMessageFlowsIdentity>` to describe how to "connect" all parts together, including _primary_ and _final_ stages:

```csharp
public sealed class InputEventFlowMetadataSource : MetadataSourceBase<MetadataMessageFlowsIdentity>
{
    private static readonly HierarchyMetadata MetadataRoot =
        PerformedOperations.Flows
                           .Primary(
                               MessageFlowMetadata.Config.For<InputEventFlow>()
                                                  .Accumulator<InputEventAccumulator>()
                                                  .Handler<InputEventHandler>()
                                                  .To.Primary().Flow<InputEventFlow>().Connect()
                                                  .To.Final().Flow<AggregatesFlow>().Connect()
                                                  .To.Final().Flow<StatisticsFlow>().Connect()
                           .Final(
                               MessageFlowMetadata.Config.For<AggregatesFlow>()
                                                  .Accumulator<AggregateOperationAccumulator<AggregatesFlow>>()
                                                  .Handler<AggregateOperationAggregatableMessageHandler>()
                                                  .To.Final().Flow<AggregatesFlow>().Connect(),

                               MessageFlowMetadata.Config.For<StatisticsFlow>()
                                                  .Accumulator<StatisticsOperationAccumulator<StatisticsFlow>>()
                                                  .Handler<StatisticsAggregatableMessageHandler>()
                                                  .To.Final().Flow<StatisticsFlow>().Connect());

    private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

    public PerformedOperationsMessageFlowsMetadataSource()
    {
        _metadata = new Dictionary<Uri, IMetadataElement> { { MetadataRoot.Identity.Id, MetadataRoot } };
    }

    public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
    {
        get { return _metadata; }
    }
}
```

The last thing needed to be covered here is `IAggregatesConstructor` abstraction:

```csharp
public interface IAggregatesConstructor
{
    void Construct(IEnumerable<AggregateOperation> operations);
}
```

It takes commands generated at the _primary_ stage (of type `IEnumerable<AggregateOperation> operations`) as input and have nothing as output, so it just changes aggregates storage state. It is used in `IMessageProcessingHandler` implementation to perform actual handling. 

The implementation of `IAggregatesConstructor` looks like this:

```csharp
 public sealed class AggregatesConstructor : IAggregatesConstructor
{
    private readonly IMetadataProvider _metadataProvider;
    private readonly IAggregateProcessorFactory _aggregateProcessorFactory;

    public AggregatesConstructor(
        IMetadataProvider metadataProvider, 
        IAggregateProcessorFactory aggregateProcessorFactory)
    {
        _metadataProvider = metadataProvider;
        _aggregateProcessorFactory = aggregateProcessorFactory;
    }

    public void Construct(IEnumerable<AggregateOperation> operations)
    {
        var slices = operations.GroupBy(x => new { Operation = x.GetType(), x.AggregateType })
                               .OrderByDescending(x => x.Key.Operation, new AggregateOperationPriorityComparer());

        foreach (var slice in slices)
        {
            var operation = slice.Key.Operation;
            var aggregateType = slice.Key.AggregateType;

            IMetadataElement aggregateMetadata;
            var metadataId = ReplicationMetadataIdentity.Instance.Id.WithRelative(new Uri($"Aggregates/{aggregateType.Name}", UriKind.Relative));
            if (!_metadataProvider.TryGetMetadata(metadataId, out aggregateMetadata))
            {
                throw new NotSupportedException($"The aggregate of type '{aggregateType}' is not supported.");
            }

            var aggregateIds = slice.Select(x => x.AggregateId).Distinct().ToArray();
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                          new TransactionOptions
                                                          {
                                                                IsolationLevel = IsolationLevel.ReadCommitted, 
                                                                Timeout = TimeSpan.Zero 
                                                          }))
            {
                using (Probe.Create("ETL2 Transforming", aggregateType.Name))
                {
                    var processor = _aggregateProcessorFactory.Create(aggregateMetadata);

                    if (operation == typeof(InitializeAggregate))
                    {
                        processor.Initialize(aggregateIds);
                    }
                    else if (operation == typeof(RecalculateAggregate))
                    {
                        processor.Recalculate(aggregateIds);
                    }
                    else if (operation == typeof(DestroyAggregate))
                    {
                        processor.Destroy(aggregateIds);
                    }
                    else
                    {
                        throw new InvalidOperationException($"The command of type {operation.Name} is not supported");
                    }
                }

                transaction.Complete();
            }
        }
    }
}
```

[terms]: ../terms.md
[nuclear-operations-libraries]: ../dependencies/nuclear-operations-libraries.md