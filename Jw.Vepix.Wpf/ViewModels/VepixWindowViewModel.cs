﻿using Jw.Data;
using Jw.Vepix.Wpf.Utilities;
using Jw.Vepix.Wpf.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Jw.Vepix.Wpf.Messages;
using System.Windows.Forms;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class VepixWindowViewModel : ViewModelBase
    {
        public VepixWindowViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
                return;

            _pictureRepo = new PictureRepository();

            OpenFolderCommand = new RelayCommand<SearchOption>(OnOpenFolder);
            OpenFilesCommand = new RelayCommand<object>(OnOpenFiles);
            SearchCommand = new RelayCommand<string>(OnSearch);
            AboutCommand = new RelayCommand<object>(OnAbout);

            CheckCommandLine();
        }

        public RelayCommand<SearchOption> OpenFolderCommand { get; private set; }
        public RelayCommand<object> OpenFilesCommand { get; private set; }
        public RelayCommand<string> SearchCommand { get; private set; }
        public RelayCommand<object> AboutCommand { get; private set; }

        private async void OnOpenFolder(SearchOption option)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                List<Picture> pictures = await (_pictureRepo.GetPicturesFromFolderAsync(folderDialog.SelectedPath, option));
                Messenger.Default.Send(new ObservableCollection<Picture>(pictures));
            }
        }

        private async void OnOpenFiles()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif"
            };
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                List<Picture> pictures = await (_pictureRepo.GetPicturesAsync(fileDialog.FileNames));
                Messenger.Default.Send(new ObservableCollection<Picture>(pictures));
            }
        }

        private void OnSearch(string filterString)
        {
            Messenger.Default.Send(new SearchFilterMessage(filterString), "SearchFilter");
        }

        private void OnAbout()
        {
            System.Windows.MessageBox.Show(new System.Text.StringBuilder()
                .Append("Version: ")
                .AppendLine(System.Diagnostics.FileVersionInfo.GetVersionInfo(
                    System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion)
                .AppendLine()
                .AppendLine("Author: Jon Wesneski").ToString(),
                "vepix - Viewer/editor picture app");
            // todo:
            //_modalDialog.ShowVepixDialog(new Views.AboutDialogView());
        }

        private async void CheckCommandLine()
        {
            List<Picture> pictures = await _pictureRepo.GetPicturesFromCommandLineAsync();
            Messenger.Default.Send(new ObservableCollection<Picture>(pictures));
        }

        private IPictureRepository _pictureRepo;
    }
}
