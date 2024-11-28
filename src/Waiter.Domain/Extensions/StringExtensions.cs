using System.Text.RegularExpressions;

namespace Waiter.Domain.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveMask(this string value)
        {
            return Regex.Replace(value, @"[^\d\w]", "", RegexOptions.None, TimeSpan.FromSeconds(5));
        }
    }
}
