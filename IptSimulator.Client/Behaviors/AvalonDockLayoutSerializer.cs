using System.IO;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace IptSimulator.Client.Behaviors
{
    public static class AvalonDockLayoutSerializer
    {
        /// <summary>
        /// Backing store for LoadLayoutCommand dependency property
        /// </summary>
        private static readonly DependencyProperty LoadLayoutCommandProperty =
            DependencyProperty.RegisterAttached("LoadLayoutCommand",
            typeof(ICommand),
            typeof(AvalonDockLayoutSerializer),
            new PropertyMetadata(null, AvalonDockLayoutSerializer.OnLoadLayoutCommandChanged));

        /// <summary>
        /// Backing store for SaveLayoutCommand dependency property
        /// </summary>
        private static readonly DependencyProperty SaveLayoutCommandProperty =
            DependencyProperty.RegisterAttached("SaveLayoutCommand",
            typeof(ICommand),
            typeof(AvalonDockLayoutSerializer),
            new PropertyMetadata(null, AvalonDockLayoutSerializer.OnSaveLayoutCommandChanged));

        /// <summary>
        /// Standard get method of <seealso cref="LoadLayoutCommandProperty"/> dependency property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetLoadLayoutCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LoadLayoutCommandProperty);
        }

        /// <summary>
        /// Standard set method of <seealso cref="LoadLayoutCommandProperty"/> dependency property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetLoadLayoutCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadLayoutCommandProperty, value);
        }

        /// <summary>
        /// This method is executed if a <seealso cref="LoadLayoutCommandProperty"/> dependency property
        /// is about to change its value (eg: The framewark assigns bindings).
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnLoadLayoutCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = d as FrameworkElement;    // Remove the handler if it exist to avoid memory leaks
            if (frameworkElement != null)
            {
                frameworkElement.Loaded -= OnFrameworkElement_Loaded;

                var command = e.NewValue as ICommand;
                if (command != null)
                {
                    // the property is attached so we attach the Drop event handler
                    frameworkElement.Loaded += OnFrameworkElement_Loaded;
                }
            }
        }

        /// <summary>
        /// This method is executed when a AvalonDock <seealso cref="DockingManager"/> instance fires the
        /// Load standard (FrameworkElement) event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnFrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;

            // Sanity check just in case this was somehow send by something else
            if (frameworkElement == null)
                return;

            ICommand loadLayoutCommand = AvalonDockLayoutSerializer.GetLoadLayoutCommand(frameworkElement);

            // There may not be a command bound to this after all
            if (loadLayoutCommand == null)
                return;

            // Check whether this attached behavior is bound to a RoutedCommand
            if (loadLayoutCommand is RoutedCommand)
            {
                // Execute the routed command
                (loadLayoutCommand as RoutedCommand).Execute(frameworkElement, frameworkElement);
            }
            else
            {
                // Execute the Command as bound delegate
                loadLayoutCommand.Execute(frameworkElement);
            }
        }
        /// <summary>
        /// Standard get method of <seealso cref="SaveLayoutCommandProperty"/> dependency property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetSaveLayoutCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(SaveLayoutCommandProperty);
        }

        /// <summary>
        /// Standard get method of <seealso cref="SaveLayoutCommandProperty"/> dependency property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetSaveLayoutCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(SaveLayoutCommandProperty, value);
        }

        /// <summary>
        /// This method is executed if a <seealso cref="SaveLayoutCommandProperty"/> dependency property
        /// is about to change its value (eg: The framework assigns bindings).
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnSaveLayoutCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = d as FrameworkElement;    // Remove the handler if it exist to avoid memory leaks
            if (frameworkElement != null)
            {
                frameworkElement.Unloaded -= OnFrameworkElement_Saveed;

                var command = e.NewValue as ICommand;
                if (command != null)
                {
                    // the property is attached so we attach the Drop event handler
                    frameworkElement.Unloaded += OnFrameworkElement_Saveed;
                }
            }
        }

        /// <summary>
        /// This method is executed when a AvalonDock <seealso cref="DockingManager"/> instance fires the
        /// Unload standard (FrameworkElement) event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnFrameworkElement_Saveed(object sender, RoutedEventArgs e)
        {
            DockingManager frameworkElement = sender as DockingManager;

            // Sanity check just in case this was somehow send by something else
            if (frameworkElement == null)
                return;

            ICommand saveLayoutCommand = GetSaveLayoutCommand(frameworkElement);

            // There may not be a command bound to this after all
            if (saveLayoutCommand == null)
                return;

            string xmlLayoutString;

            using (StringWriter fs = new StringWriter())
            {
                XmlLayoutSerializer xmlLayout = new XmlLayoutSerializer(frameworkElement);

                xmlLayout.Serialize(fs);

                xmlLayoutString = fs.ToString();
            }

            // Check whether this attached behaviour is bound to a RoutedCommand
            if (saveLayoutCommand is RoutedCommand)
            {
                // Execute the routed command
                (saveLayoutCommand as RoutedCommand).Execute(xmlLayoutString, frameworkElement);
            }
            else
            {
                // Execute the Command as bound delegate
                saveLayoutCommand.Execute(xmlLayoutString);
            }
        }

    }
}