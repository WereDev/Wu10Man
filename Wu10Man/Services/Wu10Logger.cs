using NLog;
using NLog.Targets;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using WereDev.Utils.Wu10Man.Core.Interfaces;

namespace WereDev.Utils.Wu10Man.Services
{
    internal class Wu10Logger : ILogWriter
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "nlog", Justification="nlog.config is the expected file name.")]
        public string LogFolder
        {
            get
            {
                var targets = LogManager.Configuration.AllTargets;
                foreach (var target in targets)
                {
                    if (target is FileTarget fileTarget)
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
                LogError($"{exception.GetType().ToString()}: {exception.Message}\r\n{exception.StackTrace}");
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
