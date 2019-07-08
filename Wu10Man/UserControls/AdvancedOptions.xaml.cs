using System.Windows.Controls;
using WereDev.Utils.Wu10Man.Helpers;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for AdvancedOptions.xaml
    /// </summary>
    public partial class AdvancedOptions : UserControl
    {
        public AdvancedOptions()
        {
            Wu10Logger.LogInfo("Advanced control initializing.");
            InitializeComponent();
            Wu10Logger.LogInfo("Advanced control initialized.");
        }
    }
}
