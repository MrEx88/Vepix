using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Jw.Vepix.Wpf.ViewModels
{
    public interface IPictureFolderTreeViewModel
    {
        ObservableCollection<PictureFolderTreeItemViewModel> PictureFolderItemViewModels { get; }
        void Load(List<string> folders);
    }
}
