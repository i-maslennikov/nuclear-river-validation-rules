using System;
using System.Collections;

using Moq;

using NuClear.CustomerIntelligence.Storage.Model.Erm;
using NuClear.Storage.API.Readings;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal partial class FactDependencyTests : TransformationFixtureBase
    {
        [TestCaseSource("Cases")]
        public void ShouldProcessChanges(Action<IQuery, MockLinqToDbDataBuilder, MockLinqToDbDataBuilder> run)
        {
            run(Query, SourceDb, TargetDb);
        }

        private IEnumerable Cases
        {
            get
            {
                const int NotNull = 1;

                // insert
                yield return CaseToVerifyElementInsertion<Account, Storage.Model.Facts.Account>(new Account { Id = 1 });
                yield return CaseToVerifyElementInsertion<BranchOfficeOrganizationUnit, Storage.Model.Facts.BranchOfficeOrganizationUnit>(new BranchOfficeOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementInsertion<Category, Storage.Model.Facts.Category>(new Category { Id = 1 });
                yield return CaseToVerifyElementInsertion<CategoryFirmAddress, Storage.Model.Facts.CategoryFirmAddress>(new CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementInsertion<CategoryGroup, Storage.Model.Facts.CategoryGroup>(new CategoryGroup { Id = 1 });
                yield return CaseToVerifyElementInsertion<CategoryOrganizationUnit, Storage.Model.Facts.CategoryOrganizationUnit>(new CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementInsertion<Client, Storage.Model.Facts.Client>(new Client { Id = 1 });
                yield return CaseToVerifyElementInsertion<Contact, Storage.Model.Facts.Contact>(new Contact { Id = 1 });
                yield return CaseToVerifyElementInsertion<Firm, Storage.Model.Facts.Firm>(new Firm { Id = 1 });
                yield return CaseToVerifyElementInsertion<FirmAddress, Storage.Model.Facts.FirmAddress>(new FirmAddress { Id = 1 });
                yield return CaseToVerifyElementInsertion<FirmContact, Storage.Model.Facts.FirmContact>(new FirmContact { Id = 1, ContactType = 1, FirmAddressId = NotNull });
                yield return CaseToVerifyElementInsertion<LegalPerson, Storage.Model.Facts.LegalPerson>(new LegalPerson { Id = 1, ClientId = NotNull });
                yield return CaseToVerifyElementInsertion<Order, Storage.Model.Facts.Order>(new Order { Id = 1, WorkflowStepId = 4 });
                yield return CaseToVerifyElementInsertion<Project, Storage.Model.Facts.Project>(new Project { Id = 1, OrganizationUnitId = NotNull });
                yield return CaseToVerifyElementInsertion<Territory, Storage.Model.Facts.Territory>(new Territory { Id = 1 });

                // update
                yield return CaseToVerifyElementUpdate(new Account { Id = 1 }, new Storage.Model.Facts.Account { Id = 1, Balance = 1});
                yield return CaseToVerifyElementUpdate(new BranchOfficeOrganizationUnit { Id = 1 }, new Storage.Model.Facts.BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 1});
                yield return CaseToVerifyElementUpdate(new Category { Id = 1 }, new Storage.Model.Facts.Category { Id = 1, Name = "asdf" });
                yield return CaseToVerifyElementUpdate(new CategoryFirmAddress { Id = 1 }, new Storage.Model.Facts.CategoryFirmAddress { Id = 1, CategoryId = 1});
                yield return CaseToVerifyElementUpdate(new CategoryGroup { Id = 1 }, new Storage.Model.Facts.CategoryGroup { Id = 1, Name = "asdf" });
                yield return CaseToVerifyElementUpdate(new CategoryOrganizationUnit { Id = 1 }, new Storage.Model.Facts.CategoryOrganizationUnit { Id = 1, CategoryId = 1});
                yield return CaseToVerifyElementUpdate(new Client { Id = 1 }, new Storage.Model.Facts.Client { Id = 1, Name = "asdf" });
                yield return CaseToVerifyElementUpdate(new Contact { Id = 1 }, new Storage.Model.Facts.Contact { Id = 1, ClientId = 1});
                yield return CaseToVerifyElementUpdate(new Firm { Id = 1 }, new Storage.Model.Facts.Firm { Id = 1, ClientId = 1});
                yield return CaseToVerifyElementUpdate(new FirmAddress { Id = 1 }, new Storage.Model.Facts.FirmAddress { Id = 1, FirmId = 1 });
                yield return CaseToVerifyElementUpdate(new FirmContact { Id = 1, ContactType = 1, FirmAddressId = NotNull }, new Storage.Model.Facts.FirmContact { Id = 1, HasPhone = false, FirmAddressId = NotNull});
                yield return CaseToVerifyElementUpdate(new LegalPerson { Id = 1, ClientId = NotNull }, new Storage.Model.Facts.LegalPerson { Id = 1, ClientId = 2 });
                yield return CaseToVerifyElementUpdate(new Order { Id = 1, WorkflowStepId = 4 }, new Storage.Model.Facts.Order { Id = 1, FirmId = 1 });
                yield return CaseToVerifyElementUpdate(new Project { Id = 1, OrganizationUnitId = NotNull }, new Storage.Model.Facts.Project { Id = 1, Name = "asdf" });
                yield return CaseToVerifyElementUpdate(new Territory { Id = 1 }, new Storage.Model.Facts.Territory { Id = 1, Name = "asdf" });

                // delete
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.Account { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.BranchOfficeOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.Category { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.CategoryGroup { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.Client { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.Contact { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.Firm { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.FirmAddress { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.FirmContact { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.LegalPerson { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.Order { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.Project { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Storage.Model.Facts.Territory { Id = 1 });
            }
        }

        private static TestCaseData CaseToVerifyElementInsertion<TSource, TTarget>(TSource sourceObject)
            where TSource : class, IIdentifiable<long>, new()
            where TTarget : class, IIdentifiable<long>, IFactObject, new()
        {
            return Case((query, ermDb, factsDb) => VerifyElementInsertion<TSource, TTarget>(query, ermDb, sourceObject))
                .SetName(string.Format("Should insert {0} element.", typeof(TTarget).Name));
        }

        private static TestCaseData CaseToVerifyElementUpdate<TSource, TTarget>(TSource sourceObject, TTarget target) 
            where TSource : class, IIdentifiable<long>, new()
            where TTarget : class, IIdentifiable<long>, IFactObject, new()
        {
            return Case((query, ermDb, factsDb) => VerifyElementUpdate<TSource, TTarget>(query, ermDb, factsDb, sourceObject, target))
                .SetName(string.Format("Should update {0} element.", typeof(TTarget).Name));
        }

        private static TestCaseData CaseToVerifyElementDeletion<TTarget>(TTarget targetObject) 
            where TTarget : class, IIdentifiable<long>, IFactObject, new()
        {
            return Case((query, ermDb, factsDb) => VerifyElementDeletion<TTarget>(query, factsDb, targetObject))
                .SetName(string.Format("Should delete {0} element.", typeof(TTarget).Name));
        }

        private static void VerifyElementInsertion<TSource, TTarget>(IQuery query, MockLinqToDbDataBuilder ermDb, TSource sourceObject)
            where TSource : class, IIdentifiable<long>, new()
            where TTarget : class, IIdentifiable<long>, IFactObject, new()
        {
            var entityId = new DefaultIdentityProvider().GetId(sourceObject);
            ermDb.Has(sourceObject);

            var factory = new VerifiableRepositoryFactory();
            Transformation.Create(query, factory)
                          .ApplyChanges<TTarget>(entityId);

            factory.Verify<TTarget>(
                x => x.Add(It.Is(Predicate.ById<TTarget>(entityId))),
                Times.Once,
                string.Format("The {0} element was not inserted.", typeof(TTarget).Name));
        }

        private static void VerifyElementUpdate<TSource, TTarget>(IQuery query, MockLinqToDbDataBuilder ermDb, MockLinqToDbDataBuilder factsDb, TSource sourceObject, TTarget targetObject)
            where TSource : class, IIdentifiable<long>, new()
            where TTarget : class, IIdentifiable<long>, IFactObject, new()
        {
            ermDb.Has(sourceObject);
            factsDb.Has(targetObject);

            var factory = new VerifiableRepositoryFactory();
            Transformation.Create(query, factory)
                          .ApplyChanges<TTarget>(new DefaultIdentityProvider().GetId(targetObject));

            factory.Verify<TTarget>(
                x => x.Update(It.Is(Predicate.ById<TTarget>(new DefaultIdentityProvider().GetId(targetObject)))),
                Times.Once,
                string.Format("The {0} element was not updated.", typeof(TTarget).Name));
        }

        private static void VerifyElementDeletion<TTarget>(IQuery query, MockLinqToDbDataBuilder factsDb, TTarget targetObject)
            where TTarget : class, IIdentifiable<long>, IFactObject, new()
        {
            factsDb.Has(targetObject);

            var factory = new VerifiableRepositoryFactory();
            Transformation.Create(query, factory)
                          .ApplyChanges<TTarget>(new DefaultIdentityProvider().GetId(targetObject));

            factory.Verify<TTarget>(
                x => x.Delete(It.Is(Predicate.ById<TTarget>(new DefaultIdentityProvider().GetId(targetObject)))),
                Times.Once,
                string.Format("The {0} element was not deleted.", typeof(TTarget).Name));
        }

        private static TestCaseData Case(Action<IQuery, MockLinqToDbDataBuilder, MockLinqToDbDataBuilder> action)
        {
            return new TestCaseData(action);
        }
    }
}