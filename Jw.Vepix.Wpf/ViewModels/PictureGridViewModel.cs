using Jw.Vepix.Data;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class PictureGridViewModel : ViewModelBase, IPictureGridViewModel
    {
        public PictureGridViewModel(IMessageDialogService modalDialog, IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            _pictures = new ObservableCollection<Picture>();
            _filterOn = false;
            _searchFilter = string.Empty;
            _pictureRepo = new PictureRepository();
            _modalDialog = modalDialog;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PictureNameChangedEvent>().Subscribe(OnPictureNameChanged);

            SearchCommand = new RelayCommand<string>(OnSearch);
            EditImageNameCommand = new RelayCommand<Picture>(OnEditImageName);
            CropImageCommand = new RelayCommand<Picture>(OnCropImage);
            DeleteImageCommand = new RelayCommand<Picture>(OnDeleteImage);
            CloseImageCommand = new RelayCommand<Picture>(OnCloseImage);            
        }

        public string FolderName
        {
            get { return _folderName; }
            set { _folderName = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<Picture> Pictures
        {
            get
            {
                return _filterOn ?
                    new ObservableCollection<Picture>(
                        _pictures.Where(pic => pic.ImageName.Contains(_searchFilter)).ToList())
                        : _pictures;
            }
            set
            {
                _pictures = value;
                NotifyPropertyChanged();
            }
        }

        public Picture SelectedPicture { get; private set; }
        
        public RelayCommand<string> SearchCommand { get; private set; }
        public RelayCommand<Picture> EditImageNameCommand { get; private set; }
        public RelayCommand<Picture> CropImageCommand { get; private set; }
        public RelayCommand<Picture> DeleteImageCommand { get; private set; }
        public RelayCommand<Picture> CloseImageCommand { get; private set; }

        private void OnSearch(string searchFilter)
        {
            if (searchFilter == string.Empty)
            {
                _filterOn = false;
                Pictures = _pictures;
            }
            else
            {
                _filterOn = true;
                _searchFilter = searchFilter;
                NotifyPropertyChanged("Pictures");
            }
        }

        private void OnEditImageName(Picture picture)
        {
            SelectedPicture = picture;
            ICollectionDialogService dialog = new PictureDialogService(
                DialogType.EditNames,
                _eventAggregator,
                new List<Picture>() { picture });
            dialog.ShowVepixDialog();
        }

        private void OnCropImage(Picture picture)
        {
            ICollectionDialogService dialog = new PictureDialogService(
                DialogType.CropImages,
                _eventAggregator,
                new List<Picture>() { picture });
            dialog.ShowVepixDialog();
        }

        private void OnDeleteImage(Picture picture)
        {
            if (_modalDialog.ShowQuestion(
                $"Are you sure you want to delete this image:\n\n\"{picture.FullFileName}\"\n",
                "Delete Image?"))
            {
                Pictures.Remove(picture);
                _pictureRepo.TryDelete(picture);
            }
        }

        private void OnCloseImage(Picture picture)
        {
            Pictures.Remove(picture);
        }

        private void OnPictureNameChanged(PictureNameChangePayload payload)
        {
            var changedPicture = Pictures.FirstOrDefault(pic => pic.Guid == payload.Guid);
            var index = Pictures.IndexOf(changedPicture);
            Pictures.Remove(changedPicture);
            SelectedPicture.FullFileName =
                SelectedPicture.FolderPath + payload.PictureName + SelectedPicture.FileExtension;
            Pictures.Insert(index, changedPicture);

            //var index = _pictures.IndexOf(SelectedPicture);
            //_pictures.RemoveAt(index);
            //SelectedPicture.FullFileName =
            //    SelectedPicture.FolderPath + payload.PictureName + SelectedPicture.FileExtension;
            //_pictures.Insert(index, SelectedPicture);
            //NotifyPropertyChanged("Pictures");
        }

        public void Load(List<Picture> pictures)
        {
            if (pictures.Count > 0)
                FolderName = pictures.First().FolderName;
            Pictures = new ObservableCollection<Picture>(pictures);
        }

        private IEventAggregator _eventAggregator;
        private IPictureRepository _pictureRepo;
        private IMessageDialogService _modalDialog;
        private ObservableCollection<Picture> _pictures;
        private string _folderName;
        private bool _filterOn;
        private string _searchFilter;        
    }
}
