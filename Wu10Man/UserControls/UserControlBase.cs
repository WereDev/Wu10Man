using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WereDev.Utils.Wu10Man.Core;
using WereDev.Utils.Wu10Man.Core.Interfaces;

namespace WereDev.Utils.Wu10Man.UserControls
{
    public class UserControlBase<T> : UserControl
        where T : class, new()
    {
        public UserControlBase()
            : base()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new T();
                LogWriter = DependencyManager.LogWriter;
                LogWriter.LogInfo($"{TabTitle} initializing.");
                Mouse.OverrideCursor = Cursors.Wait;
            }
        }

        protected ILogWriter LogWriter { get; private set; }

        protected T Model { get; set; }

        protected string TabTitle { get; set; } = "Wu10Man";

        protected override void OnInitialized(EventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                SetRuntimeOptionsWrapper();
                LogWriter.LogInfo($"{TabTitle} rendered.");
                Mouse.OverrideCursor = Cursors.Arrow;
            }

            DataContext = Model;
            base.OnInitialized(e);
        }

        protected virtual bool SetRuntimeOptions()
        {
            return true;
        }

        protected void ShowErrorMessage(string message)
        {
            MessageBox.Show($"{message}\r\n\r\nCheck the logs for more details.", TabTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected void ShowWarningMessage(string message)
        {
            MessageBox.Show(message, TabTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        protected void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, TabTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool SetRuntimeOptionsWrapper()
        {
            try
            {
                return SetRuntimeOptions();
            }
            catch (Exception ex)
            {
                LogWriter.LogError(ex);
                ShowErrorMessage(ex.Message);
                return false;
            }
        }
    }
}
