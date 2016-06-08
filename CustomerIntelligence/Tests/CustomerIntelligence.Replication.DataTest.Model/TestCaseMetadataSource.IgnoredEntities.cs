using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredAccount =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredAccount))
                                  .Fact(
                                        new Account { Id = 1 })
                                  .Erm(
                                       new Storage.Model.Erm.Account { Id = 1, IsArchived = false },
                                       new Storage.Model.Erm.Account { Id = 2, IsArchived = true });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredBranchOfficeOrganizationUnit =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredBranchOfficeOrganizationUnit))
                                  .Fact(
                                        new BranchOfficeOrganizationUnit { Id = 1 })
                                  .Erm(
                                       new Storage.Model.Erm.BranchOfficeOrganizationUnit { Id = 1, IsActive = true, IsDeleted = false },
                                       new Storage.Model.Erm.BranchOfficeOrganizationUnit { Id = 2, IsActive = true, IsDeleted = true },
                                       new Storage.Model.Erm.BranchOfficeOrganizationUnit { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredCategory =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredCategory))
                                  .Fact(
                                        new Category { Id = 1 })
                                  .Erm(
                                       new Storage.Model.Erm.Category { Id = 1, IsActive = true, IsDeleted = false },
                                       new Storage.Model.Erm.Category { Id = 2, IsActive = true, IsDeleted = true },
                                       new Storage.Model.Erm.Category { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredCategoryFirmAddress =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredCategoryFirmAddress))
                                  .Fact(
                                        new CategoryFirmAddress { Id = 1 })
                                  .Erm(
                                       new Storage.Model.Erm.CategoryFirmAddress { Id = 1, IsActive = true, IsDeleted = false },
                                       new Storage.Model.Erm.CategoryFirmAddress { Id = 2, IsActive = true, IsDeleted = true },
                                       new Storage.Model.Erm.CategoryFirmAddress { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredCategoryGroup =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredCategoryGroup))
                                  .Fact(
                                        new CategoryGroup { Id = 1 })
                                  .Erm(
                                       new Storage.Model.Erm.CategoryGroup { Id = 1, IsActive = true, IsDeleted = false },
                                       new Storage.Model.Erm.CategoryGroup { Id = 2, IsActive = true, IsDeleted = true },
                                       new Storage.Model.Erm.CategoryGroup { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredCategoryOrganizationUnit =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredCategoryOrganizationUnit))
                                  .Fact(
                                        new CategoryOrganizationUnit { Id = 1 })
                                  .Erm(
                                       new Storage.Model.Erm.CategoryOrganizationUnit { Id = 1, IsActive = true, IsDeleted = false },
                                       new Storage.Model.Erm.CategoryOrganizationUnit { Id = 2, IsActive = true, IsDeleted = true },
                                       new Storage.Model.Erm.CategoryOrganizationUnit { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredClient =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredClient))
                                  .Fact(
                                        new Client { Id = 1 })
                                  .Erm(
                                       new Storage.Model.Erm.Client { Id = 1, IsActive = true, IsDeleted = false },
                                       new Storage.Model.Erm.Client { Id = 2, IsActive = true, IsDeleted = true },
                                       new Storage.Model.Erm.Client { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredContact =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredContact))
                                  .Fact(
                                        new Contact { Id = 1 })
                                  .Erm(
                                       new Storage.Model.Erm.Contact { Id = 1, IsActive = true, IsDeleted = false },
                                       new Storage.Model.Erm.Contact { Id = 2, IsActive = true, IsDeleted = true },
                                       new Storage.Model.Erm.Contact { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredFirm =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredFirm))
                                  .Fact(
                                        new Firm { Id = 1 })
                                  .Erm(
                                       new Storage.Model.Erm.Firm { Id = 1, IsActive = true, IsDeleted = false },
                                       new Storage.Model.Erm.Firm { Id = 2, IsActive = true, IsDeleted = true },
                                       new Storage.Model.Erm.Firm { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredFirmAddress =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredFirmAddress))
                                  .Fact(
                                        new FirmAddress { Id = 1 })
                                  .Erm(
                                       new Storage.Model.Erm.FirmAddress { Id = 1, IsActive = true, IsDeleted = false },
                                       new Storage.Model.Erm.FirmAddress { Id = 2, IsActive = true, IsDeleted = true },
                                       new Storage.Model.Erm.FirmAddress { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredFirmContact =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredFirmContact))
                                  .Fact(
                                        new FirmContact { Id = 1, HasPhone = true, FirmAddressId = 1 },
                                        new FirmContact { Id = 2, HasWebsite = true, FirmAddressId = 1 })
                                  .Erm(
                                       new Storage.Model.Erm.FirmContact { Id = 1, FirmAddressId = 1, ContactType = 1 },
                                       new Storage.Model.Erm.FirmContact { Id = 2, FirmAddressId = 1, ContactType = 4 },
                                       new Storage.Model.Erm.FirmContact { Id = 3, FirmAddressId = 1, ContactType = 2 },
                                       new Storage.Model.Erm.FirmContact { Id = 4, FirmAddressId = null, ContactType = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredLegalPerson =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredLegalPerson))
                                  .Fact(
                                        new LegalPerson { Id = 1, ClientId = 1 })
                                  .Erm(
                                       new Storage.Model.Erm.LegalPerson { Id = 1, IsActive = true, IsDeleted = false, ClientId = 1 },
                                       new Storage.Model.Erm.LegalPerson { Id = 2, IsActive = true, IsDeleted = false, ClientId = null },
                                       new Storage.Model.Erm.LegalPerson { Id = 3, IsActive = true, IsDeleted = true, ClientId = 1 },
                                       new Storage.Model.Erm.LegalPerson { Id = 4, IsActive = false, IsDeleted = false, ClientId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredOrder =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredOrder))
                                  .Fact(
                                        new Order { Id = 1 },
                                        new Order { Id = 2 })
                                  .Erm(
                                       new Storage.Model.Erm.Order { Id = 1, IsActive = true, IsDeleted = false, WorkflowStepId = 4 },
                                       new Storage.Model.Erm.Order { Id = 2, IsActive = true, IsDeleted = false, WorkflowStepId = 6 },
                                       new Storage.Model.Erm.Order { Id = 3, IsActive = true, IsDeleted = false, WorkflowStepId = 1 },
                                       new Storage.Model.Erm.Order { Id = 4, IsActive = true, IsDeleted = true, WorkflowStepId = 4 },
                                       new Storage.Model.Erm.Order { Id = 5, IsActive = false, IsDeleted = false, WorkflowStepId = 4 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredProject =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredProject))
                                  .Fact(
                                        new Project { Id = 1, OrganizationUnitId = 1 })
                                  .Erm(new Storage.Model.Erm.Project { Id = 1, IsActive = true, OrganizationUnitId = 1 },
                                       new Storage.Model.Erm.Project { Id = 2, IsActive = true, OrganizationUnitId = null },
                                       new Storage.Model.Erm.Project { Id = 3, IsActive = false, OrganizationUnitId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredSalesModelCategoryRestriction =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredSalesModelCategoryRestriction))
                                  .Fact(new SalesModelCategoryRestriction { Id = 1 })
                                  .Erm(new Storage.Model.Erm.SalesModelCategoryRestriction { Id = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredTerritory =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredTerritory))
                                  .Fact(new Territory { Id = 1 })
                                  .Erm(
                                       new Storage.Model.Erm.Territory { Id = 1, IsActive = true },
                                       new Storage.Model.Erm.Territory { Id = 2, IsActive = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredLead =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredLead))
                                  .Fact(new Lead { Id = 1, FirmId = 2, IsInQueue = false, Type = 1})
                                  .Erm(
                                       new Storage.Model.Erm.Lead { Id = 1, FirmId = 2, OwnerId = 3, Status = 1, Type = 1},
                                       new Storage.Model.Erm.Lead { Id = 2, FirmId = null, OwnerId = 3, Status = 1, Type = 1 },
                                       new Storage.Model.Erm.Lead { Id = 3, FirmId = 2, OwnerId = 3, Status = 2, Type = 1 });

    }
}

