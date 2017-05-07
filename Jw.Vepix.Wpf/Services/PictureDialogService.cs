using Jw.Vepix.Data;
using Jw.Vepix.Wpf.ViewModels;
using Jw.Vepix.Wpf.Views;
using Prism.Events;
using System.Collections.Generic;
using System.Windows;

namespace Jw.Vepix.Wpf.Services
{
    public enum DialogType
    {
        CropImages,
        EditNames
    }

    public class PictureDialogService : ICollectionDialogService
    {
        public PictureDialogService(DialogType dialogType, IEventAggregator eventAggregator, 
            IMessageDialogService messageDialogService, List<Picture> pictures)
        {
            _dialogViewModel = CreateViewModel(dialogType, eventAggregator, messageDialogService, pictures);
            _dialog = CreateView(dialogType);
        }

        public void CloseVepixDialog()
        {
            _dialog.Close();
        }

        public void ShowVepixDialog()
        {
            _dialog.DataContext = _dialogViewModel;
            _dialog.Show();
        }

        private ViewModelBase CreateViewModel(DialogType dialogType, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService, List<Picture> pictures)
        {
            switch (dialogType)
            {
                case DialogType.CropImages:
                    return new PictureDialogViewModel(eventAggregator, messageDialogService, pictures);
                default:
                    return new EditNameDialogViewModel(new PictureRepository(), eventAggregator, messageDialogService, pictures);
            }
        }

        private Window CreateView(DialogType dialogType)
        {
            switch(dialogType)
            {
                case DialogType.CropImages:
                    return new PicturesDialogView();
                default:
                    return new EditNameDialogView();
            }
        }

        private ViewModelBase _dialogViewModel;
        private Window _dialog;
    }
}
