using NLog;
using NLog.Targets;
using System;
using System.IO;

namespace WereDev.Utils.Wu10Man
{
    class Logger
    {
        private NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public string LogFolder { get
            {
                var targets = LogManager.Configuration.AllTargets;
                foreach (var target in targets)
                {
                    var fileTarget = target as FileTarget;
                    if (fileTarget != null)
                    {
                        var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                        var fileName = fileTarget.FileName.Render(logEventInfo);
                        var folder = Path.GetDirectoryName(fileName);
                        return folder;
                    }
                }
                throw new InvalidOperationException("No file logging has been configured in nlog.config");
            }
        }

        public void LogError(Exception ex)
        {
            var exception = ex;
            while (exception != null)
            {
                LogError(string.Format("{0} \r\n {1}", ex.Message, ex.StackTrace.Replace("\r\n", "\r\n\t")));
                exception = exception.InnerException;
            }
        }

        public void LogError(string message)
        {
            _logger.Error(message);
        }

        public void LogInfo(string message)
        {
            _logger.Info(message ?? string.Empty);
        }
    }
}
