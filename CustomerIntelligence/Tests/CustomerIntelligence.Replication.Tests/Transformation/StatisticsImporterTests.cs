using System;
using System.Linq;

using Moq;

using NuClear.CustomerIntelligence.Domain;
using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.Metamodeling.Elements;
using NuClear.Replication.Core.Facts;
using NuClear.River.Common.Metadata.Elements;

using NUnit.Framework;

using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal class StatisticsImporterTests : DataFixtureBase
    {
        [Test]
        public void ShouldProduceCalculateStatisticsOperationForFirmStatisticsDto()
        {
            // Arrange
            var repositoryFactory = new VerifiableRepositoryFactory();
            var dto = new FirmStatisticsDto
            {
                ProjectId = 1,
                Firms = new[]
                                  {
                                      new FirmStatisticsDto.FirmDto
                                      {
                                            FirmId = 2,
                                            Categories = new[]
                                            {
                                                new FirmStatisticsDto.FirmDto.CategoryDto
                                                {
                                                    CategoryId = 3,
                                                    Hits = 4,
                                                    Shows = 5
                                                }
                                            }
                                      }
                                  }
            };

            SourceDb.Has(new Bit::FirmCategoryStatistics { ProjectId = 1, FirmId = 7 },
                         new Bit::FirmCategoryStatistics { ProjectId = 2, FirmId = 8 });

            var metadataSource = new ImportStatisticsMetadataSource();
            var identity = new Uri($"{typeof(FirmStatisticsDto).Name}/{typeof(Bit::FirmCategoryStatistics).Name}", UriKind.Relative);
            IMetadataElement importStatisticsMetadata;
            if (!metadataSource.Metadata.Values.TryGetElementById(identity, out importStatisticsMetadata))
            {
                throw new NotSupportedException($"The aggregate of type '{typeof(FirmStatisticsDto).Name}' is not supported.");
            }

            var importer = new StatisticsFactImporter<Bit::FirmCategoryStatistics, FirmStatisticsDto>(
                (ImportStatisticsMetadata<Bit::FirmCategoryStatistics, FirmStatisticsDto>)importStatisticsMetadata,
                Query,
                repositoryFactory.Create<Bit::FirmCategoryStatistics>());

            // Act
            var operations = importer.Import(dto).ToArray();

            // Assert
            Assert.That(operations.Count(), Is.EqualTo(1));
            repositoryFactory.Verify<Bit::FirmCategoryStatistics>(
                m => m.Delete(It.Is(Predicate.Match(new Bit::FirmCategoryStatistics { ProjectId = 1, FirmId = 7 }))),
                Times.AtLeastOnce);
            repositoryFactory.Verify<Bit::FirmCategoryStatistics>(
                m => m.Add(It.Is(Predicate.Match(new Bit::FirmCategoryStatistics { ProjectId = 1, FirmId = 2, CategoryId = 3, Hits = 4, Shows = 5 }))),
                Times.AtLeastOnce);
        }

        [Test]
        public void ShouldProduceCalculateStatisticsOperationForCategoryStatisticsDto()
        {
            // Arrange
            var repositoryFactory = new VerifiableRepositoryFactory();
            var dto = new CategoryStatisticsDto
            {
                ProjectId = 1,
                Categories = new[]
                                  {
                                      new CategoryStatisticsDto.CategoryDto
                                      {
                                          CategoryId = 2,
                                          AdvertisersCount = 3,
                                      }
                                  }
            };
            SourceDb.Has(new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 7 },
                         new Bit::ProjectCategoryStatistics { ProjectId = 2, CategoryId = 7 });

            var metadataSource = new ImportStatisticsMetadataSource();
            var identity = new Uri($"{typeof(CategoryStatisticsDto).Name}/{typeof(Bit::ProjectCategoryStatistics).Name}", UriKind.Relative);
            IMetadataElement importStatisticsMetadata;
            if (!metadataSource.Metadata.Values.TryGetElementById(identity, out importStatisticsMetadata))
            {
                throw new NotSupportedException($"The aggregate of type '{typeof(CategoryStatisticsDto).Name}' is not supported.");
            }

            var importer = new StatisticsFactImporter<Bit::ProjectCategoryStatistics, CategoryStatisticsDto>(
                (ImportStatisticsMetadata<Bit::ProjectCategoryStatistics, CategoryStatisticsDto>)importStatisticsMetadata,
                Query,
                repositoryFactory.Create<Bit::ProjectCategoryStatistics>());

            // Act
            var operations = importer.Import(dto).ToArray();

            // Assert
            Assert.That(operations.Count(), Is.EqualTo(1));
            repositoryFactory.Verify<Bit::ProjectCategoryStatistics>(
                m => m.Delete(It.Is(Predicate.Match(new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 7 }))),
                Times.AtLeastOnce);
            repositoryFactory.Verify<Bit::ProjectCategoryStatistics>(
                m => m.Add(It.Is(Predicate.Match(new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 2, AdvertisersCount = 3 }))),
                Times.AtLeastOnce);
        }
    }
}