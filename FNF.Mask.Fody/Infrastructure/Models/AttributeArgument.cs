using Mono.Cecil;

namespace FNF.ILWeaver.Infrastructure.Models
{
    internal class AttributeArgument
    {
        public string Name { get; }
        public TypeReference Type { get; }

        public object Value { get; }

        public AttributeArgument(string name, TypeReference type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}