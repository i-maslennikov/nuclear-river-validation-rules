using System.Linq;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication
{
    internal static class ValidationResultQueryableExtensions
    {
        public static IQueryable<Version.ValidationResult> ApplyVersion(this IQueryable<Version.ValidationResult> queryable, long version)
            => queryable.Select(x => new Version.ValidationResult
                {
                    VersionId = version,
                    MessageType = x.MessageType,
                    MessageParams = x.MessageParams,
                    PeriodEnd = x.PeriodEnd,
                    PeriodStart = x.PeriodStart,
                    ProjectId = x.ProjectId,
                    Result = x.Result,
                });

        public static IQueryable<Version.ValidationResult> ApplyMessageType(this IQueryable<Version.ValidationResult> queryable, int messageTypeId)
            => queryable.Select(x => new Version.ValidationResult
                {
                    VersionId = x.VersionId,
                    MessageType = messageTypeId,
                    MessageParams = x.MessageParams,
                    PeriodEnd = x.PeriodEnd,
                    PeriodStart = x.PeriodStart,
                    ProjectId = x.ProjectId,
                    Result = x.Result,
                });
    }
}