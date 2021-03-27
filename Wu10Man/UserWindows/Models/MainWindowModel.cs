using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WereDev.Utils.Wu10Man.UserControls.Models;

namespace WereDev.Utils.Wu10Man.UserWindows.Models
{
    public class MainWindowModel : ModelBase
    {
        private bool _showLegacy;

        public bool ShowLegacy
        {
            get
            {
                return _showLegacy;
            }

            set
            {
                _showLegacy = value;
                TriggerPropertyChanged(nameof(ShowLegacy), nameof(LegacyVisibility), nameof(VisibleTabItems));
            }
        }

        public Visibility LegacyVisibility => _showLegacy ? Visibility.Visible : Visibility.Hidden;

        public ITabItemModel<UserControl>[] TabItems { get; set; }

        public ITabItemModel<UserControl>[] VisibleTabItems => TabItems.Where(x => _showLegacy || !x.IsLegacy).ToArray();
    }
}
