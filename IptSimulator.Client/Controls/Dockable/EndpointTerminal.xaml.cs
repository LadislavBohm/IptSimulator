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

namespace IptSimulator.Client.Controls.Dockable
{
    /// <summary>
    /// Interaction logic for EndpointTerminal.xaml
    /// </summary>
    public partial class EndpointTerminal : UserControl
    {
        public EndpointTerminal()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TerminalNameProperty = DependencyProperty.Register(
            "TerminalName", typeof(string), typeof(EndpointTerminal), new PropertyMetadata(default(string)));

        public string TerminalName
        {
            get { return (string) GetValue(TerminalNameProperty); }
            set { SetValue(TerminalNameProperty, value); }
        }
    }
}
