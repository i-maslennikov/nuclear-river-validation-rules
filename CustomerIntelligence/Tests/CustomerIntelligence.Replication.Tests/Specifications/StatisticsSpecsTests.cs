using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.CustomerIntelligence.Storage.Model.Statistics;
using NuClear.Storage.API.Readings;

using NUnit.Framework;

using ProjectCategoryStatistics = NuClear.CustomerIntelligence.Storage.Model.Bit.ProjectCategoryStatistics;

namespace NuClear.CustomerIntelligence.Replication.Tests.Specifications
{
    [TestFixture, SetCulture("")]
    internal class StatisticsSpecsTests : DataFixtureBase
    {
        [Test]
        public void ShouldFillCategoriesWithoutStatisticsWithZeros()
        {
            SourceDb.Has(new Project { Id = 1 })
                    .Has(new Firm { Id = 1 },
                         new Firm { Id = 2 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 },
                         new FirmAddress { Id = 2, FirmId = 2 })
                    .Has(new Category { Id = 1 },
                         new Category { Id = 2 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 },
                         new CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 1 })
                    .Has(new FirmCategoryStatistics { FirmId = 2, CategoryId = 1, ProjectId = 1, Hits = 100, Shows = 200 })
                    .Has(new ProjectCategoryStatistics { ProjectId = 1, CategoryId = 1, AdvertisersCount = 1 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.FirmCategory3.Map(x),
                                           new[]
                                               {
                                                   new FirmCategory3 { FirmId = 1, CategoryId = 1, ProjectId = 1, AdvertisersShare = 0.5f, FirmCount = 2, Hits = 0, Shows = 0 },
                                                   new FirmCategory3 { FirmId = 2, CategoryId = 1, ProjectId = 1, AdvertisersShare = 0.5f, FirmCount = 2, Hits = 100, Shows = 200 }
                                               });
        }

        [Test]
        public void ShouldTransformFirmCategoryStatistics()
        {
            SourceDb.Has(new Project { Id = 1 })
                    .Has(new Firm { Id = 1 },
                         new Firm { Id = 2 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 },
                         new FirmAddress { Id = 2, FirmId = 2 })
                    .Has(new Category { Id = 1 },
                         new Category { Id = 2 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 },
                         new CategoryFirmAddress { Id = 2, FirmAddressId = 2, CategoryId = 1 },
                         new CategoryFirmAddress { Id = 3, FirmAddressId = 2, CategoryId = 2 })
                    .Has(new FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 1, Hits = 10000, Shows = 20000 })
                    .Has(new ProjectCategoryStatistics { ProjectId = 1, CategoryId = 1, AdvertisersCount = 1 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.FirmCategory3.Map(x),
                                           new[]
                                               {
                                                   new FirmCategory3 { FirmId = 1, CategoryId = 1, ProjectId = 1, AdvertisersShare = 0.5f, FirmCount = 2,Hits = 10000, Shows = 20000 },
                                                   new FirmCategory3 { FirmId = 2, CategoryId = 1, ProjectId = 1, AdvertisersShare = 0.5f, FirmCount = 2, Hits = 0, Shows = 0 },
                                                   new FirmCategory3 { FirmId = 2, CategoryId = 2, ProjectId = 1, AdvertisersShare = 0f, FirmCount = 1, Hits = 0, Shows = 0 }
                                               });
        }

        [Test]
        public void AdvertisersShareShouldNotBeMoreThanOne()
        {
            SourceDb.Has(new Project { Id = 1 })
                    .Has(new Firm { Id = 1 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new Category { Id = 1 })
                    .Has(new CategoryFirmAddress { FirmAddressId = 1, CategoryId = 1 })
                    .Has(new FirmCategoryStatistics { FirmId = 1, CategoryId = 1, ProjectId = 1, Hits = 10000, Shows = 20000 })
                    .Has(new ProjectCategoryStatistics { ProjectId = 1, CategoryId = 1, AdvertisersCount = 5 });

            Transformation.Create(Query)
                          .VerifyTransform(x => Specs.Map.Facts.ToCI.FirmCategory3.Map(x),
                                           new[]
                                               {
                                                   new FirmCategory3 { FirmId = 1, CategoryId = 1, ProjectId = 1, AdvertisersShare = 1f, FirmCount = 1, Hits = 10000, Shows = 20000 }
                                               });
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

            public Transformation VerifyTransform<T, TProjection>(Func<IQuery, IEnumerable<T>> reader, IEnumerable<T> expected, Func<T, TProjection> projector, string message = null)
            {
                // TODO: convert to a custom NUnit constraint, at least for fail logging
                Assert.That(reader(_query), Is.EquivalentTo(expected).Using(new ProjectionEqualityComparer<T, TProjection>(projector)), message);
                return this;
            }
        }
    }
}