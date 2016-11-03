using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using IptSimulator.Client.Exceptions;
using IptSimulator.Client.Views;
using IptSimulator.Core;
using NLog;
using TclCompletionManager = IptSimulator.Client.Model.TclCompletionManager;

namespace IptSimulator.Client.Controls
{
    /// <summary>
    /// Interaction logic for TclEditor.xaml
    /// </summary>
    public partial class TclEditor : UserControl
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private ImageSource _tclIcon;
        private ICompletionManager _completionManager;
        private CompletionWindow _completionWindow;

        public TclEditor()
        {
            InitializeComponent();
            InitializeTclEditor();
        }

        private void InitializeTclEditor()
        {
            try
            {
                _logger.Info("Initializing TCL editor...");

                SetSearchPanel();
                SetCompletionHandling();

                _logger.Info("TCL editor successfully initialized.");
            }
            catch (Exception e)
            {
                _logger.Info(e,"Error while initializing TCL editor.");
                throw;
            }
        }


        private void SetSearchPanel()
        {
            _logger.Debug("Setting editor search panel.");

            SearchPanel.Install(MainTextEditor);

            _logger.Debug("Editor search panel is successfully set.");
        }

        private void SetCompletionHandling()
        {
            _logger.Debug("Setting editor completion handling.");

            try
            {
                _tclIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/tcl-icon.png"));
            }
            catch
            {
                _logger.Warn("Could not read TCL icon from resources.");
            }
            _completionManager = new TclCompletionManager();

            MainTextEditor.TextArea.TextEntered += MainEditor_TextEntered;
            MainTextEditor.TextArea.TextEntering += MainEditor_TextEntering;

            _logger.Debug("Editor completion handling is successfully set.");
        }

        private void MainEditor_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == " " || (Keyboard.IsKeyDown(Key.Space) && Keyboard.IsKeyDown(Key.LeftCtrl)))
            {
                ShowCompletionWindow();
            }
        }

        private void ShowCompletionWindow()
        {
            _completionWindow = new CompletionWindow(MainTextEditor.TextArea);
            IList<ICompletionData> data = _completionWindow.CompletionList.CompletionData;

            int i = 0;
            foreach (var completionResult in _completionManager.GetCompletions(MainTextEditor.Document.Text))
            {
                i++;
                data.Add(new EditorCompletionData(_tclIcon, completionResult.Text, completionResult.Priority + i));
            }

            if (data.Count == 0)
            {
                //if no completions were found, don't display completion window
                _completionWindow = null;
            }
            else
            {
                //preselect first item
                //_completionWindow.CompletionList.SelectedItem = data[0];
                _completionWindow.Show();
                _completionWindow.Closed += delegate { _completionWindow = null; };
            }
        }

        private void MainEditor_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }



        private class EditorCompletionData : ICompletionData
        {
            public EditorCompletionData(ImageSource image, string text, double priority)
            {
                Text = text;
                Image = image;
                Priority = priority;
            }

            public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
            {
                textArea.Document.Replace(completionSegment, Text);
            }

            public ImageSource Image { get; } 
            public string Text { get; }
            public object Content => Text;
            public object Description => "Description for " + Text;
            public double Priority { get; }
        }
    }
}
