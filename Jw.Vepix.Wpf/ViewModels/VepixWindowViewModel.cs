using Jw.Vepix.Data;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class VepixWindowViewModel : ViewModelBase
    {
        public VepixWindowViewModel(IPictureFolderTreeViewModel pictureFolderTreeViewModel,
            Func<IPictureGridViewModel> pictureGridViewModelCreator,
            IFileExplorerDialogService fileExplorerDialogService,
            IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            PictureFolderTreeViewModel = pictureFolderTreeViewModel;
            _pictureGridViewModelCreator = pictureGridViewModelCreator;
            _fileExplorerDialogService = fileExplorerDialogService;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenPicturesFromFolderEvent>().Subscribe(OnOpenPicturesFromFolder);

            PictureGridViewModels = new ObservableCollection<IPictureGridViewModel>();

            OpenFolderCommand = new RelayCommand<SearchOption>(OnOpenFolderCommand);
            OpenFilesCommand = new RelayCommand<object>(OnOpenFilesCommand);
            CloseFolderTabCommand = new RelayCommand<IPictureGridViewModel>(OnCloseFolderCommand);
            AboutCommand = new RelayCommand<object>(OnAboutCommand);

            CheckCommandLine();
        }

        public IPictureFolderTreeViewModel PictureFolderTreeViewModel { get; private set; }
        public ObservableCollection<IPictureGridViewModel> PictureGridViewModels { get; private set; }
        public IPictureGridViewModel SelectedPictureGridViewModel
        {
            get
            {
                return _selectedPictureGridViewModel;
            }
            set
            {
                _selectedPictureGridViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public RelayCommand<SearchOption> OpenFolderCommand { get; private set; }
        public RelayCommand<object> OpenFilesCommand { get; private set; }
        public RelayCommand<IPictureGridViewModel> CloseFolderTabCommand { get; private set; }
        public RelayCommand<object> AboutCommand { get; private set; }

        private void OnOpenFolderCommand(SearchOption option)
        {
            string selectedPath;
            if (_fileExplorerDialogService.ShowFolderBrowserDialog(out selectedPath) == DialogResult.OK)
            {
                if (option == SearchOption.AllDirectories)
                {
                    //may want to also add a single folder to the tree; need to think about it more
                    PictureFolderTreeViewModel.Load(selectedPath);
                }

                SelectedPictureGridViewModel = CreateAndLoadPictureGridViewModel(selectedPath);
            }
        }

        private IPictureGridViewModel CreateAndLoadPictureGridViewModel(List<string> pictureFileNames)
        {
            var pictureGridViewModel = _pictureGridViewModelCreator();
            PictureGridViewModels.Add(pictureGridViewModel);
            pictureGridViewModel.Load(pictureFileNames);
            return pictureGridViewModel;
        }

        private IPictureGridViewModel CreateAndLoadPictureGridViewModel(string folderPath)
        {
            // todo: think about what to do if a folder does not contain any photos
            // maybe have Load() return false??
            // maybe just display a message in the tab saying no images in this folder??
            var pictureGridViewModel = _pictureGridViewModelCreator();
            PictureGridViewModels.Add(pictureGridViewModel);
            pictureGridViewModel.Load(folderPath);
            return pictureGridViewModel;
        }

        private void OnOpenFilesCommand()
        {
            string[] fileNames;
            if (_fileExplorerDialogService.ShowOpenFileDialog(out fileNames) == DialogResult.OK)
            {
                SelectedPictureGridViewModel = CreateAndLoadPictureGridViewModel(fileNames.ToList());
            }
        }

        private void OnCloseFolderCommand(IPictureGridViewModel tab)
        {
            PictureGridViewModels.Remove(tab);
        }

        private void OnAboutCommand()
        {
            var message = new System.Text.StringBuilder()
                .Append("Version: ")
                .AppendLine(System.Diagnostics.FileVersionInfo.GetVersionInfo(
                    System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion)
                .AppendLine()
                .AppendLine("Author: Jon Wesneski").ToString();

            new MessageDialogService().ShowMessage("vepix - About", message);
        }

        private void OnOpenPicturesFromFolder(PicturesFolderPayload picturesFolder)
        {
            SelectedPictureGridViewModel = CreateAndLoadPictureGridViewModel(picturesFolder.AbsolutePath);
        }

        private void CheckCommandLine()
        {
            // todo: need to pass the searh patterns too
            var userInputs = VepixConsoleParser.ConsoleInstance();
            foreach (var allDir in userInputs.AllDirectories)
            {
                PictureFolderTreeViewModel.Load(allDir);
            }

            var firstLoad = true;
            foreach (var topDir in userInputs.TopDirectories)
            {
                if (firstLoad)
                {
                    SelectedPictureGridViewModel = CreateAndLoadPictureGridViewModel(topDir);
                    firstLoad = false;
                }
                else
                {
                    CreateAndLoadPictureGridViewModel(topDir);
                }
            }
        }

        private IEventAggregator _eventAggregator;
        private Func<IPictureGridViewModel> _pictureGridViewModelCreator;
        private IPictureGridViewModel _selectedPictureGridViewModel;
        private IFileExplorerDialogService _fileExplorerDialogService;
    }
}
