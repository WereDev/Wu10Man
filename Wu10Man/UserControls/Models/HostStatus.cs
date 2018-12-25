using System;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class HostStatus : ModelBase
    {
        public HostStatus(string host, bool isActive)
        {
            if (String.IsNullOrWhiteSpace(host)) throw new ArgumentNullException(nameof(host));
            Host = host;
            _isActive = isActive;
        }

        public string Host { get; }

        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }
    }
}
