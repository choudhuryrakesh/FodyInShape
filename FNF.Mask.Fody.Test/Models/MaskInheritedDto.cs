using FNF.ILWeaver.Attributes;

namespace FNF.ILWeaver.Test.Models
{
    internal class MaskInheritedDto : MaskDto
    {
        [SSNMask]
        public string ChildPropertyMasked { get; set; }
    }
}