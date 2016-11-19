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

            if (ViewModelBase.IsInDesignModeStatic)
            {
                GenerateGraph();
            }
        }

        private void GraphManagerOnGraphPropertyChanged(object sender, EventArgs eventArgs)
        {
            GenerateGraph();
        }

        private void Window_OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!_loaded)
            {
                GenerateGraph();
                _loaded = true;
            }
        }

        private void GenerateGraph()
        {
            try
            {
                _logger.Info("Generating graph from FSM transitions.");

                var transitions = GenerateRandomTransitions();

                GraphArea.LogicCore = _graphManager.Generate(transitions);

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
                GraphZoomControl.ZoomToContent(EnlargeBy(GraphZoomControl.Viewport));
                
                _logger.Info("Graph successfully generated and displayed.");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while generating graph in OnLoad event.");
                throw;
            }
        }

        private ICollection<FsmTransition> GenerateRandomTransitions()
        {
            var result = new List<FsmTransition>
            {
                new FsmTransition("INIT_STATE", "ev_setup_indication", "LanguageSelected", "CallInitReqLanguage"),
                new FsmTransition("LanguageSelected", "ev_collectdigits_done", "PinEntered", "SetLanguageCheckBlockedNumber"),
                new FsmTransition("PinEntered", "ev_collectdigits_done", "TransferNotified", "SetPinFindConfNumber"),
                new FsmTransition("TransferNotified", "ev_media_done", "FirstTransferNotified", "SaveAttendeeToConnect"),
                new FsmTransition("FirstTransferNotified", "ev_media_done", "TransferredToConf", "TransferToConf"),
                new FsmTransition("TransferredToConf", "ev_setup_done", "same_state", "CheckSetupResult"),
                new FsmTransition("ErrorNotifi", "ev_media_done", "same_state", "Disconnect"),
                new FsmTransition("ConferenceFull", "ev_media_done", "same_state", "TransferToHelpdesk"),
                new FsmTransition("ConferenceFull", "ev_setup_done", "same_state", "CheckTransToHelpdesk"),
                new FsmTransition("any_state", "ev_disconnected", "AttendeeDisconnectedSaved", "SaveAttendeeDisconnected"),
                new FsmTransition("NumberIsBlocked", "ev_media_done", "AttendeeDisconnectedSaved", "DisconnectAndSaveAttendeeDisconnected"),
            };

            return result;
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
