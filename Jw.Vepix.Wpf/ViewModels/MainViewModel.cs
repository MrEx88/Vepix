using JW.Vepix.Core.Extensions;
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
using System.Windows.Forms;

namespace JW.Vepix.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IPictureFolderTreeViewModel pictureFolderTreeViewModel,
                             Func<IPictureGridViewModel> pictureGridViewModelCreator,
                             IFileExplorerDialogService fileExplorerDialogService,
                             IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            PictureFolderTreeViewModel = pictureFolderTreeViewModel;
            _pictureGridViewModelCreator = pictureGridViewModelCreator;
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

        public IPictureFolderTreeViewModel PictureFolderTreeViewModel { get; private set; }
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
                    PictureFolderTreeViewModel.TryLoad(selectedPath);
                }

                SelectedPictureGridViewModel = CreateAndLoadPictureGridViewModel(selectedPath);
            }
        }

        private IPictureGridViewModel CreateAndLoadPictureGridViewModel(List<string> pictureFileNames)
        {
            // Check if folder is already opened.
            var pictureGridViewModel = PictureGridViewModels.ToList().Find(picGrid =>
                ((PictureGridViewModel)picGrid).AbsolutePath == pictureFileNames.First().ToParentFolderPath());
            if (pictureGridViewModel != null)
            {
                // Remove any duplicates.
                pictureFileNames.RemoveAll(fileName => PictureGridViewModels.ToList()
                                .Exists(gridViewModel => gridViewModel.Pictures.ToList()
                                .Exists(pic => pic.FullFileName == fileName)));

                pictureGridViewModel.Load(pictureFileNames);
                return pictureGridViewModel;
            }

            pictureGridViewModel = _pictureGridViewModelCreator();
            
            PictureGridViewModels.Add(pictureGridViewModel);
            pictureGridViewModel.Load(pictureFileNames);
            return pictureGridViewModel;
        }

        private IPictureGridViewModel CreateAndLoadPictureGridViewModel(string folderPath)
        {
            // Check if folder is already opened.
            var pictureGridViewModel = PictureGridViewModels.ToList().Find(picGrid =>
                ((PictureGridViewModel)picGrid).AbsolutePath == folderPath);
            if (pictureGridViewModel != null)
            {
                return pictureGridViewModel;
            }
            
            pictureGridViewModel = _pictureGridViewModelCreator();
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

        private void OnStatusTextHelpInfo(string text)
        {
            HelpInfoText = text;
        }

        private void OnStatusTextUserAction(string text)
        {
            UserActionText = text;
        }

        private void CheckCommandLine()
        {
            // todo: need to pass the search patterns too
            var userInputs = VepixCommandLineParser.ConsoleInstance();
            foreach (var treeFolder in userInputs.TreeFolders)
            {
                PictureFolderTreeViewModel.TryLoad(treeFolder);
            }

            var firstLoad = true;
            foreach (var folder in userInputs.Folders)
            {
                if (firstLoad)
                {
                    SelectedPictureGridViewModel = CreateAndLoadPictureGridViewModel(folder);
                    firstLoad = false;
                }
                else
                {
                    CreateAndLoadPictureGridViewModel(folder);
                }
            }
        }

        private IEventAggregator _eventAggregator;
        private Func<IPictureGridViewModel> _pictureGridViewModelCreator;
        private IPictureGridViewModel _selectedPictureGridViewModel;
        private IFileExplorerDialogService _fileExplorerDialogService;
        private string _userActionText;
        private string _helpInfoText;
    }
}
