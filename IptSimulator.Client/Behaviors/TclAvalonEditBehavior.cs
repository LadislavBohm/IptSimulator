using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;
using IptSimulator.Client.Model.TclEditor;
using IptSimulator.Core.Utils;

namespace IptSimulator.Client.Behaviors
{
    public sealed class TclAvalonEditBehavior : Behavior<TclAvalonEditor>
    {
        public static readonly DependencyProperty ExtractedTextProperty =
            DependencyProperty.Register("ExtractedText", typeof(string), typeof(TclAvalonEditBehavior),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        public string ExtractedText
        {
            get { return (string)GetValue(ExtractedTextProperty); }
            set { SetValue(ExtractedTextProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
            {
                AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
            }
        }

        private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
        {
            var textEditor = sender as TextEditor;
            if (textEditor?.Document != null)
            {
                ExtractedText = textEditor.Document.Text;
            }
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as TclAvalonEditBehavior;
            var editor = behavior?.AssociatedObject;
            if (editor?.Document != null)
            {
                var caretOffset = editor.CaretOffset;
                editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();
                editor.CaretOffset = caretOffset;
            }
        }
    }
}
