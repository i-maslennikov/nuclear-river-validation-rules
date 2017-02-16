using System.Collections.Generic;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderMustHaveActiveLegalEntitiesAggregate
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustHaveActiveLegalEntitiesAggregate))
                .Fact(
                    new Facts::Order { Id = 1, BranchOfficeOrganizationUnitId = 1, LegalPersonId = null, LegalPersonProfileId = null },
                    new Facts::Order { Id = 2, BranchOfficeOrganizationUnitId = null, LegalPersonId = 1, LegalPersonProfileId = null },
                    new Facts::Order { Id = 3, BranchOfficeOrganizationUnitId = null, LegalPersonId = null, LegalPersonProfileId = 1 },
                    new Facts::Order { Id = 4, BranchOfficeOrganizationUnitId = 11, LegalPersonId = null, LegalPersonProfileId = null },

                    new Facts::Order { Id = 5, BranchOfficeOrganizationUnitId = 2, LegalPersonId = null, LegalPersonProfileId = null },
                    new Facts::Order { Id = 6, BranchOfficeOrganizationUnitId = null, LegalPersonId = 2, LegalPersonProfileId = null },
                    new Facts::Order { Id = 7, BranchOfficeOrganizationUnitId = null, LegalPersonId = null, LegalPersonProfileId = 2 },

                    new Facts::BranchOfficeOrganizationUnit { Id = 11, BranchOfficeId = 999 },

                    new Facts::BranchOfficeOrganizationUnit { Id = 2, BranchOfficeId = 2 },
                    new Facts::BranchOffice { Id = 2 },
                    new Facts::LegalPerson { Id = 2 },
                    new Facts::LegalPersonProfile { Id = 2 })
                .Aggregate(
                    new Aggregates::Order.InactiveReference { OrderId = 1, BranchOffice = false, BranchOfficeOrganizationUnit = true, LegalPerson = false, LegalPersonProfile = false },
                    new Aggregates::Order.InactiveReference { OrderId = 2, BranchOffice = false, BranchOfficeOrganizationUnit = false, LegalPerson = true, LegalPersonProfile = false },
                    new Aggregates::Order.InactiveReference { OrderId = 3, BranchOffice = false, BranchOfficeOrganizationUnit = false, LegalPerson = false, LegalPersonProfile = true },
                    new Aggregates::Order.InactiveReference { OrderId = 4, BranchOffice = true, BranchOfficeOrganizationUnit = false, LegalPerson = false, LegalPersonProfile = false },

                    new Aggregates::Order.InactiveReference { OrderId = 5, BranchOffice = false, BranchOfficeOrganizationUnit = false, LegalPerson = false, LegalPersonProfile = false },
                    new Aggregates::Order.InactiveReference { OrderId = 6, BranchOffice = false, BranchOfficeOrganizationUnit = false, LegalPerson = false, LegalPersonProfile = false },
                    new Aggregates::Order.InactiveReference { OrderId = 7, BranchOffice = false, BranchOfficeOrganizationUnit = false, LegalPerson = false, LegalPersonProfile = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderMustHaveActiveLegalEntitiesMessage
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustHaveActiveLegalEntitiesMessage))
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.InactiveReference { OrderId = 1, BranchOffice = true, BranchOfficeOrganizationUnit = true, LegalPerson = false, LegalPersonProfile = false },

                    new Aggregates::Order { Id = 2, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.InactiveReference { OrderId = 2, BranchOffice = false, BranchOfficeOrganizationUnit = false, LegalPerson = true, LegalPersonProfile = true })
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "branchOfficeOrganizationUnit", true }, { "branchOffice", true }, { "legalPerson", false }, { "legalPersonProfile", false } },
                                new Reference<EntityTypeOrder>(1)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.OrderMustHaveActiveLegalEntities,
                        Result = 3,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(2),
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "branchOfficeOrganizationUnit", false }, { "branchOffice", false }, { "legalPerson", true }, { "legalPersonProfile", true } },
                                new Reference<EntityTypeOrder>(2)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.OrderMustHaveActiveLegalEntities,
                        Result = 3,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(2),
                        OrderId = 2,
                    });
    }
}
