using System.Text.RegularExpressions;

namespace NuClear.ValidationRules.WebApp.Controllers
{
    internal static class Extensions
    {
        private static readonly Regex Link = new Regex("</?a.*?>");

        public static string RemoveLinks(this string s)
        {
            return Link.Replace(s, string.Empty);
        }
    }
}