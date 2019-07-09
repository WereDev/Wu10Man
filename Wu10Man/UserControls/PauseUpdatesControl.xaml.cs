using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for PauseUpdates.xaml
    /// </summary>
    public partial class PauseUpdatesControl : UserControl
    {
        public PauseUpdatesControl()
        {
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
            InitializeComponent();
        }
    }
}
