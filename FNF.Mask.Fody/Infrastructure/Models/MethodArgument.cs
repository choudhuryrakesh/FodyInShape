using Mono.Cecil;

namespace FNF.ILWeaver.Infrastructure.Models
{
    internal class MethodArgument
    {
        public string Name { get; set; }

        public MethodAttributes Attributes { get; set; }
    }
}