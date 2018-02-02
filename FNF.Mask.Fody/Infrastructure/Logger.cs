using System.Diagnostics;

namespace FNF.ILWeaver.Infrastructure
{
    internal class Logger : ILogger
    {
        private static ILogger _logger;

        private Logger()
        {
        }

        public static ILogger CurrentLogger { get; } = _logger ?? (_logger = new Logger());

        public void DebugInfo(string message)
        {
            Debug.WriteLine(message);
        }
    }
}