using System.Collections.ObjectModel;

namespace JW.Vepix.Wpf.ViewModels
{
    public interface IPictureFolderTreeViewModel
    {
        ObservableCollection<IPictureFolderTreeItemViewModel> PictureFolderItemViewModels { get; }
        bool TryLoad(string folder);
    }
}
