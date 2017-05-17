using Jw.Vepix.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Jw.Vepix.Wpf.ViewModels
{
    public interface ICollectionViewModel
    {
        ObservableCollection<Picture> Pictures { get; set; }
        void Load(List<Picture> pictures);
    }
}
