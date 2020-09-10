using System;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class HostStatus : ModelBase
    {
        private bool _isActive;

        public HostStatus(string host, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException(nameof(host));

            Host = host;
            _isActive = isActive;
        }

        public string Host { get; }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                _isActive = value;
                TriggerPropertyChanged(nameof(IsActive));
            }
        }
    }
}
