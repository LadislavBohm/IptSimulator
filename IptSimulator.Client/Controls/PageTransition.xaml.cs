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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IptSimulator.Client.Controls
{
    /// <summary>
    /// Interaction logic for PageTransition.xaml
    /// </summary>
    public partial class PageTransition : UserControl
    {
        private readonly Stack<UserControl> _pages = new Stack<UserControl>();

        public PageTransition()
        {
            InitializeComponent();
        }

        public UserControl CurrentPage { get; set; }

        public void ShowPage(UserControl newPage)
        {
            _pages.Push(newPage);

            Task.Factory.StartNew(ShowNewPage);
        }

        void ShowNewPage()
        {
            Dispatcher.Invoke(delegate
            {
                if (ContentPresenter.Content != null)
                {
                    UserControl oldPage = ContentPresenter.Content as UserControl;

                    if (oldPage != null)
                    {
                        oldPage.Loaded -= NewPage_Loaded;

                        UnloadPage(oldPage);
                    }
                }
                else
                {
                    ShowNextPage();
                }

            });
        }

        void ShowNextPage()
        {
            UserControl newPage = _pages.Pop();

            newPage.Loaded += NewPage_Loaded;

            ContentPresenter.Content = newPage;
        }

        void UnloadPage(UserControl page)
        {
            Storyboard hidePage = ((Storyboard)Resources["SlideAndFadeOut"]).Clone();

            hidePage.Completed += hidePage_Completed;

            hidePage.Begin(ContentPresenter);
        }

        void NewPage_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard showNewPage = (Storyboard)Resources["SlideAndFadeIn"];

            showNewPage.Begin(ContentPresenter);

            CurrentPage = sender as UserControl;
        }

        void hidePage_Completed(object sender, EventArgs e)
        {
            ContentPresenter.Content = null;

            ShowNextPage();
        }
    }
}

