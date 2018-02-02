using FNF.ILWeaver.Infrastructure.Models;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using Reflection = System.Reflection;

namespace FNF.ILWeaver.Infrastructure.Extensions
{
    using ParameterPredicate = Func<Reflection.ParameterInfo[], bool>;

    internal static class Module
    {
        public static CustomAttribute CreateCustomAttribute<T>(this ModuleDefinition module, IEnumerable<AttributeArgument> namedArgs = null)
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
            string methodName, ParameterPredicate singlePredicate = null)
        {
            return module.GetMethodReference(typeof(TFrom), methodName, singlePredicate);
        }

        public static MethodReference GetMethodReference(this ModuleDefinition module, Type from,
            string methodName, ParameterPredicate singlePredicate = null)
        {
            var method = from.GetMethods()
              .Where(m => m.Name == methodName)
              .Single(m => singlePredicate == null || singlePredicate(m.GetParameters()));

            return module.ImportReference(method);
        }

        public static MethodReference GetMethodReference(this ModuleDefinition module, MethodInfo methodInfo)
        {
            return module.GetType(methodInfo.ClassFullName)
                .Methods
                .Single(m => !m.IsConstructor && m.Name == methodInfo.MethodName && m.Parameters.Count == 0);
        }

        public static MethodReference GetCtorReference(this ModuleDefinition module, MethodInfo methodInfo)
        {
            return module.GetType(methodInfo.ClassFullName)
                .Methods
                .Single(m => m.IsConstructor && m.Parameters.Count == 0);
        }

        public static MethodReference GetCtorReference<TFrom>(this ModuleDefinition module,
          ParameterPredicate singlePredicate = null)
        {
            var method = typeof(TFrom).GetConstructors()
              .Single(m => singlePredicate == null || singlePredicate(m.GetParameters()));

            return module.ImportReference(method);
        }

        public static FieldReference GetFieldReference<TFrom>(this ModuleDefinition module, string fieldName)
        {
            var field = typeof(TFrom).GetFields()
                .Where(f => f.Name == fieldName).Single();

            return module.ImportReference(field);
        }
    }
}