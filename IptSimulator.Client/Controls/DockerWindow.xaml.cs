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
using NLog;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Path = System.IO.Path;

namespace IptSimulator.Client.Controls
{
    /// <summary>
    /// Interaction logic for DockerWindow.xaml
    /// </summary>
    public partial class DockerWindow : UserControl
    {
        private readonly string _layoutFilePath = Path.Combine(Directory.GetCurrentDirectory(), "LayoutState.xml");
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private bool _saving;

        public DockerWindow()
        {
            InitializeComponent();
            Unloaded += (sender, args) =>
            {
                if (!_saving)
                {
                    SaveLayout();
                }
            };
            Loaded += (sender, args) =>
            {
                LoadLayout();
            };
        }

        
        private async void SaveLayout()
        {
            try
            {
                _logger.Debug("Saving DockManager layout.");

                _saving = true;

                var serializer = new XmlLayoutSerializer(MainDockingManager);
                using (var stream = new StreamWriter(_layoutFilePath))
                {
                    serializer.Serialize(stream);
                    await stream.FlushAsync();
                }

                _logger.Debug("Layout successfully saved.");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while serializing layout.");
            }
            finally
            {
                _saving = false;
            }
        }

        private void LoadLayout()
        {
            try
            {
                _logger.Info("Restoring previous layout.");

                if (!File.Exists(_layoutFilePath))
                {
                    _logger.Info("Previous layout state file does not exist.");
                    return;
                }

                var serializer = new XmlLayoutSerializer(MainDockingManager);
                using (var stream = new StreamReader(_layoutFilePath))
                {
                    serializer.Deserialize(stream);
                }

                _logger.Info("Previous layout successfully restored.");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while loading layout.");
            }
        }

        private async void MainDockingManager_OnLoaded(object sender, RoutedEventArgs e)
        {
            //DebugWindowAnchor.Show();
            WatchWindowAnchor.Show();

            await Task.Delay(50);

            //DebugWindowAnchor.ToggleAutoHide();
            WatchWindowAnchor.ToggleAutoHide();
        }
    }
}
