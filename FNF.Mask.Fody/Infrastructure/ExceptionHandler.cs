using Fody;
using System;

namespace FNF.ILWeaver.Infrastructure
{
    internal class ExceptionHandler
    {
        public static void Handle(Exception ex)
        {
            var message = ex.ToFriendlyString();
            Logger.CurrentLogger.DebugInfo(message);
            throw new WeavingException(ex.ToFriendlyString());
        }
    }
}