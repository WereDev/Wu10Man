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
                _hostStatus = value ?? new HostStatus[0];
                TriggerPropertyChanged(nameof(HostStatus));
            }
        }
    }
}
