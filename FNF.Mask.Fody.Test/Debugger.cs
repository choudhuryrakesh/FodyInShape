using FNF.ILWeaver.Fody;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;

namespace FNF.ILWeaver.Test
{
    //[TestClass]
    public class Debugger
    {
        [TestMethod]
        public void UsedOnlyForDebugging()
        {
            var moduleWeaver = new ModuleWeaver();
            moduleWeaver.ModuleDefinition = ModuleDefinition
                .ReadModule(this.GetType().Module.FullyQualifiedName);
            moduleWeaver.Execute();
        }
    }
}