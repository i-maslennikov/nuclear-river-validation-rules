using NuClear.CustomerIntelligence.Storage.Model.CI;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.DataTest.Metamodel.Dsl;

using CategoryGroup = NuClear.CustomerIntelligence.Storage.Model.CI.CategoryGroup;
using Client = NuClear.CustomerIntelligence.Storage.Model.CI.Client;
using Firm = NuClear.CustomerIntelligence.Storage.Model.Facts.Firm;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ClientAggregate
            => ArrangeMetadataElement.Config
                                     .Name(nameof(ClientAggregate))
                                     .CustomerIntelligence(new Client { Id = 1, Name = "ClientName", CategoryGroupId = 1 },
                                                           new ClientContact { ClientId = 1, ContactId = 1, Role = 1 },

                                                           // Эти сущности в контексте теста не важны, но они появляются и лишь поэтому прописаны в тесте
                                                           new CategoryGroup { Id = 1, Rate = 2 },
                                                           new CategoryGroup { Id = 2, Rate = 1 },
                                                           new FirmActivity { FirmId = 1 },
                                                           new FirmActivity { FirmId = 2 },
                                                           new FirmTerritory { FirmId = 1, FirmAddressId = 1 },
                                                           new FirmTerritory { FirmId = 2, FirmAddressId = 2 })
                                     .Fact(new Storage.Model.Facts.Client { Id = 1, Name = "ClientName" },
                                           new Contact { Id = 1, ClientId = 1, Role = 1 },

                                           // фирма со стандартной рубрикой
                                           new Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 },
                                           new FirmAddress { Id = 1, FirmId = 1 },
                                           new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 },
                                           new CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 },
                                           new Storage.Model.Facts.CategoryGroup { Id = 1, Rate = 2f },

                                           // фирма со дорогой рубрикой
                                           new Firm { Id = 2, ClientId = 1, OrganizationUnitId = 1 },
                                           new FirmAddress { Id = 2, FirmId = 2 },
                                           new CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 2 },
                                           new CategoryOrganizationUnit { Id = 2, CategoryGroupId = 2, CategoryId = 2, OrganizationUnitId = 1 },
                                           new Storage.Model.Facts.CategoryGroup { Id = 2, Rate = 1f });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ClientAggregate_ReversedCategoryGroups
            => ArrangeMetadataElement.Config
                                     .Name(nameof(ClientAggregate_ReversedCategoryGroups))
                                     .CustomerIntelligence(new Client { Id = 1, Name = "ClientName", CategoryGroupId = 2 },
                                                           new ClientContact { ClientId = 1, ContactId = 1, Role = 1 },

                                                           // Эти сущности в контексте теста не важны, но они появляются и лишь поэтому прописаны в тесте
                                                           new CategoryGroup { Id = 1, Rate = 1 },
                                                           new CategoryGroup { Id = 2, Rate = 2 },
                                                           new FirmActivity { FirmId = 1 },
                                                           new FirmActivity { FirmId = 2 },
                                                           new FirmTerritory { FirmId = 1, FirmAddressId = 1 },
                                                           new FirmTerritory { FirmId = 2, FirmAddressId = 2 })
                                     .Fact(new Storage.Model.Facts.Client { Id = 1, Name = "ClientName" },
                                           new Contact { Id = 1, ClientId = 1, Role = 1 },

                                           // фирма со стандартной рубрикой
                                           new Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 },
                                           new FirmAddress { Id = 1, FirmId = 1 },
                                           new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 },
                                           new CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 },
                                           new Storage.Model.Facts.CategoryGroup { Id = 1, Rate = 1f },

                                           // фирма со дорогой рубрикой
                                           new Firm { Id = 2, ClientId = 1, OrganizationUnitId = 1 },
                                           new FirmAddress { Id = 2, FirmId = 2 },
                                           new CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 2 },
                                           new CategoryOrganizationUnit { Id = 2, CategoryGroupId = 2, CategoryId = 2, OrganizationUnitId = 1 },
                                           new Storage.Model.Facts.CategoryGroup { Id = 2, Rate = 2f })
                                     .Ignored();
    }
}
