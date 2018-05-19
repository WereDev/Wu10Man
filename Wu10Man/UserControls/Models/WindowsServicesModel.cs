namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class WindowsServicesModel : ModelBase
    {

        private bool _isUpdateServiceEnabled = false;
        public bool IsUpdateServiceEnabled
        {
            get { return _isUpdateServiceEnabled; }
            set
            {
                if (_isUpdateServiceEnabled != value)
                {
                    _isUpdateServiceEnabled = value;
                    OnPropertyChanged(nameof(IsUpdateServiceEnabled));
                }
            }
        }

        private bool _isModulesInstallerServiceEnabled = false;
        public bool IsModulesInstallerServiceEnabled
        {
            get { return _isModulesInstallerServiceEnabled; }
            set
            {
                if (_isModulesInstallerServiceEnabled != value)
                {
                    _isModulesInstallerServiceEnabled = value;
                    OnPropertyChanged(nameof(IsModulesInstallerServiceEnabled));
                }
            }
        }


    }
}
