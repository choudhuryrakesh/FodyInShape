using FNF.ILWeaver.Attributes;
using FNF.ILWeaver.Processors.Interfaces;
using Mono.Cecil;

namespace FNF.ILWeaver.Processors
{
    internal abstract class MaskProcessor : IPropertyProcessor
    {
        public ModuleDefinition Module { get; }

        public TypeDefinition OfClass { get; }

        public PropertyDefinition Property { get; }

        protected readonly MaskAttribute _attribute;

        protected MaskProcessor(ModuleDefinition module, PropertyDefinition property, TypeDefinition ofClass, ProtectorAttribute attribute)
        {
            Module = module;
            Property = property;
            OfClass = ofClass;
            _attribute = (MaskAttribute)attribute;
        }

        public abstract void Process();
    }
}