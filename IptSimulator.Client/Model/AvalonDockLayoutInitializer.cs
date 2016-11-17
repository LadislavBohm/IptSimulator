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
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
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
