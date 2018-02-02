using FNF.ILWeaver.Attributes;

namespace FNF.ILWeaver.Test.Models
{
    [XSSGuard]
    internal class XSSDto
    {
        private string _backingField;
        private string _anotherBackingField;
        internal string MaliciousField;

        public string Property { get; set; }

        public string PropertyWithBackingProperty
        {
            get
            {
                return _backingField;
            }
            set { _backingField = value; }
        }

        public string PropertyWithSomeCodeInSetter
        {
            get
            {
                return _anotherBackingField;
            }
            set
            {
                DoSomeMaliciousActivity(value);
                _anotherBackingField = value;
            }
        }

        public string ComputedField
        {
            get { return $"{Property} {PropertyWithBackingProperty}"; }
        }

        public string FieldIsNotPossible;

        private void DoSomeMaliciousActivity(string value)
        {
            MaliciousField = value;
        }
    }
}