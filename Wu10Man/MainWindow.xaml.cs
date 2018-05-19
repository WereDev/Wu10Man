using System;
using System.Windows;

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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}

