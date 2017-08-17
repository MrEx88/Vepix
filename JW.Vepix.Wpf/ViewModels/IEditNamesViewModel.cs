using JW.Vepix.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JW.Vepix.Wpf.ViewModels
{
    public interface IEditNamesViewModel
    {
        ObservableCollection<AffixedPictureName> EditPictureNames { get; set; }
        void Load(List<Picture> pictures);
    }
}
