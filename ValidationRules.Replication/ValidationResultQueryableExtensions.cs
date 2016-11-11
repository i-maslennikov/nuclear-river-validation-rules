using System.Collections.Generic;
using System.Linq;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication
{
    internal static class ValidationResultQueryableExtensions
    {
        public static IQueryable<Version.ValidationResult> GetValidationResults(this IQueryable<Version.ValidationResult> queryable, long versionId)
        {
            var filtered = queryable.Where(x => x.VersionId <= versionId);

            // если выше по стеку нашли resolved результаты, то отфильтровываем их
            var results = from vr in filtered.Where(x => !x.Resolved)
                          where !filtered.Any(x => x.VersionId > vr.VersionId &&
                                                   x.Resolved &&

                                                   x.MessageType == vr.MessageType &&
                                                   x.MessageParams == vr.MessageParams &&
                                                   x.PeriodStart == vr.PeriodStart &&
                                                   x.PeriodEnd == vr.PeriodEnd &&
                                                   x.ProjectId == vr.ProjectId &&
                                                   x.OrderId == vr.OrderId &&
                                                   x.Result == vr.Result)
                          select vr;
            return results;
        }

        /// <summary>
        /// Проставляет для всех сущностей VersionId
        /// </summary>
        internal static IEnumerable<Version.ValidationResult> ApplyVersionId(this IEnumerable<Version.ValidationResult> enumerable, long versionId)
            => enumerable.Select(x => new Version.ValidationResult
            {
                VersionId = versionId,

                MessageType = x.MessageType,
                MessageParams = x.MessageParams,
                PeriodEnd = x.PeriodEnd,
                PeriodStart = x.PeriodStart,
                ProjectId = x.ProjectId,
                OrderId = x.OrderId,
                Result = x.Result,
                Resolved = x.Resolved,
            });

        internal static IEnumerable<Version.ValidationResult> ApplyResolved(this IEnumerable<Version.ValidationResult> enumerable)
            => enumerable.Select(x => new Version.ValidationResult
            {
                Resolved = true,

                MessageType = x.MessageType,
                MessageParams = x.MessageParams,
                PeriodEnd = x.PeriodEnd,
                PeriodStart = x.PeriodStart,
                ProjectId = x.ProjectId,
                OrderId = x.OrderId,
                Result = x.Result,
            });

        /// <summary>
        /// Проставляет для всех сущностей MessageTypeId
        /// Не копирует VersionId, поскольку всешда применяется для сущностей с ещё незаполненным VersionId.
        /// </summary>
        internal static IQueryable<Version.ValidationResult> ApplyMessageType(this IQueryable<Version.ValidationResult> queryable, int messageTypeId)
            => queryable.Select(x => new Version.ValidationResult
                {
                    MessageType = messageTypeId,
                    MessageParams = x.MessageParams,
                    PeriodEnd = x.PeriodEnd,
                    PeriodStart = x.PeriodStart,
                    ProjectId = x.ProjectId,
                    OrderId = x.OrderId,
                    Result = x.Result,
                });
    }
}