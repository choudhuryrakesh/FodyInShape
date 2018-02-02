using Mono.Cecil;

namespace FNF.ILWeaver.Processors.Interfaces
{
    internal interface IProcessor
    {
        ModuleDefinition Module { get; }
    }
}