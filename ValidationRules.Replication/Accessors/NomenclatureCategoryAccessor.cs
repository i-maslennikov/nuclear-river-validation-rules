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
            => from pricePosition in _query.For<Erm::PricePosition>().Where(x => x.IsActive && !x.IsDeleted)
               from position in _query.For<Erm::Position>().Where(x => !x.IsDeleted && x.IsControlledByAmount).Where(x => pricePosition.PositionId == x.Id)
               group position.Id by new { pricePosition.PriceId, position.CategoryCode } into groups
               let name = _query.For<Erm::Position>().Single(x => x.Id == groups.Min()).Name
               select new NomenclatureCategory
                   {
                       Id = groups.Key.CategoryCode,
                       PriceId = groups.Key.PriceId,
                       Name = name,
                   };

        public FindSpecification<NomenclatureCategory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            // todo: Подумать о пересчёте, сейчас не идеально: изменения Position не приведёт к изменению имени. Должно ли? Пока пойдёт.
            // А ещё лучше - читать поток или как иначе общаться с мастер-системой.
            // Или как вариант, захардкодить - и пусть будет. Похоже, справочник менятся редко.
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return SpecificationFactory<NomenclatureCategory>.Contains(x => x.PriceId, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<NomenclatureCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<NomenclatureCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<NomenclatureCategory> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<NomenclatureCategory> dataObjects)
        {
            // Ничего не пересчитываем.
            // Достаточно сложно вычислить объём изменений, достаточно ненадёжно и влияет только на строковое имя в сообщении.
            return Array.Empty<IEvent>();
        }
    }
}