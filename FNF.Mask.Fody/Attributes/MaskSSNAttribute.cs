namespace FNF.Mask.Fody.Attributes
{
    public class MaskSSNAttribute : MaskAttribute
    {
        public MaskSSNAttribute() : base(@"^\d{3}-\d{2}-\d{4}$", @"XXX-XX-$1")
        {
        }

        public MaskSSNAttribute(string MatchPattern, string ReplacePattern) : base(MatchPattern, ReplacePattern)
        {

        }

    }
}