
using IptSimulator.Client.ViewModels.Data;
using IptSimulator.Client.ViewModels.Dockable;

namespace IptSimulator.Client.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            Initialize();
        }

        #region ViewModels

        public MainViewModel Main { get; private set; }
                
        public DockManagerViewModel DockManager { get; private set; }

        public MenuViewModel Menu { get; private set; }

        public DebugWindowViewModel Debug { get; private set; }

        public EventRaisingViewModel EventRaising { get; private set; }

        public TclEditorViewModel TclEditor { get; private set; }

        public WatchWindowViewModel Watch { get; private set; }

        #endregion

        private void Initialize()
        {
            Main = new MainViewModel();
            Debug = new DebugWindowViewModel();
            DockManager = new DockManagerViewModel();
            Menu = new MenuViewModel(DockManager.Documents);
            EventRaising = new EventRaisingViewModel();
            TclEditor = new TclEditorViewModel();
            Watch = new WatchWindowViewModel();
        }

        public static void Cleanup()
        {
        }
    }
}