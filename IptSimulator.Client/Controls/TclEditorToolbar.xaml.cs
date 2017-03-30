using System;
using System.Collections.Generic;
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

namespace IptSimulator.Client.Controls
{
    /// <summary>
    /// Interaction logic for TclEditorToolbar.xaml
    /// </summary>
    public partial class TclEditorToolbar : UserControl
    {
        public TclEditorToolbar()
        {
            InitializeComponent();
            EditorFontSize = 10;
            AvailableFontFamilies = Fonts.SystemFontFamilies
                .Where(ff => 
                    ff.Source.Contains("Global") || 
                    ff.Source.Contains("Consolas") || 
                    ff.Source.Contains("Arial"))
                .OrderBy(ff => ff.Source);

            EditorFontFamily = Fonts.SystemFontFamilies
                .FirstOrDefault(ff => string.Equals(ff.Source, "Consolas", StringComparison.CurrentCultureIgnoreCase));
        }

        public static readonly DependencyProperty EditorFontSizeProperty = DependencyProperty.Register(
            "EditorFontSize", typeof(int), typeof(TclEditorToolbar), new PropertyMetadata(10));

        public int EditorFontSize
        {
            get { return (int) GetValue(EditorFontSizeProperty); }
            set
            {
                if (value > FontSizeUpDown.Minimum && value < FontSizeUpDown.Maximum)
                {
                    SetValue(EditorFontSizeProperty, value);
                }
            }
        }

        public static readonly DependencyProperty EditorFontFamilyProperty = DependencyProperty.Register(
            "EditorFontFamily", typeof(FontFamily), typeof(TclEditorToolbar), new PropertyMetadata(default(FontFamily)));

        public FontFamily EditorFontFamily
        {
            get { return (FontFamily) GetValue(EditorFontFamilyProperty); }
            set { SetValue(EditorFontFamilyProperty, value); }
        }

        public IEnumerable<FontFamily> AvailableFontFamilies { get; set; }
    }
}
