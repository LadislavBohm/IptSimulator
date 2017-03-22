using System.Windows.Controls;
using System.Windows.Input;

namespace IptSimulator.Client.Controls.Dockable
{
    /// <summary>
    /// Interaction logic for WatchWindow.xaml
    /// </summary>
    public partial class WatchWindow : UserControl
    {
        public WatchWindow()
        {
            InitializeComponent();
        }

        private void TopVariableGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row != null)
            {
                TopVariableGrid.UnselectAll();
            }
        }
    }
}
