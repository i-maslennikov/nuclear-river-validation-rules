using System.Collections.Generic;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes;
using NuClear.Model.Common.Entities;
using NuClear.Replication.OperationsProcessing.Primary;

namespace NuClear.CustomerIntelligence.OperationsProcessing
{
    public sealed class ErmToFactsEntityTypeExplicitMapping : IEntityTypeExplicitMapping
    {
        private static readonly Dictionary<IEntityType, IEntityType> Dictionary = new Dictionary<IEntityType, IEntityType>
            {
                { EntityTypeAppointment.Instance, EntityTypeActivity.Instance },
                { EntityTypePhonecall.Instance, EntityTypeActivity.Instance },
                { EntityTypeTask.Instance, EntityTypeActivity.Instance },
                { EntityTypeLetter.Instance, EntityTypeActivity.Instance }
            };

        public IEntityType MapEntityType(IEntityType entittyType)
        {
            IEntityType mappedEntityType;
            return Dictionary.TryGetValue(entittyType, out mappedEntityType) ? mappedEntityType : entittyType;
        }
    }
}