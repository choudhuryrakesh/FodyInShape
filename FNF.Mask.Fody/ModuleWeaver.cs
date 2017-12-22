using FNF.Mask.Fody.Attributes;
using FNF.Mask.Fody.Infrastructure.Extensions;
using FNF.Mask.Fody.Maskers;
using Mono.Cecil;
using System;
using System.Diagnostics;
using System.Linq;

namespace FNF.Mask.Fody
{
    public class ModuleWeaver
    {
        private readonly Func<PropertyDefinition, bool> _isAnyMaskAttributes;
        private readonly Func<CustomAttribute, bool> _isMaskerAttribute;

        public ModuleDefinition ModuleDefinition { get; set; }

        public ModuleWeaver()
        {
            _isMaskerAttribute = a => a.AttributeType.Namespace == typeof(MaskAttribute).Namespace; // ToDo:
            _isAnyMaskAttributes = (p) => p.CustomAttributes.Any(_isMaskerAttribute);
        }

        public void Execute()
        {
            try
            {
                foreach (var @class in ModuleDefinition.Types.Where(t => t.IsClass))
                {
                    var propertiesToMask = @class.Properties.Where(_isAnyMaskAttributes).ToList();
                    foreach (var property in propertiesToMask)
                    {
                        var maskAttributes = property.CustomAttributes.Where(_isMaskerAttribute);
                        foreach (var customAttribute in maskAttributes)
                        {
                            var masker = Masker.For(module: ModuleDefinition, attribute: customAttribute.CastTo<MaskAttribute>());
                            masker.Process(property: property, inClass: @class);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FNF.Mask.Fody: Exception occured {Environment.NewLine} {ex.Message}");
                //ex.ToFriendlyString();
                //throw new WeavingException();
                throw ex;
            }
        }
    }
}