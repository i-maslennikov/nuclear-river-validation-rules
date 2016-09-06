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
                    MessageParams = x.MessageParams,
                    MessageType = x.MessageType,
                    PeriodEnd = x.PeriodEnd,
                    PeriodStart = x.PeriodStart,
                    ProjectId = x.ProjectId,
                    Result = x.Result,
                });
    }
}