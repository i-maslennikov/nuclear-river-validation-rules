using System.Collections.Generic;
using System.Linq;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication
{
    internal static class ValidationResultExtensions
    {
        /// <summary>
        /// Проставляет для всех сущностей VersionId
        /// Применяется последней, копирует все поля.
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
                Resolved = x.Resolved,
            });

        /// <summary>
        /// Проставляет для всех сущностей Resolved = true
        /// Не копирует VersionId, поскольку всешда применяется для сущностей с ещё незаполненным VersionId.
        /// </summary>
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
            });

        /// <summary>
        /// Проставляет для всех сущностей MessageTypeId
        /// Не копирует VersionId, поскольку всешда применяется для сущностей с ещё незаполненным VersionId.
        /// Не копирует Resolved, поскольку всешда применяется для сущностей с ещё незаполненным VersionId.
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
                });
    }
}