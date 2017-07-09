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
using System.Windows.Forms;

namespace JW.Vepix.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IFolderTreeViewModel folderTreeViewModel,
                             Func<IPictureGridViewModel> pictureGridViewModelCreator,
                             IFileService fileService,
                             IFileExplorerDialogService fileExplorerDialogService,
                             IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            FolderTreeViewModel = folderTreeViewModel;
            _pictureGridViewModelCreator = pictureGridViewModelCreator;

            _fileService = fileService;
            _fileExplorerDialogService = fileExplorerDialogService;

            _helpInfoText = "";

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenPicturesFromFolderEvent>().Subscribe(OnOpenPicturesFromFolder);
            _eventAggregator.GetEvent<StatusTextUserActionEvent>().Subscribe(OnStatusTextUserAction);
            _eventAggregator.GetEvent<StatusTextHelpInfoEvent>().Subscribe(OnStatusTextHelpInfo);

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

                pictureGridViewModel.Load(pictureFileNames);
            }

            return pictureGridViewModel;
        }

        public async Task<IPictureGridViewModel> LoadAPictureGridViewModel(string folderPath) =>
            await LoadAPictureGridViewModel(folderPath, _supportedPicturesPatterns);

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
                pictureGridViewModel.Load(pictureFileNames);
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
            if (_fileExplorerDialogService.ShowFolderBrowserDialog(out selectedPath) == DialogResult.OK)
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
            if (_fileExplorerDialogService.ShowOpenFileDialog(out fileNames) == DialogResult.OK)
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

            new MessageDialogService().ShowMessage("vepix - About", message);
        }

        private async void OnOpenPicturesFromFolder(PicturesFolderPayload picturesFolder)
        {
            SelectedPictureGridViewModel = await LoadAPictureGridViewModel(picturesFolder.AbsolutePath);
        }

        private void OnStatusTextHelpInfo(string text)
        {
            HelpInfoText = text;
        }

        private void OnStatusTextUserAction(string text)
        {
            UserActionText = text;
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

        private Func<IPictureGridViewModel> _pictureGridViewModelCreator;
        private IPictureGridViewModel _selectedPictureGridViewModel;
        private IFileService _fileService;
        private IFileExplorerDialogService _fileExplorerDialogService;
        private IEventAggregator _eventAggregator;
        private string _userActionText;
        private string _helpInfoText;
        // todo: need to figure out where i can put this globally
        private readonly List<string> _supportedPicturesPatterns = new List<string>
        {
            "*.jpg", "*.png", "*.gif", "*.bmp", "*.wmp", "*.tiff"
        };
    }
}
