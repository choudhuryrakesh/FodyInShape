using FNF.Mask.Fody.Attributes;
using Mono.Cecil;

namespace FNF.Mask.Fody.Maskers
{
    public class DefaultMasker : Masker
    {
        internal DefaultMasker(ModuleDefinition module, MaskAttribute attribute) : base(module, attribute)
        {
        }

        internal override void Process(PropertyDefinition property, TypeDefinition inClass)
        {
        }
    }
}