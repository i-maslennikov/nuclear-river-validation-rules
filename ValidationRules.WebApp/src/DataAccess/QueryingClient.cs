using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private static readonly HttpClient HttpClient = new HttpClient();

        private readonly QueryingHostSettings _settings;

        public QueryingClient(IOptions<QueryingHostSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<IReadOnlyCollection<ValidationResult>> Single(long orderId)
        {
            var request = CreateRequest(_settings.Single, new { OrderId = orderId });
            var result = await Execute(request);
            return result;
        }

        public async Task<IEnumerable<ValidationResult>> Manual(IReadOnlyCollection<long> orderIds, DateTime date, long? projectId)
        {
            var request = CreateRequest(_settings.Manual, new { OrderIds = orderIds, ReleaseDate = date, ProjectId = projectId });
            var result = await Execute(request);
            return result;
        }

        public async Task<IEnumerable<ValidationResult>> Prerelease(IReadOnlyCollection<long> orderIds, DateTime date, long projectId)
        {
            var request = CreateRequest(_settings.Prerelease, new { OrderIds = orderIds, ReleaseDate = date, ProjectId = projectId });
            var result = await Execute(request);
            return result;
        }

        public async Task<IEnumerable<ValidationResult>> Release(IReadOnlyCollection<long> orderIds, DateTime date, long projectId)
        {
            var request = CreateRequest(_settings.Release, new { OrderIds = orderIds, ReleaseDate = date, ProjectId = projectId });
            var result = await Execute(request);
            return result;
        }

        private static HttpRequestMessage CreateRequest(Uri uri, object payload)
            => new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(payload),
                        Encoding.UTF8,
                        "application/json"),
                    Headers = { AcceptLanguage = { new StringWithQualityHeaderValue("ru") } }
                };

        private static async Task<IReadOnlyCollection<ValidationResult>> Execute(HttpRequestMessage request)
        {
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IReadOnlyCollection<ValidationResult>>(responseString);
        }
    }
}
