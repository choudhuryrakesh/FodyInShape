using FNF.Mask.Fody.Attributes;
using Newtonsoft.Json;

namespace WeavableAssembly.DTO
{
    public class Person
    {
        [MaskSSN]
        public string SSN { get; set; }

        [MaskSSN(MatchPattern: @"^\d{3}#\d{2}#\d{4}$]", ReplacePattern: @"###-##-$1")]
        public string SSNCustom { get; set; }


        [MaskSSN(ReplacePattern: @"###-##-$1", MatchPattern: @"^\d{3}#\d{2}#\d{4}$]")]
        public string SSNWithDifferentlyOrderedCtor { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Student : Person
    {
    }

    public class Furniture
    {
        [MaskSSN(MatchPattern: @"^\d{3}#\d{2}#\d{4}$]", ReplacePattern: @"###-##-$1")]
        public string Id { get; set; }
    }
}