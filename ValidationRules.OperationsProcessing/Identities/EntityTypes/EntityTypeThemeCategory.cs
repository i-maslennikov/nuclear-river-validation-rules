﻿using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeThemeCategory : EntityTypeBase<EntityTypeThemeCategory>
    {
        public override int Id { get; } = EntityTypeIds.ThemeCategory;
        public override string Description { get; } = nameof(EntityTypeIds.ThemeCategory);
    }
}