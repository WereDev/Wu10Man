using System;
using WereDev.Utils.Wu10Man.Core.Models;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class WindowsTasksStatusModel : ModelBase
    {
        private string _taskPath = null;
        private string _displayName = null;
        private bool _isTaskEnabled = false;
        private bool _taskExists = false;

        public WindowsTasksStatusModel(WindowsTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            _taskExists = true;
            _taskPath = task.FullPath;
            _displayName = task.Name;
            _isTaskEnabled = task.Enabled;
        }

        public string TaskPath
        {
            get
            {
                return _taskPath;
            }

            set
            {
                if (_taskPath != value)
                {
                    _taskPath = value;
                    TriggerPropertyChanged(nameof(TaskPath));
                }
            }
        }

        public string DisplayName
        {
            get
            {
                return _displayName;
            }

            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    TriggerPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public bool IsTaskEnabled
        {
            get
            {
                return _isTaskEnabled;
            }

            set
            {
                _isTaskEnabled = value;
                TriggerPropertyChanged(nameof(IsTaskEnabled));
            }
        }

        public bool TaskExists
        {
            get
            {
                return _taskExists;
            }

            set
            {
                if (_taskExists != value)
                {
                    _taskExists = value;
                    TriggerPropertyChanged(nameof(TaskExists));
                }
            }
        }
    }
}
