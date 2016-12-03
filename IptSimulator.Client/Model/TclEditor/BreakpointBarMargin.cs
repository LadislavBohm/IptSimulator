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
        private readonly HashSet<int> _breakpointLineNumbers = new HashSet<int>();

        public void ToggleBreakpoint(int lineNumber)
        {
            if (_breakpointLineNumbers.Contains(lineNumber))
            {
                _breakpointLineNumbers.Remove(lineNumber);
            }
            else
            {
                _breakpointLineNumbers.Add(lineNumber);
            }
            OnRedrawRequested(this,EventArgs.Empty);
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

        private void RaiseBreakpointsMouseLeftButtonUpEvent(MouseButtonEventArgs e)
        {
            BreakpointsMouseLeftButtonUp?.Invoke(this,e);
        }

        public event EventHandler<MouseButtonEventArgs> BreakpointsMouseLeftButtonUp;
    }
}
