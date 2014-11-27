using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WavePlayer.UI.Behaviors
{
    public static class DeferredLoadBehavior
    {
        public static readonly DependencyProperty LoadItemsCommandProperty = DependencyProperty.RegisterAttached("LoadItemsCommand",
            typeof(ICommand),
            typeof(DeferredLoadBehavior),
            new UIPropertyMetadata(default(ICommand), LoadItemsCommandPropertyChanged));

        public static readonly DependencyProperty ScrollOrientationProperty = DependencyProperty.RegisterAttached("ScrollOrientation",
            typeof(Orientation),
            typeof(DeferredLoadBehavior),
            new UIPropertyMetadata(Orientation.Vertical));

        public static ICommand GetLoadItemsCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LoadItemsCommandProperty);
        }

        public static void SetLoadItemsCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadItemsCommandProperty, value);
        }

        public static Orientation GetScrollOrientation(DependencyObject obj)
        {
            return (Orientation)obj.GetValue(ScrollOrientationProperty);
        }

        public static void SetScrollOrientation(DependencyObject obj, Orientation value)
        {
            obj.SetValue(ScrollOrientationProperty, value);
        }

        private static void LoadItemsCommandPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var listBox = dependencyObject as ListBox;

            if (listBox == null)
            {
                return;
            }

            var eventHandler = new ScrollChangedEventHandler(OnScrollChanged);

            if (args.OldValue is ICommand)
            {
                listBox.RemoveHandler(ScrollViewer.ScrollChangedEvent, eventHandler);
            }

            if (args.NewValue is ICommand)
            {
                listBox.AddHandler(ScrollViewer.ScrollChangedEvent, eventHandler);
            }
        }

        private static void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var listBox = sender as ListBox;

            if (listBox == null)
            {
                return;
            }

            var orientation = GetScrollOrientation(listBox);

            if ((orientation == Orientation.Vertical && (
                e.VerticalChange <= 0 ||
                e.VerticalOffset + e.ViewportHeight < e.ExtentHeight)) ||
                (orientation == Orientation.Horizontal && (
                e.HorizontalChange <= 0 ||
                e.HorizontalOffset + e.ViewportWidth < e.ExtentWidth)))
            {
                return;
            }

            var command = GetLoadItemsCommand(listBox);

            if (command == null ||
                !command.CanExecute(null))
            {
                return;
            }

            command.Execute(null);
        }
    }
}
