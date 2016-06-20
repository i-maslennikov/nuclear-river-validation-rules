using NuClear.Querying.Metadata.Builders;
using NuClear.Querying.Metadata.Elements;

namespace NuClear.CustomerIntelligence.Querying.Host
{
    partial class QueryingMetadataSource
    {
        private static class CustomerIntelligence
        {
            private static readonly StructuralModelElementBuilder ConceptualModel =
                StructuralModelElement.Config
                .Types<EnumTypeElement>(
                    EnumTypeElement.Config.Name(EnumName.ContactRole)
                        .Member("Employee", 1)
                        .Member("InfluenceDecisions", 2)
                        .Member("MakingDecisions", 3),
                    EnumTypeElement.Config.Name(EnumName.SalesModel)
                        .Member("NotSet", 0)
                        .Member("CPS", 10)
                        .Member("FH", 11)
                        .Member("MFH", 12),
                    EnumTypeElement.Config.Name(EnumName.LeadType)
                        .Member("Hot", 1)
                        .Member("Warm", 2)
                        .Member("Cold", 3))
                .Elements(
                    EntityElement.Config.Name(EntityName.CategoryGroup).EntitySetName("CategoryGroups")
                        .HasKey("Id")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String)),

                    EntityElement.Config.Name(EntityName.Category).EntitySetName("Categories")
                                    .HasKey("CategoryId")
                                    .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("ProjectId").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                    .Property(EntityPropertyElement.Config.Name("Level").OfType(ElementaryTypeKind.Int32))
                                    .Property(EntityPropertyElement.Config.Name("SalesModel").OfType<EnumTypeElement>(EnumTypeElement.Config.Name(EnumName.SalesModel))),

