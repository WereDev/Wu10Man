using System;

namespace WereDev.Utils.Wu10Man
{
    static class Logger
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void LogError(Exception ex)
        {
            var exception = ex;
            while (exception != null)
            {
                LogError(string.Format("{0} \r\n {1}", ex.Message, ex.StackTrace.Replace("\r\n", "\r\n\t")));
                exception = exception.InnerException;
            }
        }

        public static void LogError(string message)
        {
            logger.Error(message);
        }

        public static void LogInfo(string message)
        {
            logger.Info(message ?? string.Empty);
        }
    }
}
