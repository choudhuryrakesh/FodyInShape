using Mono.Cecil;
using System;
using System.Linq;

namespace FNF.ILWeaver.Fody
{
    using Attributes;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Processors;
    using PropertyDefPredicate = Func<PropertyDefinition, bool>;

    public class ModuleWeaver
    {
        private readonly PropertyDefPredicate _isAnyProtectorAttributes;

        public ModuleDefinition ModuleDefinition { get; set; }

        public ModuleWeaver()
        {
            _isAnyProtectorAttributes = (p) => p.CustomAttributes.Any(a => a.Is<ProtectorAttribute>());
        }

        public void Execute()
        {
            try
            {
                foreach (var @class in ModuleDefinition.GetAllClasses())
                {
                    ProtectClass(@class);
                    ProtectMembers(@class);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        private void ProtectClass(TypeDefinition @class)
        {
            if (!@class.HaveAttribute<ProtectorAttribute>()) return;

            var protectAttributes = @class.GetAttributes<ProtectorAttribute>();
            foreach (var protectAttrib in protectAttributes)
            {
                var processor = Processor.ForClass(ModuleDefinition, @class, protectAttrib.CastTo<ProtectorAttribute>());
                processor.Process();
            }
        }

        private void ProtectMembers(TypeDefinition @class)
        {
            var propertiesToMask = @class.Properties.Where(_isAnyProtectorAttributes).ToList();
            foreach (var property in propertiesToMask)
            {
                var protectAttributes = property.CustomAttributes.Where(a => a.Is<ProtectorAttribute>());
                foreach (var protectAttrib in protectAttributes)
                {
                    var processor = Processor.ForProperty(ModuleDefinition, @class, property, protectAttrib.CastTo<ProtectorAttribute>());
                    processor.Process();
                }
            }
        }
    }
}