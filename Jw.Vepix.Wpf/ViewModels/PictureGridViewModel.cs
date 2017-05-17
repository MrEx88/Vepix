using Jw.Vepix.Common;
using Jw.Vepix.Data;
using Jw.Vepix.Data.Payloads;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class PictureGridViewModel : ViewModelBase, IPictureGridViewModel
    {
        public PictureGridViewModel(IPictureRepository pictureRepository, IMessageDialogService modalDialog, IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            _pictureRepo = pictureRepository;

            _modalDialog = modalDialog;

            _eventAggregator = eventAggregator;
            //_eventAggregator.GetEvent<PictureNameChangedEvent>().Subscribe(OnPictureNameChanged);
            _eventAggregator.GetEvent<PictureOverwrittenEvent>().Subscribe(OnPictureOverwritten);
            
            ArePicturesLoading = false;

            _pictures = new ObservableCollection<Picture>();
            _filterOn = false;
            _searchFilter = string.Empty;

            SearchCommand = new RelayCommand<string>(OnSearch);
            EditImageNameCommand = new RelayCommand<Picture>(OnEditImageName);
            EditSelectedImageNamesCommand = new RelayCommand<object>(OnEditSelectedImageNamesCommand);
            CropImageCommand = new RelayCommand<Picture>(OnCropImage);
            DeleteImageCommand = new RelayCommand<Picture>(OnDeleteImage);
            CloseImageCommand = new RelayCommand<Picture>(OnCloseImage);            
        }

        public string FolderName
        {
            get { return _folderName; }
            set
            {
                if (value != _folderName)
                {
                    _folderName = value;
                    NotifyPropertyChanged(); 
                }
            }
        }

        public bool ArePicturesLoading
        {
            get { return _arePicturesLoading; }
            set
            {
                if (value != _arePicturesLoading)
                {
                    _arePicturesLoading = value;
                    NotifyPropertyChanged(); 
                }
            }
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
        public RelayCommand<object> EditSelectedImageNamesCommand { get; private set; }
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
            new MessageDialogService().ShowInput("Edit Image Name", picture.ImageName);
        }

        private void OnEditSelectedImageNamesCommand()
        {
            _eventAggregator.GetEvent<PictureNameChangedEvent>().Subscribe(OnPictureNameChanged);
            new PicturesFyloutService(FlyoutViewType.EditNames)
                .ShowVepixFlyout<EditNamesViewModel>(Pictures.ToList());
        }

        private void OnCropImage(Picture picture)
        {
            new PicturesFyloutService(FlyoutViewType.Viewer)
                .ShowVepixFlyout<PicturesViewerViewModel>(Pictures.ToList());
        }

        private void OnDeleteImage(Picture picture)
        {
            if (_modalDialog.ShowQuestion(
                $"Are you sure you want to delete this image:\n\n\"{picture.FullFileName}\"\n",
                "Delete Image?"))
            {
                Pictures.Remove(picture);
                _pictureRepo.TryDelete(picture.FullFileName);
            }
        }

        private void OnCloseImage(Picture picture)
        {
            Pictures.Remove(picture);
        }

        private void OnPictureNameChanged(PictureNameChangePayload payload)
        {
            var changedPicture = Pictures.FirstOrDefault(pic => pic.Guid == payload.Guid);
            if (changedPicture != null)
            {
                var index = Pictures.IndexOf(changedPicture);
                Pictures.RemoveAt(index);
                changedPicture.FullFileName = changedPicture.FolderPath + payload.NewPictureName + changedPicture.FileExtension;
                Pictures.Insert(index, changedPicture);
                NotifyPropertyChanged(() => Pictures);
            }

            _eventAggregator.GetEvent<PictureNameChangedEvent>().Unsubscribe(OnPictureNameChanged);
        }

        private void OnPictureOverwritten(Guid guid)
        {
            var picture = Pictures.First(pic => pic.Guid == guid);
            var reloadedPicture = _pictureRepo.GetPicturesAsync(new string[] { picture.FullFileName }).Result;
            Pictures.Remove(picture);
            Pictures.Add(reloadedPicture.First());
        }

        public void Load(List<string> pictureFileNames)
        {
            Pictures = new ObservableCollection<Picture>();
            ArePicturesLoading = true;
            FolderName = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(pictureFileNames.First())).Name + "*";
            TaskRunner.WaitAllOneByOne(pictureFileNames, _pictureRepo.GetPictureAsync, Pictures.Add,
                () => ArePicturesLoading = false);
        }

        public async void Load(string folderPath)
        {
            Pictures = new ObservableCollection<Picture>();
            FolderName = new System.IO.DirectoryInfo(folderPath).Name;

            ArePicturesLoading = true;
            List<string> fileNames = await _pictureRepo.GetFileNamesAsync(folderPath);
            TaskRunner.WaitAllOneByOne(fileNames, _pictureRepo.GetPictureAsync, Pictures.Add,
                () => ArePicturesLoading = false);
        }

        private IEventAggregator _eventAggregator;
        private IMessageDialogService _modalDialog;
        private ObservableCollection<Picture> _pictures;
        private string _folderName;
        private bool _filterOn;
        private string _searchFilter;
        private IPictureRepository _pictureRepo;
        private bool _arePicturesLoading;
    }
}
