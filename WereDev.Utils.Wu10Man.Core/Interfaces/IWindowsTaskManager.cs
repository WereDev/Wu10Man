using WereDev.Utils.Wu10Man.Core.Models;

namespace WereDev.Utils.Wu10Man.Core.Interfaces
{
    public interface IWindowsTaskManager
    {
        WindowsTask[] GetTasks();

        WindowsTask GetTask(string path);

        WindowsTask[] DisableTasks();

        WindowsTask DisableTask(string path);

        WindowsTask[] EnableTasks();

        WindowsTask EnableTask(string path);
    }
}
