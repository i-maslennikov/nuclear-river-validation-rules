namespace NuClear.ValidationRules.Querying.Host.Composition
{
    internal static class StringExtensions
    {
        public static string ClearBrackets(this string format)
        {
            return format?.Replace("{", null).Replace("}", null);
        }
    }
}