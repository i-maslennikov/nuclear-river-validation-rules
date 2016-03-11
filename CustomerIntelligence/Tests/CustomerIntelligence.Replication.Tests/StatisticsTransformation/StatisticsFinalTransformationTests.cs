using System.Linq;

using Moq;

using NuClear.CustomerIntelligence.Domain;
using NuClear.CustomerIntelligence.Storage;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Elements;
using NuClear.Storage.API.Writings;

using NUnit.Framework;

using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;
using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;

namespace NuClear.CustomerIntelligence.Replication.Tests.StatisticsTransformation
{
    [TestFixture]
    internal sealed class StatisticsFinalTransformationTests
    {
        private static readonly object[] data =
            {
                new Facts::Project { Id = 1, OrganizationUnitId = 1 },
                new Facts::Project { Id = 2, OrganizationUnitId = 2 },
                new Facts::Project { Id = 3, OrganizationUnitId = 3 },

                new Facts::Firm { Id = 10, OrganizationUnitId = 1 },
                new Facts::Firm { Id = 11, OrganizationUnitId = 1 },
                new Facts::Firm { Id = 12, OrganizationUnitId = 2 },
                new Facts::Firm { Id = 13, OrganizationUnitId = 3 },

                new Facts::FirmAddress { Id = 10, FirmId = 10 },
                new Facts::FirmAddress { Id = 11, FirmId = 11 },
                new Facts::FirmAddress { Id = 12, FirmId = 12 },
                new Facts::FirmAddress { Id = 13, FirmId = 13 },

                new Facts::Category { Id = 100 },
                new Facts::Category { Id = 101 },
                new Facts::Category { Id = 102 },

                new Facts::CategoryFirmAddress { FirmAddressId = 10, CategoryId = 100 },
                new Facts::CategoryFirmAddress { FirmAddressId = 10, CategoryId = 101 },
                new Facts::CategoryFirmAddress { FirmAddressId = 10, CategoryId = 102 },

                new Facts::CategoryFirmAddress { FirmAddressId = 11, CategoryId = 100 },
                new Facts::CategoryFirmAddress { FirmAddressId = 11, CategoryId = 101 },

                new Facts::CategoryFirmAddress { FirmAddressId = 12, CategoryId = 100 },

                new Facts::CategoryFirmAddress { FirmAddressId = 13, CategoryId = 100 },
                new Facts::CategoryFirmAddress { FirmAddressId = 13, CategoryId = 101 },

                new Bit::FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 100, },
                new Bit::FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 101, },
                new Bit::FirmCategoryStatistics { ProjectId = 1, FirmId = 10, CategoryId = 102, },
                new Bit::FirmCategoryStatistics { ProjectId = 1, FirmId = 11, CategoryId = 100, },
                new Bit::FirmCategoryStatistics { ProjectId = 1, FirmId = 11, CategoryId = 101, },
                new Bit::FirmCategoryStatistics { ProjectId = 2, FirmId = 12, CategoryId = 100, },
                new Bit::FirmCategoryStatistics { ProjectId = 3, FirmId = 13, CategoryId = 100, Hits = 1, Shows = 2 },
                new Bit::FirmCategoryStatistics { ProjectId = 3, FirmId = 13, CategoryId = 101, Hits = 1, Shows = 2 },

