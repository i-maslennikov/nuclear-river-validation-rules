using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Actors
{
    [TestFixture]
    internal partial class SyncDataObjectsActorTests : ActorFixtureBase
    {
        [Test]
        public void ShouldInitializeClientIfClientCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.Client { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Client>(1)
                 .VerifyDistinct(DataObject.Created<Client>(1));
        }

        [Test]
        public void ShouldRecalculateClientIfClientUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.Client { Id = 1, Name = "asdf" });
            TargetDb.Has(new Client { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Client>(1)
                 .VerifyDistinct(DataObject.Updated<Client>(1));
        }

        [Test]
        public void ShouldDestroyClientIfClientDeleted()
        {
            TargetDb.Has(new Client { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Client>(1)
                 .VerifyDistinct(DataObject.Deleted<Client>(1));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.Firm { Id = 2, ClientId = 1 });
            TargetDb.Has(new Client { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Firm>(2)
                 .VerifyDistinct(e => e is RelatedDataObjectOutdatedEvent<long>, DataObject.RelatedDataObjectOutdated<Client>(1));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.Firm { Id = 2, ClientId = 3 });

            TargetDb.Has(new Client { Id = 1 })
                    .Has(new Firm { Id = 2, ClientId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Firm>(2)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Client>(1),
                                 DataObject.Updated<Firm>(2),
                                 DataObject.RelatedDataObjectOutdated<Client>(3));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmDeleted()
        {
            TargetDb.Has(new Client { Id = 1 })
                    .Has(new Firm { Id = 2, ClientId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Firm>(2)
                 .VerifyDistinct(e => e is RelatedDataObjectOutdatedEvent<long>, DataObject.RelatedDataObjectOutdated<Client>(1));
        }

        [Test]
        public void ShouldInitializeFirmIfFirmCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.Firm { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Firm>(1)
                 .VerifyDistinct(DataObject.Created<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.Firm { Id = 1, Name = "asdf" });
            TargetDb.Has(new Firm { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                          .Sync<Firm>(1)
                          .VerifyDistinct(DataObject.Updated<Firm>(1));
        }

        [Test]
        public void ShouldDestroyFirmIfFirmDeleted()
        {
            TargetDb.Has(new Firm { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                          .Sync<Firm>(1)
                          .VerifyDistinct(DataObject.Deleted<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            TargetDb.Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Client { Id = 1 })
                    .Has(new LegalPerson { Id = 1, ClientId = 1 })
                    .Has(new Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Account>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.Account { Id = 1, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 1 });

            TargetDb.Has(new Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                    .Has(new Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 })
                    .Has(new Client { Id = 1 })
                    .Has(new Client { Id = 2 })
                    .Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                    .Has(new LegalPerson { Id = 1, ClientId = 1 })
                    .Has(new LegalPerson { Id = 2, ClientId = 2 })
                    .Has(new Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Account>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1), DataObject.RelatedDataObjectOutdated<Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountDeleted()
        {
            TargetDb.Has(new Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                    .Has(new Client { Id = 2 })
                    .Has(new BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                    .Has(new LegalPerson { Id = 4, ClientId = 2 })
                    .Has(new Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Account>(5)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            TargetDb.Has(new Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 })
                   .Has(new Client { Id = 1 })
                   .Has(new LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<BranchOfficeOrganizationUnit>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            TargetDb.Has(new Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<BranchOfficeOrganizationUnit>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 });

            TargetDb.Has(new Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                    .Has(new Firm { Id = 2, ClientId = 1, OrganizationUnitId = 2 })
                    .Has(new Client { Id = 1 })
                    .Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                    .Has(new LegalPerson { Id = 1, ClientId = 1 })
                    .Has(new Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<BranchOfficeOrganizationUnit>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1), DataObject.RelatedDataObjectOutdated<Firm>(2));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 });

            TargetDb.Has(new Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                    .Has(new Firm { Id = 2, ClientId = 1, OrganizationUnitId = 2 })
                    .Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<BranchOfficeOrganizationUnit>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1), DataObject.RelatedDataObjectOutdated<Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitDeleted()
        {
            TargetDb.Has(new Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                    .Has(new Client { Id = 2 })
                    .Has(new BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                    .Has(new LegalPerson { Id = 4, ClientId = 2 })
                    .Has(new Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Actor.Create(Query, RepositoryFactory)
                          .Sync<BranchOfficeOrganizationUnit>(3)
                          .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitDeleted()
        {
            TargetDb.Has(new Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<BranchOfficeOrganizationUnit>(3)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Created()
        {
            SourceDb.Has(new Storage.Model.Erm.Category { Id = 3, Level = 3, ParentId = 2 });

            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                    .Has(new Category { Id = 1, Level = 1 })
                    .Has(new Category { Id = 2, Level = 2, ParentId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Category>(3)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Created()
        {
            SourceDb.Has(new Storage.Model.Erm.Category { Id = 2, Level = 2, ParentId = 1 });

            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                    .Has(new Category { Id = 1, Level = 1 })
                    .Has(new Category { Id = 3, Level = 3, ParentId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Category>(2)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Created()
        {
            SourceDb.Has(new Storage.Model.Erm.Category { Id = 1, Level = 1 });

            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                    .Has(new Category { Id = 2, Level = 2, ParentId = 1 })
                    .Has(new Category { Id = 3, Level = 3, ParentId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Category>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Updated()
        {
            SourceDb.Has(new Storage.Model.Erm.Firm { Id = 1 })
                    .Has(new Storage.Model.Erm.FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new Storage.Model.Erm.Category { Id = 1, Level = 1 },
                         new Storage.Model.Erm.Category { Id = 2, Level = 2, ParentId = 1 },
                         new Storage.Model.Erm.Category { Id = 3, Level = 3, ParentId = 2, Name = "asdf" });

            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                    .Has(new Category { Id = 1, Level = 1 })
                    .Has(new Category { Id = 2, Level = 2, ParentId = 1 })
                    .Has(new Category { Id = 3, Level = 3, ParentId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Category>(3)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Updated()
        {
            SourceDb.Has(new Storage.Model.Erm.Firm { Id = 1 })
                    .Has(new Storage.Model.Erm.FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new Storage.Model.Erm.Category { Id = 1, Level = 1 },
                         new Storage.Model.Erm.Category { Id = 2, Level = 2, ParentId = 1, Name = "asdf" });

            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                    .Has(new Category { Id = 1, Level = 1 })
                    .Has(new Category { Id = 2, Level = 2, ParentId = 1 })
                    .Has(new Category { Id = 3, Level = 3, ParentId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Category>(2)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Updated()
        {
            SourceDb.Has(new Storage.Model.Erm.Firm { Id = 1 })
                    .Has(new Storage.Model.Erm.FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new Storage.Model.Erm.Category { Id = 1, Level = 1, Name = "asdf" });

            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                    .Has(new Category { Id = 1, Level = 1 })
                    .Has(new Category { Id = 2, Level = 2, ParentId = 1 })
                    .Has(new Category { Id = 3, Level = 3, ParentId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Category>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Deleted()
        {
            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                    .Has(new Category { Id = 1, Level = 1 })
                    .Has(new Category { Id = 2, Level = 2, ParentId = 1 })
                    .Has(new Category { Id = 3, Level = 3, ParentId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Category>(3)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Deleted()
        {
            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                    .Has(new Category { Id = 1, Level = 1 })
                    .Has(new Category { Id = 2, Level = 2, ParentId = 1 })
                    .Has(new Category { Id = 3, Level = 3, ParentId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Category>(2)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Deleted()
        {
            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                    .Has(new Category { Id = 1, Level = 1 })
                    .Has(new Category { Id = 2, Level = 2, ParentId = 1 })
                    .Has(new Category { Id = 3, Level = 3, ParentId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Category>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.Firm { Id = 1 })
                    .Has(new Storage.Model.Erm.FirmAddress { Id = 2, FirmId = 1 })
                    .Has(new Storage.Model.Erm.CategoryFirmAddress { Id = 3, FirmAddressId = 2 });

            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 2, FirmId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryFirmAddress>(3)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.Firm { Id = 1 }, new Storage.Model.Erm.Firm { Id = 2 })
                    .Has(new Storage.Model.Erm.FirmAddress { Id = 1, FirmId = 1 }, new Storage.Model.Erm.FirmAddress { Id = 2, FirmId = 2 })
                    .Has(new Storage.Model.Erm.CategoryFirmAddress { Id = 1, FirmAddressId = 2 });

            TargetDb.Has(new Firm { Id = 1 })
                   .Has(new Firm { Id = 2 })
                   .Has(new FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryFirmAddress>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1), DataObject.RelatedDataObjectOutdated<Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressDeleted()
        {
            SourceDb.Has(new Storage.Model.Erm.Firm { Id = 1 })
                    .Has(new Storage.Model.Erm.FirmAddress { Id = 2, FirmId = 1 });

            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 2, FirmId = 1 })
                    .Has(new CategoryFirmAddress { Id = 3, FirmAddressId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryFirmAddress>(3)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.CategoryOrganizationUnit { Id = 6, OrganizationUnitId = 1, CategoryId = 2 });

            TargetDb.Has(new Firm { Id = 3, OrganizationUnitId = 1 })
                    .Has(new FirmAddress { Id = 4, FirmId = 3 })
                    .Has(new CategoryFirmAddress { Id = 5, FirmAddressId = 4, CategoryId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryOrganizationUnit>(6)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(3));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 2, CategoryId = 1 });

            TargetDb.Has(new Firm { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Firm { Id = 2, OrganizationUnitId = 2 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new FirmAddress { Id = 2, FirmId = 2 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                    .Has(new CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 1 })
                    .Has(new CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1, CategoryId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryOrganizationUnit>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1), DataObject.RelatedDataObjectOutdated<Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitDeleted()
        {
            TargetDb.Has(new Firm { Id = 3, OrganizationUnitId = 1 })
                    .Has(new FirmAddress { Id = 4, FirmId = 3 })
                    .Has(new CategoryFirmAddress { Id = 5, FirmAddressId = 4, CategoryId = 2 })
                    .Has(new CategoryOrganizationUnit { Id = 6, OrganizationUnitId = 1, CategoryId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryOrganizationUnit>(6)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(3));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.Client { Id = 1 });

            TargetDb.Has(new Firm { Id = 1, ClientId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Client>(1)
                 .VerifyDistinct(op => op is RelatedDataObjectOutdatedEvent<long>, DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.Client { Id = 1, Name = "asdf" });

            TargetDb.Has(new Client { Id = 1 })
                    .Has(new Firm { Id = 1, ClientId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Client>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1), DataObject.Updated<Client>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientDeleted()
        {
            TargetDb.Has(new Client { Id = 1 })
                    .Has(new Firm { Id = 2, ClientId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Client>(1)
                 .VerifyDistinct(op => op is RelatedDataObjectOutdatedEvent<long>, DataObject.RelatedDataObjectOutdated<Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.Contact { Id = 3, ClientId = 1 });

            TargetDb.Has(new Client { Id = 1 })
                    .Has(new Firm { Id = 2, ClientId = 1 });

            Actor.Create(Query, RepositoryFactory)
                          .Sync<Contact>(3)
                          .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Client>(1), DataObject.RelatedDataObjectOutdated<Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.Contact { Id = 1, ClientId = 2 });

            TargetDb.Has(new Client { Id = 1 })
                    .Has(new Client { Id = 2 })
                    .Has(new Firm { Id = 1, ClientId = 1 })
                    .Has(new Firm { Id = 2, ClientId = 2 })
                    .Has(new Contact { Id = 1, ClientId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Contact>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Client>(1),
                                 DataObject.RelatedDataObjectOutdated<Firm>(1),
                                 DataObject.RelatedDataObjectOutdated<Client>(2),
                                 DataObject.RelatedDataObjectOutdated<Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactDeleted()
        {
            TargetDb.Has(new Client { Id = 1 })
                    .Has(new Firm { Id = 2, ClientId = 1 })
                    .Has(new Contact { Id = 3, ClientId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Contact>(3)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Client>(1), DataObject.RelatedDataObjectOutdated<Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.FirmAddress { Id = 2, FirmId = 1 });
            TargetDb.Has(new Firm { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<FirmAddress>(2)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.FirmAddress { Id = 1, FirmId = 2 });
            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new Firm { Id = 2 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<FirmAddress>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1), DataObject.RelatedDataObjectOutdated<Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressDeleted()
        {
            TargetDb.Has(new Firm { Id = 1 })
                   .Has(new FirmAddress { Id = 2, FirmId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<FirmAddress>(2)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.FirmContact { Id = 3, FirmAddressId = 2, ContactType = 1 });

            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 2, FirmId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<FirmContact>(3)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.FirmContact { Id = 1, FirmAddressId = 2, ContactType = 1 });

            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new Firm { Id = 2 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new FirmAddress { Id = 2, FirmId = 2 })
                    .Has(new FirmContact { Id = 1, FirmAddressId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<FirmContact>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1), DataObject.RelatedDataObjectOutdated<Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactDeleted()
        {
            SourceDb.Has(new Storage.Model.Erm.FirmContact { Id = 3, FirmAddressId = 2 });

            TargetDb.Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 2, FirmId = 1 })
                    .Has(new FirmContact { Id = 3, FirmAddressId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<FirmContact>(3)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonCreated()
        {
            SourceDb.Has((new Storage.Model.Erm.LegalPerson { Id = 1, ClientId = 1 }));

            TargetDb.Has(new Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 })
                    .Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Client { Id = 1 })
                    .Has(new Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<LegalPerson>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.LegalPerson { Id = 1, ClientId = 2 });

            TargetDb.Has(new Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                    .Has(new Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 })
                    .Has(new Client { Id = 1 })
                    .Has(new Client { Id = 2 })
                    .Has(new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                    .Has(new LegalPerson { Id = 1, ClientId = 1 })
                    .Has(new Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<LegalPerson>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1), DataObject.RelatedDataObjectOutdated<Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonDeleted()
        {
            TargetDb.Has(new Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                    .Has(new Client { Id = 2 })
                    .Has(new BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                    .Has(new LegalPerson { Id = 4, ClientId = 2 })
                    .Has(new Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<LegalPerson>(4)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.Order { Id = 2, FirmId = 1, WorkflowStepId = 4 });
            TargetDb.Has(new Firm { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Order>(2)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.Order { Id = 1, FirmId = 2, WorkflowStepId = 4 });

            TargetDb.Has(new Firm { Id = 1 })
                   .Has(new Firm { Id = 2 })
                   .Has(new Order { Id = 1, FirmId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Order>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1), DataObject.RelatedDataObjectOutdated<Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderDeleted()
        {
            TargetDb.Has(new Firm { Id = 1 })
                   .Has(new Order { Id = 2, FirmId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Order>(2)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1));
        }

        private class Actor
        {
            private static readonly IReadOnlyDictionary<Type, Type> AccessorTypes = GetImplementations(typeof(IStorageBasedDataObjectAccessor<>));
            private static readonly IReadOnlyDictionary<Type, Type> DataChangesHandlerTypes = GetImplementations(typeof(IDataChangesHandler<>));

            private readonly IQuery _query;
            private readonly IRepositoryFactory _repositoryFactory;
            private readonly EqualityComparerFactory _comparerFactory;
            private readonly List<IEvent> _events;

            private Actor(IQuery query, IRepositoryFactory repositoryFactory)
            {
                _query = query;
                _repositoryFactory = repositoryFactory;
                _events = new List<IEvent>();
                _comparerFactory = new EqualityComparerFactory(new LinqToDbPropertyProvider(Schema.Erm, Schema.Facts, Schema.CustomerIntelligence));
            }

            public static Actor Create(IQuery query, IRepositoryFactory repositoryFactory)
            {
                return new Actor(query, repositoryFactory);
            }

            public Actor Sync<TDataObject>(params long[] ids) where TDataObject : class
            {
                var commands = ids.Select(x => new SyncDataObjectCommand(typeof(TDataObject), x)).ToArray();
                return Sync<TDataObject>(commands);
            }

            public Actor Sync<TDataObject>(params SyncDataObjectCommand[] commands) where TDataObject : class
            {
                var accessor = CreateAccessor<TDataObject>(_query);
                var dataChangesHandler = CreateDataChangesHandler<TDataObject>(_query);
                var actor = new SyncDataObjectsActor<TDataObject>(_query, _repositoryFactory.Create<TDataObject>(), _comparerFactory, accessor, dataChangesHandler);

                _events.AddRange(actor.ExecuteCommands(commands));

                return this;
            }

            public void VerifyDistinct(params IEvent[] events)
            {
                Assert.That(_events.Distinct(), Is.EquivalentTo(events));
            }

            public void VerifyDistinct(Func<IEvent, bool> filter, params IEvent[] operations)
            {
                Assert.That(_events.Distinct().Where(filter), Is.EquivalentTo(operations));
            }

            private static IStorageBasedDataObjectAccessor<TDataObject> CreateAccessor<TDataObject>(IQuery query)
            {
                var accessorType = AccessorTypes[typeof(TDataObject)];
                var accessorInstance = Activator.CreateInstance(accessorType, query);
                return (IStorageBasedDataObjectAccessor<TDataObject>)accessorInstance;
            }

            private static IDataChangesHandler<TDataObject> CreateDataChangesHandler<TDataObject>(IQuery query)
            {
                var dataChangesHandlerType = DataChangesHandlerTypes[typeof(TDataObject)];
                var dataChangesHandlerInstance = Activator.CreateInstance(dataChangesHandlerType, query);
                return (IDataChangesHandler<TDataObject>)dataChangesHandlerInstance;
            }

            private static IReadOnlyDictionary<Type, Type> GetImplementations(Type interfaceType)
            {
                return (from type in typeof(Specs).Assembly.ExportedTypes
                        from @interface in type.GetInterfaces()
                        where @interface.IsGenericType && @interface.GetGenericTypeDefinition() == interfaceType
                        select new { GenericArgument = @interface.GetGenericArguments()[0], Type = type })
                    .ToDictionary(x => x.GenericArgument, x => x.Type);
            }
        }
    }
}