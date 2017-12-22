using FNF.Mask.Fody.Attributes;
using FNF.Mask.Fody.Infrastructure.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace FNF.Mask.Fody.Maskers
{
    public class SSNMasker : Masker 
        // ToDo: Either most of these methods can be moved to base class as virtual. Or just have a single class.
        // Reason being, any mask logic would be using same
    {
        internal SSNMasker(ModuleDefinition module, MaskAttribute attribute) : base(module, attribute)
        {
        }

        internal override void Process(PropertyDefinition property, TypeDefinition inClass)
        {
            var maskPropertyName = $"{property.Name}Mask";

            var getMethodForMaskProperty = GetMaskPropertyGetMethod(property, maskPropertyName);
            inClass.Methods.Add(getMethodForMaskProperty);

            var maskProperty = new PropertyDefinition(maskPropertyName, PropertyAttributes.None, Module.TypeSystem.String)
            {
                HasThis = true,
                GetMethod = getMethodForMaskProperty,
                IsSpecialName = true,
            };
            inClass.Properties.Add(maskProperty);

            var dataMemberAttribute = GetDataMemberAttribute(property.Name);
            maskProperty.CustomAttributes.Add(dataMemberAttribute);

            var shouldSerialize = GetShouldSerializeMethodFor(property);
            inClass.Methods.Add(shouldSerialize);

            //var isDataContract = inClass.CustomAttributes.Have<DataContractAttribute>();
            //if (!isDataContract)
            //{
            //    var dataContractAttribute = Module.CreateCustomAttribute<DataContractAttribute>();
            //    inClass.CustomAttributes.Add(dataContractAttribute);
            //}
        }

        private CustomAttribute GetDataMemberAttribute(string alias)
        {
            var aliasArg = new NamedArg(nameof(DataMemberAttribute.Name), Module.TypeSystem.String, alias);
            var isRequiredArg = new NamedArg(nameof(DataMemberAttribute.IsRequired), Module.TypeSystem.Boolean, false);
            var emitDefaultValueArg = new NamedArg(nameof(DataMemberAttribute.EmitDefaultValue), Module.TypeSystem.Boolean, false);
            return Module.CreateCustomAttribute<DataMemberAttribute>(new List<NamedArg> { aliasArg, isRequiredArg, emitDefaultValueArg });
        }

        private MethodDefinition GetShouldSerializeMethodFor(PropertyDefinition property)
        {
            var methodName = "ShouldSerialize" + property.Name;
            var shouldSerializeMethod = new MethodDefinition(methodName, MethodAttributes.Public, Module.TypeSystem.Boolean);

            var body = shouldSerializeMethod.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(Module.TypeSystem.Boolean));

            var instructions = body.Instructions;
            instructions.Add(Instruction.Create(OpCodes.Nop));
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
            instructions.Add(Instruction.Create(OpCodes.Stloc_0));
            var ldLoc0 = Instruction.Create(OpCodes.Ldloc_0);
            instructions.Add(Instruction.Create(OpCodes.Br_S, ldLoc0));
            instructions.Add(ldLoc0);
            instructions.Add(Instruction.Create(OpCodes.Ret));

            return shouldSerializeMethod;
        }

        private MethodDefinition GetMaskPropertyGetMethod(PropertyDefinition property, string newName)
        {
            var getMethod = new MethodDefinition($"get_{newName}", MethodAttributes.Public, Module.TypeSystem.String);

            var body = getMethod.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(Module.TypeSystem.String));

            var instructions = body.Instructions;
            instructions.Add(Instruction.Create(OpCodes.Nop));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            var actualProperty = property.GetMethod;
            instructions.Add(Instruction.Create(OpCodes.Call, actualProperty));
            var stringIsNullOrEmpty = Module.GetMethodReference<string>(nameof(string.IsNullOrWhiteSpace),
                @params => @params.Length == 1 && @params[0].ParameterType == typeof(string));
            instructions.Add(Instruction.Create(OpCodes.Call, stringIsNullOrEmpty));
            var ldsStringEmpty = Instruction.Create(OpCodes.Ldsfld, Module.GetFieldReference<string>(nameof(string.Empty)));
            instructions.Add(Instruction.Create(OpCodes.Brtrue_S, ldsStringEmpty));

            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Call, actualProperty));
            instructions.Add(Instruction.Create(OpCodes.Ldstr, Attribute.MatchPattern));
            instructions.Add(Instruction.Create(OpCodes.Ldstr, Attribute.ReplacePattern));

            var masker = Module.GetMethodReference(typeof(String), nameof(String.MaskSSN),
                 @params => @params.Length == 3 && @params.All(p => p.ParameterType == typeof(string)));
            instructions.Add(Instruction.Create(OpCodes.Call, masker));
            var strLoc0 = Instruction.Create(OpCodes.Stloc_0);
            instructions.Add(Instruction.Create(OpCodes.Br_S, strLoc0));

            instructions.Add(ldsStringEmpty);

            instructions.Add(strLoc0);
            var ldLoc0 = Instruction.Create(OpCodes.Ldloc_0);
            instructions.Add(Instruction.Create(OpCodes.Br_S, ldLoc0));
            instructions.Add(ldLoc0);
            instructions.Add(Instruction.Create(OpCodes.Ret));

            return getMethod;
        }

        #region old RnD methods

        private void WeaveMaskTo(PropertyDefinition property)
        {
            var getBody = property.GetMethod.Body;

            getBody.InitLocals = true;
            getBody.Variables.Add(new VariableDefinition(Module.TypeSystem.String));

            var ilProcessor = getBody.GetILProcessor();
            var returnInstruction = getBody.Instructions.Where(i => i.OpCode == OpCodes.Ret).Single();
            foreach (var instruction in GetMaskInstructions())
            {
                ilProcessor.InsertBefore(returnInstruction, instruction);
            }
        }

        private IEnumerable<Instruction> GetMaskInstructions()
        {
            var loadLoc = Instruction.Create(OpCodes.Ldloc_0);
            var mask = Module.GetMethodReference(typeof(Members), nameof(String.MaskSSN),
                  @params => @params.Length == 3 && @params.All(p => p.ParameterType == typeof(string)));
            yield return Instruction.Create(OpCodes.Call, mask);
            yield return Instruction.Create(OpCodes.Stloc_0);
            yield return Instruction.Create(OpCodes.Br_S, loadLoc);
            yield return loadLoc;
        }

        private void WeaveDebugWriteLineTo(PropertyDefinition property)
        {
            var getBody = property.GetMethod.Body;
            var ilProcessor = getBody.GetILProcessor();

            var currentInstruction = Instruction.Create(OpCodes.Nop);
            ilProcessor.InsertBefore(getBody.Instructions.First(), currentInstruction);

            foreach (var instruction in GetWriteLineInstructions())
            {
                ilProcessor.InsertAfter(currentInstruction, instruction);
                currentInstruction = instruction;
            }
        }

        private IEnumerable<Instruction> GetWriteLineInstructions()
        {
            yield return Instruction.Create(OpCodes.Ldstr, "Fody weaved this part.");
            var debugWriteLine = typeof(Debug).GetMethods()
           .Where(method => method.Name == nameof(Debug.WriteLine))
           .Single(wl =>
           {
               var parameters = wl.GetParameters();
               return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
           });
            yield return Instruction.Create(OpCodes.Call, Module.ImportReference(debugWriteLine));
        }

        #endregion old RnD methods
    }
}