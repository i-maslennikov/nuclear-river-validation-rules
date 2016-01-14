![](../media/nuclear-logo.png)
# NuClear River Documentation

> **Warning** This documentation is a work in progress.

**NuClear River** is a customizable platform to build Read Model in sense of [CQRS (Command Query Responsibility Segregation)](https://cqrs.files.wordpress.com/2010/11/cqrs_documents.pdf).

The big picture how NuClear River can be used in your environment looks like this:

![](diagrams/nuclear-river-big-picture.png)

Basically, all you need to build Read Model here are events that generated as a result of commands execution in the the source system. Structure of these events may be different, but they must contain identities of entities that has been changed. In some case they can contain detailed changes descriptions, but it is not necessary.

It is also important that you can build Read Model just for a specific bounded context of your domain. Structure of the data of resulting Read Model can be defined based on querying needs (e.g. denormalized) and can be totally different from the data structure in the source system where it was definded based on create/update/delete needs. 

The result that you achieve when using NuClear River - read and write stacks are separated and can be developed independenly, so it is a fast way to move to CQRS-way of application design in specific part of business logic (business processes) of your application.

There are some important terms here:

* Source system
* [Bounded context](http://martinfowler.com/bliki/BoundedContext.html)
* [Metadata descriptions](desing-overview/metadata-descriptions.md)
* [Querying](desing-overview/querying-design.md)
* [Replication](desing-overview/replication-design.md)
* Events (UseCases) processing
* Specification pattern

See [Terms](terms.md) chapter for details.

High-level design of NuClear River described in [the next chapter](desing-overview\README.md).