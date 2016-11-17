using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.WebApp.DataAccess;
using NuClear.ValidationRules.WebApp.Model;

namespace NuClear.ValidationRules.WebApp.Serializers
{
    public sealed class MessageFactory
    {
        private static readonly IDictionary<ValidationResult.Level, string> PanelClasses =
            new Dictionary<ValidationResult.Level, string>
                {
                    { ValidationResult.Level.Info, "panel-info" },
                    { ValidationResult.Level.Warning, "panel-warning" },
                    { ValidationResult.Level.Error, "panel-danger" }
                };

        private readonly LinkFactory _linkFactory;
        private readonly IDictionary<long, OrderDto> _orderPeriods;

        public MessageFactory(LinkFactory linkFactory, IDictionary<long, OrderDto> orderPeriods)
        {
            _linkFactory = linkFactory;
            _orderPeriods = orderPeriods;
        }

        public string CreatePlainTextMessage(ValidationResult result)
            => string.Format(result.Template, result.References.Select(x => x.Name).ToArray());

        public Message CreateMessage(ValidationResult result)
            => new Message
                {
                    Rule = result.Rule,
                    Level = PanelClasses[result.Result],
                    MainReference = _linkFactory.CreateLink(result.MainReference),
                    Text = string.Format(result.Template, result.References.Select(_linkFactory.CreateLink).ToArray()),
                    Period = PeriodFor(result.MainReference)
                };

        private string PeriodFor(ValidationResult.EntityReference reference)
        {
            OrderDto period;
            if (string.Equals(reference.Type, "Order") && _orderPeriods.TryGetValue(reference.Id, out period))
            {
                return $"{period.Begin:Y} - {period.EndFact:Y}";
            }

            return string.Empty;
        }
    }
}