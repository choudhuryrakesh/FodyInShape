using FNF.ILWeaver.Infrastructure.Extensions;
using System.Runtime.Serialization;

namespace FNF.ILWeaver.Test.Samples
{
    internal class Dto
    {
        private string _backingField;
        private string _anotherBackingFiled;

        public Dto()
        {
            Field = "over riden fvalue";
        }

        public string Property { get; set; }

        public string PropertyWithBackingField
        {
            get
            {
                return _backingField;
            }
            set
            {
                _backingField = value;
            }
        }

        public string PropertyWithModifiedBackingField
        {
            get
            {
                return _anotherBackingFiled;
            }
            set
            {
                value = value.EncodeHtml();
                value = value + "1";
                _anotherBackingFiled = value;
            }
        }

        public string ComputedProperty
        {
            get { return $"{Property} {PropertyWithBackingField}"; }
        }

        public string Field = "a field";

        public string SSN { get; set; }

        public string SSNMask
        {
            get
            {
                if (string.IsNullOrEmpty(SSN)) return string.Empty;
                return SSN.Mask(@"^\d{3}-\d{2}-\d{4}$", "XXX-XX-$1");
            }
        }

        public string PrivateMethod
        {
            get
            {
                if (string.IsNullOrEmpty(SSN)) return string.Empty;
                var shouldMask = ShouldMaskPrivate();
                if (!shouldMask)
                    return SSN;
                else
                    return SSN.Mask(@"^\d{3}-\d{2}-\d{4}$", "XXX-XX-$1");
            }
        }

        private bool ShouldMaskPrivate()
        {
            return true;
        }

        public string PublicMethod
        {
            get
            {
                if (string.IsNullOrEmpty(SSN)) return string.Empty;
                var ignore = ShouldIgnorePublic();
                if (ignore)
                    return SSN;
                else
                    return SSN.Mask(@"^\d{3}-\d{2}-\d{4}$", "XXX-XX-$1");
            }
        }

        public bool ShouldIgnorePublic()
        {
            return false;
        }

        public bool ShouldSerializeSSN()
        {
            return false;
        }

        public bool ShouldSerializeName()
        {
            return true;
        }

        [OnSerializing]
        public void OnSerializing(StreamingContext context)
        {
            SSN = Property;
            PropertyWithBackingField = ComputedProperty;
        }
    }
}