using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace IptSimulator.Client.Model.TclEditor
{
    public class BreakpointBarMargin : AbstractMargin
    {
        private readonly IList<int> _breakpointLineNumbers = new List<int>();

        public bool HasBreakpointAt(int lineNumber)
        {
            return _breakpointLineNumbers.Contains(lineNumber);
        }

        public IReadOnlyList<int> Breakpoints => (IReadOnlyList<int>)_breakpointLineNumbers;

        public bool ToggleBreakpoint(int lineNumber)
        {
            var alreadyActive = HasBreakpointAt(lineNumber);

            if (alreadyActive)
            {
                _breakpointLineNumbers.Remove(lineNumber);
                AdjustBreakpoints(lineNumber, -1); //move up by one line
            }
            else
            {
                _breakpointLineNumbers.Add(lineNumber);
                AdjustBreakpoints(lineNumber, 1); //move down by one line
            }

            OnRedrawRequested(this, EventArgs.Empty);
            return !alreadyActive;
        }

        private void AdjustBreakpoints(int breakpointsUnder, int adjustBy)
        {
            for (int i = 0; i < _breakpointLineNumbers.Count; i++)
            {
                if (_breakpointLineNumbers[i] > breakpointsUnder)
                {
                    _breakpointLineNumbers[i] += adjustBy;
                }
            }
        }

        protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
        {
            if (oldTextView != null)
            {
                oldTextView.VisualLinesChanged -= OnRedrawRequested;
            }
            base.OnTextViewChanged(oldTextView, newTextView);
            if (newTextView != null)
            {
                newTextView.VisualLinesChanged += OnRedrawRequested;
            }
            InvalidateVisual();
        }

        private void OnRedrawRequested(object sender, EventArgs e)
        {
            // Don't invalidate the margin if it'll be invalidated again once the
            // visual lines become valid.
            if (this.TextView != null && this.TextView.VisualLinesValid)
            {
                InvalidateVisual();
            }
        }

        /// <inheritdoc/>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            // accept clicks even when clicking on the background
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(18, 0);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            TextView textView = TextView;
            Size renderSize = RenderSize;

            if (textView != null && textView.VisualLinesValid)
            {
                foreach (var lineNumber in _breakpointLineNumbers)
                {
                    if (lineNumber > textView.VisualLines.Count)
                    {
                        //there is no visual line at this lineNumber (should not happen)
                        continue;
                    }
                    var line = textView.VisualLines[lineNumber - 1];
                    drawingContext.DrawEllipse(new SolidColorBrush(Colors.Red), new Pen(new SolidColorBrush(Colors.Red), 2),
                        new Point(renderSize.Width - 9, line.VisualTop - textView.VerticalOffset + 9), 4, 4);
                }
            }
        }
    }
}
