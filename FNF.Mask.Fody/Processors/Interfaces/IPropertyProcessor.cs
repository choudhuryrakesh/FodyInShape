using Mono.Cecil;

namespace FNF.ILWeaver.Processors.Interfaces
{
    internal interface IPropertyProcessor : IProcessor
    {
        TypeDefinition OfClass { get; }
        PropertyDefinition Property { get; }

        void Process();
    }
}