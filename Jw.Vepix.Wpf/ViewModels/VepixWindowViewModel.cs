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
        public VepixWindowViewModel(IEventAggregator eventAggregator, IPictureRepository pictureRepo, 
            IPictureFolderTreeViewModel pictureFolderTreeViewModel, Func<IPictureGridViewModel> pictureGridViewModelCreator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            PictureFolderTreeViewModel = pictureFolderTreeViewModel;
            PictureGridViewModels = new ObservableCollection<IPictureGridViewModel>();

            _pictureRepo = pictureRepo;
            _pictureGridViewModelCreator = pictureGridViewModelCreator;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenPicturesFromFolderEvent>().Subscribe(OnOpenPicturesFromFolder);

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
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                List<Picture> pictures = await (_pictureRepo.GetPicturesFromFolderAsync(folderDialog.SelectedPath, option));
                if (option == SearchOption.AllDirectories)
                {
                    //may want to add a single folder to the tree; need to think about it more
                    PictureFolderTreeViewModel.Load(pictures.Select(pic => pic.FolderPath).ToList());
                }
                // Select the parent folder
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
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff;*.wmp"
            };
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                List<Picture> pictures = await (_pictureRepo.GetPicturesAsync(fileDialog.FileNames));
                SelectedPictureGridViewModel = CreateAndLoadPictureGridViewModel(pictures);
            }
        }

        private void OnCloseFolderCommand(IPictureGridViewModel tab)
        {
            PictureGridViewModels.Remove(tab);
        }

        private void OnAboutCommand()
        {
            System.Windows.MessageBox.Show(
                new System.Text.StringBuilder()
                    .Append("Version: ")
                    .AppendLine(System.Diagnostics.FileVersionInfo.GetVersionInfo(
                        System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion)
                    .AppendLine()
                    .AppendLine("Author: Jon Wesneski").ToString(),
                    "vepix - Viewer/editor picture app");
            // todo:
            //_modalDialog.ShowVepixDialog(new Views.AboutDialogView());
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
            List<Picture> pictures = await _pictureRepo.GetPicturesFromCommandLineAsync();
            //todo: add logic to get search option
            //if (option == SearchOption.AllDirectories)
            //{
            //    // may want to add a single folder to the tree; need to think about it more
            //    PictureFolderTreeViewModel.Load(pictures.Select(pic => pic.FolderPath).ToList());
            //}
            // Select the parent folder
            if (pictures.Count > 0)
            {
                SelectedPictureGridViewModel = CreateAndLoadPictureGridViewModel(pictures.Where(pic => pic.FolderPath == pictures[0].FolderPath).ToList());
            }
        }

        private IEventAggregator _eventAggregator;
        private IPictureRepository _pictureRepo;
        private Func<IPictureGridViewModel> _pictureGridViewModelCreator;
        private IPictureGridViewModel _selectedPictureGridViewModel;
    }
}
