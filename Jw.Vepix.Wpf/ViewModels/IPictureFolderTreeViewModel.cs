using System.Collections.ObjectModel;

namespace Jw.Vepix.Wpf.ViewModels
{
    public interface IPictureFolderTreeViewModel
    {
        ObservableCollection<IPictureFolderTreeItemViewModel> PictureFolderItemViewModels { get; }
        void Load(string folder);
    }
}
