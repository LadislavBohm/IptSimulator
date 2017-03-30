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
using GalaSoft.MvvmLight;
using GraphX.Controls;
using GraphX.PCL.Common.Enums;
using IptSimulator.Client.Model.FsmGraph;
using IptSimulator.Client.ViewModels.Dockable;
using NLog;
using FsmTransition = IptSimulator.CiscoTcl.Model.FsmTransition;

namespace IptSimulator.Client.Controls
{
    /// <summary>
    /// Interaction logic for FsmGraph.xaml
    /// </summary>
    public partial class FsmGraph : UserControl
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly FsmGraphManager _graphManager = new FsmGraphManager();
        private bool _loaded;

        public FsmGraph()
        {
            InitializeComponent();

            ZoomControl.SetViewFinderVisibility(GraphZoomControl, Visibility.Collapsed);

            Loaded += Window_OnLoaded;

            _graphManager.GraphPropertyChanged += GraphManagerOnGraphPropertyChanged;
        }

        private void GraphManagerOnGraphPropertyChanged(object sender, EventArgs eventArgs)
        {
            var vm = (FsmGraphViewModel)DataContext;
            if (vm.Transitions != null)
            {
                GenerateGraph(vm.CurrentState, vm.Transitions);
            }
        }

        private void Window_OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!_loaded)
            {
                var vm = (FsmGraphViewModel) DataContext;
                vm.PropertyChanged += (o, args) =>
                {
                    if (args.PropertyName == nameof(FsmGraphViewModel.CurrentState))
                    {
                        
                    }
                    if (args.PropertyName == nameof(FsmGraphViewModel.Transitions))
                    {
                        if (vm.Transitions != null)
                        {
                            GenerateGraph(vm.CurrentState, vm.Transitions);
                        }
                    }
                };
                _loaded = true;
            }
        }

        private void GenerateGraph(string initialState, ICollection<FsmTransition> transitions)
        {
            try
            {
                _logger.Info("Generating graph from FSM transitions.");

                GraphArea.LogicCore = _graphManager.Generate(initialState, transitions);

                _logger.Debug("Graph successfully generated from transitions.");

                GraphArea.GenerateGraph();
                GraphArea.ShowAllEdgesArrows();
                GraphArea.ShowAllEdgesLabels();
                GraphArea.ShowAllVerticesLabels();

                if (_graphManager.AlignEdgeLabels)
                    GraphArea.AlignAllEdgesLabels();

                _logger.Debug("Zooming to generated graph.");

                GraphZoomControl.ZoomToFill();
                //enlarge current zoom
                GraphZoomControl.ZoomToContent(EnlargeBy(GraphZoomControl.Viewport, 50));

                _logger.Info("Graph successfully generated and displayed.");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while generating graph in OnLoad event.");
                throw;
            }
        }

        private Rect EnlargeBy(Rect original, int enlargeBy = 150)
        {
            var newX = original.Location.X - enlargeBy;
            var newY = original.Location.Y - enlargeBy;
            return new Rect(
                new Point(newX, newY),
                new Point(newX + original.Width + (3*enlargeBy), newY + original.Height+ (3*enlargeBy)));
        }

        private void SaveAsImgMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            GraphArea.ExportAsImageDialog(ImageType.PNG);
        }

        private void PrintMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            GraphArea.PrintVisibleAreaDialog();
        }
    }
}
