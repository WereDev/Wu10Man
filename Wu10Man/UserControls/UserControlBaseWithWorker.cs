using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WereDev.Utils.Wu10Man.UserControls
{
    public class UserControlBaseWithWorker<T> : UserControlBase<T>, IDisposable
        where T : class, new()
    {
        private readonly ProgressBarControl _progressBar = new ProgressBarControl();
        private bool _isDisposed = false;
        private BackgroundWorker _worker;

        public UserControlBaseWithWorker()
            : base()
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                _worker.Dispose();
            }

            _isDisposed = true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "_worker part of larger scope.")]
        protected virtual void RunBackgroundProcess(int progressBarMax, DoWorkEventHandler backgroundMethod)
        {
            InitializeProgressBar(0, progressBarMax);

            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
            };
            _worker.DoWork += backgroundMethod;
            _worker.RunWorkerCompleted += RunWorkerCompleted;
            _worker.RunWorkerAsync();
        }

        protected virtual void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _progressBar.Visibility = Visibility.Hidden;
            ((Grid)Application.Current.MainWindow.Content).Children.Remove(_progressBar);
        }

        protected void InitializeProgressBar(int minValue, int maxValue)
        {
            ((Grid)Application.Current.MainWindow.Content).Children.Add(_progressBar);
            _progressBar.HorizontalAlignment = HorizontalAlignment.Stretch;
            _progressBar.VerticalAlignment = VerticalAlignment.Stretch;
            _progressBar.Visibility = Visibility.Visible;
            _progressBar.Initialize(minValue, maxValue);
        }

        protected void AdvanceProgressBar()
        {
            _progressBar.Advance();
        }
    }
}
