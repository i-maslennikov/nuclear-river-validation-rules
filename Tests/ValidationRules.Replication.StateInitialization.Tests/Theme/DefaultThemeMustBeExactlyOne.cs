using System;
using System.Collections.Generic;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement DefaultThemeMustBeExactlyOne_NoDefaultTheme
            => ArrangeMetadataElement
                .Config
                .Name(nameof(DefaultThemeMustBeExactlyOne_NoDefaultTheme))
                .Fact(
                    new Facts::Project { Id = 1, OrganizationUnitId = 2 }
                )
                .Aggregate(
                    new Aggregates::Project { Id = 1 }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "themeCount", 0 } },
                                new Reference<EntityTypeProject>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.DefaultThemeMustBeExactlyOne,
                            PeriodStart = DateTime.MinValue,
                            PeriodEnd = DateTime.MaxValue,
                            ProjectId = 1,
                        }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement DefaultThemeMustBeExactlyOne_OneDefaultTheme
            => ArrangeMetadataElement
                .Config
                .Name(nameof(DefaultThemeMustBeExactlyOne_OneDefaultTheme))
                .Fact(
                    new Facts::Project { Id = 1, OrganizationUnitId = 2 },
                    new Facts::ThemeOrganizationUnit { ThemeId = 3, OrganizationUnitId = 2 },

                    new Facts::Theme { Id = 3, BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb, IsDefault = true }
                )
                .Aggregate(
                    new Aggregates::Project { Id = 1 },
                    new Aggregates::Project.ProjectDefaultTheme { ProjectId = 1, ThemeId = 3, Start = FirstDayJan, End = FirstDayFeb },

                    new Aggregates::Theme { Id = 3, BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb, IsDefault = true }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "themeCount", 0 } },
                                new Reference<EntityTypeProject>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.DefaultThemeMustBeExactlyOne,
                            PeriodStart = DateTime.MinValue,
                            PeriodEnd = FirstDayJan,
                            ProjectId = 1,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "themeCount", 0 } },
                                new Reference<EntityTypeProject>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.DefaultThemeMustBeExactlyOne,
                            PeriodStart = FirstDayFeb,
                            PeriodEnd = DateTime.MaxValue,
                            ProjectId = 1,
                        }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement DefaultThemeMustBeExactlyOne_TwoDefaultTheme
            => ArrangeMetadataElement
                .Config
                .Name(nameof(DefaultThemeMustBeExactlyOne_TwoDefaultTheme))
                .Fact(
                    new Facts::Project { Id = 1, OrganizationUnitId = 2 },
                    new Facts::ThemeOrganizationUnit { Id = 1, ThemeId = 3, OrganizationUnitId = 2 },
                    new Facts::ThemeOrganizationUnit { Id = 2, ThemeId = 4, OrganizationUnitId = 2 },

                    new Facts::Theme { Id = 3, BeginDistribution = FirstDayJan, EndDistribution = FirstDayMar, IsDefault = true },
                    new Facts::Theme { Id = 4, BeginDistribution = FirstDayFeb, EndDistribution = FirstDayMar, IsDefault = true }
                )
                .Aggregate(
                    new Aggregates::Project { Id = 1 },
                    new Aggregates::Project.ProjectDefaultTheme { ProjectId = 1, ThemeId = 3, Start = FirstDayJan, End = FirstDayMar },
                    new Aggregates::Project.ProjectDefaultTheme { ProjectId = 1, ThemeId = 4, Start = FirstDayFeb, End = FirstDayMar },

                    new Aggregates::Theme { Id = 3, BeginDistribution = FirstDayJan, EndDistribution = FirstDayMar, IsDefault = true },
                    new Aggregates::Theme { Id = 4, BeginDistribution = FirstDayFeb, EndDistribution = FirstDayMar, IsDefault = true }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "themeCount", 0 } },
                                new Reference<EntityTypeProject>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.DefaultThemeMustBeExactlyOne,
                            PeriodStart = DateTime.MinValue,
                            PeriodEnd = FirstDayJan,
                            ProjectId = 1,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "themeCount", 2 } },
                                new Reference<EntityTypeProject>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.DefaultThemeMustBeExactlyOne,
                            PeriodStart = FirstDayFeb,
                            PeriodEnd = FirstDayMar,
                            ProjectId = 1,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "themeCount", 0 } },
                                new Reference<EntityTypeProject>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.DefaultThemeMustBeExactlyOne,
                            PeriodStart = FirstDayMar,
                            PeriodEnd = DateTime.MaxValue,
                            ProjectId = 1,
                        }
                );
    }
}
