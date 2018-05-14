using System;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class HostStatus : ModelBase
    {
        public HostStatus(string host, bool isBlocked)
        {
            if (String.IsNullOrWhiteSpace(host)) throw new ArgumentNullException(nameof(host));
            Host = host;
            _isBlocked = isBlocked;
        }

        public string Host { get; }

        private bool _isBlocked;
        public bool IsBlocked
        {
            get
            {
                return _isBlocked;
            }
            set
            {
                _isBlocked = value;
                OnPropertyChanged(nameof(IsBlocked));
            }
        }
    }
}
