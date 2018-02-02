using FNF.ILWeaver.Infrastructure.Extensions;
using FNF.ILWeaver.Test.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FNF.ILWeaver.Test
{
    [TestClass]
    public class XSSProtectTest
    {
        private const string scriptElement = @"<script source=""http://wwww.MaliciousWareHouse.com/harmfullScript.js""/>";
        private const string linkElement = @"<a href=javascript:alert('Your hacked')";
        private XSSDto _dtoGuardedAtClass;
        private XssPropertyLevel _dtoGuardedAtProperty;
        private XssInheritedDto _dtoInherited;

        [TestInitialize]
        public void Initialize()
        {
            _dtoGuardedAtClass = new XSSDto
            {
                Property = scriptElement,
                PropertyWithBackingProperty = linkElement,
                PropertyWithSomeCodeInSetter = linkElement,
                FieldIsNotPossible = linkElement,
            };

            _dtoGuardedAtProperty = new XssPropertyLevel
            {
                NonXssProperty = scriptElement,
                XssProperty = scriptElement,
            };

            _dtoInherited = new XssInheritedDto
            {
                Property = scriptElement,
                PropertyWithBackingProperty = linkElement,
                PropertyWithSomeCodeInSetter = linkElement,
                FieldIsNotPossible = linkElement,
                ChildProperty = linkElement,
            };
        }

        [TestMethod]
        public void ClassLevelShouldEncodeAllTheProperties()
        {
            Assert.AreEqual(scriptElement.EncodeHtml(), _dtoGuardedAtClass.Property,
                $"{nameof(_dtoGuardedAtClass.Property)} is not encoded.");

            Assert.AreEqual(linkElement.EncodeHtml(), _dtoGuardedAtClass.PropertyWithBackingProperty,
                $"{nameof(_dtoGuardedAtClass.PropertyWithBackingProperty)} is not encoded.");

            Assert.IsTrue(_dtoGuardedAtClass.ComputedField.EncodeHtml() != _dtoGuardedAtClass.ComputedField,
               $"{nameof(_dtoGuardedAtClass.ComputedField)} is not encoded.");

            Assert.AreEqual(linkElement.EncodeHtml(), _dtoGuardedAtClass.PropertyWithSomeCodeInSetter,
                $"{nameof(_dtoGuardedAtClass.PropertyWithSomeCodeInSetter)} is not encoded.");

            Assert.AreEqual(linkElement.EncodeHtml(), _dtoGuardedAtClass.MaliciousField,
                $"{nameof(_dtoGuardedAtClass.MaliciousField)} is not encoded.");

            Assert.IsTrue(true, $"{nameof(_dtoGuardedAtClass.FieldIsNotPossible)} is not possible to be weaved.");
        }

        [TestMethod]
        public void ClassLevelShouldEncodeInheritedProperties()
        {
            Assert.AreEqual(scriptElement.EncodeHtml(), _dtoInherited.Property,
              $"{nameof(_dtoInherited.Property)} is not encoded.");

            Assert.AreEqual(linkElement.EncodeHtml(), _dtoInherited.PropertyWithBackingProperty,
                $"{nameof(_dtoInherited.PropertyWithBackingProperty)} is not encoded.");

            Assert.IsTrue(_dtoInherited.ComputedField.EncodeHtml() != _dtoInherited.ComputedField,
               $"{nameof(_dtoInherited.ComputedField)} is not encoded.");

            Assert.AreEqual(linkElement.EncodeHtml(), _dtoInherited.PropertyWithSomeCodeInSetter,
                $"{nameof(_dtoInherited.PropertyWithSomeCodeInSetter)} is not encoded.");

            Assert.AreEqual(linkElement.EncodeHtml(), _dtoInherited.MaliciousField,
                $"{nameof(_dtoInherited.MaliciousField)} is not encoded.");

            Assert.AreEqual(linkElement.EncodeHtml(), _dtoInherited.ChildProperty,
             $"{nameof(_dtoInherited.ChildProperty)}, is not encoded.");
        }

        [TestMethod]
        public void PropertyLevelShouldEncodeOnlyThoseProperties()
        {
            Assert.AreEqual(scriptElement.EncodeHtml(), _dtoGuardedAtProperty.NonXssProperty,
                $"{nameof(_dtoGuardedAtProperty.NonXssProperty)} is not encoded.");

            Assert.AreEqual(scriptElement, _dtoGuardedAtProperty.XssProperty,
                $"{nameof(_dtoGuardedAtProperty.XssProperty)} is encoded.");
        }
    }
}