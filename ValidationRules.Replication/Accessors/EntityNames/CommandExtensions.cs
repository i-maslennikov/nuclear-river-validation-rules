using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors.EntityNames
{
    internal static class CommandExtensions
    {
        public static FindSpecification<EntityName> GetEntityNameFindSpecification(this IReadOnlyCollection<ICommand> commands, Type type, int typeId)
        {
            var ids = commands.Cast<SyncDataObjectCommand>()
                              .Where(c => c.DataObjectType == type)
                              .Select(c => new { Id = c.DataObjectId, EntityType = typeId })
                              .Distinct()
                              .ToList();

            return SpecificationFactory<EntityName>.Contains(x => new { x.Id, x.EntityType }, ids);
        }
    }
}
