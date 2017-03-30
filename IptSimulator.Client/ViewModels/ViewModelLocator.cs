
using IptSimulator.Client.ViewModels.Data;
using IptSimulator.Client.ViewModels.Dockable;
using IptSimulator.Client.ViewModels.InputDialogs;
using IptSimulator.Client.ViewModels.MenuItems;

namespace IptSimulator.Client.ViewModels
{
    public class ViewModelLocator
    {
        private static ViewModelLocator Instance { get; set; }

        public ViewModelLocator()
        {
            Initialize();
            Instance = this;
        }

        #region ViewModels

        public MainViewModel Main { get; private set; }
                
        public DockManagerViewModel DockManager { get; private set; }

        public MenuViewModel Menu { get; private set; }

        public DebugWindowViewModel Debug { get; private set; }

        public TclEditorViewModel TclEditor { get; private set; }

        public NavigationViewModel Navigation { get; private set; }

        public EndpointTerminalViewModel EndpointTerminal { get; private set; }

        public DigitInputViewModel DigitInput { get; private set; }

        #endregion

        private void Initialize()
        {
            Main = new MainViewModel();
            Debug = new DebugWindowViewModel();
            DockManager = new DockManagerViewModel();
            Menu = new MenuViewModel(DockManager.Documents);
            TclEditor = new TclEditorViewModel();
            Navigation = new NavigationViewModel();
            EndpointTerminal = new EndpointTerminalViewModel();
            DigitInput = new DigitInputViewModel();
        }

        public static void Cleanup()
        {
            Instance?.TclEditor?.Dispose();
        }
    }
}