using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class PictureFolderTreeViewModel : ViewModelBase, IPictureFolderTreeViewModel
    {
        public PictureFolderTreeViewModel(IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            _pictureFolderItemViewModels = new ObservableCollection<PictureFolderTreeItemViewModel>();
            _eventAggregator = eventAggregator;
        }

        public ObservableCollection<PictureFolderTreeItemViewModel> PictureFolderItemViewModels
        {
            get { return _pictureFolderItemViewModels; }
            set
            {
                _pictureFolderItemViewModels = value;
                NotifyPropertyChanged();
            }
        }

        public void Load(List<string> folders)
        {
            var dirInfo = new DirectoryInfo(folders.First());

            _rootFolder = new PictureFolderTreeItemViewModel(dirInfo, _eventAggregator);

            PictureFolderItemViewModels = new ObservableCollection<PictureFolderTreeItemViewModel>(
                new PictureFolderTreeItemViewModel[]
                {
                    _rootFolder
                });
        }

        private PictureFolderTreeItemViewModel _rootFolder;
        private ObservableCollection<PictureFolderTreeItemViewModel> _pictureFolderItemViewModels;
        private IEventAggregator _eventAggregator;
    }
}
