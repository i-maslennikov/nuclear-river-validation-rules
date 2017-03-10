using System.Linq;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Storage.Specifications
{
    public static class ValidationResultExtensions
    {
        public static IQueryable<Version.ValidationResult> ForVersion(this IQueryable<Version.ValidationResult> queryable, long versionId)
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
                                                   x.OrderId == vr.OrderId)
                          select vr;
            return results;
        }
    }
}