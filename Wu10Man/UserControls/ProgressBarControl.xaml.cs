using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
using WereDev.Utils.Wu10Man.UserControls.Models;

namespace WereDev.Utils.Wu10Man.UserControls
{
    /// <summary>
    /// Interaction logic for ProgressBarControl.xaml.
    /// </summary>
    public partial class ProgressBarControl : UserControl
    {
        private readonly ProgressBarModel _model;

        public ProgressBarControl()
        {
            _model = new ProgressBarModel();
            DataContext = _model;
            InitializeComponent();
        }

        public void Initialize(int minValue, int maxValue)
        {
            _model.MinValue = minValue;
            _model.MaxValue = maxValue;
            _model.CurrentValue = minValue;
        }

        public void Advance()
        {
            _model.CurrentValue++;
        }
    }
}
