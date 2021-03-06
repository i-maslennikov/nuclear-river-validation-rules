﻿using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeAdvertisement : EntityTypeBase<EntityTypeAdvertisement>
    {
        public override int Id { get; } = EntityTypeIds.Advertisement;
        public override string Description { get; } = nameof(EntityTypeIds.Advertisement);
    }
}
