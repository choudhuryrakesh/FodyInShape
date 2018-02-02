namespace FNF.ILWeaver.Infrastructure.Models
{
    public struct MaskPattern
    {
        public string Match { get; }
        public string Replace { get; }

        internal MaskPattern(string match, string replace)
        {
            Match = match;
            Replace = replace;
        }
    }
}