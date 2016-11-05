
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

        public TclEditorViewModel TclEditor { get; private set; }

        #endregion

        private void Initialize()
        {
            Main = new MainViewModel();
            Debug = new DebugWindowViewModel();
            DockManager = new DockManagerViewModel();
            Menu = new MenuViewModel(DockManager.Documents);
            TclEditor = new TclEditorViewModel();
        }

        public static void Cleanup()
        {
        }
    }
}