using System;
using System.Linq;
using System.Windows;
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;
using WereDev.Utils.Wu10Man.UserControls.Models;
using WPFSpark;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for WindowsTasksControl.xaml.
    /// </summary>
    public partial class WindowsTasksControl : UserControlBaseWithWorker<WindowsTasksModel>
    {
        private readonly IWindowsTaskManager _windowsTaskManager;

        public WindowsTasksControl()
            : base()
        {
            _windowsTaskManager = DependencyManager.WindowsTaskManager;
            TabTitle = "Windows Scheduled Tasks";
            InitializeComponent();
        }

        protected override bool SetRuntimeOptions()
        {
            BuildTaskStatus();
            return true;
        }

        private void BuildTaskStatus()
        {
            Model.Tasks = _windowsTaskManager.GetTasks()
                                             .Select(x => new WindowsTasksStatusModel(x))
                                             .ToArray();
        }

        private void ToggleTask(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            var data = (WindowsTasksStatusModel)toggle.DataContext;
            if (toggle.IsChecked.Value)
            {
                EnableTask(data.TaskPath, data.DisplayName);
                var message = $"{data.DisplayName} has been ENABLED";
                ShowInfoMessage(message);
            }
            else
            {
                DisableTask(data.TaskPath, data.DisplayName);
                ShowInfoMessage($"{data.DisplayName} has been DISABLED");
            }
        }

        private void UpdateTasks(object sender, RoutedEventArgs e)
        {
            var count = Model.Tasks.Where(x => x.TaskExists).Count();
            RunBackgroundProcess(count, TaggleTasks);
        }

        private void EnableTask(string taskPath, string displayName)
        {
            _windowsTaskManager.EnableTask(taskPath);
            SetTaskStatus(taskPath);
            LogWriter.LogInfo($"Scheduled Task ENABLED: {taskPath} - {displayName}");
        }

        private void DisableTask(string taskpath, string displayName)
        {
            _windowsTaskManager.DisableTask(taskpath);
            SetTaskStatus(taskpath);
            LogWriter.LogInfo($"Scheduled Task DISABLED: {taskpath} - {displayName}");
        }

        private void TaggleTasks(object sender, EventArgs e)
        {
            var allTasksDisabled = Model.AllTasksDisabled;
            var tasks = Model.Tasks.Where(x => x.TaskExists).ToArray();
            foreach (var task in tasks)
            {
                if (allTasksDisabled)
                {
                    EnableTask(task.TaskPath, task.DisplayName);
                }
                else if (task.IsTaskEnabled)
                {
                    DisableTask(task.TaskPath, task.DisplayName);
                }

                AdvanceProgressBar();
            }

            BuildTaskStatus();
            ShowInfoMessage("Windows Scheduled Tasks updated.");
        }

        private void SetTaskStatus(string taskPath)
        {
            var taskModel = Model.Tasks.Single(x => x.TaskPath == taskPath);
            var task = _windowsTaskManager.GetTask(taskPath);
            taskModel.TaskExists = task != null;
            if (taskModel.TaskExists)
            {
                taskModel.IsTaskEnabled = task.Enabled;
            }
        }
    }
}
