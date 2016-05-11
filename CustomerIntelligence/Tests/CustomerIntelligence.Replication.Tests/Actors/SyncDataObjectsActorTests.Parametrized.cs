using System;
using System.Collections;
using System.Linq.Expressions;

using Moq;

using NuClear.CustomerIntelligence.Replication.Tests.MemoryDb;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Storage.API.Readings;

using NUnit.Framework;

using Erm = NuClear.CustomerIntelligence.Storage.Model.Erm;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Actors
{
    [TestFixture]
    internal partial class SyncDataObjectsActorTests
    {
        [TestCaseSource("Cases")]
        public void ShouldProcessChanges(Action<IQuery, MockLinqToDbDataBuilder, MockLinqToDbDataBuilder> run)
        {
            run(Query, SourceDb, TargetDb);
        }

        private static IEnumerable Cases
        {
            get
            {
                const int NotNull = 1;

                // insert
                yield return Insertion<Erm.Account, Account>(new Erm.Account { Id = 1 }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.BranchOfficeOrganizationUnit, BranchOfficeOrganizationUnit>(new Erm.BranchOfficeOrganizationUnit { Id = 1 }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.Category, Category>(new Erm.Category { Id = 1 }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.CategoryFirmAddress, CategoryFirmAddress>(new Erm.CategoryFirmAddress { Id = 1 }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.CategoryGroup, CategoryGroup>(new Erm.CategoryGroup { Id = 1 }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.CategoryOrganizationUnit, CategoryOrganizationUnit>(new Erm.CategoryOrganizationUnit { Id = 1 }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.Client, Client>(new Erm.Client { Id = 1 }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.Contact, Contact>(new Erm.Contact { Id = 1 }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.Firm, Firm>(new Erm.Firm { Id = 1 }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.FirmAddress, FirmAddress>(new Erm.FirmAddress { Id = 1 }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.FirmContact, FirmContact>(new Erm.FirmContact { Id = 1, ContactType = 1, FirmAddressId = NotNull }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.LegalPerson, LegalPerson>(new Erm.LegalPerson { Id = 1, ClientId = NotNull }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.Order, Order>(new Erm.Order { Id = 1, WorkflowStepId = 4 }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.Project, Project>(new Erm.Project { Id = 1, OrganizationUnitId = NotNull }, x => x.Id, x => x.Id);
                yield return Insertion<Erm.Territory, Territory>(new Erm.Territory { Id = 1 }, x => x.Id, x => x.Id);

                // update
                yield return CaseToVerifyElementUpdate(new Erm.Account { Id = 1 }, new Account { Id = 1, Balance = 1}, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.BranchOfficeOrganizationUnit { Id = 1 }, new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1}, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.Category { Id = 1 }, new Category { Id = 1, Name = "asdf" }, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.CategoryFirmAddress { Id = 1 }, new CategoryFirmAddress { Id = 1, CategoryId = 1}, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.CategoryGroup { Id = 1 }, new CategoryGroup { Id = 1, Name = "asdf" }, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.CategoryOrganizationUnit { Id = 1 }, new CategoryOrganizationUnit { Id = 1, CategoryId = 1}, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.Client { Id = 1 }, new Client { Id = 1, Name = "asdf" }, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.Contact { Id = 1 }, new Contact { Id = 1, ClientId = 1}, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.Firm { Id = 1 }, new Firm { Id = 1, ClientId = 1}, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.FirmAddress { Id = 1 }, new FirmAddress { Id = 1, FirmId = 1 }, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.FirmContact { Id = 1, ContactType = 1, FirmAddressId = NotNull }, new FirmContact { Id = 1, HasPhone = false, FirmAddressId = NotNull}, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.LegalPerson { Id = 1, ClientId = NotNull }, new LegalPerson { Id = 1, ClientId = 2 }, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.Order { Id = 1, WorkflowStepId = 4 }, new Order { Id = 1, FirmId = 1 }, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.Project { Id = 1, OrganizationUnitId = NotNull }, new Project { Id = 1, Name = "asdf" }, x => x.Id, x => x.Id);
                yield return CaseToVerifyElementUpdate(new Erm.Territory { Id = 1 }, new Territory { Id = 1, Name = "asdf" }, x => x.Id, x => x.Id);

                // delete
                yield return CaseToVerifyElementDeletion(new Account { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new BranchOfficeOrganizationUnit { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new Category { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new CategoryFirmAddress { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new CategoryGroup { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new CategoryOrganizationUnit { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new Client { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new Contact { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new Firm { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new FirmAddress { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new FirmContact { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new LegalPerson { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new Order { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new Project { Id = 1 }, x => x.Id);
                yield return CaseToVerifyElementDeletion(new Territory { Id = 1 }, x => x.Id);
            }
        }

        private static TestCaseData Insertion<TSource, TTarget>(
            TSource sourceObject,
            Func<TSource, long> sourceIdProvider,
            Expression<Func<TTarget, long>> targetIdProvider)
            where TSource : class
            where TTarget : class
        {
            return Case((query, ermDb, factsDb) => VerifyElementInsertion(query, ermDb, sourceObject, sourceIdProvider, targetIdProvider))
                .SetName($"Should insert {typeof(TTarget).Name} element.");
        }

        private static TestCaseData CaseToVerifyElementUpdate<TSource, TTarget>(
            TSource sourceObject,
            TTarget target,
            Func<TSource, long> sourceIdProvider,
            Expression<Func<TTarget, long>> targetIdProvider)
            where TSource : class
            where TTarget : class
        {
            return Case((query, ermDb, factsDb) => VerifyElementUpdate(query, ermDb, factsDb, sourceObject, target, sourceIdProvider, targetIdProvider))
                .SetName($"Should update {typeof(TTarget).Name} element.");
        }

        private static TestCaseData CaseToVerifyElementDeletion<TTarget>(TTarget targetObject, Expression<Func<TTarget, long>> targetIdProvider)
            where TTarget : class
        {
            return Case((query, ermDb, factsDb) => VerifyElementDeletion(query, factsDb, targetObject, targetIdProvider))
                .SetName($"Should delete {typeof(TTarget).Name} element.");
        }

        private static void VerifyElementInsertion<TSource, TTarget>(
            IQuery query,
            MockLinqToDbDataBuilder ermDb,
            TSource sourceObject,
            Func<TSource, long> sourceIdProvider,
            Expression<Func<TTarget, long>> targetIdProvider)
            where TSource : class
            where TTarget : class
        {
            ermDb.Has(sourceObject);

            var entityId = sourceIdProvider(sourceObject);
            var factory = new VerifiableRepositoryFactory();
            Actor.Create(query, factory)
                 .Sync<TTarget>(entityId);

            var predicate = Expression.Lambda<Func<TTarget, bool>>(Expression.Equal(targetIdProvider.Body, Expression.Constant(entityId)), targetIdProvider.Parameters);
            factory.Verify<TTarget>(x => x.Add(It.Is(predicate)),
                                    Times.Once,
                                    $"The {typeof(TTarget).Name} element was not inserted.");
        }

        private static void VerifyElementUpdate<TSource, TTarget>(
            IQuery query,
            MockLinqToDbDataBuilder ermDb,
            MockLinqToDbDataBuilder factsDb,
            TSource sourceObject,
            TTarget targetObject,
            Func<TSource, long> sourceIdProvider,
            Expression<Func<TTarget, long>> targetIdProvider)
            where TSource : class
            where TTarget : class
        {
            ermDb.Has(sourceObject);
            factsDb.Has(targetObject);

            var entityId = sourceIdProvider(sourceObject);
            var factory = new VerifiableRepositoryFactory();
            Actor.Create(query, factory)
                 .Sync<TTarget>(entityId);

            var predicate = Expression.Lambda<Func<TTarget, bool>>(Expression.Equal(targetIdProvider.Body, Expression.Constant(entityId)), targetIdProvider.Parameters);
            factory.Verify<TTarget>(x => x.Update(It.Is(predicate)),
                                    Times.Once,
                                    $"The {typeof(TTarget).Name} element was not updated.");
        }

        private static void VerifyElementDeletion<TTarget>(
            IQuery query, MockLinqToDbDataBuilder factsDb, TTarget targetObject,
            Expression<Func<TTarget, long>> targetIdProvider)
            where TTarget : class
        {
            factsDb.Has(targetObject);

            var entityId = targetIdProvider.Compile()(targetObject);
            var factory = new VerifiableRepositoryFactory();
            Actor.Create(query, factory)
                 .Sync<TTarget>(entityId);

            var predicate = Expression.Lambda<Func<TTarget, bool>>(Expression.Equal(targetIdProvider.Body, Expression.Constant(entityId)), targetIdProvider.Parameters);
            factory.Verify<TTarget>(x => x.Delete(It.Is(predicate)),
                                    Times.Once,
                                    $"The {typeof(TTarget).Name} element was not deleted.");
        }

        private static TestCaseData Case(Action<IQuery, MockLinqToDbDataBuilder, MockLinqToDbDataBuilder> action)
        {
            return new TestCaseData(action);
        }
    }
}