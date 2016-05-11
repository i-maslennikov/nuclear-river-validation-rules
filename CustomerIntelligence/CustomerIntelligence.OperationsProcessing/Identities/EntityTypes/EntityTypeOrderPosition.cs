using System;

using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    [Obsolete("Сущность не имеет отношения к домену поиска, удалить после рефакторинга на стороне ERM")]
    public sealed class EntityTypeOrderPosition : EntityTypeBase<EntityTypeOrderPosition>
    {
        public override int Id
        {
            get { return EntityTypeIds.OrderPosition; }
        }

        public override string Description
        {
            get { return "OrderPosition"; }
        }
    }
}