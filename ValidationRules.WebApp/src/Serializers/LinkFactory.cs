
using Microsoft.Extensions.Options;

using NuClear.ValidationRules.WebApp.Configuration;
using NuClear.ValidationRules.WebApp.Model;

namespace NuClear.ValidationRules.WebApp.Serializers
{
    public class LinkFactory
    {
        private readonly LinkFactorySettings _settings;

        public LinkFactory(IOptions<LinkFactorySettings> settings)
        {
            _settings = settings.Value;
        }

        public string CreateLink(ValidationResult.EntityReference reference)
            => CreateLink(reference.Type, reference.Id, reference.Name);

        private string CreateLink(string entity, long entityId, string entityName)
            => $"<a target=\"_blank\" href=\"{GetUrl(entity, entityId)}\">{entityName}</a>";

        private string GetUrl(string entity, long entityId)
            => _settings.ErmUrl + "\\" + "CreateOrUpdate" + "\\" + entity + "\\" + entityId;
    }
}
