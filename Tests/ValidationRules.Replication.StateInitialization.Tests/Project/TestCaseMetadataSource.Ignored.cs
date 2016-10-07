using System;

using NuClear.DataTest.Metamodel.Dsl;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Aggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        /// <summary>
        /// Это такой специальный "тест", чтобы среда выполнения знала про все типы. Впоследствии нужно будет удалить.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ProjectContextSupport
            => ArrangeMetadataElement.Config
            .Name(nameof(ProjectContextSupport))
            .Erm(
                new Erm::CategoryOrganizationUnit(),
                new Erm::CostPerClickCategoryRestriction(),
                new Erm::OrderPositionCostPerClick())
            .Fact(
                new Facts::Category(),
                new Facts::CategoryOrganizationUnit(),
                new Facts::CostPerClickCategoryRestriction(),
                new Facts::FirmAddress(),
                new Facts::Order(),
                new Facts::OrderPosition(),
                new Facts::OrderPositionAdvertisement(),
                new Facts::OrderPositionCostPerClick(),
                new Facts::Position(),
                new Facts::PricePosition(),
                new Facts::Project(),
                new Facts::ReleaseInfo())
            .Aggregate(
                new Aggregates::Category(),
                new Aggregates::FirmAddress(),
                new Aggregates::Order(),
                new Aggregates::Order.AddressAdvertisement(),
                new Aggregates::Order.CategoryAdvertisement(),
                new Aggregates::Order.CostPerClickAdvertisement(),
                new Aggregates::Position(),
                new Aggregates::Project(),
                new Aggregates::Project.Category(),
                new Aggregates::Project.CostPerClickRestriction(),
                new Aggregates::Project.NextRelease())
            .Ignored();
    }
}
