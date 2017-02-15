using System.Collections.Generic;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmAndOrderShouldBelongTheSameOrganizationUnit
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmAndOrderShouldBelongTheSameOrganizationUnit))
                .Fact(
                    new Facts::Project { Id = 1, OrganizationUnitId = 2 },
                    new Facts::Project { Id = 2, OrganizationUnitId = 1 },

                    new Facts::Firm { Id = 1, OrganizationUnitId = 1, IsActive = true },
                    new Facts::Order { Id = 2, FirmId = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayFeb, WorkflowStep = 5 },

                    // для неактивных фирм ошибка не выводится
                    new Facts::Firm { Id = 2, OrganizationUnitId = 1, IsActive = false},
                    new Facts::Order { Id = 3, FirmId = 2, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayFeb, WorkflowStep = 5 })

                .Aggregate(
                    new Aggregates::Firm { Id = 1, ProjectId = 2 },
                    new Aggregates::Order { Id = 2, FirmId = 1, Begin = FirstDayJan, End = FirstDayFeb, ProjectId = 1 },
                    new Aggregates::Order.FirmOrganiationUnitMismatch { OrderId = 2 },

                    new Aggregates::Firm { Id = 2, ProjectId = 2 },
                    new Aggregates::Order { Id = 3, FirmId = 2, Begin = FirstDayJan, End = FirstDayFeb, ProjectId = 1 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Reference<EntityTypeFirm>(1),
                                    new Reference<EntityTypeOrder>(2)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit,
                            Result = 255,
                            PeriodStart = FirstDayJan,
                            PeriodEnd = FirstDayFeb,
                            OrderId = 2,
                        });
    }
}

