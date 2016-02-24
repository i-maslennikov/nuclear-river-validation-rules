using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;
    using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredAccount =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredAccount))
                                  .Fact(
                                        new Facts::Account { Id = 1 })
                                  .Erm(
                                       new Erm::Account { Id = 1, IsActive = true, IsDeleted = false },
                                       new Erm::Account { Id = 2, IsActive = true, IsDeleted = true },
                                       new Erm::Account { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredBranchOfficeOrganizationUnit =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredBranchOfficeOrganizationUnit))
                                  .Fact(
                                        new Facts::BranchOfficeOrganizationUnit { Id = 1 })
                                  .Erm(
                                       new Erm::BranchOfficeOrganizationUnit { Id = 1, IsActive = true, IsDeleted = false },
                                       new Erm::BranchOfficeOrganizationUnit { Id = 2, IsActive = true, IsDeleted = true },
                                       new Erm::BranchOfficeOrganizationUnit { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredCategory =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredCategory))
                                  .Fact(
                                        new Facts::Category { Id = 1 })
                                  .Erm(
                                       new Erm::Category { Id = 1, IsActive = true, IsDeleted = false },
                                       new Erm::Category { Id = 2, IsActive = true, IsDeleted = true },
                                       new Erm::Category { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredCategoryFirmAddress =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredCategoryFirmAddress))
                                  .Fact(
                                        new Facts::CategoryFirmAddress { Id = 1 })
                                  .Erm(
                                       new Erm::CategoryFirmAddress { Id = 1, IsActive = true, IsDeleted = false },
                                       new Erm::CategoryFirmAddress { Id = 2, IsActive = true, IsDeleted = true },
                                       new Erm::CategoryFirmAddress { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredCategoryGroup =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredCategoryGroup))
                                  .Fact(
                                        new Facts::CategoryGroup { Id = 1 })
                                  .Erm(
                                       new Erm::CategoryGroup { Id = 1, IsActive = true, IsDeleted = false },
                                       new Erm::CategoryGroup { Id = 2, IsActive = true, IsDeleted = true },
                                       new Erm::CategoryGroup { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredCategoryOrganizationUnit =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredCategoryOrganizationUnit))
                                  .Fact(
                                        new Facts::CategoryOrganizationUnit { Id = 1 })
                                  .Erm(
                                       new Erm::CategoryOrganizationUnit { Id = 1, IsActive = true, IsDeleted = false },
                                       new Erm::CategoryOrganizationUnit { Id = 2, IsActive = true, IsDeleted = true },
                                       new Erm::CategoryOrganizationUnit { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredClient =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredClient))
                                  .Fact(
                                        new Facts::Client { Id = 1 })
                                  .Erm(
                                       new Erm::Client { Id = 1, IsActive = true, IsDeleted = false },
                                       new Erm::Client { Id = 2, IsActive = true, IsDeleted = true },
                                       new Erm::Client { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredContact =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredContact))
                                  .Fact(
                                        new Facts::Contact { Id = 1 })
                                  .Erm(
                                       new Erm::Contact { Id = 1, IsActive = true, IsDeleted = false },
                                       new Erm::Contact { Id = 2, IsActive = true, IsDeleted = true },
                                       new Erm::Contact { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredFirm =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredFirm))
                                  .Fact(
                                        new Facts::Firm { Id = 1 })
                                  .Erm(
                                       new Erm::Firm { Id = 1, IsActive = true, IsDeleted = false },
                                       new Erm::Firm { Id = 2, IsActive = true, IsDeleted = true },
                                       new Erm::Firm { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredFirmAddress =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredFirmAddress))
                                  .Fact(
                                        new Facts::FirmAddress { Id = 1 })
                                  .Erm(
                                       new Erm::FirmAddress { Id = 1, IsActive = true, IsDeleted = false },
                                       new Erm::FirmAddress { Id = 2, IsActive = true, IsDeleted = true },
                                       new Erm::FirmAddress { Id = 3, IsActive = false, IsDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredFirmContact =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredFirmContact))
                                  .Fact(
                                        new Facts::FirmContact { Id = 1, HasPhone = true, FirmAddressId = 1 },
                                        new Facts::FirmContact { Id = 2, HasWebsite = true, FirmAddressId = 1 })
                                  .Erm(
                                       new Erm::FirmContact { Id = 1, FirmAddressId = 1, ContactType = 1 },
                                       new Erm::FirmContact { Id = 2, FirmAddressId = 1, ContactType = 4 },
                                       new Erm::FirmContact { Id = 3, FirmAddressId = 1, ContactType = 2 },
                                       new Erm::FirmContact { Id = 4, FirmAddressId = null, ContactType = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredLegalPerson =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredLegalPerson))
                                  .Fact(
                                        new Facts::LegalPerson { Id = 1, ClientId = 1 })
                                  .Erm(
                                       new Erm::LegalPerson { Id = 1, IsActive = true, IsDeleted = false, ClientId = 1 },
                                       new Erm::LegalPerson { Id = 2, IsActive = true, IsDeleted = false, ClientId = null },
                                       new Erm::LegalPerson { Id = 3, IsActive = true, IsDeleted = true, ClientId = 1 },
                                       new Erm::LegalPerson { Id = 4, IsActive = false, IsDeleted = false, ClientId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredOrder =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredOrder))
                                  .Fact(
                                        new Facts::Order { Id = 1 },
                                        new Facts::Order { Id = 2 })
                                  .Erm(
                                       new Erm::Order { Id = 1, IsActive = true, IsDeleted = false, WorkflowStepId = 4 },
                                       new Erm::Order { Id = 2, IsActive = true, IsDeleted = false, WorkflowStepId = 6 },
                                       new Erm::Order { Id = 3, IsActive = true, IsDeleted = false, WorkflowStepId = 1 },
                                       new Erm::Order { Id = 4, IsActive = true, IsDeleted = true, WorkflowStepId = 4 },
                                       new Erm::Order { Id = 5, IsActive = false, IsDeleted = false, WorkflowStepId = 4 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredProject =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredProject))
                                  .Fact(
                                        new Facts::Project { Id = 1, OrganizationUnitId = 1 })
                                  .Erm(new Erm::Project { Id = 1, IsActive = true, OrganizationUnitId = 1 },
                                       new Erm::Project { Id = 2, IsActive = true, OrganizationUnitId = null },
                                       new Erm::Project { Id = 3, IsActive = false, OrganizationUnitId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredSalesModelCategoryRestriction =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredSalesModelCategoryRestriction))
                                  .Fact(new Facts::SalesModelCategoryRestriction { Id = 1 })
                                  .Erm(new Erm::SalesModelCategoryRestriction { Id = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredTerritory =>
            ArrangeMetadataElement.Config.Name(nameof(IgnoredTerritory))
                                  .Fact(new Facts::Territory { Id = 1 })
                                  .Erm(
                                       new Erm::Territory { Id = 1, IsActive = true },
                                       new Erm::Territory { Id = 2, IsActive = false });
    }
}

