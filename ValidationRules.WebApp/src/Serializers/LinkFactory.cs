using System;

using Microsoft.Extensions.Options;

using NuClear.ValidationRules.WebApp.Configuration;

namespace NuClear.ValidationRules.WebApp.Serializers
{
    public class LinkFactory
    {
        private readonly LinkFactorySettings _settings;

        public LinkFactory(IOptions<LinkFactorySettings> settings)
        {
            _settings = settings.Value;
        }

        public string CreateLink(Tuple<string, long, string> reference)
            => CreateLink(reference.Item1, reference.Item2, reference.Item3);

        public string CreateLink(string entity, long entityId, string entityName)
            => $"<a target=\"_blank\" href=\"{GetUrl(entity, entityId)}\">{entityName}</a>";

        public string GetUrl(Tuple<string, long, string> reference)
            => GetUrl(reference.Item1, reference.Item2);

        public string GetUrl(string entity, long entityId)
            => _settings.ErmUrl + "\\" + "CreateOrUpdate" + "\\" + entity + "\\" + entityId;
    }
}
