﻿using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeFirmContact : EntityTypeBase<EntityTypeFirmContact>
    {
        public override int Id { get; } = EntityTypeIds.FirmContact;
        public override string Description { get; } = nameof(EntityTypeIds.FirmContact);
    }
}