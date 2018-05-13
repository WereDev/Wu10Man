using System;
using System.ServiceProcess;

namespace WereDev.Utils.Wu10Man.Editors
{
    class ServiceEditor : IDisposable
    {
        private readonly ServiceController _serviceController;


        public ServiceEditor(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            _serviceController = new ServiceController(serviceName);
        }

        public void SetStartupType(ServiceStartMode startMode)
        {
            RegistryEditor.WriteLocalMachineRegistryValue(@"SYSTEM\CurrentControlSet\Services\" + _serviceController.ServiceName,
                                                          "Start",
                                                          ((int)startMode).ToString(),
                                                          Microsoft.Win32.RegistryValueKind.DWord);
        }

        public void DisableService()
        {
            StopService();
            SetStartupType(ServiceStartMode.Disabled);
            //SetAccountAsFake();
        }

        public void EnableService()
        {
            SetAccountAsLocalSystem();
            SetStartupType(ServiceStartMode.Manual);
            //StartService();
        }

        public bool IsServiceEnabled()
        {
            return _serviceController.StartType != ServiceStartMode.Disabled;
        }

        //public void StartService()
        //{
        //    if (_serviceController.Status != ServiceControllerStatus.Running)
        //    {
        //        _serviceController.Start();
        //        _serviceController.WaitForStatus(ServiceControllerStatus.Running);
        //    }
        //}

        public void StopService()
        {
            if (_serviceController.Status != ServiceControllerStatus.Stopped)
            {
                _serviceController.Stop();
                _serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
            }
        }

        public void SetAccountAsFake()
        {
            ServiceCredentialsEditor.SetWindowsServiceCreds(_serviceController.ServiceName, @"fakeUser", "fakePassword");
        }

        public void SetAccountAsLocalSystem()
        {
            ServiceCredentialsEditor.SetWindowsServiceCreds_LocalService(_serviceController.ServiceName);
        }

        ~ServiceEditor()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            _serviceController.Dispose();
        }
    }

}
