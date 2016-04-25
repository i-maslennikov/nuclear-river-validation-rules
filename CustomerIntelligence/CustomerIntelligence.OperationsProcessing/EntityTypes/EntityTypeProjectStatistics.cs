using NuClear.CustomerIntelligence.Domain;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.EntityTypes
{
    public sealed class EntityTypeProjectStatistics : EntityTypeBase<EntityTypeProjectStatistics>
    {
        public override int Id => EntityTypeIds.ProjectStatistics;

        public override string Description => "ProjectStatistics";
    }
}