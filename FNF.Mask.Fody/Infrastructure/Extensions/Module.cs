using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Reflection = System.Reflection;

namespace FNF.Mask.Fody.Infrastructure.Extensions
{
    internal static class Module
    {
        public static CustomAttribute CreateCustomAttribute<T>(this ModuleDefinition module, IEnumerable<NamedArg> namedArgs = null)
        {
            var attributeCtor = module.ImportReference(typeof(T).GetConstructor(Type.EmptyTypes));
            var customAttribute = new CustomAttribute(attributeCtor);

            if (namedArgs != null)
                foreach (var arg in namedArgs)
                {
                    var aliasArgument = new CustomAttributeArgument(arg.Type, arg.Value);
                    customAttribute.Properties.Add(new CustomAttributeNamedArgument(arg.Name, aliasArgument));
                }

            return customAttribute;
        }

        public static MethodReference GetMethodReference<TFrom>(this ModuleDefinition module,
            string methodName, Func<Reflection.ParameterInfo[], bool> singlePredicate)
        {
            return module.GetMethodReference(typeof(TFrom), methodName, singlePredicate);
        }

        public static MethodReference GetMethodReference(this ModuleDefinition module, Type from,
            string methodName, Func<Reflection.ParameterInfo[], bool> singlePredicate)
        {
            var method = from.GetMethods()
              .Where(m => m.Name == methodName)
              .Single(m => singlePredicate(m.GetParameters()));

            return module.ImportReference(method);
        }

        public static FieldReference GetFieldReference<TFrom>(this ModuleDefinition module, string fieldName)
        {
            var field = typeof(TFrom).GetFields()
                .Where(f => f.Name == fieldName).Single();

            return module.ImportReference(field);
        }
    }

    internal class NamedArg
    {
        public string Name { get; }
        public TypeReference Type { get; }

        public object Value { get; }

        public NamedArg(string name, TypeReference type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}