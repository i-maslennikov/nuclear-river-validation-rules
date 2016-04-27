using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.CustomerIntelligence.Storage.Model.CI;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Storage.API.Readings;

using NUnit.Framework;

using CategoryGroup = NuClear.CustomerIntelligence.Storage.Model.Facts.CategoryGroup;
using Client = NuClear.CustomerIntelligence.Storage.Model.Facts.Client;
using Firm = NuClear.CustomerIntelligence.Storage.Model.Facts.Firm;
using Project = NuClear.CustomerIntelligence.Storage.Model.Facts.Project;
using Territory = NuClear.CustomerIntelligence.Storage.Model.Facts.Territory;

namespace NuClear.CustomerIntelligence.Replication.Tests.Specifications
{
    [TestFixture, SetCulture("")]
    internal class FactsMapToCISpecsTests : DataFixtureBase
    {
        [Test]
        public void ShouldTransformCategoryGroup()
        {
            SourceDb.Has(
                new CategoryGroup { Id = 123, Name = "category group", Rate = 1 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.CategoryGroups.Map(x).By(y => y.Id, 123),
                                           new[] { new Storage.Model.CI.CategoryGroup { Id = 123, Name = "category group", Rate = 1 } });
        }

        [Test]
        public void ShouldTransformClient()
        {
            SourceDb.Has(
                new Client { Id = 1, Name = "a client" });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Clients.Map(x).By(y => y.Id, 1),
                                           new[] { new Storage.Model.CI.Client { Name = "a client" } },
                                           x => new { x.Name },
                                           "The name should be processed.");
        }

        [Test]
        public void ShouldTransformClientContact()
        {
            SourceDb.Has(
                new Contact { Id = 1, ClientId = 1, Role = 1 },
                new Contact { Id = 3, ClientId = 3 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.ClientContacts.Map(x).By(c => c.ClientId, 1),
                                           new[] { new ClientContact { Role = 1 } },
                                           x => new { x.Role },
                                           "The role should be processed.")
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.ClientContacts.Map(x).By(c => c.ClientId, 3),
                                           new[] { new ClientContact { ClientId = 3 } },
                                           x => new { x.ClientId },
                                           "The client reference should be processed.");
        }

