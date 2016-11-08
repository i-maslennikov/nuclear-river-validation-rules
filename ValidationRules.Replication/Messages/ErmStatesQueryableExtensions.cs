using System.Collections.Generic;
using System.Linq;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Messages
{
    internal static class ErmStatesQueryableExtensions
    {
        public static IEnumerable<Version.ErmState> ApplyVersionId(this IEnumerable<Version.ErmState> enumerable, long versionId)
            => enumerable.Select(x => new Version.ErmState
            {
                VersionId = versionId,
                Token = x.Token,
            });
    }
}
