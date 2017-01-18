using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Options;

using NuClear.ValidationRules.WebApp.Configuration;
using NuClear.ValidationRules.WebApp.DataAccess;
using NuClear.ValidationRules.WebApp.Model;

namespace NuClear.ValidationRules.WebApp.Serializers
{
    public sealed class MessageFactory
    {
        private static readonly IDictionary<Level, string> PanelClasses =
            new Dictionary<Level, string>
                {
                    { Level.None, "" },
                    { Level.Info, "panel-info" },
                    { Level.Warning, "panel-warning" },
                    { Level.Error, "panel-danger" }
                };

        private readonly IDictionary<long, OrderDto> _orderPeriods;
        private readonly LinkFactorySettings _settings;

        public MessageFactory(IOptions<LinkFactorySettings> settings, IDictionary<long, OrderDto> orderPeriods)
        {
            _orderPeriods = orderPeriods;
            _settings = settings.Value;
        }

        public string CreatePlainTextMessage(ValidationResult result)
            => string.Format(result.Template, result.References.Select(x => x.Name).ToArray());

        public Message CreateMessage(ValidationResult result)
            => new Message
                {
                    Rule = result.Rule,
                    Level = result.Result,
                    Class = PanelClasses[result.Result],
                    MainReference = CreateLink(result.MainReference),
                    Text = string.Format(result.Template, result.References.Select(CreateLink).ToArray()),
                    PlainText = string.Format(result.Template, result.References.Select(x => x.Name).ToArray()),
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

        private string CreateLink(ValidationResult.EntityReference reference)
            => CreateLink(reference.Type, reference.Id, reference.Name);

        private string CreateLink(string entity, long entityId, string entityName)
            => $"<a target=\"_blank\" href=\"{GetUrl(entity, entityId)}\">{entityName}</a>";

        private string GetUrl(string entity, long entityId)
            => _settings.ErmUrl + "\\" + "CreateOrUpdate" + "\\" + entity + "\\" + entityId;
    }
}