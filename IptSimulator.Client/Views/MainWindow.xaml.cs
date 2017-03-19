using System;
using System.Windows;
using IptSimulator.Client.Controls;
using IptSimulator.Client.ViewModels;
using DockerWindow = IptSimulator.Client.Controls.DockerWindow;

namespace IptSimulator.Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            ViewModelLocator.Cleanup();
            base.OnClosed(e);
        }
    }
}
