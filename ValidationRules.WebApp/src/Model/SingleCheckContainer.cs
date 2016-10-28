using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.WebApp.Serializers;

namespace NuClear.ValidationRules.WebApp.Model
{
    public sealed class SingleCheckContainer : ModelBase
    {
        private readonly LinkFactory _linkFactory;

        public SingleCheckContainer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public IReadOnlyCollection<MessageViewModel> WhenSingle { get; set; }
        public IReadOnlyCollection<MessageViewModel> WhenMass { get; set; }
        public IReadOnlyCollection<MessageViewModel> WhenMassPrerelease { get; set; }
        public IReadOnlyCollection<MessageViewModel> WhenMassRelease { get; set; }

        public IReadOnlyCollection<OrderViewModel> GetResultsByOrder()
        {
            var single = Foo(WhenSingle, " (Единичная)").Union(Foo(WhenMass, " (Массовая ручная)")).Union(Foo(WhenMassPrerelease, " (Массовая бета)")).Union(Foo(WhenMassRelease, " (Массовая релиз)"));
            return single.ToArray();
        }

        private IEnumerable<OrderViewModel> Foo(IEnumerable<MessageViewModel> models, string mode)
            => models.OrderByDescending(x => x.Result)
                     .ThenBy(x => x.OrderId)
                     .GroupBy(x => x.Order)
                     .Select(x => new OrderViewModel(Tuple.Create(x.Key.Item1, x.Key.Item2, x.Key.Item3 + mode), x.ToArray(), _linkFactory));
    }
}