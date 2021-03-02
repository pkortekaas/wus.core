using System;
using System.Runtime.CompilerServices;

namespace CoreWUS
{
    public interface ILogger
    {
        void Log(LogLevel level, string message, Exception ex = null,
                [CallerMemberName] string memberName = null,
                [CallerFilePath] string sourceFilePath = null);
    }
}