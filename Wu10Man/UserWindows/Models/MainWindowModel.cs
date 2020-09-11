using System.Windows;
using WereDev.Utils.Wu10Man.UserControls.Models;

namespace WereDev.Utils.Wu10Man.UserWindows.Models
{
    public class MainWindowModel : ModelBase
    {
        private bool _showLegacy = false;

        public bool ShowLegacy
        {
            get
            {
                return _showLegacy;
            }

            set
            {
                _showLegacy = value;
                TriggerPropertyChanged(nameof(ShowLegacy));
                TriggerPropertyChanged(nameof(LegacyVisibility));
            }
        }

        public Visibility LegacyVisibility => _showLegacy ? Visibility.Visible : Visibility.Hidden;
    }
}
