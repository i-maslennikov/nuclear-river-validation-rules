**NuClear River** platform consists of two components: [Querying](../terms.md) and [Replication](../terms.md). Technically, they are independent from each other. But metadata descriptions used for both to describe a specific bounded context should be coordinated.

Let's take a look at **Querying**. It's a very simple application that exposed API throught **HTTP** in terms of [**OData**](http://www.odata.org/) protocol. This application built on ASP.NET Web API and uses [Microsoft.AspNet.OData](https://www.nuget.org/packages/Microsoft.AspNet.OData/) and [EntityFramework](https://www.nuget.org/packages/EntityFramework) libraries.

So, OData protocol usage gives us two main profits:

* OData metadata, a machine-readable description of the data model of the APIs
* An opportunity to client to construct an arbitrary query based on OData metadata

OData service uses an abstract data model called **Entity Data Model (EDM)** to describe the exposed data in the service. To construct EDM model **Querying** component uses [Metadata descriptions](../terms.md). Moreover, concept of bounded contexts is fully supported, so you can have as many data models as you need. From OData side, bounded contexts are implemented with [Entity containers](http://docs.oasis-open.org/odata/odata/v4.0/errata02/os/complete/part3-csdl/odata-v4.0-errata02-os-part3-csdl-complete.html#_Toc406398024).

It's important that Metadata descriptions describe not only EDM model, but the API of **Querying** too. Hense, you just need to create metadata for your bounded context and **Querying** component in will start to use it to execute queries for Read Model.

Here is the example.