﻿using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.CustomerIntelligence.Domain;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeFirmAddress : EntityTypeBase<EntityTypeFirmAddress>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmAddress; }
        }

        public override string Description
        {
            get { return "FirmAddress"; }
        }
    }
}