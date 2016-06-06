using System;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Model.Aggregates;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPeriod
            => ArrangeMetadataElement.Config
                                     .Name(nameof(OrderPeriod))
                                     .Aggregate(
                                                new Order { Id = 1 },
                                                new Period { Start = DateTime.Parse("2011-01-01"), End = DateTime.Parse("2011-05-01") },
                                                new Period { Start = DateTime.Parse("2011-05-01"), End = DateTime.MaxValue },
                                                new OrderPeriod { OrderId = 1, Start = DateTime.Parse("2011-01-01") })
                                     .Fact(
                                           new Storage.Model.Facts.Order { Id = 1, BeginDistributionDate = DateTime.Parse("2011-01-01"), EndDistributionDateFact = DateTime.Parse("2011-05-01") });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PricePeriod
            => ArrangeMetadataElement.Config
                                     .Name(nameof(PricePeriod))
                                     .Aggregate(
                                                new Price { Id = 1 },
                                                new Price { Id = 2 },
                                                new Period { Start = DateTime.Parse("2011-01-01"), End = DateTime.Parse("2011-02-01") },
                                                new Period { Start = DateTime.Parse("2011-02-01"), End = DateTime.MaxValue },
                                                new PricePeriod { PriceId = 1, Start = DateTime.Parse("2011-01-01") },
                                                new PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-02-01") })
                                     .Fact(
                                           new Storage.Model.Facts.Price { Id = 1, BeginDate = DateTime.Parse("2011-01-01") },
                                           new Storage.Model.Facts.Price { Id = 2, BeginDate = DateTime.Parse("2011-02-01") });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PriceAndOrderPeriod
            => ArrangeMetadataElement.Config
                                     .Name(nameof(PriceAndOrderPeriod))
                                     .Aggregate(
                                                new Price { Id = 1 },
                                                new Price { Id = 2 },
                                                new Price { Id = 3 },
                                                new Order { Id = 1 },
                                                new Order { Id = 2 },
                                                new Order { Id = 3 },

                                                new Period { Start = DateTime.Parse("2011-01-01"), End = DateTime.Parse("2011-02-01") },
                                                new Period { Start = DateTime.Parse("2011-02-01"), End = DateTime.Parse("2011-03-01") },
                                                new Period { Start = DateTime.Parse("2011-03-01"), End = DateTime.Parse("2011-04-01") },
                                                new Period { Start = DateTime.Parse("2011-04-01"), End = DateTime.Parse("2011-05-01") },
                                                new Period { Start = DateTime.Parse("2011-05-01"), End = DateTime.Parse("2011-06-01") },
                                                new Period { Start = DateTime.Parse("2011-06-01"), End = DateTime.MaxValue },

                                                // Для второго отделения организации должны быть свои периоды, не влияющие на первое
                                                new Period { Start = DateTime.Parse("2010-01-01"), End = DateTime.Parse("2010-02-01"), ProjectId = 1},
                                                new Period { Start = DateTime.Parse("2010-02-01"), End = DateTime.MaxValue, ProjectId = 1 },

                                                new PricePeriod { PriceId = 1, Start = DateTime.Parse("2011-01-01") },
                                                new PricePeriod { PriceId = 1, Start = DateTime.Parse("2011-02-01") },

                                                new PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-03-01") },
                                                new PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-04-01") },
                                                new PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-05-01") },
                                                new PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-06-01") },

                                                new PricePeriod { PriceId = 3, Start = DateTime.Parse("2010-01-01"), ProjectId = 1 },
                                                new PricePeriod { PriceId = 3, Start = DateTime.Parse("2010-02-01"), ProjectId = 1 },

                                                new OrderPeriod { OrderId = 1, Start = DateTime.Parse("2011-02-01") },
                                                new OrderPeriod { OrderId = 1, Start = DateTime.Parse("2011-03-01") },

                                                new OrderPeriod { OrderId = 2, Start = DateTime.Parse("2011-05-01") },

                                                new OrderPeriod { OrderId = 3, Start = DateTime.Parse("2010-01-01"), ProjectId = 1 }

                                                )
                                     .Fact(
                                           new Storage.Model.Facts.Price { Id = 1, BeginDate = DateTime.Parse("2011-01-01") },
                                           new Storage.Model.Facts.Price { Id = 2, BeginDate = DateTime.Parse("2011-03-01") },
                                           new Storage.Model.Facts.Price { Id = 3, ProjectId = 1, BeginDate = DateTime.Parse("2010-01-01") },

                                           new Storage.Model.Facts.Order { Id = 1, BeginDistributionDate = DateTime.Parse("2011-02-01"), EndDistributionDateFact = DateTime.Parse("2011-04-01") },
                                           new Storage.Model.Facts.Order { Id = 2, BeginDistributionDate = DateTime.Parse("2011-05-01"), EndDistributionDateFact = DateTime.Parse("2011-06-01") },
                                           new Storage.Model.Facts.Order { Id = 3, DestProjectId = 1, BeginDistributionDate = DateTime.Parse("2010-01-01"), EndDistributionDateFact = DateTime.Parse("2010-02-01") }
                                           );
    }
}
