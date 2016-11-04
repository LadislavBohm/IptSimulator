
namespace IptSimulator.Client.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            Initialize();
        }

        public MainViewModel Main { get; private set; }
                
        public DockManagerViewModel DockManager { get; private set; }

        public MenuViewModel Menu { get; private set; }

        private void Initialize()
        {
            Main = new MainViewModel();
            DockManager = new DockManagerViewModel();
            Menu = new MenuViewModel(DockManager.Documents);
        }

        public static void Cleanup()
        {
        }
    }
}