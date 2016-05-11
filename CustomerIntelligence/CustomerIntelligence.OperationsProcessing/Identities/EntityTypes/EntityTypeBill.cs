using System;

using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    [Obsolete("Сущность не имеет отношения к домену поиска, удалить после рефакторинга на стороне ERM")]
    public class EntityTypeBill : EntityTypeBase<EntityTypeBill>
    {
        public override int Id
        {
            get { return EntityTypeIds.Bill; }
        }

        public override string Description
        {
            get { return "Bill"; }
        }
    }
}