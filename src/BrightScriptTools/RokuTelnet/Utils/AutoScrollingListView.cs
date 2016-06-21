using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RokuTelnet.Utils
{
    public class AutoScrollingListView : ListView
    {
        private ScrollViewer _scrollViewer;

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            if (oldValue is INotifyCollectionChanged)
                ((INotifyCollectionChanged) oldValue).CollectionChanged -= ItemsCollectionChanged;

            if (newValue is INotifyCollectionChanged)
                ((INotifyCollectionChanged) newValue).CollectionChanged += ItemsCollectionChanged;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Dig out and store a reference to our internal ScrollViewer
            _scrollViewer = RecursiveVisualChildFinder<ScrollViewer>(this) as ScrollViewer;

        }

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (_scrollViewer == null) return;

            if (!_scrollViewer.VerticalOffset.Equals(_scrollViewer.ScrollableHeight)) return;

            UpdateLayout();
            _scrollViewer.ScrollToBottom();
        }

        private static DependencyObject RecursiveVisualChildFinder<T>(DependencyObject rootObject)
        {
            var child = VisualTreeHelper.GetChild(rootObject, 0);
            if (child == null) return null;

            return child.GetType() == typeof(T) ? child : RecursiveVisualChildFinder<T>(child);
        }

    }
}