                new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 100, },
                new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 101, },
                new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 102, },
                new Bit::ProjectCategoryStatistics { ProjectId = 2, CategoryId = 100, },
                new Bit::ProjectCategoryStatistics { ProjectId = 3, CategoryId = 100, AdvertisersCount = 2 },
                new Bit::ProjectCategoryStatistics { ProjectId = 3, CategoryId = 101, AdvertisersCount = 2 },

                new CI::Firm { Id = 10, ProjectId = 1, },
                new CI::Firm { Id = 11, ProjectId = 1, },
                new CI::Firm { Id = 12, ProjectId = 2, },
                new CI::Firm { Id = 13, ProjectId = 3, },

                new Statistics::FirmCategory3 { ProjectId = 1, FirmId = 10, CategoryId = 100, },
                new Statistics::FirmCategory3 { ProjectId = 1, FirmId = 10, CategoryId = 101, },
                new Statistics::FirmCategory3 { ProjectId = 1, FirmId = 10, CategoryId = 102, },
                new Statistics::FirmCategory3 { ProjectId = 1, FirmId = 11, CategoryId = 100, },
                new Statistics::FirmCategory3 { ProjectId = 1, FirmId = 11, CategoryId = 101, },
                new Statistics::FirmCategory3 { ProjectId = 2, FirmId = 12, CategoryId = 100, },
                new Statistics::FirmCategory3 { ProjectId = 3, FirmId = 13, CategoryId = 100, Shows = 2, Hits = 1, AdvertisersShare = 1f, FirmCount = 1 },
                new Statistics::FirmCategory3 { ProjectId = 3, FirmId = 13, CategoryId = 101, },
            };

        [Test]
        public void ShouldRecalculateOnlySpecifiedProjectCategory()
        {
            Mock<IRepository<Statistics::FirmCategory3>> repository;
            var processor = StatisticsProcessor(data, out repository);

            processor.RecalculateStatistics(new StatisticsProcessorSlice {ProjectId = 1, CategoryIds = new long[] { 100 } });

            repository.Verify(x => x.Add(It.IsAny<Statistics::FirmCategory3>()), Times.Never);
            repository.Verify(x => x.Delete(It.IsAny<Statistics::FirmCategory3>()), Times.Never);
            repository.Verify(x => x.Update(It.Is<Statistics::FirmCategory3>(y => y.ProjectId != 1)), Times.Never);
            repository.Verify(x => x.Update(It.Is<Statistics::FirmCategory3>(y => y.CategoryId != 100)), Times.Never);

            repository.Verify(x => x.Update(It.Is<Statistics::FirmCategory3>(y => y.ProjectId == 1 && y.CategoryId == 100)), Times.AtLeastOnce);
        }

        [Test]
        public void ShouldRecalculateOnlySpecifiedProject()
        {
            Mock<IRepository<Statistics::FirmCategory3>> repository;
            var processor = StatisticsProcessor(data, out repository);

            processor.RecalculateStatistics(new StatisticsProcessorSlice { ProjectId = 1 });

            repository.Verify(x => x.Add(It.IsAny<Statistics::FirmCategory3>()), Times.Never);
            repository.Verify(x => x.Delete(It.IsAny<Statistics::FirmCategory3>()), Times.Never);
            repository.Verify(x => x.Update(It.Is<Statistics::FirmCategory3>(y => y.ProjectId != 1)), Times.Never);

            repository.Verify(x => x.Update(It.Is<Statistics::FirmCategory3>(y => y.ProjectId == 1 && y.CategoryId == 100)), Times.AtLeastOnce);
            repository.Verify(x => x.Update(It.Is<Statistics::FirmCategory3>(y => y.ProjectId == 1 && y.CategoryId == 101)), Times.AtLeastOnce);
            repository.Verify(x => x.Update(It.Is<Statistics::FirmCategory3>(y => y.ProjectId == 1 && y.CategoryId == 102)), Times.AtLeastOnce);
        }

        [Test]
        public void ShouldUpdateOnlyChangedRecords()
        {
            Mock<IRepository<Statistics::FirmCategory3>> repository;
            var processor = StatisticsProcessor(data, out repository);

            processor.RecalculateStatistics(new StatisticsProcessorSlice { ProjectId = 3 });

            repository.Verify(x => x.Update(It.Is<Statistics::FirmCategory3>(y => y.CategoryId == 100)), Times.Never);
            repository.Verify(x => x.Update(It.Is<Statistics::FirmCategory3>(y => y.CategoryId == 101)), Times.Once);
        }

        private static IStatisticsProcessor StatisticsProcessor<T>(object[] data, out Mock<IRepository<T>> repository)
            where T : class
        {
            var metadataSource = new StatisticsRecalculationMetadataSource();
            var metadata = metadataSource.Metadata.Values.SelectMany(x => x.Elements).OfType<StatisticsRecalculationMetadata<T>>().Single();
            repository = new Mock<IRepository<T>>();
            var comparerFactory = new EqualityComparerFactory(new LinqToDbPropertyProvider(Schema.Erm, Schema.Facts, Schema.CustomerIntelligence));

            return new StatisticsProcessor<T>(metadata, new MemoryMockQuery(data), new BulkRepository<T>(repository.Object), comparerFactory);
        }
    }
}
