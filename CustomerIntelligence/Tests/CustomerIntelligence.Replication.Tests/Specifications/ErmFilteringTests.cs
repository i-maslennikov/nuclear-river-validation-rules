using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Erm;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.Specifications
{
    [TestFixture]
    internal class ErmFilteringTests : DataFixtureBase
    {
        [Test]
        public void ShouldReadAllActivityTypes()
        {
            ShouldReadActivity((q, ids) => q.For(Specs.Find.Erm.Appointments()).Where(x => ids.Contains(x.Id)));
            ShouldReadActivity((q, ids) => q.For(Specs.Find.Erm.Phonecalls()).Where(x => ids.Contains(x.Id)));
            ShouldReadActivity((q, ids) => q.For(Specs.Find.Erm.Tasks()).Where(x => ids.Contains(x.Id)));
            ShouldReadActivity((q, ids) => q.For(Specs.Find.Erm.Letters()).Where(x => ids.Contains(x.Id)));
        }

        [Test]
        public void ShouldReadAllActivityReferenceTypes()
        {
            ShouldReadActivityReference(q => q.For(Specs.Find.Erm.ClientAppointments()), q => q.For(Specs.Find.Erm.FirmAppointments()));
            ShouldReadActivityReference(q => q.For(Specs.Find.Erm.ClientPhonecalls()), q => q.For(Specs.Find.Erm.FirmPhonecalls()));
            ShouldReadActivityReference(q => q.For(Specs.Find.Erm.ClientTasks()), q => q.For(Specs.Find.Erm.FirmTasks()));
            ShouldReadActivityReference(q => q.For(Specs.Find.Erm.ClientLetters()), q => q.For(Specs.Find.Erm.FirmLetters()));
        }

        [TestCaseSource(nameof(CommonCases))]
        public void CommonFilteringTests<T>(T entity, FindSpecification<T> specification, bool expected)
            where T : class
        {
            SourceDb.Has(entity);

            Reader.Create(Query)
                  .Verify(x => x.For(specification).Any(), expected);
        }

        public IEnumerable<TestCaseData> CommonCases =
            new[]
            {
                For(Specs.Find.Erm.Accounts())
                    .Entity(new Account { Id = 1 }, true, "по умолчанию")
                    .Entity(new Account { Id = 2, IsArchived = true }, false, "архивный"),

                For(Specs.Find.Erm.BranchOfficeOrganizationUnits())
                    .Entity(new BranchOfficeOrganizationUnit { Id = 1 }, true, "по умолчанию")
                    .Entity(new BranchOfficeOrganizationUnit { Id = 2, IsActive = false, IsDeleted = false }, false, "неактивный")
                    .Entity(new BranchOfficeOrganizationUnit { Id = 3, IsActive = true, IsDeleted = true }, false, "удалённый")
                    .Entity(new BranchOfficeOrganizationUnit { Id = 4, IsActive = false, IsDeleted = true }, false, "неактивный и удалённый"),

                For(Specs.Find.Erm.Categories())
                    .Entity(new Category { Id = 1 }, true, "по умолчанию")
                    .Entity(new Category { Id = 2, IsActive = false, IsDeleted = false }, false, "неактивный")
                    .Entity(new Category { Id = 3, IsActive = true, IsDeleted = true }, false, "удалённый")
                    .Entity(new Category { Id = 4, IsActive = false, IsDeleted = true }, false, "неактивный и удалённый"),

                For(Specs.Find.Erm.CategoryFirmAddresses())
                    .Entity(new CategoryFirmAddress { Id = 1 }, true, "по умолчанию")
                    .Entity(new CategoryFirmAddress { Id = 2, IsActive = false, IsDeleted = false }, false, "неактивный")
                    .Entity(new CategoryFirmAddress { Id = 3, IsActive = true, IsDeleted = true }, false, "удалённый")
                    .Entity(new CategoryFirmAddress { Id = 4, IsActive = false, IsDeleted = true }, false, "неактивный и удалённый"),

                For(Specs.Find.Erm.CategoryGroups())
                    .Entity(new CategoryGroup { Id = 1 }, true, "по умолчанию")
                    .Entity(new CategoryGroup { Id = 2, IsActive = false, IsDeleted = false }, false, "неактивный")
                    .Entity(new CategoryGroup { Id = 3, IsActive = true, IsDeleted = true }, false, "удалённый")
                    .Entity(new CategoryGroup { Id = 4, IsActive = false, IsDeleted = true }, false, "неактивный и удалённый"),

                For(Specs.Find.Erm.CategoryOrganizationUnits())
                    .Entity(new CategoryOrganizationUnit { Id = 1 }, true, "по умолчанию")
                    .Entity(new CategoryOrganizationUnit { Id = 2, IsActive = false, IsDeleted = false }, false, "неактивный")
                    .Entity(new CategoryOrganizationUnit { Id = 3, IsActive = true, IsDeleted = true }, false, "удалённый")
                    .Entity(new CategoryOrganizationUnit { Id = 4, IsActive = false, IsDeleted = true }, false, "неактивный и удалённый"),

                For(Specs.Find.Erm.Clients())
                    .Entity(new Client { Id = 1 }, true, "по умолчанию")
                    .Entity(new Client { Id = 2, IsActive = false, IsDeleted = false }, false, "неактивный")
                    .Entity(new Client { Id = 3, IsActive = true, IsDeleted = true }, false, "удалённый")
                    .Entity(new Client { Id = 4, IsActive = false, IsDeleted = true }, false, "неактивный и удалённый"),

                For(Specs.Find.Erm.Contacts())
                    .Entity(new Contact { Id = 1 }, true, "по умолчанию")
                    .Entity(new Contact { Id = 1, IsFired = true }, false, "уволенный")
                    .Entity(new Contact { Id = 2, IsActive = false, IsDeleted = false }, false, "неактивный")
                    .Entity(new Contact { Id = 3, IsActive = true, IsDeleted = true }, false, "удалённый")
                    .Entity(new Contact { Id = 4, IsActive = false, IsDeleted = true }, false, "неактивный и удалённый"),

                For(Specs.Find.Erm.Firms())
                    .Entity(new Firm { Id = 1 }, true, "по умолчанию")
                    .Entity(new Firm { Id = 2, IsActive = false }, false, "неактивная")
                    .Entity(new Firm { Id = 3, IsDeleted = true }, false, "удалённая")
                    .Entity(new Firm { Id = 4, IsActive = false, IsDeleted = true }, false, "неактивная и удалённая")
                    .Entity(new Firm { Id = 5, ClosedForAscertainment = true }, false, "скрытая до выяснения"),

                For(Specs.Find.Erm.FirmAddresses())
                    .Entity(new FirmAddress { Id = 1 }, true, "по умолчанию")
                    .Entity(new FirmAddress { Id = 2, IsActive = false }, false, "неактивный")
                    .Entity(new FirmAddress { Id = 3, IsDeleted = true }, false, "удалённый")
                    .Entity(new FirmAddress { Id = 4, IsActive = false, IsDeleted = true }, false, "неактивный и удалённый")
                    .Entity(new FirmAddress { Id = 5, ClosedForAscertainment = true }, false, "скрытый до выяснения"),

                For(Specs.Find.Erm.LegalPersons())
                    .Entity(new LegalPerson { Id = 1 }, true, "по умолчанию")
                    .Entity(new LegalPerson { Id = 1, IsActive = false }, false, "неактивное")
                    .Entity(new LegalPerson { Id = 1, IsDeleted = true }, false, "удалённое")
                    .Entity(new LegalPerson { Id = 1, IsActive = false, IsDeleted = true }, false, "неактивное и удалённое"),

                For(Specs.Find.Erm.FirmContacts())
                    .Entity(new FirmContact { Id = 1 }, true, "по умолчанию"),

                For(Specs.Find.Erm.Orders())
                    .Entity(new Order { Id = 1 }, true, "на оформлении")
                    .Entity(new Order { Id = 2, IsActive = false }, false, "неактивный")
                    .Entity(new Order { Id = 3, IsDeleted = true }, false, "удалённый")
                    .Entity(new Order { Id = 4, IsActive = false, IsDeleted = true }, false, "неактивный и удалённый")
                    .Entity(new Order { Id = 5, WorkflowStepId = 1 }, true, "на расторжении"),

                For(Specs.Find.Erm.Projects())
                    .Entity(new Project { Id = 1 }, true, "по умолчанию")
                    .Entity(new Project { Id = 2, IsActive = false }, false, "неактивный"),

                For(Specs.Find.Erm.Territories())
                    .Entity(new Territory { Id = 1 }, true, "по умолчанию")
                    .Entity(new Territory { Id = 2, IsActive = false }, false, "неактианая"),
            }.SelectMany(x => x.Build());

        private static TestCaseDataBuilder For<T>(FindSpecification<T> specification)
        {
            return new TestCaseDataBuilder().SetSpecification(specification);
        }

        private class TestCaseDataBuilder
        {
            private object _specification;
            private readonly IList<TestCaseData> _list = new List<TestCaseData>();

            public TestCaseDataBuilder SetSpecification<T>(FindSpecification<T> specification)
            {
                _specification = specification;
                return this;
            }

            public TestCaseDataBuilder Entity<T>(T entity, bool expectation, string comment)
            {
                _list.Add(new TestCaseData(entity, _specification, expectation).SetName(typeof(T).Name + ", " + comment));
                return this;
            }

            public IEnumerable<TestCaseData> Build()
            {
                return _list;
            }
        }

        private void ShouldReadActivity<T>(Func<IQuery, IReadOnlyCollection<long>,  IEnumerable<T>> func)
            where T : ActivityBase, new()
        {
            const int ActivityStatusCompleted = 2;

            SourceDb.Has(new T { Id = 1, IsActive = true, IsDeleted = false, Status = ActivityStatusCompleted })
                 .Has(new T { Id = 2, IsActive = false, IsDeleted = false, Status = ActivityStatusCompleted })
                 .Has(new T { Id = 3, IsActive = true, IsDeleted = true, Status = ActivityStatusCompleted })
                 .Has(new T { Id = 4, IsActive = true, IsDeleted = false, Status = 0 });

            Reader.Create(Query)
                  .VerifyRead(x => func(x, new[] { 1L }), new[] { new T { Id = 1, IsActive = true, IsDeleted = false, Status = ActivityStatusCompleted } })
                  .VerifyRead(x => func(x, new[] { 2L }), Enumerable.Empty<T>())
                  .VerifyRead(x => func(x, new[] { 3L }), Enumerable.Empty<T>())
                  .VerifyRead(x => func(x, new[] { 4L }), Enumerable.Empty<T>());
        }

        private void ShouldReadActivityReference<TReference>(Func<IQuery, IEnumerable<TReference>> clientsRefs, Func<IQuery, IEnumerable<TReference>> firmsRefs)
            where TReference : ActivityReference, new()
        {
            const int ReferenceRegardingObject = 1;

            SourceDb.Has(new TReference { Reference = ReferenceRegardingObject, ReferencedType = EntityTypeIds.Firm })
                 .Has(new TReference { Reference = 0, ReferencedType = EntityTypeIds.Firm })
                 .Has(new TReference { Reference = ReferenceRegardingObject, ReferencedType = EntityTypeIds.Client })
                 .Has(new TReference { Reference = 0, ReferencedType = EntityTypeIds.Client })
                 .Has(new TReference { Reference = ReferenceRegardingObject, ReferencedType = 0 })
                 .Has(new TReference { Reference = 0, ReferencedType = 0 });

            Reader.Create(Query)
                  .VerifyRead(clientsRefs, new[] { new TReference { Reference = ReferenceRegardingObject, ReferencedType = EntityTypeIds.Client } })
                  .VerifyRead(firmsRefs, new[] { new TReference { Reference = ReferenceRegardingObject, ReferencedType = EntityTypeIds.Firm } });
        }

        private class Reader
        {
            private readonly IQuery _query;

            private Reader(IQuery query)
            {
                _query = query;
            }

            public static Reader Create(IQuery query)
            {
                return new Reader(query);
            }

            public Reader Verify(Func<IQuery, bool> reader, bool expected)
            {
                Assert.That(reader(_query), Is.EqualTo(expected));
                return this;
            }

            public Reader VerifyRead<T>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, string message = null)
            {
                VerifyRead(reader, expected, x => x, message);
                return this;
            }

            private Reader VerifyRead<T, TProjection>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, Func<T, TProjection> projector, string message = null)
            {
                Assert.That(reader(_query).ToArray(), Is.EqualTo(expected.ToArray()).Using(new ProjectionEqualityComparer<T, TProjection>(projector)), message);
                return this;
            }
        }
    }
}