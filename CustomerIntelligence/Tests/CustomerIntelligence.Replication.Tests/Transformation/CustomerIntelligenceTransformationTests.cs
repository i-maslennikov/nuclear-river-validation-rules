using System;
using System.Linq;
using System.Linq.Expressions;

using Moq;

using NuClear.CustomerIntelligence.Domain;
using NuClear.CustomerIntelligence.Domain.EntityTypes;
using NuClear.CustomerIntelligence.Storage;
using NuClear.Metamodeling.Elements;
using NuClear.Model.Common.Entities;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Writings;

using NUnit.Framework;

using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
using CI = NuClear.CustomerIntelligence.Domain.Model.CI;

namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal class CustomerIntelligenceTransformationTests : TransformationFixtureBase
    {
        [Test]
        public void ShouldInitializeClient()
        {
            SourceDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query)
                          .Initialize<CI::Client>(EntityTypeClient.Instance, 1)
                          .Verify<CI::Client>(m => m.Add(It.Is(Predicate.Match(new CI::Client { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeClientHavingContact()
        {
            SourceDb.Has(new Facts::Client { Id = 1 })
                    .Has(new Facts::Contact { Id = 1, ClientId = 1 });

            Transformation.Create(Query)
                          .Initialize<CI::Client>(EntityTypeClient.Instance, 1)
                          .Verify<CI::Client>(m => m.Add(It.Is(Predicate.Match(new CI::Client { Id = 1 }))))
                          .Verify<CI::ClientContact>(m => m.Add(It.Is(Predicate.Match(new CI::ClientContact { ClientId = 1, ContactId = 1 }))));
        }

        [Test]
        public void ShouldRecalculateClient()
        {
            SourceDb.Has(new Facts::Client { Id = 1, Name = "new name" });
            TargetDb.Has(new CI::Client { Id = 1, Name = "old name" });

            Transformation.Create(Query)
                          .Recalculate<CI::Client>(EntityTypeClient.Instance, 1)
                          .Verify<CI::Client>(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldRecalculateClientHavingContact()
        {
            SourceDb.Has(new Facts::Client { Id = 1 },
                         new Facts::Client { Id = 2 },
                         new Facts::Client { Id = 3 })
                    .Has(new Facts::Contact { Id = 1, ClientId = 1 },
                         new Facts::Contact { Id = 2, ClientId = 2 });
            TargetDb.Has(new CI::Client { Id = 1 },
                         new CI::Client { Id = 2 },
                         new CI::Client { Id = 3 })
                    .Has(new CI::ClientContact { ClientId = 2, ContactId = 2 },
                         new CI::ClientContact { ClientId = 3, ContactId = 3 });

            Transformation.Create(Query)
                          .Recalculate<CI::Client>(EntityTypeClient.Instance, 1)
                          .Recalculate<CI::Client>(EntityTypeClient.Instance, 2)
                          .Recalculate<CI::Client>(EntityTypeClient.Instance, 3)
                          .Verify<CI::Client>(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 1 }))))
                          .Verify<CI::Client>(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 2 }))))
                          .Verify<CI::Client>(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 3 }))))
                          .Verify<CI::ClientContact>(m => m.Add(It.Is(Predicate.Match(new CI::ClientContact { ClientId = 1, ContactId = 1 }))))
                          .Verify<CI::ClientContact>(m => m.Delete(It.Is(Predicate.Match(new CI::ClientContact { ClientId = 3, ContactId = 3 }))));
        }

        [Test]
        public void ShouldDestroyClient()
        {
            TargetDb.Has(new CI::Client { Id = 1 });

            Transformation.Create(Query)
                          .Destroy<CI::Client>(EntityTypeClient.Instance, 1)
                          .Verify<CI::Client>(m => m.Delete(It.Is(Predicate.Match(new CI::Client { Id = 1 }))));
        }

        [Test]
        public void ShouldDestroyClientHavingContact()
        {
            TargetDb.Has(new CI::Client { Id = 1 })
                    .Has(new CI::ClientContact { ClientId = 1, ContactId = 1 });

            Transformation.Create(Query)
                .Destroy<CI::Client>(EntityTypeClient.Instance, 1)
                .Verify<CI::Client>(m => m.Delete(It.Is(Predicate.Match(new CI::Client { Id = 1 }))))
                .Verify<CI::ClientContact>(m => m.Delete(It.Is(Predicate.Match(new CI::ClientContact { ClientId = 1, ContactId = 1 }))));
        }

        [Test]
        public void ShouldInitializeFirm()
        {
            SourceDb.Has(new Facts::Project { OrganizationUnitId = 1 })
                    .Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query)
                          .Initialize<CI::Firm>(EntityTypeFirm.Instance, 1)
                          .Verify<CI::Firm>(m => m.Add(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeFirmHavingBalance()
        {
            SourceDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 1 })

                    .Has(new Facts::Client { Id = 1 })
                    .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 })
                    .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1, Balance = 123.45m })
                    .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query)
                          .Initialize<CI::Firm>(EntityTypeFirm.Instance, 1)
                          .Verify<CI::Firm>(m => m.Add(It.Is(Predicate.Match(new CI::Firm { Id = 1, ProjectId = 1, ClientId = 1 }))))
                          .Verify<CI::FirmBalance>(m => m.Add(It.Is(Predicate.Match(new CI::FirmBalance { ProjectId = 1, FirmId = 1, AccountId = 1, Balance = 123.45m }))));
        }

        [Test]
        public void ShouldInitializeFirmHavingClient()
        {
            SourceDb.Has(new Facts::Category { Id = 1, Level = 3 })
                    .Has(new Facts::Project { OrganizationUnitId = 1 })
                    .Has(new Facts::Client { Id = 1 })
                    .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query)
                          .Initialize<CI::Firm>(EntityTypeFirm.Instance, 1)
                          .Verify<CI::Firm>(m => m.Add(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1 }))))
                          .Verify<CI::Client>(m => m.Add(It.Is(Predicate.Match(new CI::Client { Id = 1 }))), Times.Never);
        }

        [Test]
        public void ShouldRecalculateFirm()
        {
            SourceDb.Has(new Facts::Project { OrganizationUnitId = 1 })
                    .Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1 });

            TargetDb.Has(new CI::Firm { Id = 1 });

            Transformation.Create(Query)
                          .Recalculate<CI::Firm>(EntityTypeFirm.Instance, 1)
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingBalance()
        {
            SourceDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Facts::Client { Id = 1 },
                         new Facts::Client { Id = 2 })
                    .Has(new Facts::BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Facts::LegalPerson { Id = 1, ClientId = 1 },
                         new Facts::LegalPerson { Id = 2, ClientId = 2 })
                    .Has(new Facts::Account { Id = 1, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1, Balance = 123 },
                         new Facts::Account { Id = 2, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 1, Balance = 456 })
                    .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 },
                         new Facts::Firm { Id = 2, ClientId = 2, OrganizationUnitId = 1 },
                         new Facts::Firm { Id = 3, OrganizationUnitId = 1 });

            TargetDb.Has(new CI::Firm { Id = 1 },
                         new CI::Firm { Id = 2 },
                         new CI::Firm { Id = 3 })
                    .Has(new CI::FirmBalance { FirmId = 2, AccountId = 2, ProjectId = 1, Balance = 123 },
                         new CI::FirmBalance { FirmId = 3, ProjectId = 1, Balance = 123 });

            Transformation.Create(Query)
                          .Recalculate<CI::Firm>(EntityTypeFirm.Instance, 1, 2, 3)
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1, ProjectId = 1 }))))
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 2, ClientId = 2, ProjectId = 1 }))))
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 3, ProjectId = 1 }))))
                          .Verify<CI::FirmBalance>(m => m.Add(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 1, AccountId = 1, ProjectId = 1, Balance = 123 }))))
                          .Verify<CI::FirmBalance>(m => m.Add(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 2, AccountId = 2, ProjectId = 1, Balance = 456 }))))
                          .Verify<CI::FirmBalance>(m => m.Delete(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 3, ProjectId = 1, Balance = 123 }))))
                          .Verify<CI::FirmBalance>(m => m.Delete(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 2, AccountId = 2, ProjectId = 1, Balance = 123 }))));
        }

        [Test]
        public void ShouldRecalculateFirmHavingCategory()
        {
            SourceDb.Has(new Facts::Category { Id = 1, Level = 1 },
                         new Facts::Category { Id = 2, Level = 1 },
                         new Facts::Category { Id = 3, Level = 2, ParentId = 1 },
                         new Facts::Category { Id = 4, Level = 2, ParentId = 2 },
                         new Facts::Category { Id = 5, Level = 3, ParentId = 3 },
                         new Facts::Category { Id = 6, Level = 3, ParentId = 4 });
            SourceDb.Has(new Facts::CategoryOrganizationUnit { Id = 1, CategoryId = 5, OrganizationUnitId = 1 },
                         new Facts::CategoryOrganizationUnit { Id = 2, CategoryId = 6, OrganizationUnitId = 1 });
            SourceDb.Has(new Facts::Project { OrganizationUnitId = 1 });
            SourceDb.Has(new Facts::FirmAddress { Id = 1, FirmId = 1 },
                         new Facts::FirmAddress { Id = 2, FirmId = 2 });
            SourceDb.Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 5 },
                         new Facts::CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 6 });
            SourceDb.Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1 },
                         new Facts::Firm { Id = 2, OrganizationUnitId = 1 },
                         new Facts::Firm { Id = 3, OrganizationUnitId = 1 });

            TargetDb.Has(new CI::Firm { Id = 1 },
                         new CI::Firm { Id = 2 },
                         new CI::Firm { Id = 3 })
                    .Has(new CI::FirmCategory1 { FirmId = 1, CategoryId = 2 },
                         new CI::FirmCategory1 { FirmId = 2, CategoryId = 1 })
                    .Has(new CI::FirmCategory2 { FirmId = 1, CategoryId = 4 },
                         new CI::FirmCategory2 { FirmId = 2, CategoryId = 3 });

            Transformation.Create(Query)
                          .Recalculate<CI::Firm>(EntityTypeFirm.Instance, 1)
                          .Recalculate<CI::Firm>(EntityTypeFirm.Instance, 2)
                          .Recalculate<CI::Firm>(EntityTypeFirm.Instance, 3)
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1, AddressCount = 1 }))))
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 2, AddressCount = 1 }))))
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 3 }))))
                          .Verify<CI::FirmCategory1>(m => m.Add(It.Is(Predicate.Match(new CI::FirmCategory1 { FirmId = 1, CategoryId = 1 }))))
                          .Verify<CI::FirmCategory1>(m => m.Add(It.Is(Predicate.Match(new CI::FirmCategory1 { FirmId = 2, CategoryId = 2 }))))
                          .Verify<CI::FirmCategory1>(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory1 { FirmId = 1, CategoryId = 2 }))))
                          .Verify<CI::FirmCategory1>(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory1 { FirmId = 2, CategoryId = 1 }))))
                          .Verify<CI::FirmCategory2>(m => m.Add(It.Is(Predicate.Match(new CI::FirmCategory2 { FirmId = 1, CategoryId = 3 }))))
                          .Verify<CI::FirmCategory2>(m => m.Add(It.Is(Predicate.Match(new CI::FirmCategory2 { FirmId = 2, CategoryId = 4 }))))
                          .Verify<CI::FirmCategory2>(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory2 { FirmId = 1, CategoryId = 4 }))))
                          .Verify<CI::FirmCategory2>(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory2 { FirmId = 2, CategoryId = 3 }))))
                          ;
        }

        [Test]
        public void ShouldRecalculateFirmHavingClient()
        {
            SourceDb.Has(new Facts::Category { Id = 1, Level = 3 })
                    .Has(new Facts::Project { OrganizationUnitId = 1 })
                    .Has(new Facts::Client { Id = 1 })
                    .Has(new Facts::Firm { Id = 1, ClientId = 1, OrganizationUnitId = 1 });

            TargetDb.Has(new CI::Firm { Id = 1 })
                    .Has(new CI::Client { Id = 1 });

            Transformation.Create(Query)
                          .Recalculate<CI::Firm>(EntityTypeFirm.Instance, 1)
                          .Verify<CI::Firm>(m => m.Update(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1 }))))
                          .Verify<CI::Client>(m => m.Update(It.Is(Predicate.Match(new CI::Client { Id = 1 }))), Times.Never);
        }

        [Test]
        public void ShouldRecalculateFirmHavingTerritory()
        {
            SourceDb.Has(new Facts::Firm { Id = 1 })
                    .Has(new Facts::FirmAddress { FirmId = 1, TerritoryId = 2 });

            TargetDb.Has(new CI::Firm { Id = 1 })
                    .Has(new CI::FirmTerritory { FirmId = 1, TerritoryId = 3 });

            Transformation.Create(Query)
                          .Recalculate<CI::Firm>(EntityTypeFirm.Instance, 1)
                          .Verify<CI::FirmTerritory>(m => m.Add(It.Is(Predicate.Match(new CI::FirmTerritory { FirmId = 1, TerritoryId = 2 }))))
                          .Verify<CI::FirmTerritory>(m => m.Delete(It.Is(Predicate.Match(new CI::FirmTerritory { FirmId = 1, TerritoryId = 3 }))));
        }

        [Test]
        public void ShouldDestroyFirm()
        {
            TargetDb.Has(new CI::Firm { Id = 1 });

            Transformation.Create(Query)
                          .Destroy<CI::Firm>(EntityTypeFirm.Instance, 1)
                          .Verify<CI::Firm>(m => m.Delete(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingBalance()
        {
            TargetDb.Has(new CI::Firm { Id = 1 })
                    .Has(new CI::FirmBalance { FirmId = 1, Balance = 123 });

            Transformation.Create(Query)
                          .Destroy<CI::Firm>(EntityTypeFirm.Instance, 1)
                          .Verify<CI::Firm>(m => m.Delete(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))))
                          .Verify<CI::FirmBalance>(m => m.Delete(It.Is(Predicate.Match(new CI::FirmBalance { FirmId = 1, Balance = 123 }))));
        }

        [Test]
        public void ShouldDestroyFirmHavingCategory()
        {
            TargetDb.Has(new CI::Firm { Id = 1 })
                    .Has(new CI::FirmCategory1 { FirmId = 1, CategoryId = 1 })
                    .Has(new CI::FirmCategory2 { FirmId = 1, CategoryId = 2 })
                    ;

            Transformation.Create(Query)
                          .Destroy<CI::Firm>(EntityTypeFirm.Instance, 1)
                          .Verify<CI::Firm>(m => m.Delete(It.Is(Predicate.Match(new CI::Firm { Id = 1 }))))
                          .Verify<CI::FirmCategory1>(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory1 { FirmId = 1, CategoryId = 1 }))))
                          .Verify<CI::FirmCategory2>(m => m.Delete(It.Is(Predicate.Match(new CI::FirmCategory2 { FirmId = 1, CategoryId = 2 }))))
                          ;
        }

        [Test]
        public void ShouldDestroyFirmHavingClient()
        {
            TargetDb.Has(new CI::Firm { Id = 1, ClientId = 1 })
                    .Has(new CI::Client { Id = 1 });

            Transformation.Create(Query)
                          .Destroy<CI::Firm>(EntityTypeFirm.Instance, 1)
                          .Verify<CI::Firm>(m => m.Delete(It.Is(Predicate.Match(new CI::Firm { Id = 1, ClientId = 1 }))))
                          .Verify<CI::Client>(m => m.Delete(It.Is(Predicate.Match(new CI::Client { Id = 1 }))), Times.Never);
        }

        [Test]
        public void ShouldInitializeProject()
        {
            SourceDb.Has(new Facts::Project { Id = 1 });

            Transformation.Create(Query)
                          .Initialize<CI::Project>(EntityTypeProject.Instance, 1)
                          .Verify<CI::Project>(m => m.Add(It.Is(Predicate.Match(new CI::Project { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateProject()
        {
            SourceDb.Has(new Facts::Project { Id = 1, Name = "new name" });
            TargetDb.Has(new CI::Project { Id = 1, Name = "old name" });

            Transformation.Create(Query)
                          .Recalculate<CI::Project>(EntityTypeProject.Instance, 1)
                          .Verify<CI::Project>(m => m.Update(It.Is(Predicate.Match(new CI::Project { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyProject()
        {
            TargetDb.Has(new CI::Project { Id = 1 });

            Transformation.Create(Query)
                          .Destroy<CI::Project>(EntityTypeProject.Instance, 1)
                          .Verify<CI::Project>(m => m.Delete(It.Is(Predicate.Match(new CI::Project { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeTerritory()
        {
            SourceDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Facts::Territory { Id = 2, OrganizationUnitId = 1 });

            Transformation.Create(Query)
                          .Initialize<CI::Territory>(EntityTypeTerritory.Instance, 2)
                          .Verify<CI::Territory>(m => m.Add(It.Is(Predicate.Match(new CI::Territory { Id = 2, ProjectId = 1 }))));
        }

        [Test]
        public void ShouldRecalculateTerritory()
        {
            SourceDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Facts::Territory { Id = 1, OrganizationUnitId = 1, Name = "new name" });

            TargetDb.Has(new CI::Territory { Id = 1, Name = "old name" });

            Transformation.Create(Query)
                .Recalculate<CI::Territory>(EntityTypeTerritory.Instance, 1)
                .Verify<CI::Territory>(m => m.Update(It.Is(Predicate.Match(new CI::Territory { Id = 1, ProjectId = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyTerritory()
        {
            TargetDb.Has(new CI::Territory { Id = 1 });

            Transformation.Create(Query)
                          .Destroy<CI::Territory>(EntityTypeTerritory.Instance, 1)
                          .Verify<CI::Territory>(m => m.Delete(It.Is(Predicate.Match(new CI::Territory { Id = 1 }))));
        }

        [Test]
        public void ShouldInitializeCategoryGroup()
        {
            SourceDb.Has(new Facts::CategoryGroup { Id = 1 });

            Transformation.Create(Query)
                          .Initialize<CI::CategoryGroup>(EntityTypeCategoryGroup.Instance, 1)
                          .Verify<CI::CategoryGroup>(m => m.Add(It.Is(Predicate.Match(new CI::CategoryGroup { Id = 1 }))));
        }

        [Test]
        public void ShouldRecalculateCategoryGroup()
        {
            SourceDb.Has(new Facts::CategoryGroup { Id = 1, Name = "new name" });
            TargetDb.Has(new CI::CategoryGroup { Id = 1, Name = "old name" });

            Transformation.Create(Query)
                          .Recalculate<CI::CategoryGroup>(EntityTypeCategoryGroup.Instance, 1)
                          .Verify<CI::CategoryGroup>(m => m.Update(It.Is(Predicate.Match(new CI::CategoryGroup { Id = 1, Name = "new name" }))));
        }

        [Test]
        public void ShouldDestroyCategoryGroup()
        {
            TargetDb.Has(new CI::CategoryGroup { Id = 1 });

            Transformation.Create(Query)
                          .Destroy<CI::CategoryGroup>(EntityTypeCategoryGroup.Instance, 1)
                          .Verify<CI::CategoryGroup>(x => x.Delete(It.Is(Predicate.Match(new CI::CategoryGroup { Id = 1 }))));
        }

        #region Transformation

        private class Transformation
        {
            private readonly IQuery _query;
            private readonly VerifiableRepositoryFactory _repositoryFactory;
            private readonly AggregateConstructionMetadataSource _metadataSource;
            private readonly EqualityComparerFactory _comparerFactory;

            private Transformation(IQuery query)
            {
                _query = query;
                _repositoryFactory = new VerifiableRepositoryFactory();
                _metadataSource = new AggregateConstructionMetadataSource();
                _comparerFactory = new EqualityComparerFactory(new LinqToDbPropertyProvider(Schema.Erm, Schema.Facts, Schema.CustomerIntelligence));
            }

            public static Transformation Create(IQuery query)
            {
                return new Transformation(query);
            }

            public Transformation Initialize<TAggregate>(IEntityType entityType, params long[] ids) where TAggregate : class, IIdentifiable<long>
            {
                return Do<TAggregate>(x => x.Initialize(ids.Select(id => new InitializeAggregate(entityType.Id, id)).ToArray()));
            }

            public Transformation Recalculate<TAggregate>(IEntityType entityType, params long[] ids) where TAggregate : class, IIdentifiable<long>
            {
                return Do<TAggregate>(x => x.Recalculate(ids.Select(id => new RecalculateAggregate(entityType.Id, id)).ToArray()));
            }

            public Transformation Destroy<TAggregate>(IEntityType entityType, params long[] ids) where TAggregate : class, IIdentifiable<long>
            {
                return Do<TAggregate>(x => x.Destroy(ids.Select(id => new DestroyAggregate(entityType.Id, id)).ToArray()));
            }

            public Transformation Verify<T>(Expression<Action<IRepository<T>>> expression)
                where T : class
            {
                return Verify<T>(expression, Times.Once, null);
            }

            public Transformation Verify<T>(Expression<Action<IRepository<T>>> expression, Func<Times> times)
                where T : class
            {
                return Verify<T>(expression, times, null);
            }

            public Transformation Verify<T>(Expression<Action<IRepository<T>>> expression, Func<Times> times, string failMessage)
                where T : class
            {
                _repositoryFactory.Verify(expression, times, failMessage);
                return this;
            }

            private Transformation Do<TAggregate>(Action<AggregateProcessor<TAggregate>> action)
                where TAggregate : class, IIdentifiable<long>
            {
                var aggregateType = typeof(TAggregate);

                IMetadataElement aggregateMetadata;
                if (!_metadataSource.Metadata.Values.TryGetElementById(new Uri(aggregateType.Name, UriKind.Relative), out aggregateMetadata))
                {
                    throw new NotSupportedException(string.Format("The aggregate of type '{0}' is not supported.", aggregateType));
                }

                var factory = new Factory<TAggregate>(_query, _repositoryFactory, _comparerFactory);
                var processor = factory.Create(aggregateMetadata);
                action.Invoke((AggregateProcessor<TAggregate>)processor);

                return this;
            }

            private class Factory<TAggregate> : IAggregateProcessorFactory
                where TAggregate : class, IIdentifiable<long>
            {
                private readonly IQuery _query;
                private readonly IRepositoryFactory _repositoryFactory;
                private readonly EqualityComparerFactory _comparerFactory;

                public Factory(IQuery query, IRepositoryFactory repositoryFactory, EqualityComparerFactory comparerFactory)
                {
                    _query = query;
                    _repositoryFactory = repositoryFactory;
                    _comparerFactory = comparerFactory;
                }

                public IAggregateProcessor Create(IMetadataElement metadata)
                {
                    var aggregateMetadata = (AggregateMetadata<TAggregate, long>)metadata;

                    var findSpecProvider = new AggregateFindSpecificationProvider<TAggregate>(new DefaultIdentityProvider());
                    var rootEntityPricessor = new EntityProcessor<TAggregate>(
                        _repositoryFactory.Create<TAggregate>(),
                        new DataChangesDetector<TAggregate>(aggregateMetadata.MapSpecificationProviderForSource,
                                                            aggregateMetadata.MapSpecificationProviderForTarget,
                                                            _comparerFactory.CreateIdentityComparer<TAggregate>(),
                                                            _query),
                        metadata.Elements.OfType<IValueObjectMetadata>().Select(x => CreateFactory(x).Create(x)).ToArray());

                    return new AggregateProcessor<TAggregate>(findSpecProvider, rootEntityPricessor, new IChildEntityProcessor<TAggregate>[0]);
                }

                private IValueObjectProcessorFactory<TAggregate> CreateFactory(IValueObjectMetadata metadata)
                {
                    return (IValueObjectProcessorFactory<TAggregate>)Activator.CreateInstance(
                        typeof(ValueObjectProcessorFactory<,>).MakeGenericType(typeof(TAggregate), metadata.ValueObjectType),
                        _query,
                        _repositoryFactory,
                        _comparerFactory);
                }
            }

            private class ValueObjectProcessorFactory<TAggregate, TValueObject> : IValueObjectProcessorFactory<TAggregate>
                where TValueObject : class, IObject
                where TAggregate : IIdentifiable<long>
            {
                private readonly IQuery _query;
                private readonly IRepositoryFactory _repositoryFactory;
                private readonly EqualityComparerFactory _comparerFactory;

                public ValueObjectProcessorFactory(IQuery query, IRepositoryFactory repositoryFactory, EqualityComparerFactory comparerFactory)
                {
                    _query = query;
                    _repositoryFactory = repositoryFactory;
                    _comparerFactory = comparerFactory;
                }

                public IValueObjectProcessor<TAggregate> Create(IValueObjectMetadata metadata)
                {
                    return Create((ValueObjectMetadata<TValueObject, long>)metadata);
                }

                private IValueObjectProcessor<TAggregate> Create(ValueObjectMetadata<TValueObject, long> metadata)
                {
                    return new ValueObjectProcessor<TAggregate, TValueObject>(
                        new DataChangesDetector<TValueObject>(
                            metadata.MapSpecificationProviderForSource,
                            metadata.MapSpecificationProviderForTarget,
                            _comparerFactory.CreateCompleteComparer<TValueObject>(),
                            _query),
                        _repositoryFactory.Create<TValueObject>(),
                        new ValueObjectFindSpecificationProvider<TValueObject, TAggregate>(metadata, new DefaultIdentityProvider()));
                }
            }
        }

        #endregion
    }
}