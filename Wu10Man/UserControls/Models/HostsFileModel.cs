using System;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class HostsFileModel : ModelBase
    {
        private HostStatus[] _hostStatus = new HostStatus[0];
        public HostStatus[] HostStatus
        {
            get
            {
                return _hostStatus;
            }
            set
            {
                _hostStatus = value ?? throw new ArgumentNullException(nameof(value));
                OnPropertyChanged(nameof(HostStatus));
            }
        }


    }
}
