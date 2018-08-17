using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class NomenclatureCategoryAccessor : IStorageBasedDataObjectAccessor<NomenclatureCategory>, IDataChangesHandler<NomenclatureCategory>
    {
        private readonly IQuery _query;

        public NomenclatureCategoryAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<NomenclatureCategory> GetSource()
            => from nomenclatureCategory in _query.For<Erm::NomenclatureCategory>()
               select new NomenclatureCategory
                   {
                       Id = nomenclatureCategory.Id,
                       Name = nomenclatureCategory.Name,
                   };

        public FindSpecification<NomenclatureCategory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<NomenclatureCategory>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<NomenclatureCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<NomenclatureCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<NomenclatureCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<NomenclatureCategory> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id);

            var rulesetIds = _query.For<Ruleset.QuantitativeRule>()
                                   .Where(r => ids.Contains(r.NomenclatureCategoryCode))
                                   .Select(r => r.RulesetId)
                                   .Distinct();

            return new EventCollectionHelper<NomenclatureCategory> { { typeof(Ruleset), rulesetIds } };
        }
    }
}