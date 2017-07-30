using JW.Vepix.Core;
using JW.Vepix.Core.Extensions;
using JW.Vepix.Core.Interfaces;
using JW.Vepix.Core.Models;
using JW.Vepix.Wpf.Events;
using JW.Vepix.Wpf.Payloads;
using JW.Vepix.Wpf.Services;
using JW.Vepix.Wpf.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace JW.Vepix.Wpf.ViewModels
{
    public class PictureGridViewModel : ViewModelBase, IPictureGridViewModel
    {
        public PictureGridViewModel(IPictureRepository pictureRepository, 
                                    IEventAggregator eventAggregator,
                                    IMessageDialogService modalDialog, 
                                    IFileExplorerDialogService fileExplorerDialogService)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }

            _pictureRepository = pictureRepository;

            _modalDialog = modalDialog;

            _fileExplorerDialogService = fileExplorerDialogService;

            _eventAggregator = eventAggregator;
            //_eventAggregator.GetEvent<PictureNameChangedEvent>().Subscribe(OnPictureNameChanged);
            _eventAggregator.GetEvent<PictureOverwrittenEvent>().Subscribe(OnPictureOverwritten);

            ArePicturesLoading = false;

            _pictures = new ObservableCollection<Picture>();
            _filterOn = false;
            _searchFilter = string.Empty;

            SearchCommand = new RelayCommand<string>(OnSearchCommand);
            EditPictureNameCommand = new RelayCommand<Picture>(OnEditPictureNameCommand);
            EditSelectedPictureNamesCommand = new RelayCommand<List<Picture>>(OnEditSelectedPictureNamesCommand);
            ClosePictureCommand = new RelayCommand<Picture>(OnClosePictureCommand);
            ClosePicturesCommand = new RelayCommand<List<Picture>>(OnClosePicturesCommand);
            CopyPicturesCommand = new RelayCommand<List<Picture>>(OnCopyPicturesCommand);
            MovePicturesCommand = new RelayCommand<List<Picture>>(OnMovePicturesCommand);
            DeletePicturesCommand = new RelayCommand<List<Picture>>(OnDeletePicturesCommand);
            ViewEditPicturesCommand = new RelayCommand<List<Picture>>(OnViewEditPicturesCommand);
            PicturesListSelectionChangedCommand = new RelayCommand<int>(OnPicturesListSelectionChangedCommand);
        }

        public string AbsolutePath
        {
            get { return _absolutePath; }
            set
            {
                if (value != _absolutePath)
                {
                    _absolutePath = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(() => ViewTitle);
                }
            }
        }

        public override string ViewTitle => _absolutePath.ToFoldersName();

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

        public bool HasNoPictures => !(Pictures.Count > 0 || ArePicturesLoading);

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
                if (value != _pictures)
                {
                    _pictures = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(() => HasNoPictures); 
                }
            }
        }

        public Picture SelectedPicture { get; private set; }

        public void Load(List<string> pictureFileNames)
        {
            AbsolutePath = pictureFileNames.First().ToParentFolderPath();
            ArePicturesLoading = true;
            TaskRunner.WaitAllOneByOne(pictureFileNames, _pictureRepository.GetPictureAsync, Pictures.Add,
                () => ArePicturesLoading = false);
        }

        public void LoadEmptyFolder(string folderPath)
        {
            AbsolutePath = folderPath;
        }

        public RelayCommand<string> SearchCommand { get; private set; }
        public RelayCommand<Picture> EditPictureNameCommand { get; private set; }
        public RelayCommand<List<Picture>> EditSelectedPictureNamesCommand { get; private set; }
        public RelayCommand<Picture> ClosePictureCommand { get; private set; }
        public RelayCommand<List<Picture>> ClosePicturesCommand { get; private set; }
        public RelayCommand<List<Picture>> CopyPicturesCommand { get; private set; }
        public RelayCommand<List<Picture>> MovePicturesCommand { get; private set; }
        public RelayCommand<List<Picture>> DeletePicturesCommand { get; private set; }
        public RelayCommand<List<Picture>> ViewEditPicturesCommand { get; private set; }
        public RelayCommand<int> PicturesListSelectionChangedCommand { get; private set; }

        private void OnSearchCommand(string searchFilter)
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
                NotifyPropertyChanged(() => Pictures);
            }
        }

        private async void OnEditPictureNameCommand(Picture picture)
        {
            var newName = await _modalDialog.ShowInput("Edit Picture Name",
                                                       picture.ImageName,
                                                       picture.ImageName);

            if (string.IsNullOrWhiteSpace(newName))
            {
                return;
            }

            if (_pictureRepository.TryChangePictureName(picture, newName).Success.Value)
            {
                picture.FullFileName = newName;
            }
            else
            {
                _modalDialog.ShowMessage("Invalid File name", "Name was either invalid or already exists.");
            }
        }

        private void OnEditSelectedPictureNamesCommand(List<Picture> pictures)
        {
            //var b = pictures[0].IsAnythingDirty(); //just testing it out
            if (pictures.Count == 1)
            {
                OnEditPictureNameCommand(pictures.First());
            }
            else
            {
                _eventAggregator.GetEvent<PictureNameChangedEvent>().Subscribe(OnPictureNameChanged);
                new PicturesFyloutService(FlyoutViewType.EditNames)
                    .ShowVepixFlyout<EditNamesViewModel>(pictures);
            }
        }

        private void OnClosePictureCommand(Picture picture)
        {
            RemovePicture(picture);
        }

        private void OnClosePicturesCommand(List<Picture> pictures)
        {
            pictures.ForEach(pic => RemovePicture(pic));
        }

        private void OnCopyPicturesCommand(List<Picture> pictures)
        {
            string folderPath;
            if (_fileExplorerDialogService.ShowFolderBrowserDialog(out folderPath))
            { 
                var uncopiedPictures = new List<string>();
                pictures.ToList().ForEach(pic =>
                {
                    if (!_pictureRepository.TryCopy(pic, folderPath).Success.Value)
                    {
                        uncopiedPictures.Add(pic.FullFileName);
                    }
                });

                if (uncopiedPictures.Count > 0)
                {
                    var fileNames = string.Join("\n", uncopiedPictures);
                    _modalDialog.ShowMessage("Failed to copy these pictures", fileNames);
                }
                else
                {
                    _eventAggregator.GetEvent<StatusTextHelpInfoEvent>().Publish("All Pictures have successfully copied");
                }
            }
        }

        private void OnMovePicturesCommand(List<Picture> pictures)
        {
            string folderPath;
            if (_fileExplorerDialogService.ShowFolderBrowserDialog(out folderPath))
            {
                var unmovedPictures = new List<string>();
                pictures.ToList().ForEach(pic =>
                {
                    if (!_pictureRepository.TryMove(pic, folderPath).Success.Value)
                    {
                        unmovedPictures.Add(pic.FullFileName);
                    }
                    else
                    {
                        RemovePicture(pic);
                    }
                });

                if (unmovedPictures.Count > 0)
                {
                    var fileNames = string.Join("\n", unmovedPictures);
                    _modalDialog.ShowMessage("Failed to move these pictures", fileNames);
                }
                else
                {
                    _eventAggregator.GetEvent<StatusTextHelpInfoEvent>().Publish("All Pictures have successfully moved");
                }
            }
        }

        private void OnDeletePicturesCommand(List<Picture> pictures)
        {
            var pictureContext = pictures.Count > 1 ? "these pictures" : "this picture";
            var pictureNames = string.Join("\n", pictures.Select(pic => pic.FullFileName).ToList());
            if (_modalDialog.ShowQuestion(
                $"Are you sure you want to delete {pictureContext}:\n\n{pictureNames}\n",
                $"Delete {pictureContext}?"))
            {
                pictures.ForEach(pic =>
                {
                    if (_pictureRepository.TryDelete(pic.FullFileName).Success.Value)
                    {
                        Pictures.Remove(pic);
                    };
                });
            }
        }

        private void OnViewEditPicturesCommand(List<Picture> pictures)
        {
            new PicturesFyloutService(FlyoutViewType.Viewer)
                .ShowVepixFlyout<PicturesViewerViewModel>(pictures);
        }

        private void OnPicturesListSelectionChangedCommand(int count)
        {
            _eventAggregator.GetEvent<StatusTextUserActionEvent>()
                .Publish(UserActionStatusTextBuilder(count));
        }

        private void RemovePicture(Picture picture)
        {
            Pictures.Remove(picture);
            NotifyPropertyChanged(() => HasNoPictures);
        }

        private string UserActionStatusTextBuilder(int count)
        {
            var itemContext = _pictures.Count == 1 ? "item" : "items";
            return $"{_pictures.Count} {itemContext} \t{count} {itemContext} selected"; 
        }

        private void OnPictureNameChanged(PictureNameChangePayload payload)
        {
            var changedPicture = Pictures.FirstOrDefault(pic => pic.Guid == payload.Guid);
            if (changedPicture != null)
            {
                changedPicture.FullFileName = changedPicture.FolderPath + payload.NewPictureName + 
                                              changedPicture.FileExtension;
            }

            _eventAggregator.GetEvent<PictureNameChangedEvent>().Unsubscribe(OnPictureNameChanged);
        }

        private void OnPictureOverwritten(Guid guid)
        {
            var picture = Pictures.First(pic => pic.Guid == guid);
            var reloadedPicture = _pictureRepository.GetPicturesAsync(new string[] { picture.FullFileName }).Result;
            Pictures.Remove(picture);
            Pictures.Add(reloadedPicture.First());
        }

        private IPictureRepository _pictureRepository;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _modalDialog;
        private IFileExplorerDialogService _fileExplorerDialogService;
        private ObservableCollection<Picture> _pictures;
        private string _absolutePath;
        private bool _filterOn;
        private string _searchFilter;
        private bool _arePicturesLoading;
    }
}
