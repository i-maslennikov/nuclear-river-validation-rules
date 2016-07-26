using System;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class LinkFactory
    {
        public string CreateLink(Tuple<string, long, string> reference)
            => CreateLink(reference.Item1, reference.Item2, reference.Item3);

        public string CreateLink(string entityName, long entityId, string displayText)
            => $"<https://web-app20.test.erm.2gis.ru/CreateOrUpdate/{entityName}/{entityId}|{displayText}>";
    }
}
