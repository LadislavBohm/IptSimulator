using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using IptSimulator.Client.ViewModels.Dockable;

namespace IptSimulator.Client.Model.TclEditor
{
    public sealed class TclAvalonEditor : TextEditor
    {
        private readonly BreakpointBarMargin _breakpointBarMarginMargin = new BreakpointBarMargin();

        public static readonly DependencyProperty HighlightedBreakpointLineProperty = DependencyProperty.Register(
            "HighlightedBreakpointLine", typeof(int?), typeof(TclAvalonEditor), new FrameworkPropertyMetadata(OnHighlightedBreakpointLineChanged));

        public static readonly DependencyProperty BreakpointsProperty = DependencyProperty.Register(
            "Breakpoints", typeof(IEnumerable<int>), typeof(TclAvalonEditor), new PropertyMetadata(default(IEnumerable<int>)));

        public TclAvalonEditor()
        {
            TextArea.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            TextArea.LeftMargins.Add(_breakpointBarMarginMargin);
            TextArea.TextView.VisualLinesChanged += (sender, args) => HideInvalidBreakpoints();
            TextArea.TextView.BackgroundRenderers.Add(new BreakpointBackgroundRenderer(this));
        }

        #region Properties

        public IEnumerable<int> Breakpoints
        {
            get { return (IEnumerable<int>) GetValue(BreakpointsProperty); }
            set { SetValue(BreakpointsProperty, value); }
        }

        public int? HighlightedBreakpointLine
        {
            get { return (int?) GetValue(HighlightedBreakpointLineProperty); }
            set
            {
                SetValue(HighlightedBreakpointLineProperty, value);
                if (value.HasValue)
                {
                    MoveToAndHighlightLine(value.Value);
                }
            }
        }
        

        #endregion

        #region Public methods

        /// <summary>
        /// Toggles breakpoint at specified <paramref name="lineNumber"/>
        /// </summary>
        /// <param name="lineNumber"></param>
        public void ToggleBreakpoint(int lineNumber)
        {
            _breakpointBarMarginMargin.ToggleBreakpoint(lineNumber);
            Breakpoints = _breakpointBarMarginMargin.Breakpoints;
        }

        /// <summary>
        /// Toggles breakpoint at current line position.
        /// </summary>
        public void ToggleBreakpoint()
        {
            _breakpointBarMarginMargin.ToggleBreakpoint(TextArea.Caret.Line);
            Breakpoints = _breakpointBarMarginMargin.Breakpoints;
        }

        #endregion

        #region Private methods

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var point = mouseButtonEventArgs.GetPosition(this);
            var editorPosition = this.GetPositionFromPoint(point);

            //don't create breakpoints when user clicks to regular text area
            if (editorPosition.HasValue && point.X < 30)
            {
                ToggleBreakpoint(editorPosition.Value.Line);
            }

            mouseButtonEventArgs.Handled = false;
        }


        private void HideInvalidBreakpoints()
        {
            var breakpointsToHide = _breakpointBarMarginMargin.Breakpoints.Where(b => b > Document.LineCount).ToList();
            foreach (var breakpointLine in breakpointsToHide)
            {
                //this line no longer exists, remove this breakpoint
                _breakpointBarMarginMargin.ToggleBreakpoint(breakpointLine);
            }
        }

        private void MoveToAndHighlightLine(int lineNumber)
        {
            ScrollToLine(lineNumber);
        }

        private static void OnHighlightedBreakpointLineChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var editor = dependencyObject as TclAvalonEditor;
            editor?.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
        }

        #endregion
    }
}
