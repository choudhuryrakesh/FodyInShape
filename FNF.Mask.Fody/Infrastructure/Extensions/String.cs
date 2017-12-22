using System.Text.RegularExpressions;

namespace FNF.Mask.Fody.Infrastructure.Extensions
{
    public static class String
    {
        public static string MaskSSN(this string ssn, string matchPattern, string maskPattern)
        {
            return Regex.Replace(ssn, matchPattern, maskPattern);
        }
    }
}