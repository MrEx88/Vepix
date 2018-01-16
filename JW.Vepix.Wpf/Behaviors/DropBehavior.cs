using System.Windows;
using System.Windows.Input;

namespace JW.Vepix.Wpf.Behaviors
{
    public static class DropBehavior
    {
        private static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.RegisterAttached
            (
                "DropCommand",
                typeof(ICommand),
                typeof(DropBehavior),
                new PropertyMetadata(DropCommandPropertyChangedCallback)
            );

        private static void DropCommandPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uiElement = d as UIElement;
            if (uiElement != null)
            {
                uiElement.Drop += (sender, args) =>
                {
                    GetDropCommand(uiElement).Execute(args.Data);
                    args.Handled = true;
                };
            }
        }

        private static ICommand GetDropCommand(UIElement uiElement)
        {
            return (ICommand)uiElement.GetValue(DropCommandProperty);
        }

        public static void SetDropCommand(this UIElement uiElement, ICommand inCommand)
        {
            uiElement.SetValue(DropCommandProperty, inCommand);
        }
    }
}
