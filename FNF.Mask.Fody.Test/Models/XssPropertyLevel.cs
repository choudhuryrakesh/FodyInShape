using FNF.ILWeaver.Attributes;

namespace FNF.ILWeaver.Test.Models
{
    internal class XssPropertyLevel
    {
        [XSSGuard]
        public string NonXssProperty { get; set; }

        public string XssProperty { get; set; }
    }
}