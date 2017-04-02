using Prism.Events;
using System.Windows;

namespace Jw.Vepix.Wpf.Events
{
    public class CropAreaDrawnEvent : PubSubEvent<Int32Rect>
    {
    }
}
