using System;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using IptSimulator.Client.ViewModels.Dockable;

namespace IptSimulator.Client.Model.TclEditor
{
    internal class TclAvalonEditor : TextEditor
    {
        private readonly BreakpointBarMargin _breakpointBarMarginMargin = new BreakpointBarMargin();

        public TclAvalonEditor()
        {
            TextArea.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            TextArea.LeftMargins.Add(_breakpointBarMarginMargin);
            
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var point = mouseButtonEventArgs.GetPosition(this);
            var editorPosition = this.GetPositionFromPoint(point);

            //don't create breakpoints when user clicks to regular text area
            if (editorPosition.HasValue && point.X < 30)
            {
                _breakpointBarMarginMargin.ToggleBreakpoint(editorPosition.Value.Line);
            }

            mouseButtonEventArgs.Handled = false;
        }
    }
}
