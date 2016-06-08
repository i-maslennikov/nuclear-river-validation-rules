using System;

using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    using CI = Storage.Model.CI;
    using Erm = Storage.Model.Erm;
    using Facts = Storage.Model.Facts;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement MinimalFirmAggregate =>
            ArrangeMetadataElement.Config
            .Name(nameof(MinimalFirmAggregate))
            .IncludeSharedDictionary(CategoryDictionary)
            .CustomerIntelligence(
                                new CI::Firm { Id = 1, ClientId = null, CreatedOn = DateTimeOffset.MinValue, AddressCount = 1, CategoryGroupId = 0, Name = "FirmName", HasPhone = false, HasWebsite = false, LastDisqualifiedOn = null, LastDistributedOn = null, ProjectId = 1, OwnerId = 27 },
                                new CI::FirmCategory1 { FirmId = 1, CategoryId = 1 },
                                new CI::FirmCategory2 { FirmId = 1, CategoryId = 2 },
                                new CI::FirmActivity { FirmId = 1, LastActivityOn = null },
                                new CI::FirmTerritory { FirmId = 1, FirmAddressId = 1, TerritoryId = 1 },
                                new CI::Project { Id = 1, Name = "ProjectOne" })
            .Fact(
                new Facts::Firm { Id = 1, ClientId = null, CreatedOn = DateTimeOffset.MinValue, LastDisqualifiedOn = null, Name = "FirmName", OrganizationUnitId = 1, OwnerId = 27 },
                new Facts::FirmAddress { Id = 1, FirmId = 1, TerritoryId = 1 },
                new Facts::CategoryFirmAddress { Id = 1, CategoryId = 3, FirmAddressId = 1 },
                new Facts::CategoryFirmAddress { Id = 2, CategoryId = 4, FirmAddressId = 1 },
                new Facts::Project { Id = 1, Name = "ProjectOne", OrganizationUnitId = 1 })
            .Erm(
                new Erm::Firm { Id = 1, ClientId = null, ClosedForAscertainment = false, CreatedOn = DateTimeOffset.MinValue, IsActive = true, IsDeleted = false, LastDisqualifyTime = null, Name = "FirmName", OrganizationUnitId = 1, OwnerId = 27 },
                new Erm::FirmAddress { Id = 1, FirmId = 1, TerritoryId = 1, ClosedForAscertainment = false, IsActive = true, IsDeleted = false },
                new Erm::CategoryFirmAddress { Id = 1, CategoryId = 3, FirmAddressId = 1, IsActive = true, IsDeleted = false },
                new Erm::CategoryFirmAddress { Id = 2, CategoryId = 4, FirmAddressId = 1, IsActive = true, IsDeleted = false },
                new Erm::Project { Id = 1, IsActive = true, Name = "ProjectOne", OrganizationUnitId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement TestFirmAddressCount =>
            ArrangeMetadataElement.Config
            .Name(nameof(TestFirmAddressCount))
            .Fact(
                new Facts::Firm { Id = 1, Name = "no addresses" },

                new Facts::Firm { Id = 2, Name = "1 address" },
                new Facts::FirmAddress { Id = 2, FirmId = 2 },

                new Facts::Firm { Id = 3, Name = "2 addresses" },
                new Facts::FirmAddress { Id = 3, FirmId = 3 },
                new Facts::FirmAddress { Id = 4, FirmId = 3 },

                new Facts::Project()
                )
            .CustomerIntelligence(

                new CI::Firm { Id = 1, Name = "no addresses", AddressCount = 0 },
                new CI::FirmActivity { FirmId = 1 },

                new CI::Firm { Id = 2, Name = "1 address", AddressCount = 1 },
                new CI::FirmTerritory { FirmId = 2, FirmAddressId = 2 },
                new CI::FirmActivity { FirmId = 2 },

                new CI::Firm { Id = 3, Name = "2 addresses", AddressCount = 2 },
                new CI::FirmTerritory { FirmId = 3, FirmAddressId = 3 },
                new CI::FirmTerritory { FirmId = 3, FirmAddressId = 4 },
                new CI::FirmActivity { FirmId = 3 },

                new CI::Project()
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement TestFirmHasPhoneHasWebSite =>
            ArrangeMetadataElement.Config
            .Name(nameof(TestFirmHasPhoneHasWebSite))
            .Fact(

                new Facts::Firm { Id = 1, Name = "no firm contacts" },
                new Facts::FirmAddress { Id = 1, FirmId = 1 },

                new Facts::Firm { Id = 2, Name = "firm contact (no phone, no website)" },
                new Facts::FirmAddress { Id = 2, FirmId = 2 },
                new Facts::FirmContact { Id = 2, FirmAddressId = 2 },

                new Facts::Firm { Id = 3, Name = "firm contact (phone)" },
                new Facts::FirmAddress { Id = 3, FirmId = 3 },
                new Facts::FirmContact { Id = 3, FirmAddressId = 3, HasPhone = true },

                new Facts::Firm { Id = 4, Name = "firm contact (website)" },
                new Facts::FirmAddress { Id = 4, FirmId = 4 },
                new Facts::FirmContact { Id = 4, FirmAddressId = 4, HasWebsite = true },

                // client entities to reference on
                new Facts::Client { Id = 1, Name = "client (no phone, no website)" },
                new Facts::Client { Id = 2, Name = "client (phone)", HasPhone = true },
                new Facts::Client { Id = 3, Name = "client (website)", HasWebsite = true },
                new Facts::Client { Id = 4, Name = "client (no phone, no website), client contact (no phone, no website)" }, new Facts::Contact { Id = 1, ClientId = 4 },
                new Facts::Client { Id = 5, Name = "client (no phone, no website), client contact (phone)" }, new Facts::Contact { Id = 2, ClientId = 5, HasPhone = true },
                new Facts::Client { Id = 6, Name = "client (no phone, no website), client contact (website)" }, new Facts::Contact { Id = 3, ClientId = 6, HasWebsite = true },
                new Facts::Client { Id = 7, Name = "client (phone), client contact (no phone, no website)", HasPhone = true }, new Facts::Contact { Id = 4, ClientId = 7 },
                new Facts::Client { Id = 8, Name = "client (website), client contact (no phone, no website)", HasWebsite = true }, new Facts::Contact { Id = 5, ClientId = 8 },
                new Facts::Client { Id = 9, Name = "client (phone), client contact (phone)", HasPhone = true }, new Facts::Contact { Id = 6, ClientId = 9, HasPhone = true },
                new Facts::Client { Id = 10, Name = "client (website), client contact (website)", HasWebsite = true }, new Facts::Contact { Id = 7, ClientId = 10, HasWebsite = true },

                new Facts::Firm { Id = 11, ClientId = 1, Name = "no firm contacts, client (no phone, no website)" },
                new Facts::Firm { Id = 12, ClientId = 2, Name = "no firm contacts, client (phone)" },
                new Facts::Firm { Id = 13, ClientId = 3, Name = "no firm contacts, client (website)" },
                new Facts::Firm { Id = 14, ClientId = 4, Name = "no firm contacts, client (no phone, no website), client contact (no phone, no website)" },
                new Facts::Firm { Id = 15, ClientId = 5, Name = "no firm contacts, client (no phone, no website), client contact (phone)" },
                new Facts::Firm { Id = 16, ClientId = 6, Name = "no firm contacts, client (no phone, no website), client contact (website)" },
                new Facts::Firm { Id = 17, ClientId = 7, Name = "no firm contacts, client (phone), client contact (no phone, no website)" },
                new Facts::Firm { Id = 18, ClientId = 8, Name = "no firm contacts, client (website), client contact (no phone, no website)" },
                new Facts::Firm { Id = 19, ClientId = 9, Name = "no firm contacts, client (phone), client contact (phone)" },
                new Facts::Firm { Id = 110, ClientId = 10, Name = "no firm contacts, client (website), client contact (website)" },

                new Facts::Project()
                )
            .CustomerIntelligence(
                new CI::Firm { Id = 1, AddressCount = 1, Name = "no firm contacts" },
                new CI::FirmTerritory { FirmId = 1, FirmAddressId = 1 },
                new CI::FirmActivity { FirmId = 1 },

                new CI::Firm { Id = 2, AddressCount = 1, Name = "firm contact (no phone, no website)" },
                new CI::FirmTerritory { FirmId = 2, FirmAddressId = 2 },
                new CI::FirmActivity { FirmId = 2 },

                new CI::Firm { Id = 3, AddressCount = 1, Name = "firm contact (phone)", HasPhone = true },
                new CI::FirmTerritory { FirmId = 3, FirmAddressId = 3 },
                new CI::FirmActivity { FirmId = 3 },

                new CI::Firm { Id = 4, AddressCount = 1, Name = "firm contact (website)", HasWebsite = true },
                new CI::FirmTerritory { FirmId = 4, FirmAddressId = 4 },
                new CI::FirmActivity { FirmId = 4 },

                // client entities to reference on
                new CI::Client { Id = 1, Name = "client (no phone, no website)" },
                new CI::Client { Id = 2, Name = "client (phone)" },
                new CI::Client { Id = 3, Name = "client (website)" },
                new CI::Client { Id = 4, Name = "client (no phone, no website), client contact (no phone, no website)" }, new CI::ClientContact { ContactId = 1, ClientId = 4 },
                new CI::Client { Id = 5, Name = "client (no phone, no website), client contact (phone)" }, new CI::ClientContact { ContactId = 2, ClientId = 5 },
                new CI::Client { Id = 6, Name = "client (no phone, no website), client contact (website)" }, new CI::ClientContact { ContactId = 3, ClientId = 6 },
                new CI::Client { Id = 7, Name = "client (phone), client contact (no phone, no website)" }, new CI::ClientContact { ContactId = 4, ClientId = 7 },
                new CI::Client { Id = 8, Name = "client (website), client contact (no phone, no website)" }, new CI::ClientContact { ContactId = 5, ClientId = 8 },
                new CI::Client { Id = 9, Name = "client (phone), client contact (phone)" }, new CI::ClientContact { ContactId = 6, ClientId = 9 },
                new CI::Client { Id = 10, Name = "client (website), client contact (website)" }, new CI::ClientContact { ContactId = 7, ClientId = 10 },

                new CI::Firm { Id = 11, ClientId = 1, Name = "no firm contacts, client (no phone, no website)" }, new CI::FirmActivity { FirmId = 11 },
                new CI::Firm { Id = 12, ClientId = 2, Name = "no firm contacts, client (phone)", HasPhone = true }, new CI::FirmActivity { FirmId = 12 },
                new CI::Firm { Id = 13, ClientId = 3, Name = "no firm contacts, client (website)", HasWebsite = true }, new CI::FirmActivity { FirmId = 13 },
                new CI::Firm { Id = 14, ClientId = 4, Name = "no firm contacts, client (no phone, no website), client contact (no phone, no website)" }, new CI::FirmActivity { FirmId = 14 },
                new CI::Firm { Id = 15, ClientId = 5, Name = "no firm contacts, client (no phone, no website), client contact (phone)", HasPhone = true }, new CI::FirmActivity { FirmId = 15 },
                new CI::Firm { Id = 16, ClientId = 6, Name = "no firm contacts, client (no phone, no website), client contact (website)", HasWebsite = true }, new CI::FirmActivity { FirmId = 16 },
                new CI::Firm { Id = 17, ClientId = 7, Name = "no firm contacts, client (phone), client contact (no phone, no website)", HasPhone = true }, new CI::FirmActivity { FirmId = 17 },
                new CI::Firm { Id = 18, ClientId = 8, Name = "no firm contacts, client (website), client contact (no phone, no website)", HasWebsite = true }, new CI::FirmActivity { FirmId = 18 },
                new CI::Firm { Id = 19, ClientId = 9, Name = "no firm contacts, client (phone), client contact (phone)", HasPhone = true }, new CI::FirmActivity { FirmId = 19 },
                new CI::Firm { Id = 110, ClientId = 10, Name = "no firm contacts, client (website), client contact (website)", HasWebsite = true }, new CI::FirmActivity { FirmId = 110 },

                new CI::Project()
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement TestFirmLastDisqualifiedOn =>
            ArrangeMetadataElement.Config
            .Name(nameof(TestFirmLastDisqualifiedOn))
            .Fact(
                new Facts::Firm { Id = 1, Name = "firm (null), client (null)" },

                new Facts::Firm { Id = 2, ClientId = 2, Name = "firm (null), client (not null)" },
                new Facts::Client { Id = 2, LastDisqualifiedOn = DateTimeOffset.MaxValue },

                new Facts::Firm { Id = 3, ClientId = 3, Name = "firm (not null), client (null)", LastDisqualifiedOn = DateTimeOffset.MaxValue },
                new Facts::Client { Id = 3 },

                new Facts::Firm { Id = 4, ClientId = 4, Name = "firm (not null), client (not null)", LastDisqualifiedOn = DateTimeOffset.MaxValue },
                new Facts::Client { Id = 4, LastDisqualifiedOn = DateTimeOffset.MaxValue.AddDays(-1) },

                new Facts::Project()
                )
            .CustomerIntelligence(

                new CI::Firm { Id = 1, Name = "firm (null), client (null)", LastDisqualifiedOn = null },
                new CI::FirmActivity { FirmId = 1 },

                new CI::Firm { Id = 2, ClientId = 2, Name = "firm (null), client (not null)", LastDisqualifiedOn = DateTimeOffset.MaxValue },
                new CI::FirmActivity { FirmId = 2 },
                new CI::Client { Id = 2 },

                new CI::Firm { Id = 3, ClientId = 3, Name = "firm (not null), client (null)", LastDisqualifiedOn = DateTimeOffset.MaxValue },
                new CI::FirmActivity { FirmId = 3 },
                new CI::Client { Id = 3 },

                new CI::Firm { Id = 4, ClientId = 4, Name = "firm (not null), client (not null)", LastDisqualifiedOn = DateTimeOffset.MaxValue },
                new CI::FirmActivity { FirmId = 4 },
                new CI::Client { Id = 4 },

                new CI::Project()
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement TestFirmLastDistributedOn =>
            ArrangeMetadataElement.Config
            .Name(nameof(TestFirmLastDistributedOn))
            .Fact(
                new Facts::Firm { Id = 1, Name = "no orders" },

                new Facts::Firm { Id = 2, Name = "1 order" },
                new Facts::Order { Id = 2, FirmId = 2, EndDistributionDateFact = DateTimeOffset.MaxValue },

                new Facts::Firm { Id = 3, Name = "2 orders" },
                new Facts::Order { Id = 3, FirmId = 3, EndDistributionDateFact = DateTimeOffset.MaxValue },
                new Facts::Order { Id = 4, FirmId = 3, EndDistributionDateFact = DateTimeOffset.MaxValue.AddDays(-1) },

                new Facts::Project()
                )
            .CustomerIntelligence(

                new CI::Firm { Id = 1, Name = "no orders", LastDistributedOn = null },
                new CI::FirmActivity { FirmId = 1 },

                new CI::Firm { Id = 2, Name = "1 order", LastDistributedOn = DateTimeOffset.MaxValue },
                new CI::FirmActivity { FirmId = 2 },

                new CI::Firm { Id = 3, Name = "2 orders", LastDistributedOn = DateTimeOffset.MaxValue },
                new CI::FirmActivity { FirmId = 3 },

                new CI::Project()
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmWithActivity
            => ArrangeMetadataElement.Config
                .Name(nameof(FirmWithActivity))
                .IncludeSharedDictionary(MinimalFirmAggregate)
                .Erm(
                    new Erm::Phonecall { Id = 1, IsActive = true, IsDeleted = false, Status = 2, ModifiedOn = DateTimeOffset.Parse("2010-01-01") },
                    new Erm::PhonecallReference { ActivityId = 1, Reference = 1, ReferencedObjectId = 1, ReferencedType = 146 })
                .Fact(
                    new Facts::Activity { Id = 1, FirmId = 1, ModifiedOn = DateTimeOffset.Parse("2010-01-01") })
                .Mutate(m => m.Update<CI::FirmActivity>(x => x.FirmId == 1, x => x.LastActivityOn = DateTimeOffset.Parse("2010-01-01")));

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement TestFirmLead =>
            ArrangeMetadataElement.Config
            .Name(nameof(TestFirmLead))
            .Fact(
                new Facts::Lead { Id = 1, FirmId = 2, IsInQueue = true, Type = 1},
                new Facts::Lead { Id = 2, FirmId = 2, IsInQueue = false, Type = 2 }
                )
            .CustomerIntelligence(
                new CI::FirmLead { LeadId = 1, FirmId = 2, IsInQueue = true, Type = 1 },
                new CI::FirmLead { LeadId = 2, FirmId = 2, IsInQueue = false, Type = 2 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BornToFail
            => ArrangeMetadataElement.Config
                .Name(nameof(BornToFail))
                .Erm(
                    new Erm::Firm { Id = 1, ClientId = null, ClosedForAscertainment = false, CreatedOn = DateTimeOffset.MinValue, IsActive = true, IsDeleted = false, LastDisqualifyTime = null, Name = "FirmName", OrganizationUnitId = 1, OwnerId = 27 })
                .Fact()
                .Ignored();
    }
}
