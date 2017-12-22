using System;

namespace FNF.Mask.Fody.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class MaskAttribute : Attribute
    {
        protected const char DefaultReplaceChar = '#';

        internal string MatchPattern { get; }
        internal string ReplacePattern { get; }

        public MaskAttribute()
        {
            MatchPattern = ReplacePattern = string.Empty;
        }

        public MaskAttribute(string MatchPattern, string ReplacePattern)
        {
            this.MatchPattern = MatchPattern;
            this.ReplacePattern = ReplacePattern;
        }
    }
}