using FNF.ILWeaver.Infrastructure.Extensions;
using FNF.ILWeaver.Processors.Interfaces;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;

namespace FNF.ILWeaver.Processors
{
    internal class XssProcessor : IClassProcessor, IPropertyProcessor
    {
        public ModuleDefinition Module { get; }

        public TypeDefinition Class { get; }

        public TypeDefinition OfClass { get; }

        public PropertyDefinition Property { get; }

        public XssProcessor(ModuleDefinition module, TypeDefinition @class)
        {
            Module = module;
            Class = @class;
        }

        public XssProcessor(ModuleDefinition module, TypeDefinition @class, PropertyDefinition property)
        {
            Module = module;
            Class = OfClass = @class;
            Property = property;
        }

        void IClassProcessor.Process()
        {
            var publicStringProperties = Class.Properties
               .Where(p => p.SetMethod != null
                   && p.SetMethod.IsPublic
                   && p.PropertyType == Module.TypeSystem.String);
            foreach (var property in publicStringProperties)
            {
                Encode(property);
            }
        }

        void IPropertyProcessor.Process()
        {
            Encode(Property);
        }

        private void Encode(PropertyDefinition property)
        {
            var setMethod = property.SetMethod.Body;
            var loadValueInstruction = setMethod.Instructions.First(i => i.OpCode == OpCodes.Ldarg_0);

            var ilProcessor = setMethod.GetILProcessor();
            foreach (var instruction in GetInstructionsToEncode(property))
            {
                ilProcessor.InsertBefore(loadValueInstruction, instruction);
            }
        }

        private IEnumerable<Instruction> GetInstructionsToEncode(PropertyDefinition property)
        {
            yield return Instruction.Create(OpCodes.Ldarg_1);

            var encodeMethod = Module.GetMethodReference(typeof(Text), nameof(Text.EncodeHtml),
                @params => @params.Length == 1 && @params[0].ParameterType == typeof(string));
            yield return Instruction.Create(OpCodes.Call, encodeMethod);

            yield return Instruction.Create(OpCodes.Starg_S, property.SetMethod.Parameters[0]);
        }
    }
}