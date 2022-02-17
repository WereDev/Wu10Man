using System;
using System.Collections.Generic;
using System.Linq;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;
using WereDev.Utils.Wu10Man.Core.Models;

namespace WereDev.Utils.Wu10Man.Core.Services
{
    public class WindowsTaskManager : IWindowsTaskManager
    {
        private readonly IWindowsTaskProvider _taskProvider;
        private readonly WindowsTaskConfig[] _windowsTaskConfigs;

        public WindowsTaskManager(IWindowsTaskProvider taskProvider, WindowsTaskConfig[] windowsTaskConfigs)
        {
            _taskProvider = taskProvider ?? throw new ArgumentNullException(nameof(taskProvider));
            _windowsTaskConfigs = windowsTaskConfigs ?? new WindowsTaskConfig[0];
            var distinct = _windowsTaskConfigs.GroupBy(x => x.TaskPath)
                                              .Select(x => x.First())
                                              .ToArray();
            _windowsTaskConfigs = distinct;
        }

        public WindowsTask[] DisableTasks()
        {
            foreach (var config in _windowsTaskConfigs)
            {
                _taskProvider.DisableTask(config.TaskPath);
            }

            return GetTasks();
        }

        public WindowsTask DisableTask(string path)
        {
            _taskProvider.DisableTask(path);
            return _taskProvider.GetTask(path);
        }

        public WindowsTask[] EnableTasks()
        {
            foreach (var config in _windowsTaskConfigs)
            {
                _taskProvider.EnableTask(config.TaskPath);
            }

            return GetTasks();
        }

        public WindowsTask EnableTask(string path)
        {
            _taskProvider.EnableTask(path);
            return _taskProvider.GetTask(path);
        }

        public WindowsTask[] GetTasks()
        {
            var tasks = new List<WindowsTask>();
            foreach (var config in _windowsTaskConfigs)
            {
                var task = _taskProvider.GetTask(config.TaskPath);
                if (task == null)
                    continue;
                var pathSplit = task.FullPath.Split('\\');
                task.Name = pathSplit[pathSplit.Length - 2] + " - " + task.Name;
                tasks.Add(task);
            }

            return tasks.ToArray();
        }

        public WindowsTask GetTask(string path)
        {
            var task = _taskProvider.GetTask(path);
            var taskFromCollection = _windowsTaskConfigs.FirstOrDefault(x => x.TaskPath.Equals(path, StringComparison.OrdinalIgnoreCase));
            task.Name = taskFromCollection?.TaskName ?? task.Name;
            return task;
        }
    }
}
