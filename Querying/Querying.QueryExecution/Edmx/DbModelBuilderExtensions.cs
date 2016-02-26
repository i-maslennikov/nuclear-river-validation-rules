using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace NuClear.Querying.Edm.Edmx
{
    internal static class DbModelBuilderExtensions
    {
        public static TypeConventionConfiguration RegisterEntity(this DbModelBuilder builder, Type entityType)
        {
            builder.RegisterEntityType(entityType);

            return builder.Types().Where(x => x == entityType);
        }
    }
}