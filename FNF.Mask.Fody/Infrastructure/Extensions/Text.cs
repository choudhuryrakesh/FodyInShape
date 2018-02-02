using System.Net;
using System.Text.RegularExpressions;

namespace FNF.ILWeaver.Infrastructure.Extensions
{
    public static class Text
    {
        public static string Mask(this string source, string matchPattern, string maskPattern)
        {
            return Regex.Replace(source, matchPattern, maskPattern);
        }

        public static string EncodeHtml(this string html)
        {
            return WebUtility.HtmlEncode(html);
        }
    }
}