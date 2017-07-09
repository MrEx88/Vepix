using JW.Vepix.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JW.Vepix.Wpf.ViewModels
{
    public interface IPictureGridViewModel
    {
        string FolderName { get; }
        ObservableCollection<Picture> Pictures { get; }
        void Load(List<string> pictureFileNames);
    }
}
