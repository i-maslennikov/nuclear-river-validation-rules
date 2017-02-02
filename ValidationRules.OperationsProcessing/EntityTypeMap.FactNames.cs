using System.Collections.Generic;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes;

namespace NuClear.ValidationRules.OperationsProcessing
{
    internal static partial class EntityTypeMap
    {
        private static readonly HashSet<int> EntityNames = new HashSet<int>()
            .AddName<EntityTypeAdvertisement>()
            .AddName<EntityTypeAdvertisementElementTemplate>()
            .AddName<EntityTypeCategory>()
            .AddName<EntityTypeFirm>()
            .AddName<EntityTypeFirmAddress>()
            .AddName<EntityTypeLegalPersonProfile>()
            .AddName<EntityTypeOrder>()
            .AddName<EntityTypePosition>()
            .AddName<EntityTypeProject>()
            .AddName<EntityTypeTheme>()
            ;

        public static bool IsEntityName(int entityTypeId)
        {
            return EntityNames.Contains(entityTypeId);
        }

        private static HashSet<int> AddName<TEntityType>(this HashSet<int> list)
            where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
        {
            list.Add(IdentityBase<TEntityType>.Instance.Id);
            return list;
        }
    }
}