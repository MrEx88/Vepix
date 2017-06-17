using Jw.Vepix.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Jw.Vepix.Wpf.ViewModels
{
    public interface ICollectionViewModel
    {
        ObservableCollection<Picture> Pictures { get; set; }
        string ViewTitle { get; }
        void Load(List<Picture> pictures);
    }
}
