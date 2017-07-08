using Prism.Events;
using System.Windows;

namespace J.Vepix.Wpf.Events
{
    public class CropAreaDrawnEvent : PubSubEvent<Int32Rect>
    {
    }
}
