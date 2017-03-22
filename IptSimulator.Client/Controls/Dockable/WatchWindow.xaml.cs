using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IptSimulator.CiscoTcl.Model;
using IptSimulator.Client.ViewModels.Data;

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

        private void TopVariableGrid_RowSelectedChanged(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            var vm = row?.DataContext as WatchVariableViewModel;
            if (vm == null)
            {
                return;
            }

            if (row.IsSelected && !vm.HasSubValues)
            {
                //disable selection of single value rows
                row.IsSelected = false;
            }
        }
    }
}
