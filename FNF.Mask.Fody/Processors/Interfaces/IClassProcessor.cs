using Mono.Cecil;

namespace FNF.ILWeaver.Processors.Interfaces
{
    internal interface IClassProcessor : IProcessor
    {
        TypeDefinition Class { get; }

        void Process();
    }
}