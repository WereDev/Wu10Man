namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class WindowsServiceStatusModel : ModelBase
    {

        public WindowsServiceStatusModel(string serviceName)
        {
            _serviceName = serviceName;
        }

        private string _serviceName;
        public string ServiceName
        {
            get { return _serviceName; }
            set
            {
                if (_serviceName != value)
                {
                    _serviceName = value;
                    OnPropertyChanged(nameof(ServiceName));
                }
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        private bool _isServiceEnabled = false;
        public bool IsServiceEnabled
        {
            get { return _isServiceEnabled; }
            set
            {
                if (_isServiceEnabled != value)
                {
                    _isServiceEnabled = value;
                    OnPropertyChanged(nameof(IsServiceEnabled));
                }
            }
        }

        private bool _serviceExists = false;
        public bool ServiceExists
        {
            get { return _serviceExists; }
            set
            {
                if (_serviceExists != value)
                {
                    _serviceExists = value;
                    OnPropertyChanged(nameof(ServiceExists));
                }
            }
        }
    }
}
