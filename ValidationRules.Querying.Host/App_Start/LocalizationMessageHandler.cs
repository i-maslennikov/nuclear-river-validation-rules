using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace NuClear.ValidationRules.Querying.Host
{
    public sealed class LocalizationMessageHandler : DelegatingHandler
    {
        private static readonly IReadOnlyDictionary<string, CultureInfo> AllCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).ToDictionary(x => x.Name, x => x);
        private readonly ResourceManager _resourceManager;

        public LocalizationMessageHandler(Type resourcesType)
        {
            _resourceManager = new ResourceManager(resourcesType);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var cultureInfo = GetCultureInfoFromUrl(request) ?? GetCultureInfoFromAcceptLanguageHeader(request);
            if (cultureInfo != null)
            {
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        private CultureInfo GetCultureInfoFromAcceptLanguageHeader(HttpRequestMessage request)
        {
            foreach (var language in request.Headers.AcceptLanguage)
            {
                CultureInfo cultureInfo;
                if (AllCultures.TryGetValue(language.Value, out cultureInfo))
                {
                    var resourceSet = _resourceManager.GetResourceSet(
                        cultureInfo,
                        true,   // true to cache resourceSet for better performance
                        false); /* false to disable resource fallback detection, it is client responsibility to
                                   specify all acceptable languages including possible fallbacks */
                    if (resourceSet != null)
                    {
                        resourceSet.Dispose();
                        return cultureInfo;
                    }
                }
            }

            return null;
        }

        private CultureInfo GetCultureInfoFromUrl(HttpRequestMessage request)
        {
            var queryArguments = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var cultureName = queryArguments.Get("culture");
            return string.IsNullOrEmpty(cultureName)
                       ? null
                       : CultureInfo.GetCultureInfo(cultureName);
        }
    }
}