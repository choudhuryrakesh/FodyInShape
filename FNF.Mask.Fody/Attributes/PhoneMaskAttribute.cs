using FNF.ILWeaver.Infrastructure.Models;

namespace FNF.ILWeaver.Attributes
{
    public class PhoneMaskAttribute : MaskAttribute
    {
        public static MaskPattern MaskPattern = new MaskPattern(@"[^-]", @"X");

        public PhoneMaskAttribute() : base(MaskPattern.Match, MaskPattern.Replace)
        {
        }

        public PhoneMaskAttribute(string matchPattern, string replacePattern)
            : base(matchPattern, replacePattern)
        {
        }
    }
}