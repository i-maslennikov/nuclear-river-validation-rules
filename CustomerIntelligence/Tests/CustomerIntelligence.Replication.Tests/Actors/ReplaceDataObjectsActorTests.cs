using Moq;

using NuClear.CustomerIntelligence.Replication.Accessors;
using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.Replication.Core.Actors;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Actors
{
    [TestFixture]
    internal class ReplaceDataObjectsActorTests : DataFixtureBase
    {
        [Test]
        public void ShouldProduceCalculateStatisticsOperationForFirmStatisticsDto()
        {
            // Arrange
            var firmPopularity = new DTO.FirmPopularity
            {
                ProjectId = 1,
                Firms = new[]
                        {
                            new DTO.FirmPopularity.Firm
                                {
                                    FirmId = 2,
                                    Categories = new[]
                                        {
                                            new DTO.FirmPopularity.Firm.Category
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

            var repositoryFactory = new VerifiableRepositoryFactory();
            var accessor = new FirmCategoryStatisticsAccessor();
            var actor = new ReplaceDataObjectsActor<FirmCategoryStatistics>(
                Query,
                repositoryFactory.Create<FirmCategoryStatistics>(),
                accessor,
                accessor);

            // Act
            var events = actor.ExecuteCommands(new[] { new ReplaceFirmPopularityCommand(firmPopularity) });

            // Assert
            Assert.That(events.Count, Is.EqualTo(1));
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
            var rubricPopularity = new DTO.RubricPopularity
            {
                ProjectId = 1,
                Categories = new[]
                        {
                            new DTO.RubricPopularity.Category
                                {
                                    CategoryId = 2,
                                    AdvertisersCount = 3,
                                }
                        }
            };

            SourceDb.Has(new ProjectCategoryStatistics { ProjectId = 1, CategoryId = 7 },
                         new ProjectCategoryStatistics { ProjectId = 2, CategoryId = 7 });

            var repositoryFactory = new VerifiableRepositoryFactory();
            var accessor = new ProjectCategoryStatisticsAccessor();
            var actor = new ReplaceDataObjectsActor<ProjectCategoryStatistics>(
                Query,
                repositoryFactory.Create<ProjectCategoryStatistics>(),
                accessor,
                accessor);

            // Act
            var events = actor.ExecuteCommands(new[] { new ReplaceRubricPopularityCommand(rubricPopularity) });

            // Assert
            Assert.That(events.Count, Is.EqualTo(1));
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
            var firmForecast = new DTO.FirmForecast
            {
                ProjectId = 1,
                Firms = new[]
                        {
                            new DTO.FirmForecast.Firm
                                {
                                    Id = 1,
                                    ForecastClick = 1,
                                    ForecastAmount = 1,
                                    Categories = new[]
                                        {
                                            new DTO.FirmForecast.Category
                                                {
                                                    Id = 1,
                                                    ForecastAmount = 1,
                                                    ForecastClick = 1,
                                                }
                                        }
                                }
                        }
            };

            SourceDb.Has(new FirmForecast { ProjectId = 1, FirmId = 1 },
                         new FirmForecast { ProjectId = 2, FirmId = 2 });

            var repositoryFactory = new VerifiableRepositoryFactory();
            var accessor = new FirmForecastAccessor();
            var actor = new ReplaceDataObjectsActor<FirmForecast>(
                Query,
                repositoryFactory.Create<FirmForecast>(),
                accessor,
                accessor);

            // Act
            var events = actor.ExecuteCommands(new[] { new ReplaceFirmForecastCommand(firmForecast) });

            // Assert
            Assert.That(events.Count, Is.EqualTo(1));
            repositoryFactory.Verify<FirmForecast>(
                m => m.Delete(It.Is(Predicate.Match(new FirmForecast { ProjectId = 1, FirmId = 1 }))), Times.AtLeastOnce);
            repositoryFactory.Verify<FirmForecast>(
                m => m.Delete(It.Is(Predicate.Match(new FirmForecast { ProjectId = 2, FirmId = 2 }))), Times.Never);
            repositoryFactory.Verify<FirmForecast>(
                m => m.Add(It.Is(Predicate.Match(new FirmForecast { ProjectId = 1, FirmId = 1, ForecastClick = 1, ForecastAmount = 1 }))), Times.AtLeastOnce);
        }

        [Test]
        public void ShouldProcessFirmCategoryForecastFromFirmForecastDto()
        {
            // Arrange
            var firmForecast = new DTO.FirmForecast
            {
                ProjectId = 1,
                Firms = new[]
                        {
                            new DTO.FirmForecast.Firm
                                {
                                    Id = 1,
                                    ForecastClick = 1,
                                    ForecastAmount = 1,
                                    Categories = new[]
                                        {
                                            new DTO.FirmForecast.Category
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

            var repositoryFactory = new VerifiableRepositoryFactory();
            var accessor = new FirmCategoryForecastAccessor();
            var actor = new ReplaceDataObjectsActor<FirmCategoryForecast>(
                Query,
                repositoryFactory.Create<FirmCategoryForecast>(),
                accessor,
                accessor);

            // Act
            var events = actor.ExecuteCommands(new[] { new ReplaceFirmCategoryForecastCommand(firmForecast) });

            // Assert
            Assert.That(events.Count, Is.EqualTo(1));
            repositoryFactory.Verify<FirmCategoryForecast>(
                m => m.Delete(It.Is(Predicate.Match(new FirmCategoryForecast { ProjectId = 1, FirmId = 1, CategoryId = 1 }))), Times.AtLeastOnce);
            repositoryFactory.Verify<FirmCategoryForecast>(
                m => m.Delete(It.Is(Predicate.Match(new FirmCategoryForecast { ProjectId = 2, FirmId = 2, CategoryId = 1 }))), Times.Never);
            repositoryFactory.Verify<FirmCategoryForecast>(
                m => m.Add(It.Is(Predicate.Match(new FirmCategoryForecast { ProjectId = 1, FirmId = 1, CategoryId = 1, ForecastClick = 1, ForecastAmount = 1 }))), Times.AtLeastOnce);
        }
    }
}