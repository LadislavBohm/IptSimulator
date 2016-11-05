using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using IptSimulator.Client.ViewModels.Abstractions;
using NLog;
using Xceed.Wpf.AvalonDock.Layout;

namespace IptSimulator.Client.Model
{
    public class AvalonDockLayoutInitializer : ILayoutUpdateStrategy
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        #region ILayoutUpdateStrategy implementation

        //    public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        //    {
        //        return BeforeInsertContent(layout, anchorableToShow);
        //    }

        //    public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        //    {
        //        return BeforeInsertContent(layout, anchorableToShow);
        //    }

        //    public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown) { }
        //    public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown) { }

        #endregion

        //    private bool BeforeInsertContent(LayoutRoot layout, LayoutContent anchorableToShow)
        //    {
        //        try
        //        {
        //            _logger.Debug("Restoring previous content.");

        //            var viewModel = (DockWindowViewModel)anchorableToShow.Content;
        //            var layoutContent = layout
        //                .Descendents()
        //                .OfType<LayoutContent>()
        //                .FirstOrDefault(x => x.ContentId == viewModel.ContentId);

        //            if (layoutContent == null)
        //                return false;

        //            layoutContent.Content = anchorableToShow.Content;

        //            var layoutContainer = layoutContent
        //                .GetType()
        //                .GetProperty("PreviousContainer", BindingFlags.NonPublic | BindingFlags.Instance)
        //                .GetValue(layoutContent, null) as ILayoutContainer;

        //            if (layoutContainer is LayoutAnchorablePane)
        //            {
        //                (layoutContainer as LayoutAnchorablePane).Children.Add(layoutContent as LayoutAnchorable);
        //            }
        //            else if (layoutContainer is LayoutDocumentPane)
        //            {
        //                (layoutContainer as LayoutDocumentPane).Children.Add(layoutContent);
        //            }
        //            else
        //            {
        //                throw new NotSupportedException("Only LayoutAnchorablePane and LayoutDocumentPane are supported.");
        //            }

        //            _logger.Debug("Previous content successfully restored.");
        //            return true;
        //        }
        //        catch (Exception e)
        //        {
        //            _logger.Error(e, "Error while restoring previous dock content.");
        //            throw;
        //        }
        //    }

        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            //AD wants to add the anchorable into destinationContainer
            //just for test provide a new anchorablepane 
            //if the pane is floating let the manager go ahead
            LayoutAnchorablePane destPane = destinationContainer as LayoutAnchorablePane;
            if (destinationContainer?.FindParent<LayoutFloatingWindow>() != null)
                return false;

            var toolsPane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == "ToolsPane");
            if (toolsPane != null)
            {
                toolsPane.Children.Add(anchorableToShow);
                return true;
            }

            return false;

        }

        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown) { }

        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer) { return false; }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown) { }
    }
}
