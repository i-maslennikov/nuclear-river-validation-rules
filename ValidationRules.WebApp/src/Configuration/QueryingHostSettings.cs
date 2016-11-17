using System;

namespace NuClear.ValidationRules.WebApp.Configuration
{
    public class QueryingHostSettings
    {
        public Uri Host { get; set; }

        public Uri Single => new Uri(Host, "api/Single");
        public Uri Manual => new Uri(Host, "api/Manual");
        public Uri Prerelease => new Uri(Host, "api/Prerelease");
        public Uri Release => new Uri(Host, "api/Release");
    }
}
