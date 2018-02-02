using FNF.ILWeaver.Infrastructure.Models;

namespace FNF.ILWeaver.Attributes
{
    public class EmailMaskAttribute : MaskAttribute
    {
        public static MaskPattern MaskPattern = new MaskPattern(@"[a-z]|[A-Z]", @"X");

        public EmailMaskAttribute() : base(MaskPattern.Match, MaskPattern.Replace)
        {
        }

        public EmailMaskAttribute(string matchPattern, string replacePattern)
            : base(matchPattern, replacePattern)
        {
        }
    }
}