using System;
using System.Collections;

using Moq;

using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;

using NUnit.Framework;

using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;

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
                yield return CaseToVerifyElementInsertion<Erm::Account, Facts::Account>(new Erm::Account { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::BranchOfficeOrganizationUnit, Facts::BranchOfficeOrganizationUnit>(new Erm::BranchOfficeOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::Category, Facts::Category>(new Erm::Category { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::CategoryFirmAddress, Facts::CategoryFirmAddress>(new Erm::CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::CategoryGroup, Facts::CategoryGroup>(new Erm::CategoryGroup { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::CategoryOrganizationUnit, Facts::CategoryOrganizationUnit>(new Erm::CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::Client, Facts::Client>(new Erm::Client { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::Contact, Facts::Contact>(new Erm::Contact { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::Firm, Facts::Firm>(new Erm::Firm { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::FirmAddress, Facts::FirmAddress>(new Erm::FirmAddress { Id = 1 });
                yield return CaseToVerifyElementInsertion<Erm::FirmContact, Facts::FirmContact>(new Erm::FirmContact { Id = 1, ContactType = 1, FirmAddressId = NotNull });
                yield return CaseToVerifyElementInsertion<Erm::LegalPerson, Facts::LegalPerson>(new Erm::LegalPerson { Id = 1, ClientId = NotNull });
                yield return CaseToVerifyElementInsertion<Erm::Order, Facts::Order>(new Erm::Order { Id = 1, WorkflowStepId = 4 });
                yield return CaseToVerifyElementInsertion<Erm::Project, Facts::Project>(new Erm::Project { Id = 1, OrganizationUnitId = NotNull });
                yield return CaseToVerifyElementInsertion<Erm::Territory, Facts::Territory>(new Erm::Territory { Id = 1 });

                // update
                yield return CaseToVerifyElementUpdate(new Erm::Account { Id = 1 }, new Facts::Account { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::BranchOfficeOrganizationUnit { Id = 1 }, new Facts::BranchOfficeOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Category { Id = 1 }, new Facts::Category { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::CategoryFirmAddress { Id = 1 }, new Facts::CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::CategoryGroup { Id = 1 }, new Facts::CategoryGroup { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::CategoryOrganizationUnit { Id = 1 }, new Facts::CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Client { Id = 1 }, new Facts::Client { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Contact { Id = 1 }, new Facts::Contact { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Firm { Id = 1 }, new Facts::Firm { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::FirmAddress { Id = 1 }, new Facts::FirmAddress { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::FirmContact { Id = 1, ContactType = 1, FirmAddressId = NotNull }, new Facts::FirmContact { Id = 1, HasPhone = true, FirmAddressId = NotNull });
                yield return CaseToVerifyElementUpdate(new Erm::LegalPerson { Id = 1, ClientId = NotNull }, new Facts::LegalPerson { Id = 1, ClientId = NotNull });
                yield return CaseToVerifyElementUpdate(new Erm::Order { Id = 1, WorkflowStepId = 4 }, new Facts::Order { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Project { Id = 1, OrganizationUnitId = NotNull }, new Facts::Project { Id = 1 });
                yield return CaseToVerifyElementUpdate(new Erm::Territory { Id = 1 }, new Facts::Territory { Id = 1 });

                // delete
                yield return CaseToVerifyElementDeletion(new Facts::Account { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::BranchOfficeOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Category { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::CategoryFirmAddress { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::CategoryGroup { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::CategoryOrganizationUnit { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Client { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Contact { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Firm { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::FirmAddress { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::FirmContact { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::LegalPerson { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Order { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Project { Id = 1 });
                yield return CaseToVerifyElementDeletion(new Facts::Territory { Id = 1 });
            }
        }

        private static TestCaseData CaseToVerifyElementInsertion<TSource, TTarget>(TSource sourceObject)
            where TSource : class, IIdentifiable, new()
            where TTarget : class, IIdentifiable, IFactObject, new()
        {
            return Case((query, ermDb, factsDb) => VerifyElementInsertion<TSource, TTarget>(query, ermDb, sourceObject))
                .SetName(string.Format("Should insert {0} element.", typeof(TTarget).Name));
        }

        private static TestCaseData CaseToVerifyElementUpdate<TSource, TTarget>(TSource sourceObject, TTarget target) 
            where TSource : class, IIdentifiable, new()
            where TTarget : class, IIdentifiable, IFactObject, new()
        {
            return Case((query, ermDb, factsDb) => VerifyElementUpdate<TSource, TTarget>(query, ermDb, factsDb, sourceObject, target))
                .SetName(string.Format("Should update {0} element.", typeof(TTarget).Name));
        }

        private static TestCaseData CaseToVerifyElementDeletion<TTarget>(TTarget targetObject) 
            where TTarget : class, IIdentifiable, IFactObject, new()
        {
            return Case((query, ermDb, factsDb) => VerifyElementDeletion<TTarget>(query, factsDb, targetObject))
                .SetName(string.Format("Should delete {0} element.", typeof(TTarget).Name));
        }

        private static void VerifyElementInsertion<TSource, TTarget>(IQuery query, MockLinqToDbDataBuilder ermDb, TSource sourceObject)
            where TSource : class, IIdentifiable, new()
            where TTarget : class, IIdentifiable, IFactObject, new()
        {
            var entityId = sourceObject.Id;
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
            where TSource : class, IIdentifiable, new()
            where TTarget : class, IIdentifiable, IFactObject, new()
        {
            ermDb.Has(sourceObject);
            factsDb.Has(targetObject);

            var factory = new VerifiableRepositoryFactory();
            Transformation.Create(query, factory)
                          .ApplyChanges<TTarget>(targetObject.Id);

            factory.Verify<TTarget>(
                x => x.Update(It.Is(Predicate.ById<TTarget>(targetObject.Id))),
                Times.Once,
                string.Format("The {0} element was not updated.", typeof(TTarget).Name));
        }

        private static void VerifyElementDeletion<TTarget>(IQuery query, MockLinqToDbDataBuilder factsDb, TTarget targetObject)
            where TTarget : class, IIdentifiable, IFactObject, new()
        {
            factsDb.Has(targetObject);

            var factory = new VerifiableRepositoryFactory();
            Transformation.Create(query, factory)
                          .ApplyChanges<TTarget>(targetObject.Id);

            factory.Verify<TTarget>(
                x => x.Delete(It.Is(Predicate.ById<TTarget>(targetObject.Id))),
                Times.Once,
                string.Format("The {0} element was not deleted.", typeof(TTarget).Name));
        }

        private static TestCaseData Case(Action<IQuery, MockLinqToDbDataBuilder, MockLinqToDbDataBuilder> action)
        {
            return new TestCaseData(action);
        }
    }
}