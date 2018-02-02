using FNF.ILWeaver.Infrastructure;
using FNF.ILWeaver.Processors.Interfaces;
using Mono.Cecil;

namespace FNF.ILWeaver.Processors
{
    internal class DefaultProcessor : IClassProcessor, IPropertyProcessor
    {
        public ModuleDefinition Module { get; }

        public TypeDefinition Class { get; }

        public PropertyDefinition Property { get; }

        public TypeDefinition OfClass { get; }

        public DefaultProcessor(ModuleDefinition module, TypeDefinition @class, PropertyDefinition property)
        {
            Module = module;
            OfClass = Class = @class;
            Property = property;
            Logger.CurrentLogger.DebugInfo($"Processor: {nameof(DefaultProcessor)}. No matching processor found.");
        }

        public void Process()
        {
            ShowDefaultProcessedMessage();
        }

        private void ShowDefaultProcessedMessage()
        {
            Logger.CurrentLogger.DebugInfo($"Processor: {nameof(DefaultProcessor)}. No changes done.");
        }
    }
}