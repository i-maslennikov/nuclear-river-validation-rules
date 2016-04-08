using System;
using System.Linq;

using Moq;

using NuClear.CustomerIntelligence.Domain;
using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.Metamodeling.Elements;
using NuClear.Replication.Core.API.Facts;
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
            var dto = new FirmPopularity
                {
                    ProjectId = 1,
                    Firms = new[]
                        {
                            new FirmPopularity.Firm
                                {
                                    FirmId = 2,
                                    Categories = new[]
                                        {
                                            new FirmPopularity.Firm.Category
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

            var importer = CreateProcessor<FirmPopularity, Bit::FirmCategoryStatistics>(repositoryFactory);

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
            var dto = new RubricPopularity
                {
                    ProjectId = 1,
                    Categories = new[]
                        {
                            new RubricPopularity.Category
                                {
                                    CategoryId = 2,
                                    AdvertisersCount = 3,
                                }
                        }
                };

            SourceDb.Has(new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 7 },
                         new Bit::ProjectCategoryStatistics { ProjectId = 2, CategoryId = 7 });

            var importer = CreateProcessor<RubricPopularity, Bit::ProjectCategoryStatistics>(repositoryFactory);

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

        [Test]
        public void ShouldProcessFirmForecastFromFirmForecastDto()
        {
            // Arrange
            var repositoryFactory = new VerifiableRepositoryFactory();
            var dto = new FirmForecast
                {
                    ProjectId = 1,
                    Firms = new[]
                        {
                            new FirmForecast.Firm
                                {
                                    Id = 1,
                                    ForecastClick = 1,
                                    ForecastAmount = 1,
                                    Categories = new[]
                                        {
                                            new FirmForecast.Category
                                                {
                                                    Id = 1,
                                                    ForecastAmount = 1,
                                                    ForecastClick = 1,
                                                }
                                        }
                                }
                        }
                };

            SourceDb.Has(new Bit::FirmForecast { ProjectId = 1, FirmId = 1 },
                         new Bit::FirmForecast { ProjectId = 2, FirmId = 2 });

            var importer = CreateProcessor<FirmForecast, Bit::FirmForecast>(repositoryFactory);

            // Act
            var operations = importer.Import(dto).ToArray();

            // Assert
            Assert.That(operations.Count(), Is.EqualTo(1));
            repositoryFactory.Verify<Bit::FirmForecast>(
                m => m.Delete(It.Is(Predicate.Match(new Bit::FirmForecast { ProjectId = 1, FirmId = 1 }))), Times.AtLeastOnce);
            repositoryFactory.Verify<Bit::FirmForecast>(
                m => m.Delete(It.Is(Predicate.Match(new Bit::FirmForecast { ProjectId = 2, FirmId = 2 }))), Times.Never);
            repositoryFactory.Verify<Bit::FirmForecast>(
                m => m.Add(It.Is(Predicate.Match(new Bit::FirmForecast { ProjectId = 1, FirmId = 1, ForecastClick = 1, ForecastAmount = 1 }))), Times.AtLeastOnce);
        }

        [Test]
        public void ShouldProcessFirmCategoryForecastFromFirmForecastDto()
        {
            // Arrange
            var repositoryFactory = new VerifiableRepositoryFactory();
            var dto = new FirmForecast
            {
                ProjectId = 1,
                Firms = new[]
                        {
                            new FirmForecast.Firm
                                {
                                    Id = 1,
                                    ForecastClick = 1,
                                    ForecastAmount = 1,
                                    Categories = new[]
                                        {
                                            new FirmForecast.Category
                                                {
                                                    Id = 1,
                                                    ForecastAmount = 1,
                                                    ForecastClick = 1,
                                                }
                                        }
                                }
                        }
            };

            SourceDb.Has(new Bit::FirmCategoryForecast { ProjectId = 1, FirmId = 1, CategoryId = 1 },
                         new Bit::FirmCategoryForecast { ProjectId = 2, FirmId = 2, CategoryId = 1 });

            var importer = CreateProcessor<FirmForecast, Bit::FirmCategoryForecast>(repositoryFactory);

            // Act
            var operations = importer.Import(dto).ToArray();

            // Assert
            Assert.That(operations.Count(), Is.EqualTo(1));
            repositoryFactory.Verify<Bit::FirmCategoryForecast>(
                m => m.Delete(It.Is(Predicate.Match(new Bit::FirmCategoryForecast { ProjectId = 1, FirmId = 1, CategoryId = 1 }))), Times.AtLeastOnce);
            repositoryFactory.Verify<Bit::FirmCategoryForecast>(
                m => m.Delete(It.Is(Predicate.Match(new Bit::FirmCategoryForecast { ProjectId = 2, FirmId = 2, CategoryId = 1 }))), Times.Never);
            repositoryFactory.Verify<Bit::FirmCategoryForecast>(
                m => m.Add(It.Is(Predicate.Match(new Bit::FirmCategoryForecast { ProjectId = 1, FirmId = 1, CategoryId = 1, ForecastClick = 1, ForecastAmount = 1 }))), Times.AtLeastOnce);
        }

        private IImportDocumentMetadataProcessor CreateProcessor<TDto, TFact>(IRepositoryFactory repositoryFactory)
            where TFact : class
            where TDto : class
        {
            var metadataSource = new ImportDocumentMetadataSource();
            var identity = new Uri($"{typeof(TDto).Name}", UriKind.Relative);
            IMetadataElement metadata;
            if (!metadataSource.Metadata.Values.TryGetElementById(identity, out metadata))
            {
                throw new NotSupportedException($"The aggregate of type '{typeof(TDto).Name}' is not supported.");
            }

            var feature = metadata.Features.OfType<ImportDocumentFeature<TDto, TFact>>().Single();
            var featureProcessor = new ImportDocumentFeatureProcessor<TDto, TFact>(feature, Query, repositoryFactory.Create<TFact>());
            var processor = new ImportDocumentMetadataProcessor<TDto>((ImportDocumentMetadata<TDto>)metadata, new [] { featureProcessor });

            return processor;
        }
    }
}