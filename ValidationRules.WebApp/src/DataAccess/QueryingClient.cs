using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using NuClear.ValidationRules.WebApp.Configuration;
using NuClear.ValidationRules.WebApp.Model;

namespace NuClear.ValidationRules.WebApp.DataAccess
{
    public class QueryingClient
    {
        private readonly QueryingHostSettings _settings;

        public QueryingClient(IOptions<QueryingHostSettings> settings)
        {
            _settings = settings.Value;
        }

        public IReadOnlyCollection<ValidationResult> Single(long orderId)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(new { OrderId = orderId }), Encoding.UTF8, "application/json");
            var response = new HttpClient().PostAsync(_settings.Single, httpContent).Result;
            var result = JsonConvert.DeserializeObject<IReadOnlyCollection<ValidationResult>>(response.Content.ReadAsStringAsync().Result);
            return result;
        }

        public IEnumerable<ValidationResult> Manual(IReadOnlyCollection<long> orderIds, DateTime date, long? projectId)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(new { OrderIds = orderIds, ReleaseDate = date, ProjectId = projectId }),
                                                Encoding.UTF8,
                                                "application/json");
            var response = new HttpClient().PostAsync(_settings.Manual, httpContent).Result;
            var result = JsonConvert.DeserializeObject<IReadOnlyCollection<ValidationResult>>(response.Content.ReadAsStringAsync().Result);
            return result;
        }

        public IEnumerable<ValidationResult> Prerelease(IReadOnlyCollection<long> orderIds, DateTime date, long projectId)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(new { OrderIds = orderIds, ReleaseDate = date, ProjectId = projectId }),
                                                Encoding.UTF8,
                                                "application/json");
            var response = new HttpClient().PostAsync(_settings.Prerelease, httpContent).Result;
            var result = JsonConvert.DeserializeObject<IReadOnlyCollection<ValidationResult>>(response.Content.ReadAsStringAsync().Result);
            return result;
        }

        public IEnumerable<ValidationResult> Release(IReadOnlyCollection<long> orderIds, DateTime date, long projectId)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(new { OrderIds = orderIds, ReleaseDate = date, ProjectId = projectId }),
                                                Encoding.UTF8,
                                                "application/json");
            var response = new HttpClient().PostAsync(_settings.Release, httpContent).Result;
            var result = JsonConvert.DeserializeObject<IReadOnlyCollection<ValidationResult>>(response.Content.ReadAsStringAsync().Result);
            return result;
        }
    }
}
