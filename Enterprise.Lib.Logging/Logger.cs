using Enterprise.Lib.Core.Helper;
using NLog;
using System;
using System.Runtime.CompilerServices;

namespace Enterprise.Lib.Logging
{
    public class OrgLogger : Enterprise.Lib.Core.Interface.ILogger
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public string ApplicationName { get; set; }

        public Logger DefaultLogger 
        {
            get
            {
                return logger;
            }
        }

        public OrgLogger()
        {
            if (string.IsNullOrEmpty(ApplicationName))
            {
                ApplicationName = string.Empty;
            }
        }

        public virtual void Debug(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var defaultMessageHeader = GetDefaultMessageHeader(callerName, callerLineNumber);
            logger.Debug($"{defaultMessageHeader}|{message}");
        }

        public virtual void Error(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var defaultMessageHeader = GetDefaultMessageHeader(callerName, callerLineNumber);
            logger.Error($"{defaultMessageHeader}|{message}");
        }

        public virtual void ErrorException(string message, Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var defaultMessageHeader = GetDefaultMessageHeader(callerName, callerLineNumber);
            //logger.Error($"{defaultMessageHeader} >> {message}", exception);
            logger.Error(exception, $"{defaultMessageHeader}|{message}");
        }

        public virtual void Fatal(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var defaultMessageHeader = GetDefaultMessageHeader(callerName, callerLineNumber);
            logger.Fatal($"{defaultMessageHeader}|{message}");
        }

        public virtual void Fatal(string message, Exception exception, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var defaultMessageHeader = GetDefaultMessageHeader(callerName, callerLineNumber);
            //logger.Fatal($"{defaultMessageHeader} >> {message}", exception);
            logger.Fatal(exception, $"{defaultMessageHeader}|{message}");
        }

        public virtual void Info(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var defaultMessageHeader = GetDefaultMessageHeader(callerName, callerLineNumber);
            logger.Info($"{defaultMessageHeader}|{message}");
        }

        public virtual void Trace(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var defaultMessageHeader = GetDefaultMessageHeader(callerName, callerLineNumber);
            logger.Trace($"{defaultMessageHeader}|{message}");
        }

        public virtual void Warn(string message, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            var defaultMessageHeader = GetDefaultMessageHeader(callerName, callerLineNumber);
            logger.Warn($"{defaultMessageHeader}|{message}");
        }

        private string GetDefaultMessageHeader(string callerName = "", int callerLineNumber = 0)
        {

            var ipAddress = HttpHelper.GetRequestIp() ?? string.Empty;
            var hostName = HttpHelper.GetHostname() ?? string.Empty;

            return $"IP:{ipAddress}|Host:{hostName}|AppName:{ApplicationName}|{callerName}|{callerLineNumber}";
        }
    }
}