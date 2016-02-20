# Pipeline phases
Replication in **NuClear River** implemented in a pipeline-based way. The structure of that pipeline is unified and consists of the following phases: _receiving_ message from the transport level, _transforming_, _accumulating_ and _handling_.

### Receiving message from the transport level
```csharp
public interface IMessageReceiver
{
    IReadOnlyList<IMessage> Peek();
    void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages);
}
```

`Peek` method is used to receive messages from the transport, but without deleting them in transport infrastructure to be able to restore them in case of fail.

`Complete` method is used to acknowledge `successfullyProcessedMessages` and delete them from the transport. It also restores `failedProcessedMessages` that gives an ability to handle them in the next loop.

Abstract class `MessageReceiverBase` is here to simplify many things.

### Transforming
```csharp
public interface IMessageTransformer
{
    bool CanTransform(IMessage originalMessage);
    IMessage Transform(IMessage originalMessage);
}
```

`CanTransform` method is used to determine if that instance can deal with the particular message. If there are many transformers, they will be iterated through to find the first who returns `true`.

`Transform` method is used to execute the transformation of messages from a serialized form to the form that is convenient to the later usage.

If no transformation needed, then the Null-pattern can be used. At least one transformer should be configured.

Abstract class `MessageTransformerBase` is here to simplify many things.

### Accumulating
```csharp
public interface IMessageProcessingContextAccumulator
{
    bool CanProcess(IMessage message);
    IAggregatableMessage Process(IMessage message);
}
```

`CanProcess` method is used to determine if that instance can deal with the particular message. If there are many transformers, they will be iterated through to find the first who returns `true`.

`Process` method is used to spread input messages to a one or many buckets. Value of the `Id` property of type `Guid` of returned `IAggregatableMessage` object is used to archieve this. Messages with the same `Id` will be "packed" to the same bucket.

Abstract class `MessageProcessingContextAccumulatorBase` is here to simplify many things.

### Handling
```csharp
public interface IMessageProcessingHandler
{
    IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap);
}
```

`Handle` is used to process messages and mark them as handled or failed. Messages passed to that method are grouped in a one or many buckets by 'Id'.

If you need to start an another one pipeline, method `Handle` is the point where you can generate events and create create commands that should be send futher using transport infrastructure.

Just to highlight, `Complete` method of the `IMessageReceiver` instance should be called here.

### Connecting phases with each other
* You need to implement `IMessageFlowReceiverResolveStrategy` and register that type in DI-container. Implementation should return an instance of type that implements `IMessageReceiver` based on a value of parameter of type `MessageFlowMetadata`.
* You need to implement `IMessageTransformerResolveStrategy` and register that type in DI-container. Implementation should return an instance of type that implements `IMessageTransformer` based on a value of parameter of type `MessageFlowMetadata`.
* You need to connect the message flow with `IMessageProcessingContextAccumulator` implementation and `IMessageProcessingHandler` implementation by flow's identifier using [metadata descriptions](../terms.md). See for an example in [this article](replication-design.md).

> **Note:**
> In fact, there is also some other dependencies of `IMessageFlowProcessor` implementation to make all things work together. For details, see an article about [NuClear.Operations* libraries](dependencies/nuclear-operations-libraries.md) that are used by **NuClear.River**.

### Connecting replication stages (two or more pipelines)
All you need here is to send messages (events or commands) in `IMessageProcessingHandler.Handle` method using the transport level and specify the flow for them. Then, you need to configure **NuClear River** to execute all pipelines and transport-level infrastructure with flows you needed.