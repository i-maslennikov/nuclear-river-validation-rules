using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Features;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;
using NuClear.CustomerIntelligence.Domain;
using NuClear.Metamodeling.Elements;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.Facts;
using NuClear.Storage.API.Readings;

using NUnit.Framework;

using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;
using CI = NuClear.CustomerIntelligence.Domain.Model.CI;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal partial class FactDependencyTests : TransformationFixtureBase
    {
        [Test]
        public void ShouldInitializeClientIfClientCreated()
        {
            SourceDb.Has(new Erm::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Client>(1)
                          .VerifyDistinct(Aggregate.Initialize<CI.Client>(1));
        }

        [Test]
        public void ShouldRecalculateClientIfClientUpdated()
        {
            SourceDb.Has(new Erm::Client { Id = 1 });
            TargetDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Client>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Client>(1));
        }

        [Test]
        public void ShouldDestroyClientIfClientDeleted()
        {
            TargetDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Client>(1)
                          .VerifyDistinct(Aggregate.Destroy<CI.Client>(1));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmCreated()
        {
            SourceDb.Has(new Erm::Firm { Id = 2, ClientId = 1 });
            TargetDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Firm>(2)
                          .VerifyDistinct(op => op is RecalculateAggregate, Aggregate.Recalculate<CI.Client>(1));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmUpdated()
        {
            SourceDb.Has(new Erm::Firm { Id = 2, ClientId = 3 });

            TargetDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Firm>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Client>(1), Aggregate.Recalculate<CI.Firm>(2), Aggregate.Recalculate<CI.Client>(3));
        }

        [Test]
        public void ShouldRecalculateClientIfFirmDeleted()
        {
            TargetDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Firm>(2)
                          .VerifyDistinct(op => op is RecalculateAggregate, Aggregate.Recalculate<CI.Client>(1));
        }

        [Test]
        public void ShouldInitializeFirmIfFirmCreated()
        {
            SourceDb.Has(new Erm::Firm { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Firm>(1)
                          .VerifyDistinct(Aggregate.Initialize<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmUpdated()
        {
            SourceDb.Has(new Erm::Firm { Id = 1 });
            TargetDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Firm>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldDestroyFirmIfFirmDeleted()
        {
            TargetDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Firm>(1)
                          .VerifyDistinct(Aggregate.Destroy<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountCreated()
        {
            SourceDb.Has(new Erm::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            TargetDb.Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Account>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountUpdated()
        {
            SourceDb.Has(new Erm::Account { Id = 1, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 1 });

            TargetDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::LegalPerson { Id = 2, ClientId = 2 })
                   .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Account>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfAccountDeleted()
        {
            TargetDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new Facts::Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Account>(5)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitCreated()
        {
            SourceDb.Has(new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            TargetDb.Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::BranchOfficeOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitCreated()
        {
            SourceDb.Has(new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            TargetDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::BranchOfficeOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitUpdated()
        {
            SourceDb.Has(new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 });

            TargetDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1, OrganizationUnitId = 2 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::BranchOfficeOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitUpdated()
        {
            SourceDb.Has(new Erm::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 });

            TargetDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1, OrganizationUnitId = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::BranchOfficeOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfBranchOfficeOrganizationUnitDeleted()
        {
            TargetDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new Facts::Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::BranchOfficeOrganizationUnit>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateDetachedFirmIfBranchOfficeOrganizationUnitDeleted()
        {
            TargetDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::BranchOfficeOrganizationUnit>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Created()
        {
            SourceDb.Has(new Erm::Category { Id = 3, Level = 3, ParentId = 2 });

            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Created()
        {
            SourceDb.Has(new Erm::Category { Id = 2, Level = 2, ParentId = 1 });

            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Created()
        {
            SourceDb.Has(new Erm::Category { Id = 1, Level = 1 });

            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Updated()
        {
            SourceDb.Has(new Erm::Firm { Id = 1 })
                 .Has(new Erm::FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Erm::Category { Id = 1, Level = 1 },
                      new Erm::Category { Id = 2, Level = 2, ParentId = 1 },
                      new Erm::Category { Id = 3, Level = 3, ParentId = 2 });

            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Updated()
        {
            SourceDb.Has(new Erm::Firm { Id = 1 })
                 .Has(new Erm::FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Erm::Category { Id = 1, Level = 1 },
                      new Erm::Category { Id = 2, Level = 2, ParentId = 1 });

            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Updated()
        {
            SourceDb.Has(new Erm::Firm { Id = 1 })
                 .Has(new Erm::FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Erm::Category { Id = 1, Level = 1 });

            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel3Deleted()
        {
            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel2Deleted()
        {
            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOfLevel1Deleted()
        {
            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 3 })
                   .Has(new Facts::Category { Id = 1, Level = 1 })
                   .Has(new Facts::Category { Id = 2, Level = 2, ParentId = 1 })
                   .Has(new Facts::Category { Id = 3, Level = 3, ParentId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Category>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressCreated()
        {
            SourceDb.Has(new Erm::Firm { Id = 1 })
                 .Has(new Erm::FirmAddress { Id = 2, FirmId = 1 })
                 .Has(new Erm::CategoryFirmAddress { Id = 3, FirmAddressId = 2 });

            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryFirmAddress>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressUpdated()
        {
            SourceDb.Has(new Erm::Firm { Id = 1 }, new Erm::Firm { Id = 2 })
                 .Has(new Erm::FirmAddress { Id = 1, FirmId = 1 }, new Erm::FirmAddress { Id = 2, FirmId = 2 })
                 .Has(new Erm::CategoryFirmAddress { Id = 1, FirmAddressId = 2 });

            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryFirmAddress>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryFirmAddressDeleted()
        {
            SourceDb.Has(new Erm::Firm { Id = 1 })
                 .Has(new Erm::FirmAddress { Id = 2, FirmId = 1 });

            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 3, FirmAddressId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryFirmAddress>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitCreated()
        {
            SourceDb.Has(new Erm::CategoryOrganizationUnit { Id = 6, OrganizationUnitId = 1, CategoryId = 2 });

            TargetDb.Has(new Facts::Firm { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::FirmAddress { Id = 4, FirmId = 3 })
                   .Has(new Facts::CategoryFirmAddress { Id = 5, FirmAddressId = 4, CategoryId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryOrganizationUnit>(6)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(3));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitUpdated()
        {
            SourceDb.Has(new Erm::CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 2, CategoryId = 1 });

            TargetDb.Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, OrganizationUnitId = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                   .Has(new Facts::CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 1 })
                   .Has(new Facts::CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1, CategoryId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfCategoryOrganizationUnitDeleted()
        {
            TargetDb.Has(new Facts::Firm { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::FirmAddress { Id = 4, FirmId = 3 })
                   .Has(new Facts::CategoryFirmAddress { Id = 5, FirmAddressId = 4, CategoryId = 2 })
                   .Has(new Facts::CategoryOrganizationUnit { Id = 6, OrganizationUnitId = 1, CategoryId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryOrganizationUnit>(6)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(3));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientCreated()
        {
            SourceDb.Has(new Erm::Client { Id = 1 });

            TargetDb.Has(new Facts::Firm { Id = 1, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Client>(1)
                          .VerifyDistinct(op => op is RecalculateAggregate, Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientUpdated()
        {
            SourceDb.Has(new Erm::Client { Id = 1 });

            TargetDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Client>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Client>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfClientDeleted()
        {
            TargetDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Client>(1)
                          .VerifyDistinct(op => op is RecalculateAggregate, Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactCreated()
        {
            SourceDb.Has(new Erm::Contact { Id = 3, ClientId = 1 });

            TargetDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Contact>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Client>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactUpdated()
        {
            SourceDb.Has(new Erm::Contact { Id = 1, ClientId = 2 });

            TargetDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 2 })
                   .Has(new Facts::Contact { Id = 1, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Contact>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Client>(1), Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Client>(2), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfContactDeleted()
        {
            TargetDb.Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 1 })
                   .Has(new Facts::Contact { Id = 3, ClientId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Contact>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Client>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressCreated()
        {
            SourceDb.Has(new Erm::FirmAddress { Id = 2, FirmId = 1 });
            TargetDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmAddress>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressUpdated()
        {
            SourceDb.Has(new Erm::FirmAddress { Id = 1, FirmId = 2 });
            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmAddress>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmAddressDeleted()
        {
            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmAddress>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactCreated()
        {
            SourceDb.Has(new Erm::FirmContact { Id = 3, FirmAddressId = 2, ContactType = 1 });

            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmContact>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactUpdated()
        {
            SourceDb.Has(new Erm::FirmContact { Id = 1, FirmAddressId = 2, ContactType = 1 });

            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 2 })
                   .Has(new Facts::FirmContact { Id = 1, FirmAddressId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmContact>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfFirmContactDeleted()
        {
            SourceDb.Has(new Erm::FirmContact { Id = 3, FirmAddressId = 2 });

            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::FirmAddress { Id = 2, FirmId = 1 })
                   .Has(new Facts::FirmContact { Id = 3, FirmAddressId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::FirmContact>(3)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonCreated()
        {
            SourceDb.Has((new Erm::LegalPerson { Id = 1, ClientId = 1 }));

            TargetDb.Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::LegalPerson>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonUpdated()
        {
            SourceDb.Has(new Erm::LegalPerson { Id = 1, ClientId = 2 });

            TargetDb.Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                   .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::LegalPerson>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfLegalPersonDeleted()
        {
            TargetDb.Has(new Facts::Firm { Id = 1, ClientId = 2, OrganizationUnitId = 1 })
                   .Has(new Facts::Client { Id = 2 })
                   .Has(new Facts::BranchOfficeOrganizationUnit { Id = 3, OrganizationUnitId = 1 })
                   .Has(new Facts::LegalPerson { Id = 4, ClientId = 2 })
                   .Has(new Facts::Account { Id = 5, LegalPersonId = 4, BranchOfficeOrganizationUnitId = 3 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::LegalPerson>(4)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderCreated()
        {
            SourceDb.Has(new Erm::Order { Id = 2, FirmId = 1, WorkflowStepId = 4 });
            TargetDb.Has(new Facts::Firm { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Order>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderUpdated()
        {
            SourceDb.Has(new Erm::Order { Id = 1, FirmId = 2, WorkflowStepId = 4 });

            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Firm { Id = 2 })
                   .Has(new Facts::Order { Id = 1, FirmId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Order>(1)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1), Aggregate.Recalculate<CI.Firm>(2));
        }

        [Test]
        public void ShouldRecalculateFirmIfOrderDeleted()
        {
            TargetDb.Has(new Facts::Firm { Id = 1 })
                   .Has(new Facts::Order { Id = 2, FirmId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Order>(2)
                          .VerifyDistinct(Aggregate.Recalculate<CI.Firm>(1));
        }

        #region Transformation

        private class Transformation
        {
            private readonly IQuery _query;
            private readonly IRepositoryFactory _repositoryFactory;
            private readonly List<IOperation> _operations;
            private readonly FactsReplicationMetadataSource _metadataSource;

            private Transformation(IQuery query, IRepositoryFactory repositoryFactory)
            {
                _query = query;
                _repositoryFactory = repositoryFactory;
                _operations = new List<IOperation>();
                _metadataSource = new FactsReplicationMetadataSource();
            }

            public static Transformation Create(IQuery query, IRepositoryFactory repositoryFactory)
            {
                return new Transformation(query, repositoryFactory);
            }

            public Transformation ApplyChanges<TFact>(params long[] ids)
                where TFact : class, IIdentifiable<long>, IFactObject
            {
                var factType = typeof(TFact);

                IMetadataElement factMetadata;
                if (!_metadataSource.Metadata.Values.TryGetElementById(new Uri(factType.Name, UriKind.Relative), out factMetadata))
                {
                    throw new NotSupportedException(string.Format("The fact of type '{0}' is not supported.", factType));
                }
                
                var repository = _repositoryFactory.Create<TFact>();
                var factory = new Factory<TFact>(_query, repository);
                var processor = factory.Create(factMetadata);

                _operations.AddRange(processor.ApplyChanges(ids));

                return this;
            }

            public void VerifyDistinct(params IOperation[] operations)
            {
                Assert.That(_operations.Distinct(), Is.EquivalentTo(operations));
            }

            public void VerifyDistinct(Func<IOperation, bool> filter, params IOperation[] operations)
            {
                Assert.That(_operations.Distinct().Where(filter), Is.EquivalentTo(operations));
            }

            private class Factory<TFact> : IFactProcessorFactory, IFactDependencyProcessorFactory
                where TFact : class, IIdentifiable<long>, IFactObject
            {
                private readonly IQuery _query;
                private readonly IBulkRepository<TFact> _repository;

                public Factory(IQuery query, IBulkRepository<TFact> repository)
                {
                    _query = query;
                    _repository = repository;
                }

                public IFactProcessor Create(IMetadataElement factMetadata)
                {
                    return new FactProcessor<TFact>((FactMetadata<TFact>)factMetadata, this, _query, _repository);
                }

                public IFactDependencyProcessor Create(IFactDependencyFeature metadata)
                {
                    return new FactDependencyProcessor<TFact>((IFactDependencyFeature<TFact, long>)metadata, _query);
                }
            }
        }

        #endregion
    }
}