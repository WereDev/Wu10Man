using Microsoft.Win32.TaskScheduler;
using System;
using System.Security.Principal;
using System.Threading;
using WereDev.Utils.Wu10Man.Core.Interfaces.Providers;
using WereDev.Utils.Wu10Man.Core.Models;

namespace WereDev.Utils.Wu10Man.Providers
{
    public class WindowsTaskProvider : IWindowsTaskProvider
    {
        private readonly string _wu10TaskPath = "Wu10Man Admin Task";

        public WindowsTaskProvider()
        {
        }

        public void DisableTask(string fullPath)
        {
            RunAdminTask("Disable-ScheduledTask", fullPath);
        }

        public void EnableTask(string fullPath)
        {
            RunAdminTask("Enable-ScheduledTask", fullPath);
        }

        public WindowsTask GetTask(string fullPath)
        {
            using (var task = TaskService.Instance.GetTask(fullPath))
            {
                if (task == null)
                    return null;

                return new WindowsTask
                {
                    Enabled = task.Enabled,
                    FullPath = fullPath,
                    Name = task.Name,
                };
            }
        }

        private void RunAdminTask(string toExecute, string fullPath)
        {
            CleanupAdminTask();

            using (var task = CreateHelperTask(toExecute, fullPath))
            {
                task.Run();
            }

            WaitForAdminTaskCompletion();

            CleanupAdminTask();
        }

        private Task CreateHelperTask(string toExecute, string fullPath)
        {
            var sid = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, WindowsIdentity.GetCurrent().User.AccountDomainSid);
            var account = sid.Translate(typeof(NTAccount));
            var username = account.Value;

            return TaskService.Instance.AddTask(
                    _wu10TaskPath,
                    new TimeTrigger() { StartBoundary = DateTime.Now, Enabled = false },
                    new ExecAction(
                        @"powershell",
                        $" -command \"& {{ {toExecute}  -TaskName '{fullPath}' }}\"",
                        Environment.CurrentDirectory),
                    userId: username,
                    logonType: TaskLogonType.ServiceAccount,
                    description: "Task used by Wu10Man to enable other tasks");
        }

        private void CleanupAdminTask()
        {
            using (var oldTask = TaskService.Instance.GetTask(_wu10TaskPath))
            {
                if (oldTask != null)
                {
                    oldTask.Stop();
                    TaskService.Instance.RootFolder.DeleteTask(_wu10TaskPath);
                }
            }
        }

        private void WaitForAdminTaskCompletion()
        {
            while (true)
            {
                using (var task = TaskService.Instance.GetTask(_wu10TaskPath))
                {
                    if (task.State != TaskState.Running)
                        break;
                    else
                        Thread.Sleep(100);
                }
            }
        }
    }
}
