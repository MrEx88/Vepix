using JW.Vepix.Core;
using JW.Vepix.Core.Extensions;
using JW.Vepix.Core.Interfaces;
using JW.Vepix.Wpf.Events;
using JW.Vepix.Wpf.Payloads;
using JW.Vepix.Wpf.Services;
using JW.Vepix.Wpf.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JW.Vepix.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private Func<IPictureGridViewModel> _pictureGridViewModelCreator;
        private IPictureGridViewModel _selectedPictureGridViewModel;
        private IPictureRepository _pictureRepository;
        private IFileService _fileService;
        private IFileExplorerDialogService _fileExplorerDialogService;
        private IEventAggregator _eventAggregator;
        private string _userActionText;
        private string _helpInfoText;

        public MainViewModel(IFolderTreeViewModel folderTreeViewModel,
                             Func<IPictureGridViewModel> pictureGridViewModelCreator,
                             IPictureRepository pictureRepository,
                             IFileService fileService,
                             IFileExplorerDialogService fileExplorerDialogService,
                             IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }
            
            FolderTreeViewModel = folderTreeViewModel;
            _pictureGridViewModelCreator = pictureGridViewModelCreator;
            _pictureRepository = pictureRepository;
            _fileService = fileService;
            _fileExplorerDialogService = fileExplorerDialogService;

            _helpInfoText = "";

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenPicturesFromFolderEvent>().Subscribe(OnOpenPicturesFromFolder);
            _eventAggregator.GetEvent<StatusTextUserActionEvent>().Subscribe(OnStatusTextUserAction);
            _eventAggregator.GetEvent<StatusTextHelpInfoEvent>().Subscribe(OnStatusTextHelpInfo);
            _eventAggregator.GetEvent<MovingPicturesEvent>().Subscribe(OnMovingPicturesEvent);

            PictureGridViewModels = new ObservableCollection<IPictureGridViewModel>();

            OpenFolderCommand = new RelayCommand<SearchOption>(OnOpenFolderCommand);
            OpenFilesCommand = new RelayCommand<object>(OnOpenFilesCommand);
            CloseFolderTabCommand = new RelayCommand<IPictureGridViewModel>(OnCloseFolderCommand);
            AboutCommand = new RelayCommand<object>(OnAboutCommand);

            CheckCommandLine();
        }

        public IFolderTreeViewModel FolderTreeViewModel { get; private set; }

        public ObservableCollection<IPictureGridViewModel> PictureGridViewModels { get; private set; }

        public IPictureGridViewModel SelectedPictureGridViewModel
        {
            get { return _selectedPictureGridViewModel; }
            set
            {
                if (value != _selectedPictureGridViewModel)
                {
                    _selectedPictureGridViewModel = value;
                    NotifyPropertyChanged(); 
                }
            }
        }

        public string UserActionText
        {
            get { return _userActionText; }
            set
            {
                if (value != _userActionText)
                {
                    _userActionText = value;
                    NotifyPropertyChanged(); 
                }
            }
        }        

        public string  HelpInfoText
        {
            get { return _helpInfoText; }
            set
            {
                if (value != _helpInfoText)
                {
                    _helpInfoText = value;
                    NotifyPropertyChanged(); 
                }
            }
        }

        public IPictureGridViewModel LoadAPictureGridViewModel(List<string> pictureFileNames)
        {
            // Check if folder is already opened.
            var pictureGridViewModel = PictureGridViewModels.ToList().Find(picGrid =>
                ((PictureGridViewModel)picGrid).AbsolutePath == pictureFileNames.First().ToParentFolderPath());
            if (pictureGridViewModel == null)
            {
                pictureGridViewModel = _pictureGridViewModelCreator();
                PictureGridViewModels.Add(pictureGridViewModel);
                pictureGridViewModel.Load(pictureFileNames);
            }
            else
            {
                // Remove any duplicates.
                pictureFileNames.RemoveAll(fileName => PictureGridViewModels.ToList()
                                .Exists(gridViewModel => gridViewModel.Pictures.ToList()
                                .Exists(pic => pic.FullFileName == fileName)));

                if (pictureFileNames.Count > 0)
                {
                    pictureGridViewModel.Load(pictureFileNames);
                }
                else
                {
                    pictureGridViewModel = SelectedPictureGridViewModel;
                }
            }

            return pictureGridViewModel;
        }

        public async Task<IPictureGridViewModel> LoadAPictureGridViewModel(string folderPath) =>
            await LoadAPictureGridViewModel(folderPath, Global.ALL_SUPPORTED_PATTERNS);

        public async Task<IPictureGridViewModel> LoadAPictureGridViewModel(string folderPath, List<string> searchPatterns)
        {
            // Check if folder is already opened.
            //todo: think about if user opens the same folder but with different search patterns
            var pictureGridViewModel = PictureGridViewModels.ToList().Find(picGrid =>
                ((PictureGridViewModel)picGrid).AbsolutePath == folderPath);
            if (pictureGridViewModel == null)
            {
                var pictureFileNames = await _fileService.GetFileNamesFromDirectoryAsync(folderPath, searchPatterns);
                pictureGridViewModel = _pictureGridViewModelCreator();
                PictureGridViewModels.Add(pictureGridViewModel);
                if (pictureFileNames.Count > 0)
                {
                    pictureGridViewModel.Load(pictureFileNames);
                }
                else
                {
                    pictureGridViewModel.LoadEmptyFolder(folderPath);
                }
            }

            return pictureGridViewModel;
        }

        public RelayCommand<SearchOption> OpenFolderCommand { get; private set; }
        public RelayCommand<object> OpenFilesCommand { get; private set; }
        public RelayCommand<IPictureGridViewModel> CloseFolderTabCommand { get; private set; }
        public RelayCommand<object> AboutCommand { get; private set; }

        private async void OnOpenFolderCommand(SearchOption option)
        {
            string selectedPath;
            if (_fileExplorerDialogService.ShowFolderBrowserDialog(out selectedPath))
            {
                if (option == SearchOption.AllDirectories)
                {
                    FolderTreeViewModel.TryLoad(selectedPath);
                }

                SelectedPictureGridViewModel = await LoadAPictureGridViewModel(selectedPath);
            }
        }

        private void OnOpenFilesCommand()
        {
            string[] fileNames;
            if (_fileExplorerDialogService.ShowOpenFileDialog(out fileNames))
            {
                SelectedPictureGridViewModel = LoadAPictureGridViewModel(fileNames.ToList());
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

            new MessageDialogService().ShowMessage("Vepix - About", message);
        }

        private async void OnOpenPicturesFromFolder(PicturesFolderPayload picturesFolder)
        {
            SelectedPictureGridViewModel = await LoadAPictureGridViewModel(picturesFolder.AbsolutePath);
        }

        private void OnStatusTextHelpInfo(string text)
        {
            HelpInfoText = text;
            Task.Factory.StartNew(() =>
            {
                HelpInfoText = text;
                System.Threading.Thread.Sleep(5000);
                HelpInfoText = string.Empty;
            });
        }

        private void OnStatusTextUserAction(string text)
        {
            UserActionText = text;
        }

        private void OnMovingPicturesEvent(MovingPicturesPayload payload)
        {
            var oldFolderPath = payload.Pictures.Select(pic => pic.FolderPath).First();
            var oldPictureGridViewModel = PictureGridViewModels.First(viewModel =>
                ((PictureGridViewModel)viewModel).AbsolutePath == oldFolderPath);

            var newPictureGridViewModel = PictureGridViewModels.First(viewModel =>
                ((PictureGridViewModel)viewModel).AbsolutePath == payload.NewFolderPath);

            payload.Pictures.ForEach(pic =>
                {
                    if (_pictureRepository.TryMove(pic, payload.NewFolderPath).Success.Value)
                    {
                        oldPictureGridViewModel.Pictures.Remove(pic);

                        pic.FullFileName = $"{payload.NewFolderPath}\\{pic.FileName}";
                        newPictureGridViewModel.Pictures.Add(pic);
                    }
                });
        }

        private async void CheckCommandLine()
        {
            var userInputs = VepixCommandLineParser.ResultsInstance();
            foreach (var treeFolder in userInputs.TreeFolders)
            {
                FolderTreeViewModel.TryLoad(treeFolder);
            }

            var searchPatterns = userInputs.SearchPatterns;
            var firstLoad = true;
            foreach (var folder in userInputs.Folders)
            {
                if (firstLoad)
                {
                    SelectedPictureGridViewModel = await LoadAPictureGridViewModel(folder, searchPatterns);
                    firstLoad = false;
                }
                else
                {
                    await LoadAPictureGridViewModel(folder, searchPatterns);
                }
            }
        }
    }
}
