# Details on Replication component

As it was mentioned in [high-level overview article](README.md), **Replication** component's control flow consists at least of two parts/stages - _primary_ and _final_, so it's a pipeline-based engine. 

### Primary stage

On the _primary_ stage you have a set of events from the [source system][terms] that needed to be processed. Based on your knowledge about business cases that executed in source system, you can distinguish events that you're interested in for particular bounded context. All other events can be safely skipped.

The main goal at _primary_ stage is to sync [facts storage][terms] with storage of the source system. So, processing input events leads to changes in facts storage, but not only. At this stage we also need to generate commands to be executed on the next stage. 

Now it's time to deep dive into implementation detais. Let's see what we need to do to customize the _primary_ stage to support a new bounded context. We assume that the domain analysis is already carried out.

First, we need to describe facts storage with [metadata descriptions][terms]:

```csharp
HierarchyMetadata factsReplicationMetadataRoot =
    HierarchyMetadata
        .Config
        .Id.Is(Id.For<ReplicationMetadataIdentity>(ReplicationMetadataName.Facts))
        .Childs(
            FactMetadata<Account>
                .Config
                .HasSource(Specs.Map.Erm.ToFacts.Accounts)
                .HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByAccount),

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

Here we have hierarchical descriptions of objects in the facts storage. For every such object we define [specification][terms] (e.g. `HasSource(Specs.Map.Erm.ToFacts.Accounts)` method call) that should be used to get a data from the source system storage and one or more [specifications][terms] (e.g. `HasDependentAggregate<CI::Firm>(Specs.Map.Facts.ToFirmAggregate.ByAccount)` method call) that should be used to determine types and identities of [aggregates][terms] that we need to deal with on the next stage. Using this information and the information about changes in facts storage, we generate commands to initialize/recalculate/destroy a corresponding aggregate.

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
        get { return "Example input event flow"; }
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

Responsibility of this class is to see on input events and make decisions about them. It is the place where event can be skipped.

Next, create a class derived from `IMessageProcessingHandler`:

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
At the end, create a class derived from `MetadataSourceBase<MetadataMessageFlowsIdentity>` to describe how to "connect" all parts together:

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

That's almost everything you need to customize the _primary_ stage of **Replication** component. Now, you'll be able to consume events from source system and sync the stage of facts storage of **NuClear River**.

Here is more details on how everything works under the hood. We have a simple abstraction `IFactsReplicator` in `NuClear.Replication.Core.API.Facts` namespace:

```csharp
public interface IFactsReplicator
{
    IReadOnlyCollection<IOperation> Replicate(IEnumerable<FactOperation> operations);
}
```

It takes source system's event descriptors (`IEnumerable<FactOperation>`) as input and a have a set of commands (`IReadOnlyCollection<IOperation>`) as output. It should be used in `IMessageProcessingHandler` implementation to perform actual handling. 

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

        foreach (var operation in operations)
        {   
            var factType = operation.Key.FactType;

            // Take metadata descriptions for a particular fact type
            IMetadataElement factMetadata;
            var metadataId = ReplicationMetadataIdentity.Instance.Id.WithRelative(new Uri(string.Format("Facts/{0}", factType.Name), UriKind.Relative));
            if (!_metadataProvider.TryGetMetadata(metadataId, out factMetadata))
            {
                throw new NotSupportedException(string.Format("The fact of type '{0}' is not supported.", factType));
            }

            // Create facts processor
            var processor = _factProcessorFactory.Create(factMetadata);

            foreach (var batch in factIds.CreateBatches(_replicationSettings.ReplicationBatchSize))
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

The actual impementation of `IFactsReplicator` is [here](https://github.com/2gis/nuclear-river/blob/master/Replication/Replication.Core/Facts/FactsReplicator.cs)

### Final stage

The design of the _final_ stage is very similar. But the key difference here - we have a set of **commands** as the input to that stage. Commands cannot be skipped, we need to process them in any case - change an aggregate stage or throw an exception.

[terms]: ../terms.md
[nuclear-operations-libraries]: ../dependencies/nuclear-operations-libraries.md