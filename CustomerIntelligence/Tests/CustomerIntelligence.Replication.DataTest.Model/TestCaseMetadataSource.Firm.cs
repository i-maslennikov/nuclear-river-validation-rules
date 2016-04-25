using System;

using NuClear.CustomerIntelligence.Storage.Model.CI;
using NuClear.CustomerIntelligence.Storage.Model.Erm;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.DataTest.Metamodel.Dsl;

using CategoryFirmAddress = NuClear.CustomerIntelligence.Storage.Model.Facts.CategoryFirmAddress;
using Client = NuClear.CustomerIntelligence.Storage.Model.Facts.Client;
using Contact = NuClear.CustomerIntelligence.Storage.Model.Facts.Contact;
using Firm = NuClear.CustomerIntelligence.Storage.Model.CI.Firm;
using FirmAddress = NuClear.CustomerIntelligence.Storage.Model.Facts.FirmAddress;
using FirmContact = NuClear.CustomerIntelligence.Storage.Model.Facts.FirmContact;
using Order = NuClear.CustomerIntelligence.Storage.Model.Facts.Order;
using Project = NuClear.CustomerIntelligence.Storage.Model.CI.Project;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement MinimalFirmAggregate
            => ArrangeMetadataElement.Config
                                                  .Name(nameof(MinimalFirmAggregate))
                                                  .IncludeSharedDictionary(CategoryDictionary)
                                                  .CustomerIntelligence(
                                                                        new Firm { Id = 1, ClientId = null, CreatedOn = DateTimeOffset.MinValue, AddressCount = 1, CategoryGroupId = 0, Name = "FirmName", HasPhone = false, HasWebsite = false, LastDisqualifiedOn = null, LastDistributedOn = null, ProjectId = 1, OwnerId = 27 },
                                                                        new FirmCategory1 { FirmId = 1, CategoryId = 1 },
                                                                        new FirmCategory2 { FirmId = 1, CategoryId = 2 },
                                                                        new FirmActivity { FirmId = 1, LastActivityOn = null },
                                                                        new FirmTerritory { FirmId = 1, FirmAddressId = 1, TerritoryId = 1 },
                                                                        new Project { Id = 1, Name = "ProjectOne" })
                                                  .Fact(
                                                        new Storage.Model.Facts.Firm { Id = 1, ClientId = null, CreatedOn = DateTimeOffset.MinValue, LastDisqualifiedOn = null, Name = "FirmName", OrganizationUnitId = 1, OwnerId = 27 },
                                                        new FirmAddress { Id = 1, FirmId = 1, TerritoryId = 1 },
                                                        new CategoryFirmAddress { Id = 1, CategoryId = 3, FirmAddressId = 1 },
                                                        new CategoryFirmAddress { Id = 2, CategoryId = 4, FirmAddressId = 1 },
                                                        new Storage.Model.Facts.Project { Id = 1, Name = "ProjectOne", OrganizationUnitId = 1 })
                                                  .Erm(
                                                       new Storage.Model.Erm.Firm { Id = 1, ClientId = null, ClosedForAscertainment = false, CreatedOn = DateTimeOffset.MinValue, IsActive = true, IsDeleted = false, LastDisqualifyTime = null, Name = "FirmName", OrganizationUnitId = 1, OwnerId = 27 },
                                                       new Storage.Model.Erm.FirmAddress { Id = 1, FirmId = 1, TerritoryId = 1, ClosedForAscertainment = false, IsActive = true, IsDeleted = false },
                                                       new Storage.Model.Erm.CategoryFirmAddress { Id = 1, CategoryId = 3, FirmAddressId = 1, IsActive = true, IsDeleted = false },
                                                       new Storage.Model.Erm.CategoryFirmAddress { Id = 2, CategoryId = 4, FirmAddressId = 1, IsActive = true, IsDeleted = false },
                                                       new Storage.Model.Erm.Project { Id = 1, IsActive = true, Name = "ProjectOne", OrganizationUnitId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement TestFirmAddressCount =>
            ArrangeMetadataElement.Config
            .Name(nameof(TestFirmAddressCount))
            .Fact(
                new Storage.Model.Facts.Firm { Id = 1, Name = "no addresses" },

                new Storage.Model.Facts.Firm { Id = 2, Name = "1 address" },
                new FirmAddress { Id = 2, FirmId = 2 },

                new Storage.Model.Facts.Firm { Id = 3, Name = "2 addresses" },
                new FirmAddress{ Id = 3, FirmId = 3},
                new FirmAddress { Id = 4, FirmId = 3 },

                new Storage.Model.Facts.Project()
                )
            .CustomerIntelligence(

                new Firm { Id = 1, Name = "no addresses", AddressCount = 0 },
                new FirmActivity { FirmId = 1 },

                new Firm { Id = 2, Name = "1 address", AddressCount = 1 },
                new FirmTerritory { FirmId = 2, FirmAddressId = 2 },
                new FirmActivity { FirmId = 2 },

                new Firm { Id = 3, Name = "2 addresses", AddressCount = 2},
                new FirmTerritory { FirmId = 3, FirmAddressId = 3 },
                new FirmTerritory { FirmId = 3, FirmAddressId = 4 },
                new FirmActivity { FirmId = 3 },

                new Project()
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement TestFirmHasPhoneHasWebSite =>
            ArrangeMetadataElement.Config
            .Name(nameof(TestFirmHasPhoneHasWebSite))
            .Fact(

                new Storage.Model.Facts.Firm { Id = 1, Name = "no firm contacts" },
                new FirmAddress { Id = 1, FirmId = 1 },

                new Storage.Model.Facts.Firm { Id = 2, Name = "firm contact (no phone, no website)" },
                new FirmAddress { Id = 2, FirmId = 2 },
                new FirmContact { Id = 2, FirmAddressId = 2},

                new Storage.Model.Facts.Firm { Id = 3, Name = "firm contact (phone)" },
                new FirmAddress { Id = 3, FirmId = 3 },
                new FirmContact { Id = 3, FirmAddressId = 3, HasPhone = true },

                new Storage.Model.Facts.Firm { Id = 4, Name = "firm contact (website)" },
                new FirmAddress { Id = 4, FirmId = 4 },
                new FirmContact { Id = 4, FirmAddressId = 4, HasWebsite = true },

                // client entities to reference on
                new Client { Id = 1, Name = "client (no phone, no website)"},
                new Client { Id = 2, Name = "client (phone)", HasPhone = true },
                new Client { Id = 3, Name = "client (website)", HasWebsite = true },
                new Client { Id = 4, Name = "client (no phone, no website), client contact (no phone, no website)" }, new Contact { Id = 1, ClientId = 4 },
                new Client { Id = 5, Name = "client (no phone, no website), client contact (phone)"}, new Contact { Id = 2, ClientId = 5, HasPhone = true },
                new Client { Id = 6, Name = "client (no phone, no website), client contact (website)" }, new Contact { Id = 3, ClientId = 6, HasWebsite = true },
                new Client { Id = 7, Name = "client (phone), client contact (no phone, no website)", HasPhone = true }, new Contact { Id = 4, ClientId = 7 },
                new Client { Id = 8, Name = "client (website), client contact (no phone, no website)", HasWebsite = true }, new Contact { Id = 5, ClientId = 8 },
                new Client { Id = 9, Name = "client (phone), client contact (phone)", HasPhone = true }, new Contact { Id = 6, ClientId = 9, HasPhone = true },
                new Client { Id = 10, Name = "client (website), client contact (website)", HasWebsite = true }, new Contact { Id = 7, ClientId = 10, HasWebsite = true },

                new Storage.Model.Facts.Firm { Id = 11, ClientId = 1, Name = "no firm contacts, client (no phone, no website)" },
                new Storage.Model.Facts.Firm { Id = 12, ClientId = 2, Name = "no firm contacts, client (phone)" },
                new Storage.Model.Facts.Firm { Id = 13, ClientId = 3, Name = "no firm contacts, client (website)" },
                new Storage.Model.Facts.Firm { Id = 14, ClientId = 4, Name = "no firm contacts, client (no phone, no website), client contact (no phone, no website)" },
                new Storage.Model.Facts.Firm { Id = 15, ClientId = 5, Name = "no firm contacts, client (no phone, no website), client contact (phone)" },
                new Storage.Model.Facts.Firm { Id = 16, ClientId = 6, Name = "no firm contacts, client (no phone, no website), client contact (website)" },
                new Storage.Model.Facts.Firm { Id = 17, ClientId = 7, Name = "no firm contacts, client (phone), client contact (no phone, no website)" },
                new Storage.Model.Facts.Firm { Id = 18, ClientId = 8, Name = "no firm contacts, client (website), client contact (no phone, no website)" },
                new Storage.Model.Facts.Firm { Id = 19, ClientId = 9, Name = "no firm contacts, client (phone), client contact (phone)" },
                new Storage.Model.Facts.Firm { Id = 110, ClientId = 10, Name = "no firm contacts, client (website), client contact (website)" },

                new Storage.Model.Facts.Project()
                )
            .CustomerIntelligence(
                new Firm { Id = 1, AddressCount = 1, Name = "no firm contacts" },
                new FirmTerritory { FirmId = 1, FirmAddressId = 1 },
                new FirmActivity { FirmId = 1 },

                new Firm { Id = 2, AddressCount = 1, Name = "firm contact (no phone, no website)" },
                new FirmTerritory { FirmId = 2, FirmAddressId = 2 },
                new FirmActivity { FirmId = 2 },

                new Firm { Id = 3, AddressCount = 1, Name = "firm contact (phone)", HasPhone = true },
                new FirmTerritory { FirmId = 3, FirmAddressId = 3 },
                new FirmActivity { FirmId = 3 },

                new Firm { Id = 4, AddressCount = 1, Name = "firm contact (website)", HasWebsite = true },
                new FirmTerritory { FirmId = 4, FirmAddressId = 4 },
                new FirmActivity { FirmId = 4 },

                // client entities to reference on
                new Storage.Model.CI.Client { Id = 1, Name = "client (no phone, no website)" },
                new Storage.Model.CI.Client { Id = 2, Name = "client (phone)" },
                new Storage.Model.CI.Client { Id = 3, Name = "client (website)" },
                new Storage.Model.CI.Client { Id = 4, Name = "client (no phone, no website), client contact (no phone, no website)" }, new ClientContact { ContactId = 1, ClientId = 4 },
                new Storage.Model.CI.Client { Id = 5, Name = "client (no phone, no website), client contact (phone)" }, new ClientContact { ContactId = 2, ClientId = 5 },
                new Storage.Model.CI.Client { Id = 6, Name = "client (no phone, no website), client contact (website)" }, new ClientContact { ContactId = 3, ClientId = 6 },
                new Storage.Model.CI.Client { Id = 7, Name = "client (phone), client contact (no phone, no website)" }, new ClientContact { ContactId = 4, ClientId = 7 },
                new Storage.Model.CI.Client { Id = 8, Name = "client (website), client contact (no phone, no website)" }, new ClientContact { ContactId = 5, ClientId = 8 },
                new Storage.Model.CI.Client { Id = 9, Name = "client (phone), client contact (phone)" }, new ClientContact { ContactId = 6, ClientId = 9 },
                new Storage.Model.CI.Client { Id = 10, Name = "client (website), client contact (website)" }, new ClientContact { ContactId = 7, ClientId = 10 },

                new Firm { Id = 11, ClientId = 1, Name = "no firm contacts, client (no phone, no website)" }, new FirmActivity { FirmId = 11 },
                new Firm { Id = 12, ClientId = 2, Name = "no firm contacts, client (phone)", HasPhone = true }, new FirmActivity { FirmId = 12 },
                new Firm { Id = 13, ClientId = 3, Name = "no firm contacts, client (website)", HasWebsite = true }, new FirmActivity { FirmId = 13 },
                new Firm { Id = 14, ClientId = 4, Name = "no firm contacts, client (no phone, no website), client contact (no phone, no website)" }, new FirmActivity { FirmId = 14 },
                new Firm { Id = 15, ClientId = 5, Name = "no firm contacts, client (no phone, no website), client contact (phone)", HasPhone = true }, new FirmActivity { FirmId = 15 },
                new Firm { Id = 16, ClientId = 6, Name = "no firm contacts, client (no phone, no website), client contact (website)", HasWebsite = true }, new FirmActivity { FirmId = 16 },
                new Firm { Id = 17, ClientId = 7, Name = "no firm contacts, client (phone), client contact (no phone, no website)", HasPhone = true }, new FirmActivity { FirmId = 17 },
                new Firm { Id = 18, ClientId = 8, Name = "no firm contacts, client (website), client contact (no phone, no website)", HasWebsite = true }, new FirmActivity { FirmId = 18 },
                new Firm { Id = 19, ClientId = 9, Name = "no firm contacts, client (phone), client contact (phone)", HasPhone = true }, new FirmActivity { FirmId = 19 },
                new Firm { Id = 110, ClientId = 10, Name = "no firm contacts, client (website), client contact (website)", HasWebsite = true }, new FirmActivity { FirmId = 110 },

                new Project()
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement TestFirmLastDisqualifiedOn =>
            ArrangeMetadataElement.Config
            .Name(nameof(TestFirmLastDisqualifiedOn))
            .Fact(
                new Storage.Model.Facts.Firm { Id = 1, Name= "firm (null), client (null)" },

                new Storage.Model.Facts.Firm { Id = 2, ClientId = 2, Name = "firm (null), client (not null)" },
                new Client { Id = 2, LastDisqualifiedOn = DateTimeOffset.MaxValue },

                new Storage.Model.Facts.Firm { Id = 3, ClientId = 3, Name = "firm (not null), client (null)", LastDisqualifiedOn = DateTimeOffset.MaxValue },
                new Client { Id = 3 },

                new Storage.Model.Facts.Firm { Id = 4, ClientId = 4, Name = "firm (not null), client (not null)", LastDisqualifiedOn = DateTimeOffset.MaxValue },
                new Client { Id = 4, LastDisqualifiedOn = DateTimeOffset.MaxValue.AddDays(-1) },

                new Storage.Model.Facts.Project()
                )
            .CustomerIntelligence(

                new Firm { Id = 1, Name= "firm (null), client (null)", LastDisqualifiedOn = null },
                new FirmActivity { FirmId = 1 },

                new Firm { Id = 2, ClientId = 2, Name = "firm (null), client (not null)", LastDisqualifiedOn = DateTimeOffset.MaxValue },
                new FirmActivity { FirmId = 2 },
                new Storage.Model.CI.Client { Id = 2 },

                new Firm { Id = 3, ClientId = 3, Name = "firm (not null), client (null)", LastDisqualifiedOn = DateTimeOffset.MaxValue },
                new FirmActivity { FirmId = 3 },
                new Storage.Model.CI.Client { Id = 3 },

                new Firm { Id = 4, ClientId = 4, Name = "firm (not null), client (not null)", LastDisqualifiedOn = DateTimeOffset.MaxValue },
                new FirmActivity { FirmId = 4 },
                new Storage.Model.CI.Client { Id = 4 },

                new Project()
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement TestFirmLastDistributedOn =>
            ArrangeMetadataElement.Config
            .Name(nameof(TestFirmLastDistributedOn))
            .Fact(
                new Storage.Model.Facts.Firm { Id = 1, Name = "no orders" },

                new Storage.Model.Facts.Firm { Id = 2, Name = "1 order" },
                new Order { Id = 2, FirmId = 2, EndDistributionDateFact = DateTimeOffset.MaxValue },

                new Storage.Model.Facts.Firm { Id = 3, Name = "2 orders" },
                new Order { Id = 3, FirmId = 3, EndDistributionDateFact = DateTimeOffset.MaxValue },
                new Order { Id = 4, FirmId = 3, EndDistributionDateFact = DateTimeOffset.MaxValue.AddDays(-1) },

                new Storage.Model.Facts.Project()
                )
            .CustomerIntelligence(

                new Firm { Id = 1, Name = "no orders", LastDistributedOn = null },
                new FirmActivity { FirmId = 1 },

                new Firm { Id = 2, Name = "1 order", LastDistributedOn = DateTimeOffset.MaxValue },
                new FirmActivity { FirmId = 2 },

                new Firm { Id = 3, Name = "2 orders", LastDistributedOn = DateTimeOffset.MaxValue },
                new FirmActivity { FirmId = 3 },

                new Project()
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmWithActivity
            => ArrangeMetadataElement.Config
                .Name(nameof(FirmWithActivity))
                .IncludeSharedDictionary(MinimalFirmAggregate)
                .Erm(
                    new Phonecall { Id = 1, IsActive = true, IsDeleted = false, Status = 2, ModifiedOn = DateTimeOffset.Parse("2010-01-01") },
                    new PhonecallReference { ActivityId = 1, Reference = 1, ReferencedObjectId = 1, ReferencedType = 146 })
                .Fact(
                    new Activity { Id = 1, FirmId = 1, ModifiedOn = DateTimeOffset.Parse("2010-01-01") })
                .Mutate(m => m.Update<FirmActivity>(x => x.FirmId == 1, x => x.LastActivityOn = DateTimeOffset.Parse("2010-01-01")));

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BornToFail
            => ArrangeMetadataElement.Config
                .Name(nameof(BornToFail))
                .Erm(
                    new Storage.Model.Erm.Firm { Id = 1, ClientId = null, ClosedForAscertainment = false, CreatedOn = DateTimeOffset.MinValue, IsActive = true, IsDeleted = false, LastDisqualifyTime = null, Name = "FirmName", OrganizationUnitId = 1, OwnerId = 27 })
                .Fact()
                .Ignored();
    }
}
