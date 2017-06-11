﻿using Jw.Vepix.Core;
using Jw.Vepix.Core.Extensions;
using Jw.Vepix.Core.Interfaces;
using Jw.Vepix.Core.Models;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Payloads;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class PictureGridViewModel : ViewModelBase, IPictureGridViewModel
    {
        public PictureGridViewModel(IPictureRepository pictureRepository, IEventAggregator eventAggregator, IMessageDialogService modalDialog)
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

            SearchCommand = new RelayCommand<string>(OnSearchCommand);
            EditPictureNameCommand = new RelayCommand<Picture>(OnEditPictureNameCommand);
            EditSelectedPictureNamesCommand = new RelayCommand<List<Picture>>(OnEditSelectedPictureNamesCommand);
            CopyPicturesCommand = new RelayCommand<List<Picture>>(OnCopyPicturesCommand);
            MovePicturesCommand = new RelayCommand<List<Picture>>(OnMovePicturesCommand);
            DeletePicturesCommand = new RelayCommand<List<Picture>>(OnDeletePicturesCommand);
            ClosePictureCommand = new RelayCommand<Picture>(OnClosePictureCommand);
            ClosePicturesCommand = new RelayCommand<List<Picture>>(OnClosePicturesCommand);
            ViewEditPicturesCommand = new RelayCommand<List<Picture>>(OnViewEditPicturesCommand);
            PicturesListSelectionChangedCommand = new RelayCommand<int>(OnPicturesListSelectionChangedCommand);
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
        public RelayCommand<Picture> EditPictureNameCommand { get; private set; }
        public RelayCommand<List<Picture>> EditSelectedPictureNamesCommand { get; private set; }
        public RelayCommand<List<Picture>> CopyPicturesCommand { get; private set; }
        public RelayCommand<List<Picture>> MovePicturesCommand { get; private set; }
        public RelayCommand<List<Picture>> DeletePicturesCommand { get; private set; }
        public RelayCommand<Picture> ClosePictureCommand { get; private set; }
        public RelayCommand<List<Picture>> ClosePicturesCommand { get; private set; }
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
                NotifyPropertyChanged("Pictures");
            }
        }

        private void OnEditPictureNameCommand(Picture picture)
        {
            new MessageDialogService().ShowInput("Edit Picture Name", picture.ImageName);
        }

        private void OnEditSelectedPictureNamesCommand(List<Picture> pictures)
        {
            if (pictures.Count == 1)
            {
                new MessageDialogService().ShowInput("Edit Picture Name", pictures.First().ImageName);
            }
            else
            {
                _eventAggregator.GetEvent<PictureNameChangedEvent>().Subscribe(OnPictureNameChanged);
                new PicturesFyloutService(FlyoutViewType.EditNames)
                    .ShowVepixFlyout<EditNamesViewModel>(pictures);
            }
        }

        private void OnCopyPicturesCommand(List<Picture> pictures)
        {
            // todo: open folder dialog
            throw new NotImplementedException();

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                _eventAggregator.GetEvent<StatusTextHelpInfoEvent>().Publish("Pictures have successfully copied");
                System.Threading.Thread.Sleep(5000);
                _eventAggregator.GetEvent<StatusTextHelpInfoEvent>().Publish(string.Empty);
            });
        }

        private void OnMovePicturesCommand(List<Picture> pictures)
        {
            // todo: open folder dialog
            throw new NotImplementedException();

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                _eventAggregator.GetEvent<StatusTextHelpInfoEvent>().Publish("Pictures have successfully moved");
                System.Threading.Thread.Sleep(5000);
                _eventAggregator.GetEvent<StatusTextHelpInfoEvent>().Publish(string.Empty);
            });
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
                    Pictures.Remove(pic);
                    _pictureRepo.TryDelete(pic.FullFileName);
                });
            }
        }

        private void OnClosePictureCommand(Picture picture)
        {
            Pictures.Remove(picture);
        }

        private void OnClosePicturesCommand(List<Picture> pictures)
        {
            pictures.ForEach(pic => Pictures.Remove(pic));
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
                var index = Pictures.IndexOf(changedPicture);
                Pictures.RemoveAt(index);
                changedPicture.FullFileName = changedPicture.FolderPath + payload.NewPictureName + 
                                              changedPicture.FileExtension;
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
            // "*" is used to differentiate some pictures in a folder as opposed to all pictures in a folder.
            FolderName = pictureFileNames.First().ToFilesFolderName() + "*"; 
            TaskRunner.WaitAllOneByOne(pictureFileNames, _pictureRepo.GetPictureAsync, Pictures.Add,
                () => ArePicturesLoading = false);
        }

        public async void Load(string folderPath)
        {
            Pictures = new ObservableCollection<Picture>();
            FolderName = folderPath.ToFoldersName();

            ArePicturesLoading = true;
            List<string> fileNames = await _pictureRepo.GetFileNamesAsync(folderPath);
            TaskRunner.WaitAllOneByOne(fileNames, _pictureRepo.GetPictureAsync, Pictures.Add,
                () => ArePicturesLoading = false);
        }

        private IPictureRepository _pictureRepo;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _modalDialog;
        private ObservableCollection<Picture> _pictures;
        private string _folderName;
        private bool _filterOn;
        private string _searchFilter;
        private bool _arePicturesLoading;
    }
}
