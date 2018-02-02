using FNF.ILWeaver.Attributes;
using FNF.ILWeaver.Infrastructure.Extensions;
using FNF.ILWeaver.Test.Infrastructure.Extensions;
using FNF.ILWeaver.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace FNF.ILWeaver.Test
{
    [TestClass]
    public class MaskerTest
    {
        private readonly string _maskPrefix = "Mask";
        private MaskDto _testDto;
        private MaskDtoWithSerializationMethod _testDtoForMethods;
        private MaskInheritedDto _testInheritedDto;

        private const string forBasic = "123-123-1234";
        private const string forWithPattern = "654-654-0987";
        private const string email = "tester@unitTests.com";
        private const string phoneNo = "091-900-4567";

        [TestInitialize]
        public void Initialize()
        {
            _testDto = new MaskDto
            {
                Basic = forBasic,
                WithPattern = forWithPattern,
                Email = email,
                Phone = phoneNo,
            };

            _testDtoForMethods = new MaskDtoWithSerializationMethod
            {
                Basic = forBasic,
                WithPattern = forWithPattern,
                AnotherProp = "The assigned value doesnt matter",
            };

            _testInheritedDto = new MaskInheritedDto
            {
                Basic = forBasic,
                WithPattern = forWithPattern,
                ChildPropertyMasked = forBasic,
            };
        }

        private string MaskSSN(string value, bool isDefault = false)
        {
            return isDefault
                ? value.Mask(SSNMaskAttribute.MaskPattern.Match, SSNMaskAttribute.MaskPattern.Replace)
                : value.Mask(MaskDto.MatchPattern, MaskDto.ReplacePattern);
        }

        [TestMethod]
        public void ShouldNotHaveNewPropertiesWhenJsonSerialized()
        {
            var jObject = JObject.Parse(JsonConvert.SerializeObject(_testDto));
            var maskProperties = jObject.SelectTokens("*")
                .Where(t => t.Path.EndsWith(_maskPrefix));

            Assert.IsTrue(maskProperties.Count() == 0);
        }

        [TestMethod]
        public void ShouldHaveMaskedValuesWhenJsonSerialized()
        {
            var jObject = JObject.Parse(JsonConvert.SerializeObject(_testDto));

            Assert.IsTrue(jObject[nameof(_testDto.Basic)].ToString() == MaskSSN(_testDto.Basic, true));
            Assert.IsTrue(jObject[nameof(_testDto.WithPattern)].ToString() == MaskSSN(_testDto.WithPattern));
            Assert.IsTrue(jObject[nameof(_testDto.Email)].ToString() == _testDto.Email
                .Mask(EmailMaskAttribute.MaskPattern.Match, EmailMaskAttribute.MaskPattern.Replace));
            Assert.IsTrue(jObject[nameof(_testDto.Phone)].ToString() == _testDto.Phone
                .Mask(PhoneMaskAttribute.MaskPattern.Match, PhoneMaskAttribute.MaskPattern.Replace));
        }

        [TestMethod]
        public void ShouldMaskInheritedClass()
        {
            var jObject = JObject.Parse(JsonConvert.SerializeObject(_testInheritedDto));

            Assert.IsTrue(jObject[nameof(_testInheritedDto.Basic)].ToString() == MaskSSN(_testInheritedDto.Basic, true));
            Assert.IsTrue(jObject[nameof(_testInheritedDto.WithPattern)].ToString() == MaskSSN(_testInheritedDto.WithPattern));
            Assert.IsTrue(jObject[nameof(_testInheritedDto.ChildPropertyMasked)].ToString() == MaskSSN(_testInheritedDto.ChildPropertyMasked, true));
        }

        [TestMethod]
        public void ShouldNotOverwriteSerializationMethodWhenJsonSerialized()
        {
            var jObject = JObject.Parse(JsonConvert.SerializeObject(_testDtoForMethods));
            Assert.IsTrue(jObject[nameof(_testDtoForMethods.AnotherProp)].ToString() == MaskDtoWithSerializationMethod.ValueSetAtSerialization);
        }

        [TestMethod]
        public void ShouldNotMaskValuesWhenJsonDeSerialized()
        {
            var jObject = JObject.Parse(JsonConvert.SerializeObject(_testDto));
            var newValue = "999-999-9999";
            jObject[nameof(_testDto.Basic)] = newValue;
            jObject[nameof(_testDto.WithPattern)] = newValue;

            var dto = JsonConvert.DeserializeObject<MaskDto>(jObject.ToString());

            Assert.IsTrue(dto.Basic == newValue);
            Assert.IsTrue(dto.WithPattern == newValue);
        }

        [TestMethod]
        public void ShouldHonourMaskMethods()
        {
            var claimsPrincipal = (ClaimsPrincipal)Thread.CurrentPrincipal;

            claimsPrincipal.AddOrUpdateClaim(MaskDto.IsBasicClaim, true.ToString());
            claimsPrincipal.AddOrUpdateClaim(MaskDto.IsEmailClaim, true.ToString());
            var jObject = JObject.Parse(JsonConvert.SerializeObject(_testDto));
            Assert.IsTrue(jObject[nameof(_testDto.Basic)].ToString() == forBasic, "Initial ALL mode - email");
            Assert.IsTrue(jObject[nameof(_testDto.Email)].ToString() == email, "Initial ALL mode - email");

            Initialize();
            claimsPrincipal.AddOrUpdateClaim(MaskDto.IsBasicClaim, false.ToString());
            jObject = JObject.Parse(JsonConvert.SerializeObject(_testDto));
            Assert.IsTrue(jObject[nameof(_testDto.Basic)].ToString() == MaskSSN(forBasic, true), "Email only mode - basic");
            Assert.IsTrue(jObject[nameof(_testDto.Email)].ToString() == email, "Email only mode - email");

            Initialize();
            claimsPrincipal.AddOrUpdateClaim(MaskDto.IsEmailClaim, false.ToString());
            jObject = JObject.Parse(JsonConvert.SerializeObject(_testDto));
            Assert.IsTrue(jObject[nameof(_testDto.Basic)].ToString() == MaskSSN(forBasic, true), "None mode - basic");
            Assert.IsTrue(jObject[nameof(_testDto.Email)].ToString()
                == email.Mask(EmailMaskAttribute.MaskPattern.Match, EmailMaskAttribute.MaskPattern.Replace), "None mode - email");

            Initialize();
            claimsPrincipal.AddOrUpdateClaim(MaskDto.IsBasicClaim, true.ToString());
            claimsPrincipal.AddOrUpdateClaim(MaskDto.IsEmailClaim, true.ToString());
            jObject = JObject.Parse(JsonConvert.SerializeObject(_testDto));
            Assert.IsTrue(jObject[nameof(_testDto.Basic)].ToString() == forBasic, "Reset to All mode - basic");
            Assert.IsTrue(jObject[nameof(_testDto.Email)].ToString() == email, "Reset to All mode - email");
        }
    }
}