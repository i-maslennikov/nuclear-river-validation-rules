using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    using CI = Storage.Model.CI;
    using Facts = Storage.Model.Facts;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ClientAggregate =>
            ArrangeMetadataElement.Config
                .Name(nameof(ClientAggregate))
                .CustomerIntelligence(new CI::Client { Id = 1, Name = "ClientName", CategoryGroupId = 1 },
                                      new CI.ClientContact { ClientId = 1, ContactId = 1, Role = 1 },

                                      // Эти сущности в контексте теста не важны, но они появляются и лишь поэтому прописаны в тесте
                                      new CI::CategoryGroup { Id = 1, Rate = 2 },
                                      new CI::CategoryGroup { Id = 2, Rate = 1 },
                                      new CI::FirmActivity { FirmId = 1 },
                                      new CI::FirmActivity { FirmId = 2 },
                                      new CI::FirmTerritory { FirmId = 1, FirmAddressId = 1 },
                                      new CI::FirmTerritory { FirmId = 2, FirmAddressId = 2 })
                .Fact(new Facts::Client { Id = 1, Name = "ClientName" },
                      new Facts::Contact { Id = 1, ClientId = 1, Role = 1 },

                      // фирма со стандартной рубрикой
                      new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 },
                      new Facts::FirmAddress { Id = 1, FirmId = 1 },
                      new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 },
                      new Facts::CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 },
                      new Facts::CategoryGroup { Id = 1, Rate = 2f },

                      // фирма со дорогой рубрикой
                      new Facts::Firm { Id = 2, ClientId = 1, OrganizationUnitId = 1 },
                      new Facts::FirmAddress { Id = 2, FirmId = 2 },
                      new Facts::CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 2 },
                      new Facts::CategoryOrganizationUnit { Id = 2, CategoryGroupId = 2, CategoryId = 2, OrganizationUnitId = 1 },
                      new Facts::CategoryGroup { Id = 2, Rate = 1f });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ClientAggregate_ReversedCategoryGroups =>
            ArrangeMetadataElement.Config
                    .Name(nameof(ClientAggregate_ReversedCategoryGroups))
                    .CustomerIntelligence(new CI::Client { Id = 1, Name = "ClientName", CategoryGroupId = 2 },
                                        new CI::ClientContact { ClientId = 1, ContactId = 1, Role = 1 },

                                        // Эти сущности в контексте теста не важны, но они появляются и лишь поэтому прописаны в тесте
                                        new CI::CategoryGroup { Id = 1, Rate = 1 },
                                        new CI::CategoryGroup { Id = 2, Rate = 2 },
                                        new CI::FirmActivity { FirmId = 1 },
                                        new CI::FirmActivity { FirmId = 2 },
                                        new CI::FirmTerritory { FirmId = 1, FirmAddressId = 1 },
                                        new CI::FirmTerritory { FirmId = 2, FirmAddressId = 2 })
                    .Fact(new Facts::Client { Id = 1, Name = "ClientName" },
                          new Facts::Contact { Id = 1, ClientId = 1, Role = 1 },

                          // фирма со стандартной рубрикой
                          new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 },
                          new Facts::FirmAddress { Id = 1, FirmId = 1 },
                          new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 },
                          new Facts::CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 },
                          new Facts::CategoryGroup { Id = 1, Rate = 1f },

                          // фирма со дорогой рубрикой
                          new Facts::Firm { Id = 2, ClientId = 1, OrganizationUnitId = 1 },
                          new Facts::FirmAddress { Id = 2, FirmId = 2 },
                          new Facts::CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 2 },
                          new Facts::CategoryOrganizationUnit { Id = 2, CategoryGroupId = 2, CategoryId = 2, OrganizationUnitId = 1 },
                          new Facts::CategoryGroup { Id = 2, Rate = 2f })
                    .Ignored();
    }
}
