using System;

namespace WereDev.Utils.Wu10Man.Core.Interfaces
{
    public interface ILogWriter
    {
        void LogError(Exception ex);

        void LogError(string message);

        void LogInfo(string message);
    }
}
