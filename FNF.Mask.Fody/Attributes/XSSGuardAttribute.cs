using System;

namespace FNF.ILWeaver.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class XSSGuardAttribute : ProtectorAttribute
    {
    }
}