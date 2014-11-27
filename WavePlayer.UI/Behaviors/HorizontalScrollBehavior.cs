using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace WavePlayer.UI.Behaviors
{
    public class HorizontalScrollBehavior : Behavior<ItemsControl>
    {
        /// <summary>
        /// A reference to the internal ScrollViewer.
        /// </summary>
        private ScrollViewer ScrollViewer { get; set; }

        /// <summary>
        /// By default, scrolling down on the wheel translates to right, and up to left.
        /// Set this to true to invert that translation.
        /// </summary>
        public bool IsInverted { get; set; }

        /// <summary>
        /// The ScrollViewer is not available in the visual tree until the control is loaded.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= OnLoaded;

            ScrollViewer = FindVisualChild<ScrollViewer>(AssociatedObject);

            if (ScrollViewer != null)
            {
                ScrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (ScrollViewer != null)
            {
                ScrollViewer.PreviewMouseWheel -= OnPreviewMouseWheel;
            }
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var newOffset = IsInverted ?
                ScrollViewer.HorizontalOffset + e.Delta :
                ScrollViewer.HorizontalOffset - e.Delta;

            ScrollViewer.ScrollToHorizontalOffset(newOffset);
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }
}
