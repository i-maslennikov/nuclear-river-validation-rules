using System;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class LinkFactory
    {
        private readonly ResultDeliverySettingsAspect _settings;

        public LinkFactory(ResultDeliverySettingsAspect settings)
        {
            _settings = settings;
        }

        public string CreateLink(Tuple<string, long, string> reference)
            => CreateLink(reference.Item1, reference.Item2, reference.Item3);

        public string CreateLink(string entityName, long entityId, string displayText)
            => $"<{_settings.ErmProduction}/CreateOrUpdate/{entityName}/{entityId}|{displayText}>";
    }
}
