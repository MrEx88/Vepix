using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace JW.Vepix.Wpf.Controls
{
    public static class ControlHelper
    {
        public static IEnumerable<T> RecurseChildren<T>(DependencyObject root) where T : UIElement
        {
            if (root is T)
            {
                yield return root as T;
            }

            if (root != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(root);

                for (var index = 0; index < count; index++)
                {
                    foreach (var child in RecurseChildren<T>(VisualTreeHelper.GetChild(root, index)))
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}
