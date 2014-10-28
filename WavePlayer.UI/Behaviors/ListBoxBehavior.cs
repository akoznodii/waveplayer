using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using ListBox = System.Windows.Controls.ListBox;

namespace WavePlayer.UI.Behaviors
{
    public static class ListBoxBehavior
    {
        public static readonly DependencyProperty LoadItemsCommandProperty = DependencyProperty.RegisterAttached("LoadItemsCommand",
            typeof(ICommand),
            typeof(ListBoxBehavior),
            new UIPropertyMetadata(default(ICommand), LoadItemsCommandPropertyChanged));

        public static ICommand GetLoadItemsCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LoadItemsCommandProperty);
        }

        public static void SetLoadItemsCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadItemsCommandProperty, value);
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

            if (e.VerticalChange <= 0 || 
                e.VerticalOffset + e.ViewportHeight < e.ExtentHeight)
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
