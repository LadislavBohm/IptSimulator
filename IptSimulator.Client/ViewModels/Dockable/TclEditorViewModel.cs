using IptSimulator.Client.ViewModels.Abstractions;
using PropertyChanged;

namespace IptSimulator.Client.ViewModels.Dockable
{
    [ImplementPropertyChanged]
    public class TclEditorViewModel : DockWindowViewModel
    {
        public override string Title { get; set; } = "TCL editor";

        public override int Order => 1;
    }
}
