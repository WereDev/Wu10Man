namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class HostFileItem : ModelBase
    {
        private string _url;
        public string Url
        {
            get { return _url; }
            set
            {
                if (_url != value)
                {
                    _url = value;
                    OnPropertyChanged(nameof(Url));
                }
            }
        }

        private bool _isInHostsFile;
        public bool IsInHostsFile
        {
            get { return _isInHostsFile; }
            set
            {
                if (_isInHostsFile != value)
                {
                    _isInHostsFile = value;
                    OnPropertyChanged(nameof(IsInHostsFile));
                }
            }
        }

    }
}
