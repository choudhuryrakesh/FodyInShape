using FNF.ILWeaver.Attributes;
using FNF.ILWeaver.Infrastructure.Extensions;
using FNF.ILWeaver.Infrastructure.Models;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FNF.ILWeaver.Processors
{
    internal class CommonMaskProcessor : MaskProcessor
    {
        private string ShouldMaskMethodName => $"ShouldMask{Property.Name}";

        internal CommonMaskProcessor(ModuleDefinition module, PropertyDefinition property, TypeDefinition ofClass, ProtectorAttribute attribute)
            : base(module, property, ofClass, attribute)
        {
        }

        public override void Process()
        {
            var maskingProperty = CreateMaskingProperty();
            OfClass.Properties.Add(maskingProperty);

            var shouldMaskExists = OfClass.Methods.Any(m => m.Name == ShouldMaskMethodName);
            if (!shouldMaskExists)
            {
                var shouldMaskMethod = Property.CreateAssociatedBoolMethod(new MethodArgument
                {
                    Name = ShouldMaskMethodName,
                    Attributes = MethodAttributes.Private,
                }, true);
                OfClass.Methods.Add(shouldMaskMethod);
            }

            var maskingPropertyGetMethod = CreateGetMethodForMaskingProperty(maskingProperty);
            OfClass.Methods.Add(maskingPropertyGetMethod);
            maskingProperty.GetMethod = maskingPropertyGetMethod;

            var shouldSerializeMethod = maskingProperty.CreateShouldSerialize(false);
            OfClass.Methods.Add(shouldSerializeMethod);

            var isOnSerializingMethodExist = OfClass.Methods.Any(m => m.CustomAttributes.Have<OnSerializingAttribute>());
            if (isOnSerializingMethodExist)
            {
                WeaveOnSerializationMethod(maskingProperty);
            }
            else
            {
                var onSerializingMethod = CreateOnSerializationMethod(maskingProperty);
                OfClass.Methods.Add(onSerializingMethod);
            }
        }

        private PropertyDefinition CreateMaskingProperty()
        {
            return new PropertyDefinition($"{Property.Name}Mask", PropertyAttributes.None, Module.TypeSystem.String)
            {
                HasThis = true,
                IsSpecialName = true,
            };
        }

        private MethodDefinition CreateGetMethodForMaskingProperty(PropertyDefinition maskingProperty)
        {
            var getMethod = new MethodDefinition($"get_{maskingProperty.Name}", MethodAttributes.Public, Module.TypeSystem.String);
            getMethod.Body.InitLocals = true;
            foreach (var variable in GetVariablesForMaskPropertyGetBody())
            {
                getMethod.Body.Variables.Add(variable);
            }

            foreach (var instruction in GetInstructionsForMaskPropertyGetBody())
            {
                getMethod.Body.Instructions.Add(instruction);
            }

            return getMethod;
        }

        private IEnumerable<VariableDefinition> GetVariablesForMaskPropertyGetBody()
        {
            yield return new VariableDefinition(Module.TypeSystem.Boolean);
            yield return new VariableDefinition(Module.TypeSystem.Boolean);
            yield return new VariableDefinition(Module.TypeSystem.String);
            yield return new VariableDefinition(Module.TypeSystem.Boolean);
        }

        private IEnumerable<Instruction> GetInstructionsForMaskPropertyGetBody()
        {
            yield return Instruction.Create(OpCodes.Nop);

            yield return Instruction.Create(OpCodes.Ldarg_0);
            yield return Instruction.Create(OpCodes.Call, Property.GetMethod);
            var stringIsNullOrEmpty = Module.GetMethodReference<string>(nameof(string.IsNullOrWhiteSpace),
                @params => @params.Length == 1 && @params[0].ParameterType == typeof(string));
            yield return Instruction.Create(OpCodes.Call, stringIsNullOrEmpty);
            yield return Instruction.Create(OpCodes.Stloc_1);
            yield return Instruction.Create(OpCodes.Ldloc_1);
            var ldArg0ShouldMask = Instruction.Create(OpCodes.Ldarg_0);
            yield return Instruction.Create(OpCodes.Brfalse_S, ldArg0ShouldMask);

            yield return Instruction.Create(OpCodes.Ldsfld, Module.GetFieldReference<string>(nameof(string.Empty)));
            yield return Instruction.Create(OpCodes.Stloc_2);
            var ldLoc2 = Instruction.Create(OpCodes.Ldloc_2);
            yield return Instruction.Create(OpCodes.Br_S, ldLoc2);

            yield return (ldArg0ShouldMask);
            var shouldMask = OfClass.Methods.Single(m => m.Name == ShouldMaskMethodName);
            yield return Instruction.Create(OpCodes.Call, shouldMask);
            yield return Instruction.Create(OpCodes.Stloc_0);
            yield return Instruction.Create(OpCodes.Ldloc_0);
            yield return Instruction.Create(OpCodes.Ldc_I4_0);
            yield return Instruction.Create(OpCodes.Ceq);
            yield return Instruction.Create(OpCodes.Stloc_3);
            yield return Instruction.Create(OpCodes.Ldloc_3);
            var ldArg0Mask = Instruction.Create(OpCodes.Ldarg_0);
            yield return Instruction.Create(OpCodes.Brfalse_S, ldArg0Mask);

            yield return Instruction.Create(OpCodes.Ldarg_0);
            yield return Instruction.Create(OpCodes.Call, Property.GetMethod);
            yield return Instruction.Create(OpCodes.Stloc_2);
            yield return Instruction.Create(OpCodes.Br_S, ldLoc2);

            yield return ldArg0Mask;
            yield return Instruction.Create(OpCodes.Call, Property.GetMethod);
            yield return Instruction.Create(OpCodes.Ldstr, _attribute.Pattern.Match);
            yield return Instruction.Create(OpCodes.Ldstr, _attribute.Pattern.Replace);
            var masker = Module.GetMethodReference(typeof(Text), nameof(Text.Mask),
                @params => @params.Length == 3 && @params.All(p => p.ParameterType == typeof(string)));
            yield return Instruction.Create(OpCodes.Call, masker);
            yield return Instruction.Create(OpCodes.Stloc_2);
            yield return Instruction.Create(OpCodes.Br_S, ldLoc2);

            yield return ldLoc2;
            yield return Instruction.Create(OpCodes.Ret);
        }

        private void WeaveOnSerializationMethod(PropertyDefinition maskingProperty)
        {
            var onSerializingMethod = OfClass.Methods.FirstOrDefault(m => m.CustomAttributes.Have<OnSerializingAttribute>());
            var retInstruction = onSerializingMethod.Body.Instructions.Single(i => i.OpCode == OpCodes.Ret);
            var ilProcessor = onSerializingMethod.Body.GetILProcessor();

            var instructions = GetInstructionsForOnSerializingMethod(maskingProperty, false);
            foreach (var instruction in instructions)
            {
                ilProcessor.InsertBefore(retInstruction, instruction);
            }
        }

        private MethodDefinition CreateOnSerializationMethod(PropertyDefinition maskingProperty)
        {
            var instructions = GetInstructionsForOnSerializingMethod(maskingProperty, true);
            return Module.CreateOnSerializing(instructions);
        }

        private IEnumerable<Instruction> GetInstructionsForOnSerializingMethod(PropertyDefinition maskingProperty,
            bool isNew)
        {
            if (isNew)
                yield return Instruction.Create(OpCodes.Nop);
            yield return Instruction.Create(OpCodes.Ldarg_0);
            yield return Instruction.Create(OpCodes.Ldarg_0);
            yield return Instruction.Create(OpCodes.Call, maskingProperty.GetMethod);
            yield return Instruction.Create(OpCodes.Call, Property.SetMethod);
            yield return Instruction.Create(OpCodes.Nop);
            if (isNew)
                yield return Instruction.Create(OpCodes.Ret);
        }
    }
}