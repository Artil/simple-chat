using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatDesktop.Helpers
{
    public static class AutoScrollHelper
    {
        public static readonly DependencyProperty AutoScrollProperty =
        DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(AutoScrollHelper), new PropertyMetadata(false, AutoScrollPropertyChanged));

        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.Register("Command",typeof(ICommand),typeof(AutoScrollHelper));


        public static void AutoScrollPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var scrollViewer = obj as ScrollViewer;
            if (scrollViewer != null && (bool)args.NewValue)
            {
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                scrollViewer.ScrollToEnd();
            }
            else
                scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
        }

        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;

            if (scrollViewer is null || scrollViewer.ActualHeight == 0)
                return;

            if (scrollViewer.VerticalOffset == 0)
            {
                if (e.ExtentHeightChange > 0)
                {
                    scrollViewer?.ScrollToVerticalOffset(e.ExtentHeightChange);
                    return;
                }

                var command = GetCommand(scrollViewer);
                if (command is null) return;

                if (command.CanExecute(null))
                    command.Execute(null);
            }

            // Only scroll to bottom when the extent changed. Otherwise you can't scroll up
            if (e.ExtentHeightChange != 0)
            {
                if (e.ExtentHeightChange < 0 && scrollViewer.VerticalOffset != scrollViewer.ScrollableHeight)
                {
                    scrollViewer?.ScrollToVerticalOffset(scrollViewer.VerticalOffset + e.ExtentHeightChange);
                    return;
                }

                scrollViewer?.ScrollToBottom();
            }

        }

        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }
    }
}