        [Test]
        public void ShouldTransformFirm()
        {
            var now = DateTimeOffset.UtcNow;
			now = now.AddTicks(-now.Ticks % (10000000)); // SqlLite округляет до секунды, из-за этого тест не проходит
            var dayAgo = now.AddDays(-1);
            var monthAgo = now.AddMonths(-1);

            SourceDb.Has(new Project { Id = 1, OrganizationUnitId = 1 },
                         new Project { Id = 2, OrganizationUnitId = 2 })
                    .Has(new Firm { Id = 1, Name = "1st firm", CreatedOn = monthAgo, LastDisqualifiedOn = dayAgo, OrganizationUnitId = 1 },
                         new Firm { Id = 2, Name = "2nd firm", CreatedOn = monthAgo, LastDisqualifiedOn = null, ClientId = 1, OrganizationUnitId = 2 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1, TerritoryId = 1 },
                         new FirmAddress { Id = 2, FirmId = 1, TerritoryId = 2 })
                    .Has(new Client { Id = 1, LastDisqualifiedOn = now })
                    .Has(new LegalPerson { Id = 1, ClientId = 1 })
                    .Has(new Order { FirmId = 1, EndDistributionDateFact = dayAgo });

			// TODO: split into several tests
            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, 1),
                                           new[] { new Storage.Model.CI.Firm { Name = "1st firm" } },
                                           x => new { x.Name },
                                           "The name should be processed.")
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, 1),
                                           new[] { new Storage.Model.CI.Firm { CreatedOn = monthAgo } },
                                           x => new { x.CreatedOn },
                                           "The createdOn should be processed.")
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, In(1, 2)),
                                           new[] { new Storage.Model.CI.Firm { LastDisqualifiedOn = dayAgo }, new Storage.Model.CI.Firm { LastDisqualifiedOn = now } },
                                           x => new { x.LastDisqualifiedOn },
                                           "The disqualifiedOn should be processed.")
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, In(1, 2)),
                                           new[] { new Storage.Model.CI.Firm { LastDistributedOn = dayAgo }, new Storage.Model.CI.Firm { LastDistributedOn = null } },
                                           x => new { x.LastDistributedOn },
                                           "The distributedOn should be processed.")
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, In(1, 2)),
                                           new[] { new Storage.Model.CI.Firm { AddressCount = 2 }, new Storage.Model.CI.Firm { AddressCount = 0 } },
                                           x => new { x.AddressCount },
                                           "The address count should be processed.")
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.FirmTerritories.Map(x),
                                           new[] { new FirmTerritory { FirmId = 1, TerritoryId = 1 }, new FirmTerritory { FirmId = 1, TerritoryId = 2 } },
                                           x => new { x.FirmId, x.TerritoryId },
                                           "Firm territories should be processed.")
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, In(1, 2)),
                                           new[]
                                               {
                                                   new Storage.Model.CI.Firm { Id = 1, ClientId = null, ProjectId = 1 },
                                                   new Storage.Model.CI.Firm { Id = 2, ClientId = 1, ProjectId = 2 }
                                               },
                                           x => new { x.Id, x.ClientId, x.ProjectId },
                                           "The references should be processed.");
        }

        [Test]
        public void ShouldTransformFirmContactInfoFromClient()
        {
            SourceDb.Has(new Project { Id = 1, OrganizationUnitId = 0 })
                    .Has(new Firm { Id = 1, },
                         new Firm { Id = 2, ClientId = 1 },
                         new Firm { Id = 3, ClientId = 2 },
                         new Firm { Id = 4, ClientId = 3 })
                    .Has(new Client { Id = 1, HasPhone = true, HasWebsite = true },
                         new Client { Id = 2, HasPhone = false, HasWebsite = false },
                         new Client { Id = 3, HasPhone = false, HasWebsite = false })
                    .Has(new Contact { Id = 1, ClientId = 2, HasPhone = true, HasWebsite = true },
                         new Contact { Id = 2, ClientId = 3, HasPhone = true, HasWebsite = false },
                         new Contact { Id = 3, ClientId = 3, HasPhone = false, HasWebsite = true });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, 1),
                                           new[] { new Storage.Model.CI.Firm { HasPhone = false, HasWebsite = false } },
                                           x => new { x.HasPhone, x.HasWebsite })
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, 2),
                                           new[] { new Storage.Model.CI.Firm { HasPhone = true, HasWebsite = true } },
                                           x => new { x.HasPhone, x.HasWebsite })
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, 3),
                                           new[] { new Storage.Model.CI.Firm { HasPhone = true, HasWebsite = true } },
                                           x => new { x.HasPhone, x.HasWebsite })
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, 4),
                                           new[] { new Storage.Model.CI.Firm { HasPhone = true, HasWebsite = true } },
                                           x => new { x.HasPhone, x.HasWebsite });
        }

        [Test]
        public void ShouldTransformFirmContactInfoFromFirm()
        {
            SourceDb.Has(new Project { Id = 1, OrganizationUnitId = 0 })
                    .Has(new Firm { Id = 1, Name = "has no addresses" },
                         new Firm { Id = 2, Name = "has addresses, but no contacts" },
                         new Firm { Id = 3, Name = "has one phone contact" },
                         new Firm { Id = 4, Name = "has one website contact" },
                         new Firm { Id = 5, Name = "has an unknown contact" })
                    .Has(new FirmAddress { Id = 1, FirmId = 2 },
                         new FirmAddress { Id = 2, FirmId = 3 },
                         new FirmAddress { Id = 3, FirmId = 4 },
                         new FirmAddress { Id = 4, FirmId = 5 })
                    .Has(new FirmContact { Id = 1, HasPhone = true, FirmAddressId = 2 },
                         new FirmContact { Id = 2, HasWebsite = true, FirmAddressId = 3 },
                         new FirmContact { Id = 3, FirmAddressId = 4 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, 1),
                                           new[] { new Storage.Model.CI.Firm { HasPhone = false, HasWebsite = false } },
                                           x => new { x.HasPhone, x.HasWebsite })
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, 2),
                                           new[] { new Storage.Model.CI.Firm { HasPhone = false, HasWebsite = false } },
                                           x => new { x.HasPhone, x.HasWebsite })
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, 3),
                                           new[] { new Storage.Model.CI.Firm { HasPhone = true, HasWebsite = false } },
                                           x => new { x.HasPhone, x.HasWebsite })
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, 4),
                                           new[] { new Storage.Model.CI.Firm { HasPhone = false, HasWebsite = true } },
                                           x => new { x.HasPhone, x.HasWebsite })
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Firms.Map(x).By(y => y.Id, 5),
                                           new[] { new Storage.Model.CI.Firm { HasPhone = false, HasWebsite = false } },
                                           x => new { x.HasPhone, x.HasWebsite });
        }

        [Test]
        public void ShouldTransformFirmBalance()
        {
            SourceDb.Has(new Project { Id = 1, OrganizationUnitId = 1 },
                         new Project { Id = 2, OrganizationUnitId = 2 })
                    .Has(new Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 },
                         new Firm { Id = 2, ClientId = 2, OrganizationUnitId = 2 },
                         new Firm { Id = 3, ClientId = 1, OrganizationUnitId = 1 })
                    .Has(new Client { Id = 1 },
                         new Client { Id = 2 })
                    .Has(new LegalPerson { Id = 1, ClientId = 1 },
                         new LegalPerson { Id = 2, ClientId = 2 })
                    .Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 },
                         new BranchOfficeOrganizationUnit { Id = 2, OrganizationUnitId = 2 })
                    .Has(new Account { Id = 1, Balance = 123, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 },
                         new Account { Id = 2, Balance = 234, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 2 },
                         new Account { Id = 3, Balance = 345, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 2 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.FirmBalances.Map(x).By(b => b.FirmId, In(1, 2, 3)).OrderBy(fb => fb.FirmId),
                                           new[]
                                               {
                                                   new FirmBalance { FirmId = 1, Balance = 123, AccountId = 1, ProjectId = 1 },
                                                   new FirmBalance { FirmId = 2, Balance = 345, AccountId = 3, ProjectId = 2 },
                                                   new FirmBalance { FirmId = 3, Balance = 123, AccountId = 1, ProjectId = 1 },
                                                   new FirmBalance { FirmId = 1, Balance = 234, AccountId = 2, ProjectId = 2 },
                                                   new FirmBalance { FirmId = 3, Balance = 234, AccountId = 2, ProjectId = 2 }
                                               },
                                           "The balance should be processed.");
        }

        [Test]
        public void ShouldTransformFirmCategory()
        {
            SourceDb.Has(new Category { Id = 1, Level = 1 },
                         new Category { Id = 2, Level = 2, ParentId = 1 },
                         new Category { Id = 3, Level = 3, ParentId = 2 },
                         new Category { Id = 4, Level = 3, ParentId = 2 });
            SourceDb.Has(new FirmAddress { Id = 1, FirmId = 1 },
                         new FirmAddress { Id = 2, FirmId = 1 });
            SourceDb.Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 },
                         new CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 4 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.FirmCategories1.Map(x).By(c => c.FirmId, 1),
                                           new[] { new FirmCategory1 { FirmId = 1, CategoryId = 1 } },
                                           "The firm categories1 should be processed.");
            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.FirmCategories2.Map(x).By(c => c.FirmId, 1),
                                           new[] { new FirmCategory2 { FirmId = 1, CategoryId = 2 } },
                                           "The firm categories2 should be processed.");
        }

        [Test]
        public void ShouldTransformProject()
        {
            SourceDb.Has(
                new Project { Id = 123, Name = "p1" },
                new Project { Id = 456, Name = "p2", OrganizationUnitId = 1 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Projects.Map(x).By(y => y.Id, In(123, 456)),
                                           new[]
                                               {
                                                   new Storage.Model.CI.Project { Id = 123, Name = "p1" },
                                                   new Storage.Model.CI.Project { Id = 456, Name = "p2" }
                                               },
                                           "The projects should be processed.");
        }

        [Test]
        public void ShouldTransformProjectCategory()
        {
            SourceDb.Has(new Project { Id = 1, OrganizationUnitId = 2 })
                    .Has(new CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 2, CategoryId = 3 },
                         new CategoryOrganizationUnit { Id = 2, OrganizationUnitId = 2, CategoryId = 4 })
                    .Has(new SalesModelCategoryRestriction { Id = 1, ProjectId = 1, CategoryId = 3, SalesModel = 10})
                    .Has(new Category { Id = 3 },
                         new Category { Id = 4 })
                    .Has(new ProjectCategoryStatistics { ProjectId = 1, AdvertisersCount = 1, CategoryId = 3 });

            // Десять фирм в проекте, каждая с рубрикой #3
            for (var i = 0; i < 10; i++)
            {
                SourceDb.Has(new Firm { Id = i, OrganizationUnitId = 2 });
                SourceDb.Has(new FirmAddress { Id = i, FirmId = i });
                SourceDb.Has(new CategoryFirmAddress { Id = i, FirmAddressId = i, CategoryId = 3 });
            }

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.ProjectCategories.Map(x).By(c => c.ProjectId, 1),
                                           new[]
                                               {
                                                   new ProjectCategory { ProjectId = 1, CategoryId = 3, SalesModel = 10 },
                                                   new ProjectCategory { ProjectId = 1, CategoryId = 4, SalesModel = 0 }
                                               });
        }

        [Test]
        public void ShouldTransformTerritories()
        {
            SourceDb.Has(new Project { Id = 1, OrganizationUnitId = 1 },
                         new Project { Id = 2, OrganizationUnitId = 2 })
                    .Has(new Territory { Id = 1, Name = "name1", OrganizationUnitId = 1 },
                         new Territory { Id = 2, Name = "name2", OrganizationUnitId = 2 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.Territories.Map(x).By(y => y.Id, In(1, 2)),
                                           new[]
                                               {
                                                   new Storage.Model.CI.Territory { Id = 1, Name = "name1", ProjectId = 1 },
                                                   new Storage.Model.CI.Territory { Id = 2, Name = "name2", ProjectId = 2 }
                                               });
        }

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
    }
}