                    EntityElement.Config.Name(EntityName.Territory).EntitySetName("Territories")
                                    .HasKey("Id")
                                    .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("ProjectId").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String)),

                     EntityElement.Config.Name(EntityName.Firm).EntitySetName("Firms")
                                    .HasKey("Id")
                                    .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("ProjectId").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                    .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(ElementaryTypeKind.DateTimeOffset))
                                    .Property(EntityPropertyElement.Config.Name("LastDisqualifiedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                    .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                    .Property(EntityPropertyElement.Config.Name("LastActivityOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                    .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(ElementaryTypeKind.Boolean))
                                    .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(ElementaryTypeKind.Boolean))
                                    .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(ElementaryTypeKind.Int32))
                                    .Property(EntityPropertyElement.Config.Name("ForecastClick").OfType(ElementaryTypeKind.Int32).Nullable())
                                    .Property(EntityPropertyElement.Config.Name("ForecastAmount").OfType(ElementaryTypeKind.Decimal).Nullable())
                                    .Relation(EntityRelationElement.Config.Name("Balances")
                                        .DirectTo(
                                            EntityElement.Config.Name(EntityName.FirmBalance)
                                                .HasKey("AccountId")
                                                .Property(EntityPropertyElement.Config.Name("AccountId").OfType(ElementaryTypeKind.Int64))
                                                .Property(EntityPropertyElement.Config.Name("ProjectId").OfType(ElementaryTypeKind.Int64))
                                                .Property(EntityPropertyElement.Config.Name("Balance").OfType(ElementaryTypeKind.Decimal))
                                        ).AsMany())
                                    .Relation(EntityRelationElement.Config.Name("Leads")
                                        .DirectTo(
                                            EntityElement.Config.Name(EntityName.FirmLead)
                                                .HasKey("LeadId")
                                                .Property(EntityPropertyElement.Config.Name("LeadId").OfType(ElementaryTypeKind.Int64))
                                                .Property(EntityPropertyElement.Config.Name("IsInQueue").OfType(ElementaryTypeKind.Boolean))
                                                .Property(EntityPropertyElement.Config.Name("Type").OfType<EnumTypeElement>(EnumTypeElement.Config.Name(EnumName.LeadType)))
                                        ).AsMany())
                                    .Relation(EntityRelationElement.Config.Name("Categories1")
                                        .DirectTo(
                                            EntityElement.Config.Name(EntityName.FirmCategory1)
                                                .HasKey("CategoryId")
                                                .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                        ).AsMany().AsContainment())
                                    .Relation(EntityRelationElement.Config.Name("Categories2")
                                        .DirectTo(
                                            EntityElement.Config.Name(EntityName.FirmCategory2)
                                                .HasKey("CategoryId")
                                                .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                        ).AsMany().AsContainment())
                                    .Relation(EntityRelationElement.Config.Name("Categories3")
                                        .DirectTo(
                                            EntityElement.Config.Name(EntityName.FirmCategory3)
                                                .HasKey("CategoryId")
                                                .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                                .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                                .Property(EntityPropertyElement.Config.Name("AdvertisersShare").OfType(ElementaryTypeKind.Double))
                                                .Property(EntityPropertyElement.Config.Name("FirmCount").OfType(ElementaryTypeKind.Int32))
                                                .Property(EntityPropertyElement.Config.Name("Hits").OfType(ElementaryTypeKind.Int32))
                                                .Property(EntityPropertyElement.Config.Name("Shows").OfType(ElementaryTypeKind.Int32))
                                                .Property(EntityPropertyElement.Config.Name("ForecastClick").OfType(ElementaryTypeKind.Int32).Nullable())
                                                .Property(EntityPropertyElement.Config.Name("ForecastAmount").OfType(ElementaryTypeKind.Decimal).Nullable())
                                        ).AsMany().AsContainment())
                        .Relation(EntityRelationElement.Config.Name("Territories")
                            .DirectTo(
                                EntityElement.Config.Name(EntityName.FirmTerritory)
                                    .HasKey("FirmAddressId")
                                    .Property(EntityPropertyElement.Config.Name("FirmAddressId").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("TerritoryId").OfType(ElementaryTypeKind.Int64).Nullable())
                            ).AsMany())
                                    .Relation(EntityRelationElement.Config.Name("CategoryGroup").DirectTo(EntityElement.Config.Name(EntityName.CategoryGroup)).AsOne())
                                    .Relation(EntityRelationElement.Config.Name("Client")
                                        .DirectTo(
                                            EntityElement.Config.Name(EntityName.Client)
                                                .HasKey("Id")
                                                .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                                .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                                .Relation(EntityRelationElement.Config.Name("CategoryGroup").DirectTo(EntityElement.Config.Name(EntityName.CategoryGroup)).AsOne())
                                                .Relation(
                                                    EntityRelationElement.Config.Name("Contacts")
                                                        .DirectTo(
                                                            EntityElement.Config.Name(EntityName.ClientContact)
                                                                .HasKey("ContactId")
                                                                .Property(EntityPropertyElement.Config.Name("ContactId").OfType(ElementaryTypeKind.Int64))
                                                                .Property(EntityPropertyElement.Config.Name("Role").OfType<EnumTypeElement>(EnumTypeElement.Config.Name(EnumName.ContactRole)))
                                                        )
                                                        .AsMany()
                                                )
                                        )
                                        .AsOneOptionally())
                        .Property(EntityPropertyElement.Config.Name("OwnerId").OfType(ElementaryTypeKind.Int64)),

                    EntityElement.Config.Name(EntityName.Project).EntitySetName("Projects")
                        .HasKey("Id")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                        .Relation(EntityRelationElement.Config.Name("Categories").DirectTo(EntityElement.Config.Name(EntityName.Category)).AsMany().AsContainment())
                        .Relation(EntityRelationElement.Config.Name("Territories").DirectTo(EntityElement.Config.Name(EntityName.Territory)).AsMany().AsContainment())
                        .Relation(EntityRelationElement.Config.Name("Firms").DirectTo(EntityElement.Config.Name(EntityName.Firm)).AsMany().AsContainment())
            );

            private static readonly StructuralModelElementBuilder StoreModel =
                StructuralModelElement.Config.Elements(
                    EntityElement.Config.Name(TableName.CategoryGroup)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                 .Property(EntityPropertyElement.Config.Name("Rate").OfType(ElementaryTypeKind.Single)),
                    EntityElement.Config.Name(TableName.Project)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String)),
                    EntityElement.Config.Name(TableName.ProjectCategory)
                                 .HasKey("ProjectId", "CategoryId")
                                 .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                 .Property(EntityPropertyElement.Config.Name("Level").OfType(ElementaryTypeKind.Int32))
                                 .Property(EntityPropertyElement.Config.Name("SalesModel").OfType(ElementaryTypeKind.Int32))
                                 .Property(EntityPropertyElement.Config.Name("ParentId").OfType(ElementaryTypeKind.Int64).Nullable())
                                 .Relation(EntityRelationElement.Config.Name("ProjectId").DirectTo(EntityElement.Config.Name(TableName.Project)).AsOne()),
                    EntityElement.Config.Name(TableName.Territory)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                 .Relation(EntityRelationElement.Config.Name("ProjectId").DirectTo(EntityElement.Config.Name(TableName.Project)).AsOne()),
                    EntityElement.Config.Name(TableName.Client)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                 .Relation(EntityRelationElement.Config.Name("CategoryGroupId").DirectTo(EntityElement.Config.Name(TableName.CategoryGroup)).AsOne()),
                    EntityElement.Config.Name(TableName.ClientContact)
                                 .HasKey("ClientId", "ContactId")
                                 .Property(EntityPropertyElement.Config.Name("ContactId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Role").OfType(ElementaryTypeKind.Int32))
                                 .Relation(EntityRelationElement.Config.Name("ClientId").DirectTo(EntityElement.Config.Name(TableName.Client)).AsOne()),
                    EntityElement.Config.Name(ViewName.Firm)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                 .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(ElementaryTypeKind.DateTimeOffset))
                                 .Property(EntityPropertyElement.Config.Name("LastDisqualifiedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("LastActivityOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(ElementaryTypeKind.Boolean))
                                 .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(ElementaryTypeKind.Boolean))
                                 .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(ElementaryTypeKind.Int32))
                                 .Property(EntityPropertyElement.Config.Name("OwnerId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("ForecastClick").OfType(ElementaryTypeKind.Int32).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("ForecastAmount").OfType(ElementaryTypeKind.Decimal).Nullable())
                                 .Relation(EntityRelationElement.Config.Name("CategoryGroupId").DirectTo(EntityElement.Config.Name(TableName.CategoryGroup)).AsOne())
                                 .Relation(EntityRelationElement.Config.Name("ProjectId").DirectTo(EntityElement.Config.Name(TableName.Project)).AsOne())
                                 .Relation(EntityRelationElement.Config.Name("ClientId").DirectTo(EntityElement.Config.Name(TableName.Client)).AsOneOptionally()),
                    EntityElement.Config.Name(TableName.FirmLead)
                                 .HasKey("FirmId", "LeadId")
                                 .Property(EntityPropertyElement.Config.Name("LeadId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("IsInQueue").OfType(ElementaryTypeKind.Boolean))
                                 .Property(EntityPropertyElement.Config.Name("Type").OfType(ElementaryTypeKind.Decimal))
                                 .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(ViewName.Firm)).AsOne()),

                    EntityElement.Config.Name(TableName.FirmBalance)
                                 .HasKey("FirmId", "AccountId")
                                 .Property(EntityPropertyElement.Config.Name("AccountId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("ProjectId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Balance").OfType(ElementaryTypeKind.Decimal))
                                 .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(ViewName.Firm)).AsOne()),
                    EntityElement.Config.Name(TableName.FirmCategory1)
                                 .HasKey("FirmId", "CategoryId")
                                 .Property(EntityPropertyElement.Config.Name("FirmId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                 .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(ViewName.Firm)).AsOne()),
                    EntityElement.Config.Name(TableName.FirmCategory2)
                                 .HasKey("FirmId", "CategoryId")
                                 .Property(EntityPropertyElement.Config.Name("FirmId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                 .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(ViewName.Firm)).AsOne()),
                    EntityElement.Config.Name(TableName.FirmCategory3)
                                 .HasKey("FirmId", "CategoryId")
                                 .Property(EntityPropertyElement.Config.Name("FirmId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                 .Property(EntityPropertyElement.Config.Name("AdvertisersShare").OfType(ElementaryTypeKind.Double))
                                 .Property(EntityPropertyElement.Config.Name("FirmCount").OfType(ElementaryTypeKind.Int32))
                                 .Property(EntityPropertyElement.Config.Name("Hits").OfType(ElementaryTypeKind.Int32))
                                 .Property(EntityPropertyElement.Config.Name("Shows").OfType(ElementaryTypeKind.Int32))
                                 .Property(EntityPropertyElement.Config.Name("ForecastClick").OfType(ElementaryTypeKind.Int32).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("ForecastAmount").OfType(ElementaryTypeKind.Decimal).Nullable())
                                 .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(ViewName.Firm)).AsOne()),
                    EntityElement.Config.Name(TableName.FirmTerritory)
                                 .HasKey("FirmId", "FirmAddressId")
                                 .Property(EntityPropertyElement.Config.Name("FirmAddressId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("TerritoryId").OfType(ElementaryTypeKind.Int64).Nullable())
                                 .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(ViewName.Firm)).AsOne()));

            public static readonly BoundedContextElement Context =
                BoundedContextElement.Config.Name("CustomerIntelligence")
                    .ConceptualModel(ConceptualModel)
                    .StoreModel(StoreModel)
                    .Map(EntityName.CategoryGroup, TableName.CategoryGroup)
                    .Map(EntityName.Project, TableName.Project)
                    .Map(EntityName.Category, TableName.ProjectCategory)
                    .Map(EntityName.Territory, TableName.Territory)
                    .Map(EntityName.Firm, ViewName.Firm)
                    .Map(EntityName.FirmBalance, TableName.FirmBalance)
                    .Map(EntityName.FirmLead, TableName.FirmLead)
                    .Map(EntityName.FirmCategory1, TableName.FirmCategory1)
                    .Map(EntityName.FirmCategory2, TableName.FirmCategory2)
                    .Map(EntityName.FirmCategory3, TableName.FirmCategory3)
                    .Map(EntityName.FirmTerritory, TableName.FirmTerritory)
                    .Map(EntityName.Client, TableName.Client)
                    .Map(EntityName.ClientContact, TableName.ClientContact);

            private static class EnumName
            {
                public const string ContactRole = "ContactRole";
                public const string SalesModel = "SalesModel";
                public const string LeadType = "LeadType";
            }

            private static class EntityName
            {
                public const string CategoryGroup = "CategoryGroup";
                public const string Project = "Project";
                public const string Category = "Category";
                public const string Territory = "Territory";
                public const string Client = "Client";
                public const string ClientContact = "ClientContact";
                public const string Firm = "Firm";
                public const string FirmLead = "FirmLead";
                public const string FirmBalance = "FirmBalance";
                public const string FirmCategory1 = "FirmCategory1";
                public const string FirmCategory2 = "FirmCategory2";
                public const string FirmCategory3 = "FirmCategory3";
                public const string FirmTerritory = "FirmTerritory";
            }

            private static class TableName
            {
                public const string CategoryGroup = TableSchema + "." + "CategoryGroup";
                public const string Project = TableSchema + "." + "Project";
                public const string ProjectCategory = TableSchema + "." + "ProjectCategory";
                public const string Territory = TableSchema + "." + "Territory";
                public const string FirmBalance = TableSchema + "." + "FirmBalance";
                public const string FirmLead = TableSchema + "." + "FirmLead";
                public const string FirmCategory1 = TableSchema + "." + "FirmCategory1";
                public const string FirmCategory2 = TableSchema + "." + "FirmCategory2";
                public const string FirmCategory3 = TableSchema + "." + "FirmCategory3";
                public const string FirmTerritory = TableSchema + "." + "FirmTerritory";
                public const string Client = TableSchema + "." + "Client";
                public const string ClientContact = TableSchema + "." + "ClientContact";

                private const string TableSchema = "CustomerIntelligence";
            }

            private static class ViewName
            {
                public const string Firm = ViewSchema + "." + "FirmView";

                private const string ViewSchema = "CustomerIntelligence";
            }
        }
    }
}