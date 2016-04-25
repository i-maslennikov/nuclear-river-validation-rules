using System;
using System.Linq;

using Moq;

using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.Metamodeling.Elements;

using NUnit.Framework;

using FirmForecast = NuClear.CustomerIntelligence.Domain.DTO.FirmForecast;

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

            SourceDb.Has(new FirmCategoryStatistics { ProjectId = 1, FirmId = 7 },
                         new FirmCategoryStatistics { ProjectId = 2, FirmId = 8 });

            var importer = CreateProcessor<FirmPopularity, FirmCategoryStatistics>(repositoryFactory);

            // Act
            var operations = importer.Import(dto).ToArray();

            // Assert
            Assert.That(operations.Count(), Is.EqualTo(1));
            repositoryFactory.Verify<FirmCategoryStatistics>(
                m => m.Delete(It.Is(Predicate.Match(new FirmCategoryStatistics { ProjectId = 1, FirmId = 7 }))),
                Times.AtLeastOnce);
            repositoryFactory.Verify<FirmCategoryStatistics>(
                m => m.Add(It.Is(Predicate.Match(new FirmCategoryStatistics { ProjectId = 1, FirmId = 2, CategoryId = 3, Hits = 4, Shows = 5 }))),
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

            SourceDb.Has(new ProjectCategoryStatistics { ProjectId = 1, CategoryId = 7 },
                         new ProjectCategoryStatistics { ProjectId = 2, CategoryId = 7 });

            var importer = CreateProcessor<RubricPopularity, ProjectCategoryStatistics>(repositoryFactory);

            // Act
            var operations = importer.Import(dto).ToArray();

            // Assert
            Assert.That(operations.Count(), Is.EqualTo(1));
            repositoryFactory.Verify<ProjectCategoryStatistics>(
                m => m.Delete(It.Is(Predicate.Match(new ProjectCategoryStatistics { ProjectId = 1, CategoryId = 7 }))),
                Times.AtLeastOnce);
            repositoryFactory.Verify<ProjectCategoryStatistics>(
                m => m.Add(It.Is(Predicate.Match(new ProjectCategoryStatistics { ProjectId = 1, CategoryId = 2, AdvertisersCount = 3 }))),
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

            SourceDb.Has(new Storage.Model.Bit.FirmForecast { ProjectId = 1, FirmId = 1 },
                         new Storage.Model.Bit.FirmForecast { ProjectId = 2, FirmId = 2 });

            var importer = CreateProcessor<FirmForecast, Storage.Model.Bit.FirmForecast>(repositoryFactory);

            // Act
            var operations = importer.Import(dto).ToArray();

            // Assert
            Assert.That(operations.Count(), Is.EqualTo(1));
            repositoryFactory.Verify<Storage.Model.Bit.FirmForecast>(
                m => m.Delete(It.Is(Predicate.Match(new Storage.Model.Bit.FirmForecast { ProjectId = 1, FirmId = 1 }))), Times.AtLeastOnce);
            repositoryFactory.Verify<Storage.Model.Bit.FirmForecast>(
                m => m.Delete(It.Is(Predicate.Match(new Storage.Model.Bit.FirmForecast { ProjectId = 2, FirmId = 2 }))), Times.Never);
            repositoryFactory.Verify<Storage.Model.Bit.FirmForecast>(
                m => m.Add(It.Is(Predicate.Match(new Storage.Model.Bit.FirmForecast { ProjectId = 1, FirmId = 1, ForecastClick = 1, ForecastAmount = 1 }))), Times.AtLeastOnce);
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

            SourceDb.Has(new FirmCategoryForecast { ProjectId = 1, FirmId = 1, CategoryId = 1 },
                         new FirmCategoryForecast { ProjectId = 2, FirmId = 2, CategoryId = 1 });

            var importer = CreateProcessor<FirmForecast, FirmCategoryForecast>(repositoryFactory);

            // Act
            var operations = importer.Import(dto).ToArray();

            // Assert
            Assert.That(operations.Count(), Is.EqualTo(1));
            repositoryFactory.Verify<FirmCategoryForecast>(
                m => m.Delete(It.Is(Predicate.Match(new FirmCategoryForecast { ProjectId = 1, FirmId = 1, CategoryId = 1 }))), Times.AtLeastOnce);
            repositoryFactory.Verify<FirmCategoryForecast>(
                m => m.Delete(It.Is(Predicate.Match(new FirmCategoryForecast { ProjectId = 2, FirmId = 2, CategoryId = 1 }))), Times.Never);
            repositoryFactory.Verify<FirmCategoryForecast>(
                m => m.Add(It.Is(Predicate.Match(new FirmCategoryForecast { ProjectId = 1, FirmId = 1, CategoryId = 1, ForecastClick = 1, ForecastAmount = 1 }))), Times.AtLeastOnce);
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