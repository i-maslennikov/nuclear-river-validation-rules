using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors.EntityNames
{
    public sealed class FirmNameAccessor : IStorageBasedEntityNameAccessor<Firm>
    {
        private readonly IQuery _query;

        public FirmNameAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<EntityName> GetSource() => _query
            .For(Specs.Find.Erm.Firm)
            .Select(x => new EntityName
            {
                Id = x.Id,
                EntityType = EntityTypeIds.Firm,
                Name = x.Name
            });

        public FindSpecification<EntityName> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            => commands.GetEntityNameFindSpecification(typeof(Firm), EntityTypeIds.Firm);
    }
}