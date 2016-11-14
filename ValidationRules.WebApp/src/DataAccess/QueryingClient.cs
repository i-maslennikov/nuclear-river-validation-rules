using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IReadOnlyCollection<ValidationResult>> Single(long orderId)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(new { OrderId = orderId }), Encoding.UTF8, "application/json");
            var response = await new HttpClient().PostAsync(_settings.Single, httpContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IReadOnlyCollection<ValidationResult>>(responseString);
            return result;
        }

        public async Task<IEnumerable<ValidationResult>> Manual(IReadOnlyCollection<long> orderIds, DateTime date, long? projectId)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(new { OrderIds = orderIds, ReleaseDate = date, ProjectId = projectId }),
                                                Encoding.UTF8,
                                                "application/json");
            var response = await new HttpClient().PostAsync(_settings.Manual, httpContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IReadOnlyCollection<ValidationResult>>(responseString);
            return result;
        }

        public async Task<IEnumerable<ValidationResult>> Prerelease(IReadOnlyCollection<long> orderIds, DateTime date, long projectId)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(new { OrderIds = orderIds, ReleaseDate = date, ProjectId = projectId }),
                                                Encoding.UTF8,
                                                "application/json");
            var response = await new HttpClient().PostAsync(_settings.Prerelease, httpContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IReadOnlyCollection<ValidationResult>>(responseString);
            return result;
        }

        public async Task<IEnumerable<ValidationResult>> Release(IReadOnlyCollection<long> orderIds, DateTime date, long projectId)
        {
            var httpContent = new StringContent(JsonConvert.SerializeObject(new { OrderIds = orderIds, ReleaseDate = date, ProjectId = projectId }),
                                                Encoding.UTF8,
                                                "application/json");
            var response = await new HttpClient().PostAsync(_settings.Release, httpContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IReadOnlyCollection<ValidationResult>>(responseString);
            return result;
        }
    }
}
