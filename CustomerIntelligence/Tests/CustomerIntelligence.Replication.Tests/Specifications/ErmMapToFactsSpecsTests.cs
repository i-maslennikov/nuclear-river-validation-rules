using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Erm;
using NuClear.Storage.API.Readings;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.Specifications
{
    [TestFixture, SetCulture("")]
    internal partial class ErmMapToFactsSpecsTests : DataFixtureBase
    {
        private static readonly DateTimeOffset Date = new DateTimeOffset(2015, 04, 03, 12, 30, 00, new TimeSpan());

        [Test]
        public void ShouldTransformAccount()
        {
            SourceDb.Has(
                new Account { Id = 1, Balance = 123.45m, LegalPersonId = 2 });

            Transformation.Create(Query)
                          .VerifyTransform(q => Specs.Map.Erm.ToFacts.Accounts.Map(q).By(x => x.Id, 1),
                                           new Storage.Model.Facts.Account { Id = 1, Balance = 123.45m, LegalPersonId = 2 });
        }

        [Test]
        public void ShouldTransformBranchOfficeOrganizationUnit()
        {
            SourceDb.Has(
                new BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 });

            Transformation.Create(Query)
                          .VerifyTransform(q => Specs.Map.Erm.ToFacts.BranchOfficeOrganizationUnits.Map(q).By(x => x.Id, 1),
                                           new Storage.Model.Facts.BranchOfficeOrganizationUnit { Id = 1, OrganizationUnitId = 2 });
        }

        [Test]
        public void ShouldTransformCategory()
        {
            SourceDb.Has(
                new Category { Id = 1, Level = 2, ParentId = 3 });

            Transformation.Create(Query)
                          .VerifyTransform(q => Specs.Map.Erm.ToFacts.Categories.Map(q).By(x => x.Id, 1),
                                           new Storage.Model.Facts.Category { Id = 1, Level = 2, ParentId = 3 });
        }

        [Test]
        public void ShouldTransformCategoryFirmAddress()
        {
            SourceDb.Has(
                new CategoryFirmAddress { Id = 1, CategoryId = 2, FirmAddressId = 3 });

            Transformation.Create(Query)
                          .VerifyTransform(q => Specs.Map.Erm.ToFacts.CategoryFirmAddresses.Map(q).By(x => x.Id, 1),
                                           new Storage.Model.Facts.CategoryFirmAddress { Id = 1, CategoryId = 2, FirmAddressId = 3 });
        }

        [Test]
        public void ShouldTransformCategoryGroup()
        {
            SourceDb.Has(
                new CategoryGroup { Id = 1, Name = "name", Rate = 1 });

            Transformation.Create(Query)
                          .VerifyTransform(q => Specs.Map.Erm.ToFacts.CategoryGroups.Map(q).By(x => x.Id, 1),
                                           new Storage.Model.Facts.CategoryGroup { Id = 1, Name = "name", Rate = 1 });
        }

        [Test]
        public void ShouldTransformCategoryOrganizationUnit()
        {
            SourceDb.Has(
                new CategoryOrganizationUnit { Id = 1, CategoryId = 2, CategoryGroupId = 3, OrganizationUnitId = 4 });

            Transformation.Create(Query)
                          .VerifyTransform(q => Specs.Map.Erm.ToFacts.CategoryOrganizationUnits.Map(q).By(x => x.Id, 1),
                                           new Storage.Model.Facts.CategoryOrganizationUnit { Id = 1, CategoryId = 2, CategoryGroupId = 3, OrganizationUnitId = 4 });
        }

        [Test]
        public void ShouldTransformClient()
        {
            SourceDb.Has(
                new Client { Id = 1, Name = "client", LastDisqualifyTime = Date },
                new Client { Id = 2, MainPhoneNumber = "phone" },
                new Client { Id = 3, AdditionalPhoneNumber1 = "phone" },
                new Client { Id = 4, AdditionalPhoneNumber2 = "phone" },
                new Client { Id = 5, Website = "site" });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Clients.Map(x).By(y => y.Id, 1), new Storage.Model.Facts.Client { Id = 1, Name = "client", LastDisqualifiedOn = Date })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Clients.Map(x).By(y => y.Id, 2), new Storage.Model.Facts.Client { Id = 2, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Clients.Map(x).By(y => y.Id, 3), new Storage.Model.Facts.Client { Id = 3, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Clients.Map(x).By(y => y.Id, 4), new Storage.Model.Facts.Client { Id = 4, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Clients.Map(x).By(y => y.Id, 5), new Storage.Model.Facts.Client { Id = 5, HasWebsite = true });
        }

        [Test]
        public void ShouldTransformContact()
        {
            SourceDb.Has(
                new Contact { Id = 1, ClientId = 2 },
                new Contact { Id = 2, Role = 200000 },
                new Contact { Id = 3, Role = 200001 },
                new Contact { Id = 4, Role = 200002 },
                new Contact { Id = 5, MainPhoneNumber = "phone" },
                new Contact { Id = 6, MobilePhoneNumber = "phone" },
                new Contact { Id = 7, HomePhoneNumber = "phone" },
                new Contact { Id = 8, AdditionalPhoneNumber = "phone" },
                new Contact { Id = 9, Website = "site" });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).By(y => y.Id, 1), new Storage.Model.Facts.Contact { Id = 1, ClientId = 2 })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).By(y => y.Id, 2), new Storage.Model.Facts.Contact { Id = 2, Role = 1 })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).By(y => y.Id, 3), new Storage.Model.Facts.Contact { Id = 3, Role = 2 })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).By(y => y.Id, 4), new Storage.Model.Facts.Contact { Id = 4, Role = 3 })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).By(y => y.Id, 5), new Storage.Model.Facts.Contact { Id = 5, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).By(y => y.Id, 6), new Storage.Model.Facts.Contact { Id = 6, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).By(y => y.Id, 7), new Storage.Model.Facts.Contact { Id = 7, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).By(y => y.Id, 8), new Storage.Model.Facts.Contact { Id = 8, HasPhone = true })
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Contacts.Map(x).By(y => y.Id, 9), new Storage.Model.Facts.Contact { Id = 9, HasWebsite = true });
        }

        [Test]
        public void ShouldTransformFirm()
        {
            SourceDb.Has(
                new Firm { Id = 1, Name = "firm", CreatedOn = Date, LastDisqualifyTime = Date.AddDays(1), ClientId = 2, OrganizationUnitId = 3, OwnerId = 5 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Firms.Map(x).By(y => y.Id, 1),
                                           new Storage.Model.Facts.Firm
                                               {
                                                   Id = 1,
                                                   Name = "firm",
                                                   CreatedOn = Date,
                                                   LastDisqualifiedOn = Date.AddDays(1),
                                                   ClientId = 2,
                                                   OrganizationUnitId = 3,
                                                   OwnerId = 5
                                               });
        }

        [Test]
        public void ShouldTransformLead()
        {
            SourceDb.Has(
                new Lead
                {
                    Id = 1,
                    FirmId = 2,
                    OwnerId = 3,
                    Type = 1,
                    Status = 1
                });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Leads.Map(x).By(y => y.Id, 1),
                                           new Storage.Model.Facts.Lead
                                           {
                                               Id = 1,
                                               FirmId = 2,
                                               IsInQueue = false,
                                               Type = 1
                                           });
        }

        [Test]
        public void ShouldTransformFirmAddress()
        {
            SourceDb.Has(
                new FirmAddress { Id = 1, FirmId = 2, TerritoryId = 3 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.FirmAddresses.Map(x).By(y => y.Id, 1),
                                           new Storage.Model.Facts.FirmAddress { Id = 1, FirmId = 2, TerritoryId = 3 });
        }

        [Test]
        public void ShouldTransformFirmContact()
        {
            const long NotNull = 123;

            SourceDb.Has(
                new FirmContact { Id = 1, ContactType = 1, FirmAddressId = NotNull },
                new FirmContact { Id = 2, ContactType = 1, FirmAddressId = null },
                new FirmContact { Id = 3, ContactType = 2, FirmAddressId = NotNull },
                new FirmContact { Id = 4, ContactType = 4, FirmAddressId = NotNull });

            Transformation.Create(Query)
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.FirmContacts.Map(x).By(y => y.Id, 1), new Storage.Model.Facts.FirmContact { Id = 1, HasPhone = true, FirmAddressId = NotNull })
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.FirmContacts.Map(x).By(y => y.Id, 2), Enumerable.Empty<Storage.Model.Facts.FirmContact>())
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.FirmContacts.Map(x).By(y => y.Id, 3), Enumerable.Empty<Storage.Model.Facts.FirmContact>())
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.FirmContacts.Map(x).By(y => y.Id, 4), new Storage.Model.Facts.FirmContact { Id = 4, HasWebsite = true, FirmAddressId = NotNull });
        }

        [Test]
        public void ShouldTransformLegalPerson()
        {
            SourceDb.Has(
                new LegalPerson { Id = 1, ClientId = 2 },
                new LegalPerson { Id = 2, ClientId = null });

            Transformation.Create(Query)
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.LegalPersons.Map(x).By(y => y.Id, 1), new Storage.Model.Facts.LegalPerson { Id = 1, ClientId = 2 })
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.LegalPersons.Map(x).By(y => y.Id, 2), Enumerable.Empty<Storage.Model.Facts.LegalPerson>());
        }

        [Test]
        public void ShouldTransformOrder()
        {
            SourceDb.Has(
                new Order { Id = 1, EndDistributionDateFact = Date, WorkflowStepId = 1, FirmId = 2 },
                new Order { Id = 2, EndDistributionDateFact = Date, WorkflowStepId = 4 /* on termination*/, FirmId = 2 });

            Transformation.Create(Query)
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.Orders.Map(x).By(y => y.Id, 1), Enumerable.Empty<Storage.Model.Facts.Order>())
                .VerifyTransform(x => Specs.Map.Erm.ToFacts.Orders.Map(x).By(y => y.Id, 2), new Storage.Model.Facts.Order { Id = 2, EndDistributionDateFact = Date, FirmId = 2 });
        }

        [Test]
        public void ShouldTransformProject()
        {
            SourceDb.Has(
                new Project { Id = 1, Name = "name", OrganizationUnitId = 2 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Projects.Map(x).By(y => y.Id, 1),
                                           new Storage.Model.Facts.Project { Id = 1, Name = "name", OrganizationUnitId = 2 });
        }

        [Test]
        public void ShouldTransformTerritory()
        {
            SourceDb.Has(
                new Territory { Id = 1, Name = "name", OrganizationUnitId = 2 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Erm.ToFacts.Territories.Map(x).By(y => y.Id, 1),
                                           new Storage.Model.Facts.Territory { Id = 1, Name = "name", OrganizationUnitId = 2 });
        }

        private class Transformation
        {
            private readonly IQuery _query;

            private Transformation(IQuery query)
            {
                _query = query;
            }

            public static Transformation Create(IQuery query)
            {
                return new Transformation(query);
            }

            public Transformation VerifyTransform<T>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, string message = null)
            {
                VerifyTransform(reader, expected, x => x, message);
                return this;
            }

            public Transformation VerifyTransform<T>(Func<IQuery, IEnumerable<T>> reader, params T[] expected)
            {
                VerifyTransform(reader, expected, x => x);
                return this;
            }

            public Transformation VerifyTransform<T, TProjection>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, Func<T, TProjection> projector, string message = null)
            {
                // TODO: convert to a custom NUnit constraint, at least for fail logging
                Assert.That(reader(_query).ToArray, Is.EqualTo(expected.ToArray()).Using(new ProjectionEqualityComparer<T, TProjection>(projector)), message);
                return this;
            }
        }
    }
}