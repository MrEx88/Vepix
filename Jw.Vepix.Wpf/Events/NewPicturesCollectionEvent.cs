using Jw.Data;
using Prism.Events;
using System.Collections.Generic;

namespace Jw.Vepix.Wpf.Events
{
    public class NewPicturesCollectionEvent : PubSubEvent<List<Picture>>
    {
    }
}
