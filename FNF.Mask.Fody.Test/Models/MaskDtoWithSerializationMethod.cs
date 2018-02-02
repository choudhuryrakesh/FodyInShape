using System.Runtime.Serialization;

namespace FNF.ILWeaver.Test.Models
{
    internal class MaskDtoWithSerializationMethod : MaskDto
    {
        internal const string ValueSetAtSerialization = "Set during run";
        public string AnotherProp { get; set; }

        [OnSerializing]
        public void OnSerializing(StreamingContext context)
        {
            AnotherProp = ValueSetAtSerialization;
        }
    }
}