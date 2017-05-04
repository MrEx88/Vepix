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
            IEventAggregator eventAggregator, IPictureRepository pictureRepo)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            PictureFolderTreeViewModel = pictureFolderTreeViewModel;
            PictureGridViewModels = new ObservableCollection<IPictureGridViewModel>();
            _pictureGridViewModelCreator = pictureGridViewModelCreator;

            _fileExplorerDialogService = fileExplorerDialogService;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenPicturesFromFolderEvent>().Subscribe(OnOpenPicturesFromFolder);

            _pictureRepo = pictureRepo;

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

        private async void OnOpenFolderCommand(SearchOption option)
        {
            string selectedPath;
            DialogResult d = _fileExplorerDialogService.ShowFolderBrowserDialog(out selectedPath);
            if (_fileExplorerDialogService.ShowFolderBrowserDialog(out selectedPath) == DialogResult.OK)
            {
                if (option == SearchOption.AllDirectories)
                {
                    //may want to also add a single folder to the tree; need to think about it more
                    PictureFolderTreeViewModel.Load(selectedPath);
                }
                List<Picture> pictures = await (_pictureRepo.GetPicturesFromFolderAsync(selectedPath, option));
                SelectedPictureGridViewModel = CreateAndLoadPictureGridViewModel(pictures.Where(pic => pic.FolderPath == pictures[0].FolderPath).ToList());
            }
        }

        private IPictureGridViewModel CreateAndLoadPictureGridViewModel(List<Picture> pictures)
        {
            var pictureGridViewModel = _pictureGridViewModelCreator();
            PictureGridViewModels.Add(pictureGridViewModel);
            pictureGridViewModel.Load(pictures);
            return pictureGridViewModel;
        }

        private async void OnOpenFilesCommand()
        {
            string[] fileNames;
            if (_fileExplorerDialogService.ShowOpenFileDialog(out fileNames) == DialogResult.OK)
            {
                List<Picture> pictures = await (_pictureRepo.GetPicturesAsync(fileNames));
                SelectedPictureGridViewModel = CreateAndLoadPictureGridViewModel(pictures);
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

            new MessageDialogService().ShowDialog(message, "vepix - About");
        }

        private async void OnOpenPicturesFromFolder(PicturesFolderPayload picturesFolder)
        {
            List<Picture> pictures = await(_pictureRepo.GetPicturesFromFolderAsync(picturesFolder.AbsolutePath, SearchOption.TopDirectoryOnly));
            if (pictures.Count > 0)
            {
                SelectedPictureGridViewModel = CreateAndLoadPictureGridViewModel(pictures);
            }
        }

        private async void CheckCommandLine()
        {
            var userInputs = VepixConsoleParser.ConsoleInstance();
            foreach (var allDir in userInputs.AllDirectories)
            {
                PictureFolderTreeViewModel.Load(allDir);
            }

            var firstLoad = true;
            foreach (var topDir in userInputs.TopDirectories)
            {
                List<Picture> pictures = await (_pictureRepo.GetPicturesFromFolderAsync(topDir, SearchOption.TopDirectoryOnly, userInputs.SearchPatterns.ToArray()));
                if (pictures.Count > 0)
                {
                    if (firstLoad)
                    {
                        SelectedPictureGridViewModel = CreateAndLoadPictureGridViewModel(pictures.Where(pic => pic.FolderPath == pictures[0].FolderPath).ToList());
                        firstLoad = false;
                    }
                    else
                    {
                         CreateAndLoadPictureGridViewModel(pictures.Where(pic => pic.FolderPath == pictures[0].FolderPath).ToList());
                    }
                }
            }
        }

        private IEventAggregator _eventAggregator;
        private IPictureRepository _pictureRepo;
        private Func<IPictureGridViewModel> _pictureGridViewModelCreator;
        private IPictureGridViewModel _selectedPictureGridViewModel;
        private IFileExplorerDialogService _fileExplorerDialogService;
    }
}
