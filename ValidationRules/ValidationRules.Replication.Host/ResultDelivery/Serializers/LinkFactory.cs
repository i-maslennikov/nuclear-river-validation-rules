using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers
{
    public sealed class LinkFactory
    {
        public string CreateLink(string entityName, long entityId, string displayText)
            => $"<https://web-app20.test.erm.2gis.ru/CreateOrUpdate/{entityName}/{entityId}|{displayText}>";
    }
}
