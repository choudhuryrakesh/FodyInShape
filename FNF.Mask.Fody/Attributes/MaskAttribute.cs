using FNF.ILWeaver.Infrastructure.Models;
using System;

namespace FNF.ILWeaver.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class MaskAttribute : ProtectorAttribute
    {
        internal MaskPattern Pattern { get; }

        protected MaskAttribute()
        {
            Pattern = new MaskPattern(string.Empty, string.Empty);
        }

        protected MaskAttribute(string matchPattern, string replacePattern) : this()
        {
            Pattern = new MaskPattern(matchPattern, replacePattern);
        }
    }
}