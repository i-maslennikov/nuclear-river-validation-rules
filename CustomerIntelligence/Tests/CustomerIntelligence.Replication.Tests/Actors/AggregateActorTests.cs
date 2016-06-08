using System;
using System.Linq;
using System.Linq.Expressions;

using Moq;

using NuClear.CustomerIntelligence.Replication.Actors;
using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Storage;
using NuClear.CustomerIntelligence.Storage.Model.CI;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Writings;

using NUnit.Framework;


namespace NuClear.CustomerIntelligence.Replication.Tests.Actors
{
    [TestFixture]
    internal class AggregateActorTests : ActorFixtureBase
    {
        [Test]
        public void ShouldInitializeClient()
        {
            SourceDb.Has(new Storage.Model.Facts.Client { Id = 1 });

            Factory.CreateClientAggregateActor(Query)
                   .Initialize<Client>(1)
                   .Verify<Client>(m => m.Add(It.Is(Predicate.Match(new Client { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeClientHavingContact()
        {
            SourceDb.Has(new Storage.Model.Facts.Client { Id = 1 })
                    .Has(new Storage.Model.Facts.Contact { Id = 1, ClientId = 1 });

            Factory.CreateClientAggregateActor(Query)
                   .Initialize<Client>(1)
                   .Verify<Client>(m => m.Add(It.Is(Predicate.Match(new Client { Id = 1 }))))
                   .Verify<ClientContact>(m => m.Add(It.Is(Predicate.Match(new ClientContact { ClientId = 1, ContactId = 1 }))));
        }

        [Test]
        public void ShouldRecalculateClient()
        {
            SourceDb.Has(new Storage.Model.Facts.Client { Id = 1, Name = "new name" });
            TargetDb.Has(new Client { Id = 1, Name = "old name" });

            Factory.CreateClientAggregateActor(Query)
                   .Recalculate<Client>(1)
                   .Verify<Client>(m => m.Update(It.Is(Predicate.Match(new Client { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldRecalculateClientHavingContact()
        {
            SourceDb.Has(new Storage.Model.Facts.Client { Id = 1 },
                         new Storage.Model.Facts.Client { Id = 2 },
                         new Storage.Model.Facts.Client { Id = 3 })
                    .Has(new Storage.Model.Facts.Contact { Id = 1, ClientId = 1 },
                         new Storage.Model.Facts.Contact { Id = 2, ClientId = 2 });
            TargetDb.Has(new Client { Id = 1, Name = "name" },
                         new Client { Id = 2, Name = "name" },
                         new Client { Id = 3, Name = "name" })
                    .Has(new ClientContact { ClientId = 2, ContactId = 2 },
                         new ClientContact { ClientId = 3, ContactId = 3 });

            Factory.CreateClientAggregateActor(Query)
                   .Recalculate<Client>(1)
                   .Recalculate<Client>(2)
                   .Recalculate<Client>(3)
                   .Verify<Client>(m => m.Update(It.Is(Predicate.Match(new Client { Id = 1 }))))
                   .Verify<Client>(m => m.Update(It.Is(Predicate.Match(new Client { Id = 2 }))))
                   .Verify<Client>(m => m.Update(It.Is(Predicate.Match(new Client { Id = 3 }))))
                   .Verify<ClientContact>(m => m.Add(It.Is(Predicate.Match(new ClientContact { ClientId = 1, ContactId = 1 }))))
                   .Verify<ClientContact>(m => m.Delete(It.Is(Predicate.Match(new ClientContact { ClientId = 3, ContactId = 3 }))));
        }

        [Test]
        public void ShouldDestroyClient()
        {
            TargetDb.Has(new Client { Id = 1 });

            Factory.CreateClientAggregateActor(Query)
                   .Destroy<Client>(1)
                   .Verify<Client>(m => m.Delete(It.Is(Predicate.Match(new Client { Id = 1 }))));
        }

        [Test]
        public void ShouldDestroyClientHavingContact()
        {
            TargetDb.Has(new Client { Id = 1 })
                    .Has(new ClientContact { ClientId = 1, ContactId = 1 });

            Factory.CreateClientAggregateActor(Query)
                   .Destroy<Client>(1)
                   .Verify<Client>(m => m.Delete(It.Is(Predicate.Match(new Client { Id = 1 }))))
                   .Verify<ClientContact>(m => m.Delete(It.Is(Predicate.Match(new ClientContact { ClientId = 1, ContactId = 1 }))));
        }

        [Test]
        public void ShouldInitializeFirm()
        {
            SourceDb.Has(new Storage.Model.Facts.Project { OrganizationUnitId = 1 })
                    .Has(new Storage.Model.Facts.Firm { Id = 1, OrganizationUnitId = 1 });

            Factory.CreateFirmAggregateActor(Query)
                   .Initialize<Firm>(1)
                   .Verify<Firm>(m => m.Add(It.Is(Predicate.Match(new Firm { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeFirmHavingBalance()
        {
            SourceDb.Has(new Storage.Model.Facts.Project { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Storage.Model.Facts.Client { Id = 1 })
                    .Has(new Storage.Model.Facts.BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Storage.Model.Facts.LegalPerson { Id = 1, ClientId = 1 })
                    .Has(new Storage.Model.Facts.Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1, Balance = 123.45m })
                    .Has(new Storage.Model.Facts.Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Factory.CreateFirmAggregateActor(Query)
                   .Initialize<Firm>(1)
                   .Verify<Firm>(m => m.Add(It.Is(Predicate.Match(new Firm { Id = 1, ProjectId = 1, ClientId = 1 }))))
                   .Verify<FirmBalance>(m => m.Add(It.Is(Predicate.Match(new FirmBalance { ProjectId = 1, FirmId = 1, AccountId = 1, Balance = 123.45m }))));
        }

        [Test]
        public void ShouldInitializeFirmHavingLead()
        {
            SourceDb.Has(new Storage.Model.Facts.Project { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Storage.Model.Facts.Lead { Id = 1, FirmId = 1, IsInQueue = true, Type = 1})
                    .Has(new Storage.Model.Facts.Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Factory.CreateFirmAggregateActor(Query)
                   .Initialize<Firm>(1)
                   .Verify<Firm>(m => m.Add(It.Is(Predicate.Match(new Firm { Id = 1, ProjectId = 1, ClientId = 1 }))))
                   .Verify<FirmLead>(m => m.Add(It.Is(Predicate.Match(new FirmLead { FirmId = 1, LeadId = 1, IsInQueue = true, Type = 1 }))));
        }

        [Test]
        public void ShouldInitializeFirmHavingClient()
        {
            SourceDb.Has(new Storage.Model.Facts.Category { Id = 1, Level = 3 })
                    .Has(new Storage.Model.Facts.Project { OrganizationUnitId = 1 })
                    .Has(new Storage.Model.Facts.Client { Id = 1 })
                    .Has(new Storage.Model.Facts.Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Factory.CreateFirmAggregateActor(Query)
                   .Initialize<Firm>(1)
                   .Verify<Firm>(m => m.Add(It.Is(Predicate.Match(new Firm { Id = 1, ClientId = 1 }))))
                   .Verify<Client>(m => m.Add(It.Is(Predicate.Match(new Client { Id = 1 }))), Times.Never);
        }

        [Test]
        public void ShouldRecalculateFirm()
        {
            SourceDb.Has(new Storage.Model.Facts.Project { OrganizationUnitId = 1 })
                    .Has(new Storage.Model.Facts.Firm { Id = 1, OrganizationUnitId = 1 });

            TargetDb.Has(new Firm { Id = 1, Name = "name" });

            Factory.CreateFirmAggregateActor(Query)
                   .Recalculate<Firm>(1)
                   .Verify<Firm>(m => m.Update(It.Is(Predicate.Match(new Firm { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingBalance()
        {
            SourceDb.Has(new Storage.Model.Facts.Project { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Storage.Model.Facts.Client { Id = 1 },
                         new Storage.Model.Facts.Client { Id = 2 })
                    .Has(new Storage.Model.Facts.BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Storage.Model.Facts.LegalPerson { Id = 1, ClientId = 1 },
                         new Storage.Model.Facts.LegalPerson { Id = 2, ClientId = 2 })
                    .Has(new Storage.Model.Facts.Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1, Balance = 123 },
                         new Storage.Model.Facts.Account { Id = 2, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 1, Balance = 456 })
                    .Has(new Storage.Model.Facts.Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 },
                         new Storage.Model.Facts.Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 },
                         new Storage.Model.Facts.Firm { Id = 3, OrganizationUnitId = 1 });

            TargetDb.Has(new Firm { Id = 1 },
                         new Firm { Id = 2 },
                         new Firm { Id = 3 })
                    .Has(new FirmBalance { FirmId = 2, AccountId = 2, ProjectId = 1, Balance = 123 },
                         new FirmBalance { FirmId = 3, ProjectId = 1, Balance = 123 });

            Factory.CreateFirmAggregateActor(Query)
                   .Recalculate<Firm>(1, 2, 3)
                   .Verify<Firm>(m => m.Update(It.Is(Predicate.Match(new Firm { Id = 1, ClientId = 1, ProjectId = 1 }))))
                   .Verify<Firm>(m => m.Update(It.Is(Predicate.Match(new Firm { Id = 2, ClientId = 2, ProjectId = 1 }))))
                   .Verify<Firm>(m => m.Update(It.Is(Predicate.Match(new Firm { Id = 3, ProjectId = 1 }))))
                   .Verify<FirmBalance>(m => m.Add(It.Is(Predicate.Match(new FirmBalance { FirmId = 1, AccountId = 1, ProjectId = 1, Balance = 123 }))))
                   .Verify<FirmBalance>(m => m.Add(It.Is(Predicate.Match(new FirmBalance { FirmId = 2, AccountId = 2, ProjectId = 1, Balance = 456 }))))
                   .Verify<FirmBalance>(m => m.Delete(It.Is(Predicate.Match(new FirmBalance { FirmId = 3, ProjectId = 1, Balance = 123 }))))
                   .Verify<FirmBalance>(m => m.Delete(It.Is(Predicate.Match(new FirmBalance { FirmId = 2, AccountId = 2, ProjectId = 1, Balance = 123 }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingCategory()
        {
            SourceDb.Has(new Storage.Model.Facts.Category { Id = 1, Level = 1 },
                         new Storage.Model.Facts.Category { Id = 2, Level = 1 },
                         new Storage.Model.Facts.Category { Id = 3, Level = 2, ParentId = 1 },
                         new Storage.Model.Facts.Category { Id = 4, Level = 2, ParentId = 2 },
                         new Storage.Model.Facts.Category { Id = 5, Level = 3, ParentId = 3 },
                         new Storage.Model.Facts.Category { Id = 6, Level = 3, ParentId = 4 });
            SourceDb.Has(new Storage.Model.Facts.CategoryOrganizationUnit { Id = 1, CategoryId = 5, OrganizationUnitId = 1 },
                         new Storage.Model.Facts.CategoryOrganizationUnit { Id = 2, CategoryId = 6, OrganizationUnitId = 1 });
            SourceDb.Has(new Storage.Model.Facts.Project { OrganizationUnitId = 1 });
            SourceDb.Has(new Storage.Model.Facts.FirmAddress { Id = 1, FirmId = 1 },
                         new Storage.Model.Facts.FirmAddress { Id = 2, FirmId = 2 });
            SourceDb.Has(new Storage.Model.Facts.CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 5 },
                         new Storage.Model.Facts.CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 6 });
            SourceDb.Has(new Storage.Model.Facts.Firm { Id = 1, OrganizationUnitId = 1 },
                         new Storage.Model.Facts.Firm { Id = 2, OrganizationUnitId = 1 },
                         new Storage.Model.Facts.Firm { Id = 3, OrganizationUnitId = 1 });

            TargetDb.Has(new Firm { Id = 1, Name = "name" },
                         new Firm { Id = 2, Name = "name" },
                         new Firm { Id = 3, Name = "name" })
                    .Has(new FirmCategory1 { FirmId = 1, CategoryId = 2 },
                         new FirmCategory1 { FirmId = 2, CategoryId = 1 })
                    .Has(new FirmCategory2 { FirmId = 1, CategoryId = 4 },
                         new FirmCategory2 { FirmId = 2, CategoryId = 3 });

            Factory.CreateFirmAggregateActor(Query)
                   .Recalculate<Firm>(1)
                   .Recalculate<Firm>(2)
                   .Recalculate<Firm>(3)
                   .Verify<Firm>(m => m.Update(It.Is(Predicate.Match(new Firm { Id = 1, AddressCount = 1 }))))
                   .Verify<Firm>(m => m.Update(It.Is(Predicate.Match(new Firm { Id = 2, AddressCount = 1 }))))
                   .Verify<Firm>(m => m.Update(It.Is(Predicate.Match(new Firm { Id = 3 }))))
                   .Verify<FirmCategory1>(m => m.Add(It.Is(Predicate.Match(new FirmCategory1 { FirmId = 1, CategoryId = 1 }))))
                   .Verify<FirmCategory1>(m => m.Add(It.Is(Predicate.Match(new FirmCategory1 { FirmId = 2, CategoryId = 2 }))))
                   .Verify<FirmCategory1>(m => m.Delete(It.Is(Predicate.Match(new FirmCategory1 { FirmId = 1, CategoryId = 2 }))))
                   .Verify<FirmCategory1>(m => m.Delete(It.Is(Predicate.Match(new FirmCategory1 { FirmId = 2, CategoryId = 1 }))))
                   .Verify<FirmCategory2>(m => m.Add(It.Is(Predicate.Match(new FirmCategory2 { FirmId = 1, CategoryId = 3 }))))
                   .Verify<FirmCategory2>(m => m.Add(It.Is(Predicate.Match(new FirmCategory2 { FirmId = 2, CategoryId = 4 }))))
                   .Verify<FirmCategory2>(m => m.Delete(It.Is(Predicate.Match(new FirmCategory2 { FirmId = 1, CategoryId = 4 }))))
                   .Verify<FirmCategory2>(m => m.Delete(It.Is(Predicate.Match(new FirmCategory2 { FirmId = 2, CategoryId = 3 }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingClient()
        {
            SourceDb.Has(new Storage.Model.Facts.Category { Id = 1, Level = 3 })
                    .Has(new Storage.Model.Facts.Project { OrganizationUnitId = 1 })
                    .Has(new Storage.Model.Facts.Client { Id = 1 })
                    .Has(new Storage.Model.Facts.Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new Client { Id = 1 });

            Factory.CreateFirmAggregateActor(Query)
                   .Recalculate<Firm>(1)
                   .Verify<Firm>(m => m.Update(It.Is(Predicate.Match(new Firm { Id = 1, ClientId = 1 }))))
                   .Verify<Client>(m => m.Update(It.Is(Predicate.Match(new Client { Id = 1 }))), Times.Never);
        }

        [Test]
        public void ShouldRecalculateFirmHavingTerritory()
        {
            SourceDb.Has(new Storage.Model.Facts.Firm { Id = 1 })
                    .Has(new Storage.Model.Facts.FirmAddress { FirmId = 1, TerritoryId = 2 });

            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmTerritory { FirmId = 1, TerritoryId = 3 });

            Factory.CreateFirmAggregateActor(Query)
                   .Recalculate<Firm>(1)
                   .Verify<FirmTerritory>(m => m.Add(It.Is(Predicate.Match(new FirmTerritory { FirmId = 1, TerritoryId = 2 }))))
                   .Verify<FirmTerritory>(m => m.Delete(It.Is(Predicate.Match(new FirmTerritory { FirmId = 1, TerritoryId = 3 }))));
        }

        [Test]
        public void ShouldDestroyFirm()
        {
            TargetDb.Has(new Firm { Id = 1 });

            Factory.CreateFirmAggregateActor(Query)
                   .Destroy<Firm>(1)
                   .Verify<Firm>(m => m.Delete(It.Is(Predicate.Match(new Firm { Id = 1 }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingBalance()
        {
            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmBalance { FirmId = 1, Balance = 123 });

            Factory.CreateFirmAggregateActor(Query)
                   .Destroy<Firm>(1)
                   .Verify<Firm>(m => m.Delete(It.Is(Predicate.Match(new Firm { Id = 1 }))))
                   .Verify<FirmBalance>(m => m.Delete(It.Is(Predicate.Match(new FirmBalance { FirmId = 1, Balance = 123 }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingCategory()
        {
            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmCategory1 { FirmId = 1, CategoryId = 1 })
                    .Has(new FirmCategory2 { FirmId = 1, CategoryId = 2 });

            Factory.CreateFirmAggregateActor(Query)
                   .Destroy<Firm>(1)
                   .Verify<Firm>(m => m.Delete(It.Is(Predicate.Match(new Firm { Id = 1 }))))
                   .Verify<FirmCategory1>(m => m.Delete(It.Is(Predicate.Match(new FirmCategory1 { FirmId = 1, CategoryId = 1 }))))
                   .Verify<FirmCategory2>(m => m.Delete(It.Is(Predicate.Match(new FirmCategory2 { FirmId = 1, CategoryId = 2 }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingClient()
        {
            TargetDb.Has(new Firm { Id = 1, ClientId = 1 })
                    .Has(new Client { Id = 1 });

            Factory.CreateFirmAggregateActor(Query)
                   .Destroy<Firm>(1)
                   .Verify<Firm>(m => m.Delete(It.Is(Predicate.Match(new Firm { Id = 1, ClientId = 1 }))))
                   .Verify<Client>(m => m.Delete(It.Is(Predicate.Match(new Client { Id = 1 }))), Times.Never);
        }

        [Test]
        public void ShouldInitializeProject()
        {
            SourceDb.Has(new Storage.Model.Facts.Project { Id = 1 });

            Factory.CreateProjectAggregateActor(Query)
                   .Initialize<Project>(1)
                   .Verify<Project>(m => m.Add(It.Is(Predicate.Match(new Project { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateProject()
        {
            SourceDb.Has(new Storage.Model.Facts.Project { Id = 1, Name = "new name" });
            TargetDb.Has(new Project { Id = 1, Name = "old name" });

            Factory.CreateProjectAggregateActor(Query)
                   .Recalculate<Project>(1)
                   .Verify<Project>(m => m.Update(It.Is(Predicate.Match(new Project { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyProject()
        {
            TargetDb.Has(new Project { Id = 1 });

            Factory.CreateProjectAggregateActor(Query)
                   .Destroy<Project>(1)
                   .Verify<Project>(m => m.Delete(It.Is(Predicate.Match(new Project { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeTerritory()
        {
            SourceDb.Has(new Storage.Model.Facts.Project { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Storage.Model.Facts.Territory { Id = 2, OrganizationUnitId = 1 });

            Factory.CreateTerritoryAggregateActor(Query)
                   .Initialize<Territory>(2)
                   .Verify<Territory>(m => m.Add(It.Is(Predicate.Match(new Territory { Id = 2, ProjectId = 1 }))));
        }

        [Test]
        public void ShouldRecalculateTerritory()
        {
            SourceDb.Has(new Storage.Model.Facts.Project { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Storage.Model.Facts.Territory { Id = 1, OrganizationUnitId = 1, Name = "new name" });

            TargetDb.Has(new Territory { Id = 1, Name = "old name" });

            Factory.CreateTerritoryAggregateActor(Query)
                   .Recalculate<Territory>(1)
                   .Verify<Territory>(m => m.Update(It.Is(Predicate.Match(new Territory { Id = 1, ProjectId = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyTerritory()
        {
            TargetDb.Has(new Territory { Id = 1 });

            Factory.CreateTerritoryAggregateActor(Query)
                   .Destroy<Territory>(1)
                   .Verify<Territory>(m => m.Delete(It.Is(Predicate.Match(new Territory { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeCategoryGroup()
        {
            SourceDb.Has(new Storage.Model.Facts.CategoryGroup { Id = 1 });

            Factory.CreateCategoryGroupAggregateActor(Query)
                   .Initialize<CategoryGroup>(1)
                   .Verify<CategoryGroup>(m => m.Add(It.Is(Predicate.Match(new CategoryGroup { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateCategoryGroup()
        {
            SourceDb.Has(new Storage.Model.Facts.CategoryGroup { Id = 1, Name = "new name" });
            TargetDb.Has(new CategoryGroup { Id = 1, Name = "old name" });

            Factory.CreateCategoryGroupAggregateActor(Query)
                   .Recalculate<CategoryGroup>(1)
                   .Verify<CategoryGroup>(m => m.Update(It.Is(Predicate.Match(new CategoryGroup { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyCategoryGroup()
        {
            TargetDb.Has(new CategoryGroup { Id = 1 });

            Factory.CreateCategoryGroupAggregateActor(Query)
                   .Destroy<CategoryGroup>(1)
                   .Verify<CategoryGroup>(x => x.Delete(It.Is(Predicate.Match(new CategoryGroup { Id = 1 }))));
        }

        private class Factory
        {
            private static readonly EqualityComparerFactory ComparerFactory =
                new EqualityComparerFactory(new LinqToDbPropertyProvider(Schema.Erm, Schema.Facts, Schema.CustomerIntelligence));

            private readonly IAggregateActor _aggregateActor;
            private readonly VerifiableRepositoryFactory _repositoryFactory;

            private Factory(IAggregateRootActor aggregateRootActor, VerifiableRepositoryFactory repositoryFactory)
            {
                _repositoryFactory = repositoryFactory;
                _aggregateActor = new AggregateActor(aggregateRootActor);
            }

            public static Factory CreateClientAggregateActor(IQuery query)
            {
                var repositoryFactory = new VerifiableRepositoryFactory();
                var aggregateRootActor = new ClientAggregateRootActor(
                    query,
                    repositoryFactory.Create<Client>(),
                    repositoryFactory.Create<ClientContact>(),
                    ComparerFactory);
                return new Factory(aggregateRootActor, repositoryFactory);
            }

            public static Factory CreateFirmAggregateActor(IQuery query)
            {
                var repositoryFactory = new VerifiableRepositoryFactory();
                var firmAggregateRootActor = new FirmAggregateRootActor(
                    query,
                    repositoryFactory.Create<Firm>(),
                    repositoryFactory.Create<FirmActivity>(),
                    repositoryFactory.Create<FirmLead>(),
                    repositoryFactory.Create<FirmBalance>(),
                    repositoryFactory.Create<FirmCategory1>(),
                    repositoryFactory.Create<FirmCategory2>(),
                    repositoryFactory.Create<FirmTerritory>(),
                    ComparerFactory);
                return new Factory(firmAggregateRootActor, repositoryFactory);
            }

            public static Factory CreateProjectAggregateActor(IQuery query)
            {
                var repositoryFactory = new VerifiableRepositoryFactory();
                var firmAggregateRootActor = new ProjectAggregateRootActor(
                    query,
                    repositoryFactory.Create<Project>(),
                    repositoryFactory.Create<ProjectCategory>(),
                    ComparerFactory);
                return new Factory(firmAggregateRootActor, repositoryFactory);
            }

            public static Factory CreateTerritoryAggregateActor(IQuery query)
            {
                var repositoryFactory = new VerifiableRepositoryFactory();
                var firmAggregateRootActor = new TerritoryAggregateRootActor(
                    query,
                    repositoryFactory.Create<Territory>(),
                    ComparerFactory);
                return new Factory(firmAggregateRootActor, repositoryFactory);
            }

            public static Factory CreateCategoryGroupAggregateActor(IQuery query)
            {
                var repositoryFactory = new VerifiableRepositoryFactory();
                var firmAggregateRootActor = new CategoryGroupAggregateRootActor(
                    query,
                    repositoryFactory.Create<CategoryGroup>(),
                    ComparerFactory);
                return new Factory(firmAggregateRootActor, repositoryFactory);
            }

            public Factory Initialize<TAggregateRoot>(params long[] ids) where TAggregateRoot : class
            {
                var commands = ids.Select(x => new InitializeAggregateCommand(typeof(TAggregateRoot), x)).ToArray();
                _aggregateActor.ExecuteCommands(commands);
                return this;
            }

            public Factory Recalculate<TAggregateRoot>(params long[] ids) where TAggregateRoot : class
            {
                var commands = ids.Select(x => new RecalculateAggregateCommand(typeof(TAggregateRoot), x)).ToArray();
                _aggregateActor.ExecuteCommands(commands);
                return this;
            }

            public Factory Destroy<TAggregateRoot>(params long[] ids) where TAggregateRoot : class
            {
                var commands = ids.Select(x => new DestroyAggregateCommand(typeof(TAggregateRoot), x)).ToArray();
                _aggregateActor.ExecuteCommands(commands);
                return this;
            }

            public Factory Verify<T>(Expression<Action<IRepository<T>>> expression)
                where T : class
            {
                return Verify<T>(expression, Times.Once, null);
            }

            public Factory Verify<T>(Expression<Action<IRepository<T>>> expression, Func<Times> times)
                where T : class
            {
                return Verify<T>(expression, times, null);
            }

            public Factory Verify<T>(Expression<Action<IRepository<T>>> expression, Func<Times> times, string failMessage)
                where T : class
            {
                _repositoryFactory.Verify(expression, times, failMessage);
                return this;
            }
        }
    }
}