**Metadata descriptions** are descriptions that could be used for configuration of **NuClear River** components. It's a DSL based on C#.This is the way to customize **NuClear River** behaviour for a specific [bounded context](http://martinfowler.com/bliki/BoundedContext.html). 

Let's take a short tour on main parts of metadata descriptions. Yon can find all classes and interfaces discussed here in [2GIS.NuClear.Metamodeling](#) library.

> **Note** Code of **2GIS.NuClear.Metamodeling** library is in a progress of opensourcing. The detailed documentation would be found there.

At first, you need to put an attention on `MetadataElement` abstract class. When you need to describe some valuable thing in you domain, you'll derive from this class. Every `MetadataElement` has an `Identity` of `IMetadataElementIdentity` type:

```csharp
public interface IMetadataElementIdentity : IEquatable<IMetadataElementIdentity>
{
    Uri Id { get; }
}
```

For example, let's take an entities area and create metadata descriptions for it. So, every entity instance have key properties, regular properties and some relations with other instances. Hence, we have 3 types of elements (classes derived from `MetadataElement`):

* `EntityElement`
* `EntityPropertyElement`
* `EntityRelationElement`

For simplicity, let `EntityElement` will look like this:

```csharp
public sealed class EntityElement : MetadataElement
{
    internal EntityElement(
    	IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
        : base(identity, features)
    {
    }

    public IEnumerable<EntityPropertyElement> KeyProperties
    {
        get
        {
            return ResolveFeature<EntityIdentityFeature, IEnumerable<EntityPropertyElement>>(
        		f => f.IdentifyingProperties, Enumerable.Empty<EntityPropertyElement>());
        }
    }

    public IEnumerable<EntityPropertyElement> Properties
    {
        get { return Elements.OfType<EntityPropertyElement>(); }
    }

    public IEnumerable<EntityRelationElement> Relations
    {
        get { return Elements.OfType<EntityRelationElement>(); }
    }
}
```

`EntityPropertyElement` and `EntityRelationElement` are more simplier:

```csharp
public sealed class EntityPropertyElement : MetadataElement
{
    private readonly IStructuralModelTypeElement _typeElement;

    internal EntityPropertyElement(
    	IMetadataElementIdentity identity, 
    	IStructuralModelTypeElement typeElement, 
    	IEnumerable<IMetadataFeature> features)
        : base(identity, features)
    {
        if (typeElement == null)
        {
            throw new ArgumentNullException("typeElement");
        }

        _typeElement = typeElement;
    }

    public IStructuralModelTypeElement PropertyType
    {
        get { return _typeElement; }
    }

    public bool IsNullable
    {
        get { return ResolveFeature<EntityPropertyNullableFeature, bool>(f => f.IsNullable); }
    }
}

public sealed class EntityRelationElement : MetadataElement
{
    internal EntityRelationElement(
    	IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
        : base(identity, features)
    {
    }

    public EntityElement Target
    {
        get
        {
            return ResolveFeature<EntityRelationCardinalityFeature, EntityElement>(
                f => f.Target,
                () => { throw new InvalidOperationException("The cardinality was not specified."); });
        }
    }

    //...
}
```

The second important point here - is the abstract `MetadataElementBuilder` class. You'll need to derive from this class to be able to "connect" metadata elements with each other using fluent syntax. Just take a look at `EntityElementBuilder` class:

```csharp
public sealed class EntityElementBuilder : MetadataElementBuilder<EntityElementBuilder, EntityElement>
{
    private readonly HashSet<Uri> _keyNames = new HashSet<Uri>();
    private readonly List<EntityPropertyElementBuilder> _propertyConfigs = new List<EntityPropertyElementBuilder>();
    private readonly List<EntityRelationElementBuilder> _relationConfigs = new List<EntityRelationElementBuilder>();

    private Uri _entityId;

    public Uri EntityId
    {
        get
        {
            if (_entityId == null)
            {
                throw new InvalidOperationException("The id was not set.");
            }

            return _entityId;
        }
    }

    public EntityElementBuilder Name(string name)
    {
        _entityId = UriExtensions.AsUri(name);
        return this;
    }

    public EntityElementBuilder HasKey(params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            _keyNames.Add(UriExtensions.AsUri(propertyName));
        }
        return this;
    }

    public EntityElementBuilder Property(EntityPropertyElementBuilder property)
    {
        _propertyConfigs.Add(property);
        return this;
    }

    public EntityElementBuilder Relation(EntityRelationElementBuilder relation)
    {
        _relationConfigs.Add(relation);
        return this;
    }

    //...
}
```

So, after that work done, you'll be able to describe entities in your domain:

```csharp
StructuralModelElementBuilder ConceptualModel =
StructuralModelElement.Config
    .Elements(
        EntityElement.Config.Name(EntityName.Project).EntitySetName("Projects")
            .HasKey("Id")
            .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
            .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
            .Relation(EntityRelationElement.Config.Name("Categories"))
            .Relation(EntityRelationElement.Config.Name("Firms")),

        EntityElement.Config
            .Name(EntityName.Category).EntitySetName("Categories")
            .HasKey("CategoryId")
            .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
            .Property(EntityPropertyElement.Config.Name("ProjectId").OfType(ElementaryTypeKind.Int64))
            .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
            .Property(EntityPropertyElement.Config.Name("Level").OfType(ElementaryTypeKind.Int32)),

         EntityElement.Config
            .Name(EntityName.Firm).EntitySetName("Firms")
            .HasKey("Id")
            .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
            .Property(EntityPropertyElement.Config.Name("ProjectId").OfType(ElementaryTypeKind.Int64))
            .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
            .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(ElementaryTypeKind.DateTimeOffset))
            .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(ElementaryTypeKind.Boolean));
```

Next, you can use this description in your code with `IMetadataProvider`. Everything you need to get it from provider is appropriate `IMetadataElementIdentity` instance:

```csharp
public sealed class QueryingMetadataIdentity : MetadataKindIdentityBase<QueryingMetadataIdentity>
{
    private readonly Uri _id = Metamodeling.Elements.Identities.Builder.Metadata.Id.For("Querying");

    public override Uri Id
    {
        get { return _id; }
    }

    public override string Description
    {
        get { return "Querying identity"; }
    }
}

internal static class Program
{
    private static void Main(string[] args)
    {
        IMetadataProvider metadataProvider;

        //...

        MetadataSet metadataSet;
        metadataProvider.TryGetMetadata<QueryingMetadataIdentity>(out metadataSet);

        //...
    }
}
```

To learn more about metadata descriptions, see [NuClear Metamodeling's project](#) docs.