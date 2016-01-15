Here are definitions for terms that actively used in docs

#### Source system
The system where commands are executing. That leads to changes in a specific source [bounded context](http://martinfowler.com/bliki/BoundedContext.html) and the data in it's storage

#### [Querying](desing-overview/querying-design.md)
High-level NuClear River platform component responsible for serving client's queries to Read Model

#### [Replication](desing-overview/replication-design.md) 
High-level NuClear River platform component responsible for external events processing to sync the data in Read Model's storage with the data of the origin source system

#### Metadata descriptions
Usage of DSL based on C# for configuration descriptions. This is the way to customize NuClear River behaviour for a specific [bounded context](http://martinfowler.com/bliki/BoundedContext.html). Here is the sample:
```c#
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
UseCases is an another name for [Domain Events](http://martinfowler.com/eaaDev/DomainEvent.html). These events originated in a source system as a result of commands execution. NuClear River processes these events  throught ETL-pipeline to build data in Read Model's storage. Events processing is a part of a [Replication](#Replication)

#### Specification pattern
[Specifications](https://en.wikipedia.org/wiki/Specification_pattern) makes it possible to use expressions as objects. It gives high level code reuse possibility and makes NuClear River customizable. See [NuClear Aggregates Layer project docs](https://github.com/2gis/nuclear-aggregates-layer) for details