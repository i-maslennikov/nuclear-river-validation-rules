using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Storage.API.Readings;

using NUnit.Framework;

using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;
using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;

namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture, SetCulture("")]
    internal class FactsMapToCISpecsTests : TransformationFixtureBase
    {
        [Test]
        public void ShouldTransformCategoryGroup()
        {
            SourceDb.Has(
                new Facts::CategoryGroup { Id = 123, Name = "category group", Rate = 1 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.CategoryGroups.Map(x).ById(123), Inquire(new CI::CategoryGroup { Id = 123, Name = "category group", Rate = 1 }));
        }

        [Test]
        public void ShouldTransformClient()
        {
            SourceDb.Has(
                new Facts::Client { Id = 1, Name = "a client" });

            Transformation.Create(Query)
                .VerifyTransform(x => Specs.Map.Facts.ToCI.Clients.Map(x).ById(1), Inquire(new CI::Client { Name = "a client" }), x => new { x.Name }, "The name should be processed.");
        }

        [Test]
        public void ShouldTransformClientContact()
        {
            SourceDb.Has(
                new Facts::Contact { Id = 1, ClientId = 1, Role = 1 },
                new Facts::Contact { Id = 3, ClientId = 3 });

            Transformation.Create(Query)
                .VerifyTransform(x => Specs.Map.Facts.ToCI.ClientContacts.Map(x).Where(c => c.ClientId == 1), Inquire(new CI::ClientContact { Role = 1 }), x => new { x.Role }, "The role should be processed.")
                .VerifyTransform(x => Specs.Map.Facts.ToCI.ClientContacts.Map(x).Where(c => c.ClientId == 3), Inquire(new CI::ClientContact { ClientId = 3 }), x => new { x.ClientId }, "The client reference should be processed.");
        }

        [Test]
        public void ShouldTransformFirm()
        {
            var now = DateTimeOffset.UtcNow;
			now = now.AddTicks(-now.Ticks % (10000000)); // SqlLite округляет до секунды, из-за этого тест не проходит
            var dayAgo = now.AddDays(-1);
            var monthAgo = now.AddMonths(-1);

            SourceDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 1 },
                         new Facts::Project { Id = 2, OrganizationUnitId = 2 })
                    .Has(new Facts::Firm { Id = 1, Name = "1st firm", CreatedOn = monthAgo, LastDisqualifiedOn = dayAgo, OrganizationUnitId = 1 },
                         new Facts::Firm { Id = 2, Name = "2nd firm", CreatedOn = monthAgo, LastDisqualifiedOn = null, ClientId = 1, OrganizationUnitId = 2 })
                    .Has(new Facts::FirmAddress { Id = 1, FirmId = 1, TerritoryId = 1 },
                         new Facts::FirmAddress { Id = 2, FirmId = 1, TerritoryId = 2 })
                    .Has(new Facts::Client { Id = 1, LastDisqualifiedOn = now })
                    .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                    .Has(new Facts::Order { FirmId = 1, EndDistributionDateFact = dayAgo });

			// TODO: split into several tests
			Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(1),
                                           Inquire(new CI::Firm { Name = "1st firm" }),
                                           x => new { x.Name },
                                           "The name should be processed.")
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(1),
                                           Inquire(new CI::Firm { CreatedOn = monthAgo }),
                                           x => new { x.CreatedOn },
                                           "The createdOn should be processed.")
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(1, 2),
                                           Inquire(new CI::Firm { LastDisqualifiedOn = dayAgo },
                                                   new CI::Firm { LastDisqualifiedOn = now }),
                                           x => new { x.LastDisqualifiedOn },
                                           "The disqualifiedOn should be processed.")
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(1, 2),
                                           Inquire(new CI::Firm { LastDistributedOn = dayAgo },
                                                   new CI::Firm { LastDistributedOn = null }),
                                           x => new { x.LastDistributedOn },
                                           "The distributedOn should be processed.")
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(1, 2),
                                           Inquire(new CI::Firm { AddressCount = 2 },
                                                   new CI::Firm { AddressCount = 0 }),
                                           x => new { x.AddressCount },
                                           "The address count should be processed.")
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.FirmTerritories.Map(x),
                                           Inquire(new CI::FirmTerritory { FirmId = 1, TerritoryId = 1 },
                                                   new CI::FirmTerritory { FirmId = 1, TerritoryId = 2 }),
                                           x => new { x.FirmId, x.TerritoryId }, 
                                           "Firm territories should be processed.")
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(1, 2),
                                           Inquire(new CI::Firm { Id = 1, ClientId = null, ProjectId = 1 },
                                                   new CI::Firm { Id = 2, ClientId = 1, ProjectId = 2 }),
                                           x => new { x.Id, x.ClientId, x.ProjectId },
                                           "The references should be processed.");
        }

        [Test]
        public void ShouldTransformFirmContactInfoFromClient()
        {
            SourceDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 0 })
                    .Has(new Facts::Firm { Id = 1, },
                         new Facts::Firm { Id = 2, ClientId = 1 },
                         new Facts::Firm { Id = 3, ClientId = 2 },
                         new Facts::Firm { Id = 4, ClientId = 3 })
                    .Has(new Facts::Client { Id = 1, HasPhone = true, HasWebsite = true },
                         new Facts::Client { Id = 2, HasPhone = false, HasWebsite = false },
                         new Facts::Client { Id = 3, HasPhone = false, HasWebsite = false })
                    .Has(new Facts::Contact { Id = 1, ClientId = 2, HasPhone = true, HasWebsite = true },
                         new Facts::Contact { Id = 2, ClientId = 3, HasPhone = true, HasWebsite = false },
                         new Facts::Contact { Id = 3, ClientId = 3, HasPhone = false, HasWebsite = true });

            Transformation.Create(Query)
                .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(1), Inquire(new CI::Firm { HasPhone = false, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(2), Inquire(new CI::Firm { HasPhone = true, HasWebsite = true }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(3), Inquire(new CI::Firm { HasPhone = true, HasWebsite = true }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(4), Inquire(new CI::Firm { HasPhone = true, HasWebsite = true }), x => new { x.HasPhone, x.HasWebsite });
        }

        [Test]
        public void ShouldTransformFirmContactInfoFromFirm()
        {
            SourceDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 0 })
                    .Has(new Facts::Firm { Id = 1, Name = "has no addresses" },
                         new Facts::Firm { Id = 2, Name = "has addresses, but no contacts" },
                         new Facts::Firm { Id = 3, Name = "has one phone contact" },
                         new Facts::Firm { Id = 4, Name = "has one website contact" },
                         new Facts::Firm { Id = 5, Name = "has an unknown contact" })
                    .Has(new Facts::FirmAddress { Id = 1, FirmId = 2 },
                         new Facts::FirmAddress { Id = 2, FirmId = 3 },
                         new Facts::FirmAddress { Id = 3, FirmId = 4 },
                         new Facts::FirmAddress { Id = 4, FirmId = 5 })
                    .Has(new Facts::FirmContact { Id = 1, HasPhone = true, FirmAddressId = 2 },
                         new Facts::FirmContact { Id = 2, HasWebsite = true, FirmAddressId = 3 },
                         new Facts::FirmContact { Id = 3, FirmAddressId = 4 });

            Transformation.Create(Query)
                .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(1), Inquire(new CI::Firm { HasPhone = false, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(2), Inquire(new CI::Firm { HasPhone = false, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(3), Inquire(new CI::Firm { HasPhone = true, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(4), Inquire(new CI::Firm { HasPhone = false, HasWebsite = true }), x => new { x.HasPhone, x.HasWebsite })
                .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).ById(5), Inquire(new CI::Firm { HasPhone = false, HasWebsite = false }), x => new { x.HasPhone, x.HasWebsite });
        }

        [Test]
        public void ShouldTransformFirmBalance()
        {
            SourceDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 1 },
                         new Facts::Project { Id = 2, OrganizationUnitId = 2 })
                    .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 },
                         new Facts::Firm { Id = 2, ClientId = 2, OrganizationUnitId = 2 },
                         new Facts::Firm { Id = 3, ClientId = 1, OrganizationUnitId = 1 })
                    .Has(new Facts::Client { Id = 1 },
                         new Facts::Client { Id = 2 })
                    .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 },
                         new Facts::LegalPerson { Id = 2, ClientId = 2 })
                    .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 },
                         new Facts::BranchOfficeOrganizationUnit { Id = 2, OrganizationUnitId = 2 })
                    .Has(new Facts::Account { Id = 1, Balance = 123, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 },
                         new Facts::Account { Id = 2, Balance = 234, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 2 },
                         new Facts::Account { Id = 3, Balance = 345, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 2 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.FirmBalances.Map(x).Where(b => new long[] { 1, 2, 3 }.Contains(b.FirmId)).OrderBy(fb => fb.FirmId),
                                           Inquire(new CI::FirmBalance { FirmId = 1, Balance = 123, AccountId = 1, ProjectId = 1 },
                                                   new CI::FirmBalance { FirmId = 2, Balance = 345, AccountId = 3, ProjectId = 2 },
                                                   new CI::FirmBalance { FirmId = 3, Balance = 123, AccountId = 1, ProjectId = 1 },
                                                   new CI::FirmBalance { FirmId = 1, Balance = 234, AccountId = 2, ProjectId = 2 },
                                                   new CI::FirmBalance { FirmId = 3, Balance = 234, AccountId = 2, ProjectId = 2 }),
                                           "The balance should be processed.");
        }

        [Test]
        public void ShouldTransformFirmCategory()
        {
            SourceDb.Has(new Facts::Category { Id = 1, Level = 1 },
                         new Facts::Category { Id = 2, Level = 2, ParentId = 1 },
                         new Facts::Category { Id = 3, Level = 3, ParentId = 2 },
                         new Facts::Category { Id = 4, Level = 3, ParentId = 2 });
            SourceDb.Has(new Facts::FirmAddress { Id = 1, FirmId = 1 },
                         new Facts::FirmAddress { Id = 2, FirmId = 1 });
            SourceDb.Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 },
                         new Facts::CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 4 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.FirmCategories1.Map(x).Where(c => c.FirmId == 1),
                                           Inquire(new CI::FirmCategory1 { FirmId = 1, CategoryId = 1 }),
                                           "The firm categories1 should be processed.");
            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.FirmCategories2.Map(x).Where(c => c.FirmId == 1),
                                           Inquire(new CI::FirmCategory2 { FirmId = 1, CategoryId = 2 }),
                                           "The firm categories2 should be processed.");
        }

        [Test]
        public void ShouldTransformProject()
        {
            SourceDb.Has(
                new Facts::Project { Id = 123, Name = "p1" },
                new Facts::Project { Id = 456, Name = "p2", OrganizationUnitId = 1 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Projects.Map(x).ById(123, 456),
                                           Inquire(new CI::Project { Id = 123, Name = "p1" },
                                                   new CI::Project { Id = 456, Name = "p2" }),
                                           "The projects should be processed.");
        }

        [Test]
        public void ShouldTransformProjectCategory()
        {
            SourceDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 2 })
                    .Has(new Facts::CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 2, CategoryId = 3 },
                         new Facts::CategoryOrganizationUnit { Id = 2, OrganizationUnitId = 2, CategoryId = 4 })
                    .Has(new Facts::SalesModelCategoryRestriction { Id = 1, ProjectId = 1, CategoryId = 3, SalesModel = 10})
                    .Has(new Facts::Category { Id = 3 },
                         new Facts::Category { Id = 4 })
                    .Has(new Bit::ProjectCategoryStatistics { ProjectId = 1, AdvertisersCount = 1, CategoryId = 3 });

            // Десять фирм в проекте, каждая с рубрикой #3
            for (var i = 0; i < 10; i++)
            {
                SourceDb.Has(new Facts::Firm { Id = i, OrganizationUnitId = 2 });
                SourceDb.Has(new Facts::FirmAddress { Id = i, FirmId = i });
                SourceDb.Has(new Facts::CategoryFirmAddress { Id = i, FirmAddressId = i, CategoryId = 3 });
            }

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.ProjectCategories.Map(x).Where(c => c.ProjectId == 1),
                                           Inquire(new CI::ProjectCategory { ProjectId = 1, CategoryId = 3, SalesModel = 10},
                                                   new CI::ProjectCategory { ProjectId = 1, CategoryId = 4, SalesModel = 0}));
        }

        [Test]
        public void ShouldTransformTerritories()
        {
            SourceDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 1 },
                         new Facts::Project { Id = 2, OrganizationUnitId = 2 })
                    .Has(new Facts::Territory { Id = 1, Name = "name1", OrganizationUnitId = 1 },
                         new Facts::Territory { Id = 2, Name = "name2", OrganizationUnitId = 2 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Territories.Map(x).ById(1, 2),
                                           Inquire(new CI::Territory { Id = 1, Name = "name1", ProjectId = 1 },
                                                   new CI::Territory { Id = 2, Name = "name2", ProjectId = 2 }));
        }

        #region Transformation

        private class Transformation
        {
            private readonly IQuery _query;

            private Transformation(IQuery query)
            {
                _query = query;
            }

            public static Transformation Create(IQuery source = null)
            {
                return new Transformation(source ?? new Mock<IQuery>().Object);
            }

            public Transformation VerifyTransform<T>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, string message = null)
            {
                VerifyTransform(reader, expected, x => x, message);
                return this;
            }

            public Transformation VerifyTransform<T, TProjection>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, Func<T, TProjection> projector, string message = null)
            {
                // TODO: convert to a custom NUnit constraint, at least for fail logging
                Assert.That(reader(_query).ToArray, Is.EquivalentTo(expected).Using(new ProjectionEqualityComparer<T, TProjection>(projector)), message);
                return this;
            }
        }

        #endregion
    }
}