using FNF.ILWeaver.Infrastructure.Models;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FNF.ILWeaver.Infrastructure.Extensions
{
    internal static class Members
    {
        internal static IEnumerable<TypeDefinition> GetAllClasses(this ModuleDefinition module)
        {
            return module.Types.Where(t => t.IsClass);
        }

        internal static MethodDefinition CreateShouldSerialize(this PropertyDefinition property, bool shouldSerialize)
        {
            return property.CreateAssociatedBoolMethod(new MethodArgument
            {
                Name = $"ShouldSerialize{property.Name}",
                Attributes = MethodAttributes.Public,
            }, shouldSerialize);
        }

        internal static MethodDefinition CreateAssociatedBoolMethod(this PropertyDefinition property, MethodArgument methodArg, bool @return)
        {
            var method = new MethodDefinition(methodArg.Name, methodArg.Attributes, property.Module.TypeSystem.Boolean);

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(property.Module.TypeSystem.Boolean));

            var instructions = body.Instructions;
            instructions.Add(Instruction.Create(OpCodes.Nop));
            instructions.Add(Instruction.Create(@return ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0));
            instructions.Add(Instruction.Create(OpCodes.Stloc_0));
            var ldLoc0 = Instruction.Create(OpCodes.Ldloc_0);
            instructions.Add(Instruction.Create(OpCodes.Br_S, ldLoc0));

            instructions.Add(ldLoc0);
            instructions.Add(Instruction.Create(OpCodes.Ret));

            return method;
        }

        internal static MethodDefinition CreateOnSerializing(this ModuleDefinition module, IEnumerable<Instruction> instructions)
        {
            var onSerializingMethod = new MethodDefinition("OnSerializing", MethodAttributes.Public, module.TypeSystem.Void);

            var streamingContext = module.ImportReference(typeof(StreamingContext));
            onSerializingMethod.Parameters.Add(new ParameterDefinition("context", ParameterAttributes.None, streamingContext));

            var onSerializingAttribute = module.CreateCustomAttribute<OnSerializingAttribute>();
            onSerializingMethod.CustomAttributes.Add(onSerializingAttribute);

            foreach (var instruction in instructions)
            {
                onSerializingMethod.Body.Instructions.Add(instruction);
            }

            return onSerializingMethod;
        }

        internal static bool HaveAttribute<T>(this TypeDefinition @class) where T : Attribute
        {
            if (@class == null) return false;

            if (@class.CustomAttributes.Any(a => a.Is<T>()))
                return true;
            else
                return ((@class as TypeDefinition)?.BaseType as TypeDefinition).HaveAttribute<T>();
        }

        internal static IEnumerable<CustomAttribute> GetAttributes<T>(this TypeDefinition @class) where T : Attribute
        {
            if (@class == null) yield break;

            foreach (var attribute in @class.CustomAttributes.Where(a => a.Is<T>()))
                yield return attribute;

            var baseClass = (@class as TypeDefinition)?.BaseType as TypeDefinition;
            if (baseClass != null)
            {
                foreach (var attribute in baseClass.GetAttributes<T>())
                    yield return attribute;
            }
        }
    }
}