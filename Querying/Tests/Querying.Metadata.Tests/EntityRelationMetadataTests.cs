using System;
using System.Collections;

using NuClear.Querying.Metadata.Builders;
using NuClear.Querying.Metadata.Elements;

using NUnit.Framework;

namespace NuClear.Querying.Metadata.Metadata.Tests
{
    [TestFixture]
    internal class EntityRelationMetadataTests : BaseMetadataFixture<EntityRelationElement, EntityRelationElementBuilder>
    {
        public override IEnumerable TestCases
        {
            get
            {
                yield return Case(EntityRelationElement.Config)
                    .Throws(typeof(InvalidOperationException))
                    .SetName("ShouldFailIfNoName");
                yield return Case(EntityRelationElement.Config.Name("Relation"))
                    .Throws(typeof(InvalidOperationException))
                    .SetName("ShouldFailIfNoCardinality");
                yield return Case(EntityRelationElement.Config.Name("Relation").DirectTo(EntityElement.Config.Name("Target")).AsOne(), MetadataKind.Identity)
                    .Returns("{'Identity':{'Id':'Relation'}}")
                    .SetName("ShouldDeclareRelation");
                yield return Case(EntityRelationElement.Config.Name("Relation").DirectTo(EntityElement.Config.Name("Target")).AsOneOptionally(), MetadataKind.Features)
                    .Returns("{'Features':[{'Cardinality':'OptionalOne','Target':{'Features':[]}}]}")
                    .SetName("ShouldDeclareRelationAsOneOrZero");
                yield return Case(EntityRelationElement.Config.Name("Relation").DirectTo(EntityElement.Config.Name("Target")).AsOne(), MetadataKind.Features)
                    .Returns("{'Features':[{'Cardinality':'One','Target':{'Features':[]}}]}")
                    .SetName("ShouldDeclareRelationAsOne");
                yield return Case(EntityRelationElement.Config.Name("Relation").DirectTo(EntityElement.Config.Name("Target")).AsMany(), MetadataKind.Features)
                    .Returns("{'Features':[{'Cardinality':'Many','Target':{'Features':[]}}]}")
                    .SetName("ShouldDeclareRelationAsMany");
            }
        }
    }
}
