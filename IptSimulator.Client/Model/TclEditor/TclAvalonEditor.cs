using System;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
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
                var alreadySet = _breakpointBarMarginMargin.HasBreakpointAt(editorPosition.Value.Line);
                var offset = Document.GetOffset(editorPosition.Value.Location);
                var breakpoint = CreateBreakpointText(editorPosition.Value.Line);

                if (alreadySet)
                {
                    Document.Remove(offset, breakpoint.Length);
                }
                else
                {
                    Document.Insert(offset, breakpoint);
                }

                _breakpointBarMarginMargin.ToggleBreakpoint(editorPosition.Value.Line);
                AdjustBreakpointTexts(editorPosition.Value.Line);
            }

            mouseButtonEventArgs.Handled = false;
        }

        private void AdjustBreakpointTexts(int fromLine)
        {
            foreach (var line in _breakpointBarMarginMargin.Breakpoints)
            {
                if (line >= fromLine)
                {
                    var existingLine = Document.GetLineByNumber(line);

                    Document.Replace(existingLine.Offset, existingLine.TotalLength, CreateBreakpointText(line));
                }
            }
        }

        private string CreateBreakpointText(int line)
        {
            return "breakpoint " + line + Environment.NewLine;
        }
    }

    class TclVisualLineGenerator : VisualLineElementGenerator
    {
        public override int GetFirstInterestedOffset(int startOffset)
        {
            //base.CurrentContext.
            throw new NotImplementedException();
        }

        public override VisualLineElement ConstructElement(int offset)
        {
            throw new NotImplementedException();
        }
    }
}
