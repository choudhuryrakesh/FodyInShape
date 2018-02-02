namespace FNF.ILWeaver.Infrastructure.Models
{
    public class MethodInfo
    {
        internal string ClassFullName;
        internal string MethodName;

        public MethodInfo(string classFullName, string methodName)
        {
            ClassFullName = classFullName;
            MethodName = methodName;
        }
    }
}