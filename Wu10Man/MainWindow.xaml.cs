using System.Windows;
using System.Windows.Controls;

namespace WereDev.Utils.Wu10Man
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if (Elevator.IsElevated)
                InitializeComponent();
            else
              Elevator.Elevate();
        }
    }
}

