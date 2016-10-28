using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.WebApp.Serializers;

namespace NuClear.ValidationRules.WebApp.Model
{
    public class OrderViewModel
    {
        private static readonly Random Random = new Random();

        private static readonly IDictionary<Result, string> Classes =
            new Dictionary<Result, string> { { Result.Info, "panel-info" }, { Result.Warning, "panel-warning" }, { Result.Error, "panel-danger" } };

        private readonly Tuple<string, long, string> _order;
        private readonly IReadOnlyCollection<MessageViewModel> _messages;
        private readonly Result _result;
        private readonly LinkFactory _linkFactory;
        private readonly int _random;

        public OrderViewModel(Tuple<string, long, string> order, IReadOnlyCollection<MessageViewModel> messages, LinkFactory linkFactory)
        {
            _random = Random.Next();
            _order = order;
            _messages = messages;
            _result = messages.Max(x => x.Result);
            _linkFactory = linkFactory;
        }

        public string Number => _order.Item3;

        public string PanelId => $"collapse_{_order.Item2}_{_random}";

        public long Id => _order.Item2;

        public string Link => _linkFactory.GetUrl(_order);

        public string Class => GetClass(_result);

        public IReadOnlyCollection<MessageViewModel> Messages => _messages;

        private static string GetClass(Result result)
        {
            string cls;
            if (!Classes.TryGetValue(result, out cls))
            {
                throw new ArgumentException($"Not supported: {result}", nameof(result));
            }

            return cls;
        }
    }
}