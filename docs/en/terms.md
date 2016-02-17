Here are definitions for terms that actively used in docs

#### Source system
The system where commands are executing. That leads to changes in a specific source [bounded context](http://martinfowler.com/bliki/BoundedContext.html) and the data in it's storage

#### [Querying](design-overview/querying-design.md)
High-level **NuClear River** platform component responsible for serving client's queries to Read Model

#### [Replication](design-overview/replication-design.md) 
High-level **NuClear River** platform component responsible for external events processing to sync the data in Read Model's storage with the data of the source system

#### Metadata descriptions
Descriptions on DSL based on C# for configuration. This is the way to customize **NuClear River** behaviour for a specific [bounded context](http://martinfowler.com/bliki/BoundedContext.html). Here is the sample:
```csharp
HierarchyMetadata aggregateConstructionMetadataRoot =
    HierarchyMetadata
        .Config
        .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ReplicationMetadataIdentity>(ReplicationMetadataName.Aggregates))
        .Childs(AggregateMetadata<Firm>
                    .Config
                    .HasSource(Specs.Map.Facts.ToCI.Firms)
                    .HasValueObject(Specs.Map.Facts.ToCI.FirmActivities, Specs.Find.CI.FirmActivities)
                    .HasValueObject(Specs.Map.Facts.ToCI.FirmBalances, Specs.Find.CI.FirmBalances)
                    .HasValueObject(Specs.Map.Facts.ToCI.FirmCategories, Specs.Find.CI.FirmCategories)
                    .HasValueObject(Specs.Map.Facts.ToCI.FirmTerritories, Specs.Find.CI.FirmTerritories),
                AggregateMetadata<Client>
                    .Config
                    .HasSource(Specs.Map.Facts.ToCI.Clients)
                    .HasValueObject(Specs.Map.Facts.ToCI.ClientContacts, Specs.Find.CI.ClientContacts),
                AggregateMetadata<Project>
                    .Config
                    .HasSource(Specs.Map.Facts.ToCI.Projects)
                    .HasValueObject(Specs.Map.Facts.ToCI.ProjectCategories, Specs.Find.CI.ProjectCategories),
                AggregateMetadata<Territory>
                    .Config
                    .HasSource(Specs.Map.Facts.ToCI.Territories),
                AggregateMetadata<CategoryGroup>
                    .Config
                    .HasSource(Specs.Map.Facts.ToCI.CategoryGroups));
```

#### Events (UseCases) processing
UseCases is an another name for [Domain Events](http://martinfowler.com/eaaDev/DomainEvent.html). These events originated in a source system as a result of commands execution. **NuClear River** processes these events  throught ETL-pipeline to build data in Read Model's storage. Events processing is a part of a [Replication](#Replication)

#### Specification pattern
[Specifications](https://en.wikipedia.org/wiki/Specification_pattern) makes it possible to use expressions as objects. It gives high level code reuse possibility and makes **NuClear River** customizable. See [NuClear Aggregates Layer project docs](https://github.com/2gis/nuclear-aggregates-layer) for details

#### Facts storage
Storage used in Replication component that store data from source system. Usually, it use the same storage type and similar data schema as source system. The data stored here is used to track changes in the source system for further aggregates costruction. It's a subset of the data of the source system that needed for specific bounded context only

#### Aggregates (storage)
Term used for objects from bounded context's description in sense of [Domain-Driven Design, DDD](https://en.wikipedia.org/wiki/Domain-driven_design). [Aggregates](http://martinfowler.com/bliki/DDD_Aggregate.html) is a cluster of domain objects that can be treated as a single unit. _Aggregates storage_ is the storage within Replication component to store aggregates

#### Invariant
An Assertion about some design element that must be true at all times, except during specifically transient situations such as the middle of the execution of a method, or the middle of an uncommitted database transaction