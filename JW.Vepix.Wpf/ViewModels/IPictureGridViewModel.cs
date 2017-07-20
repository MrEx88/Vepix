using JW.Vepix.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JW.Vepix.Wpf.ViewModels
{
    public interface IPictureGridViewModel
    {
        ObservableCollection<Picture> Pictures { get; }
        void Load(List<string> pictureFileNames);
        void LoadEmptyFolder(string folderPath);
    }
}
