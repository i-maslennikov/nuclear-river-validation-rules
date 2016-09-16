using System.Linq;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication
{
    internal static class ValidationResultQueryableExtensions
    {
        /// <summary>
        /// Проставляет для всех сущностей VersionId
        /// </summary>
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

        /// <summary>
        /// Проставляет для всех сущностей MessageTypeId
        /// Не копирует VersionId, поскольку всешда применяется для сущностей с ещё незаполненным VersionId.
        /// </summary>
        public static IQueryable<Version.ValidationResult> ApplyMessageType(this IQueryable<Version.ValidationResult> queryable, int messageTypeId)
            => queryable.Select(x => new Version.ValidationResult
                {
                    MessageType = messageTypeId,
                    MessageParams = x.MessageParams,
                    PeriodEnd = x.PeriodEnd,
                    PeriodStart = x.PeriodStart,
                    ProjectId = x.ProjectId,
                    Result = x.Result,
                });
    }
}