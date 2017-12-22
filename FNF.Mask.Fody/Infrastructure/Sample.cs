using FNF.Mask.Fody.Infrastructure.Extensions;
using FNF.Mask.Fody.Maskers;
using System.Runtime.Serialization;

namespace FNF.Mask.Fody.Infrastructure
{
    /// <summary>
    /// This class is soley used to see how the compiled code is using IL viewer (ilDasm)
    /// </summary>
    [DataContract]
    public class Sample
    {
        public string SSN { get; set; }

        [DataMember(Name = "SSN")]
        public string SSNMask
        {
            get
            {

                return string.IsNullOrWhiteSpace(SSN)
                    ? string.Empty : SSN.MaskSSN(@"^\d{3}-\d{2}-\d{4}$", "XXX-XX-$1");
            }
        }

        public string Name { get; set; }

        public bool ShouldSerializeSSN()
        {
            return false;
        }
    }
}