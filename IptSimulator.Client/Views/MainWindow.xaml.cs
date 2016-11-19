using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
using Fluent;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using IptSimulator.Client.Controls;
using IptSimulator.Client.Controls.Dockable;
using IptSimulator.Client.ViewModels;

namespace IptSimulator.Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private readonly DockerWindow _dockerWindow = new DockerWindow();
        private readonly FsmGraph _fsmGraph = new FsmGraph();
        private bool _graphVisible;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (sender, args) => PageTransitionControl.ShowPage(_dockerWindow);
        }

        private void FsmGraphButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_graphVisible)
            {
                PageTransitionControl.ShowPage(_dockerWindow);
            }
            else
            {
                PageTransitionControl.ShowPage(_fsmGraph);
            }

            _graphVisible = !_graphVisible;
        }
    }
}
