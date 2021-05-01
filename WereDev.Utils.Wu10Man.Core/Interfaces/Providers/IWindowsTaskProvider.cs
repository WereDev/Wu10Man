using WereDev.Utils.Wu10Man.Core.Models;

namespace WereDev.Utils.Wu10Man.Core.Interfaces.Providers
{
    public interface IWindowsTaskProvider
    {
        WindowsTask GetTask(string fullPath);

        void EnableTask(string fullPath);

        void DisableTask(string fullPath);
    }
}
