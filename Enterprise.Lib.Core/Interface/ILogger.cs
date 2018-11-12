using System;
using System.Runtime.CompilerServices;

namespace Enterprise.Lib.Core.Interface
{
    public interface ILogger
    {
        void Debug(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0);
        void Trace(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0);
        void Info(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0);
        void Warn(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0);
        void Error(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0);
        void ErrorException(string message, Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0);
        void Fatal(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0);
        void Fatal(string message, Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0);
    }
}