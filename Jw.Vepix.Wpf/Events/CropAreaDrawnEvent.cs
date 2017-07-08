using Prism.Events;
using System.Windows;

namespace JW.Vepix.Wpf.Events
{
    public class CropAreaDrawnEvent : PubSubEvent<Int32Rect>
    {
    }
}
