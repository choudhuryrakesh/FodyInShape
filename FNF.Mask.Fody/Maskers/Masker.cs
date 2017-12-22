using FNF.Mask.Fody.Attributes;
using Mono.Cecil;

namespace FNF.Mask.Fody.Maskers
{
    public abstract class Masker
    {
        protected readonly ModuleDefinition Module;
        protected readonly MaskAttribute Attribute;

        protected Masker(ModuleDefinition module, MaskAttribute attribute)
        {
            Module = module;
            Attribute = attribute;
        }

        internal abstract void Process(PropertyDefinition property, TypeDefinition inClass);

        internal static Masker For(ModuleDefinition module, MaskAttribute attribute)
        {
            if (attribute is MaskSSNAttribute)
                return new SSNMasker(module, attribute);

            return new DefaultMasker(module, attribute);
        }
    }
}