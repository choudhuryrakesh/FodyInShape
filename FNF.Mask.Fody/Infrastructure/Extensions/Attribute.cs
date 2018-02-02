using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Linq;

namespace FNF.ILWeaver.Infrastructure.Extensions
{
    internal static class CustomAttributeExtensions
    {
        public static bool Is<T>(this CustomAttribute attribute) where T : Attribute
        {
            return attribute.AttributeType.Namespace == typeof(T).Namespace;
        }

        public static bool TypeOf<T>(this CustomAttribute attribute) where T : Attribute
        {
            return attribute.AttributeType.FullName == typeof(T).FullName;
        }

        public static bool Have<T>(this Collection<CustomAttribute> attributes) where T : Attribute
        {
            return attributes.Any(a => a.TypeOf<T>());
        }

        public static T CastTo<T>(this CustomAttribute attribute) where T : Attribute
        {
            var attributeName = attribute.AttributeType.FullName;
            var ctorParams = attribute.ConstructorArguments.Select(a => a.Value).ToArray();
            var maskAttribute = Activator.CreateInstance(Type.GetType(attributeName), ctorParams) as T;
            var type = maskAttribute.GetType();
            foreach (var property in attribute.Properties)
            {
                var targetProperty = type.GetProperty(property.Name);
                if (targetProperty == null || targetProperty.PropertyType.FullName != property.Argument.Type.FullName)
                    continue;
                else
                    targetProperty.SetValue(maskAttribute, property.Argument.Value);
            }

            return maskAttribute;
        }
    }
}