using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace IptSimulator.Client.Model.TclEditor
{
    public class BreakpointBackgroundRenderer : IBackgroundRenderer
    {
        private readonly TclAvalonEditor _editor;
        private readonly Brush _background = new SolidColorBrush(Color.FromArgb(148, 251, 237, 126));

        public BreakpointBackgroundRenderer(TclAvalonEditor editor)
        {
            _editor = editor;
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor.HighlightedBreakpointLine.HasValue)
            {
                textView.EnsureVisualLines();

                var line = _editor.Document.GetLineByNumber(_editor.HighlightedBreakpointLine.Value);
                var rects = BackgroundGeometryBuilder.GetRectsForSegment(textView, line);

                foreach (var rect in rects)
                {
                    drawingContext.DrawRectangle(_background, new Pen(_background, 1),
                        new Rect(rect.Location, new Size(textView.ActualWidth, rect.Height)));
                }
            }
        }

        public KnownLayer Layer => KnownLayer.Background;
    }
}