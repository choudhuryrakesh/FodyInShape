using FNF.ILWeaver.Attributes;
using FNF.ILWeaver.Test.Infrastructure.Extensions;
using System.Security.Claims;
using System.Threading;

namespace FNF.ILWeaver.Test.Models
{
    internal class MaskDto
    {
        internal const string MatchPattern = @"\d(?!\d{0,3}$)";
        internal const string ReplacePattern = "J";
        internal const string IgnoreClass = "FNF.ILWeaver.Test.Infrastructure.SSNIgnore";

        internal const string ClaimAuthType = "mask.test";
        internal const string IsBasicClaim = "mask.test.IsBasic";
        internal const string IsEmailClaim = "mask.test.IsEmail";

        private ClaimsPrincipal claimsPrincipal => (ClaimsPrincipal)Thread.CurrentPrincipal;

        [SSNMask]
        public string Basic { get; set; }

        [SSNMask(MatchPattern, ReplacePattern)]
        public string WithPattern { get; set; }

        [EmailMask]
        public string Email { get; set; }

        [PhoneMask]
        public string Phone { get; set; }

        private bool ShouldMaskBasic()
        {
            return claimsPrincipal?.GetClaim(IsBasicClaim) != true.ToString();
        }

        public bool ShouldMaskEmail()
        {
            return claimsPrincipal?.GetClaim(IsEmailClaim) != true.ToString();
        }
    }
}