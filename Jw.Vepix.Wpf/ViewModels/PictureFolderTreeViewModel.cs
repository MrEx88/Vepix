using Jw.Vepix.Core.Extensions;
using Prism.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class PictureFolderTreeViewModel : ViewModelBase, IPictureFolderTreeViewModel
    {
        public PictureFolderTreeViewModel(IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            _eventAggregator = eventAggregator;

            _pictureFolderItemViewModels = new ObservableCollection<IPictureFolderTreeItemViewModel>();
        }

        public ObservableCollection<IPictureFolderTreeItemViewModel> PictureFolderItemViewModels
        {
            get { return _pictureFolderItemViewModels; }
            set
            {
                _pictureFolderItemViewModels = value;
                NotifyPropertyChanged();
            }
        }

        public void Load(string folder)
        {
            var dirInfo = new DirectoryInfo(folder);

            var treeItem = new PictureFolderTreeItemViewModel(dirInfo, _eventAggregator);
            TryAdd(treeItem);
        }

        private bool TryAdd(PictureFolderTreeItemViewModel treeItem)
        {
            if (PictureFolderItemViewModels.Count != 0)
            {
                // Check if folder already exists.
                foreach (var folderTreeItem in PictureFolderItemViewModels)
                {
                    if(folderTreeItem.TreeItemAlreadyExists(treeItem.AbsolutePath))
                    {
                        return false;
                    }
                }

                // Check if folder is Parent to existing folder(s).
                PictureFolderItemViewModels.RemoveAll(folderItem => folderItem.IsAParentTo(treeItem));
            }

            PictureFolderItemViewModels.Add(treeItem);

            return true;
        }

        private ObservableCollection<IPictureFolderTreeItemViewModel> _pictureFolderItemViewModels;
        private IEventAggregator _eventAggregator;
    }
}
