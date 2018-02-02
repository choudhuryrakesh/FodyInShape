using FNF.ILWeaver.Infrastructure.Models;

namespace FNF.ILWeaver.Attributes
{
    public class SSNMaskAttribute : MaskAttribute
    {
        public static MaskPattern MaskPattern = new MaskPattern(@"\d(?!\d{0,3}$)", @"X");

        public SSNMaskAttribute() : base(MaskPattern.Match, MaskPattern.Replace)
        {
        }

        public SSNMaskAttribute(string matchPattern, string replacePattern)
            : base(matchPattern, replacePattern)
        {
        }
    }
}