using FNF.ILWeaver.Attributes;
using FNF.ILWeaver.Processors.Interfaces;
using Mono.Cecil;

namespace FNF.ILWeaver.Processors
{
    internal class Processor
    {
        public static IClassProcessor ForClass(ModuleDefinition module, TypeDefinition @class, ProtectorAttribute attribute)
        {
            if (attribute is XSSGuardAttribute)
                return new XssProcessor(module, @class);

            return new DefaultProcessor(module, @class, null);
        }

        public static IPropertyProcessor ForProperty(ModuleDefinition module, TypeDefinition ofClass, PropertyDefinition property, ProtectorAttribute attribute)
        {
            if (attribute is XSSGuardAttribute)
                return new XssProcessor(module, ofClass, property);
            if (attribute is MaskAttribute)
                return new CommonMaskProcessor(module, property, ofClass, attribute);

            return new DefaultProcessor(module, ofClass, property);
        }
    }
}