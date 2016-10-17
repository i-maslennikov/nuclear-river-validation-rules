using System;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.ThemeRules.Facts;

using Messages = NuClear.ValidationRules.Storage.Model.Messages;

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
                    new Facts::Project { Id = 1, OrganizationUnitId = 2, Name = "Project1" }
                )
                .Aggregate(
                    new Aggregates::Project { Id = 1, Name = "Project1" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><project id = \"1\" name=\"Project1\" /><message themeCount=\"0\" /></root>"),
                        MessageType = (int)MessageTypeCode.DefaultThemeMustBeExactlyOne,
                        Result = 252,
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
                    new Facts::Project {Id = 1, OrganizationUnitId = 2, Name = "Project1" },
                    new Facts::ThemeOrganizationUnit { ThemeId = 3, OrganizationUnitId = 2 },

                    new Facts::Theme { Id = 3, Name = "Theme3", BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb, IsDefault = true }
                )
                .Aggregate(
                    new Aggregates::Project { Id = 1, Name = "Project1" },
                    new Aggregates::Project.ProjectDefaultTheme { ProjectId = 1, ThemeId = 3, Start = FirstDayJan, End = FirstDayFeb },

                    new Aggregates::Theme { Id = 3, Name = "Theme3", BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb, IsDefault = true }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><project id = \"1\" name=\"Project1\" /><message themeCount=\"0\" /></root>"),
                        MessageType = (int)MessageTypeCode.DefaultThemeMustBeExactlyOne,
                        Result = 252,
                        PeriodStart = DateTime.MinValue,
                        PeriodEnd = FirstDayJan,
                        ProjectId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><project id = \"1\" name=\"Project1\" /><message themeCount=\"0\" /></root>"),
                        MessageType = (int)MessageTypeCode.DefaultThemeMustBeExactlyOne,
                        Result = 252,
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
                    new Facts::Project { Id = 1, OrganizationUnitId = 2, Name = "Project1" },
                    new Facts::ThemeOrganizationUnit { Id = 1, ThemeId = 3, OrganizationUnitId = 2 },
                    new Facts::ThemeOrganizationUnit { Id = 2, ThemeId = 4, OrganizationUnitId = 2 },

                    new Facts::Theme { Id = 3, Name = "Theme3", BeginDistribution = FirstDayJan, EndDistribution = FirstDayMar, IsDefault = true },
                    new Facts::Theme { Id = 4, Name = "Theme4", BeginDistribution = FirstDayFeb, EndDistribution = FirstDayMar, IsDefault = true }
                )
                .Aggregate(
                    new Aggregates::Project { Id = 1, Name = "Project1" },
                    new Aggregates::Project.ProjectDefaultTheme { ProjectId = 1, ThemeId = 3, Start = FirstDayJan, End = FirstDayMar },
                    new Aggregates::Project.ProjectDefaultTheme { ProjectId = 1, ThemeId = 4, Start = FirstDayFeb, End = FirstDayMar },

                    new Aggregates::Theme { Id = 3, Name = "Theme3", BeginDistribution = FirstDayJan, EndDistribution = FirstDayMar, IsDefault = true },
                    new Aggregates::Theme { Id = 4, Name = "Theme4", BeginDistribution = FirstDayFeb, EndDistribution = FirstDayMar, IsDefault = true }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><project id = \"1\" name=\"Project1\" /><message themeCount=\"0\" /></root>"),
                        MessageType = (int)MessageTypeCode.DefaultThemeMustBeExactlyOne,
                        Result = 252,
                        PeriodStart = DateTime.MinValue,
                        PeriodEnd = FirstDayJan,
                        ProjectId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><project id = \"1\" name=\"Project1\" /><message themeCount=\"2\" /></root>"),
                        MessageType = (int)MessageTypeCode.DefaultThemeMustBeExactlyOne,
                        Result = 252,
                        PeriodStart = FirstDayFeb,
                        PeriodEnd = FirstDayMar,
                        ProjectId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><project id = \"1\" name=\"Project1\" /><message themeCount=\"0\" /></root>"),
                        MessageType = (int)MessageTypeCode.DefaultThemeMustBeExactlyOne,
                        Result = 252,
                        PeriodStart = FirstDayMar,
                        PeriodEnd = DateTime.MaxValue,
                        ProjectId = 1,
                    }
                ).RunOnlyThis();
    }
}
