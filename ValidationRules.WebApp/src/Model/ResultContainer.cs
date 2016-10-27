using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.WebApp.DataAccess;
using NuClear.ValidationRules.WebApp.Serializers;

namespace NuClear.ValidationRules.WebApp.Model
{
    public sealed class ResultContainer : ModelBase
    {
        private readonly LinkFactory _linkFactory;

        public ResultContainer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public IReadOnlyCollection<MessageViewModel> Results { get; set; }
        public IDictionary<long, OrderDto> Orders { get; set; }

        public string PeriodFor(long id)
        {
            OrderDto period;
            if(Orders.TryGetValue(id, out period))
                return $"{period.Begin:Y} - {period.EndFact:Y}";
            return string.Empty;
        }

        public IReadOnlyCollection<OrderViewModel> GetResultsByOrder()
        {
            return Results.OrderByDescending(x => x.Result)
                          .ThenBy(x => x.OrderId)
                          .GroupBy(x => x.Order)
                          .Select(x => new OrderViewModel(x.Key, x.ToArray(), _linkFactory))
                          .ToArray();
        }

        public IReadOnlyCollection<ProjectViewModel> GetResultsByType()
        {
            return Results.OrderByDescending(x => x.Result)
                          .ThenBy(x => x.OrderId)
                          .GroupBy(x => x.MessageType)
                          .Select(x => new ProjectViewModel(x.ToArray(), _linkFactory))
                          .ToArray();
        }
    }
}