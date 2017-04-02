using Jw.Vepix.Data;
using Jw.Vepix.Wpf.Events;
using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class PictureDialogViewModel
    {
        public Picture ViewingPicture { get; set; }
        public System.Windows.Controls.Image Imagesrc { get; set; }
        public CropSelectionCanvas CropCanvas { get; set; }
        public RelayCommand<object> SaveCommand { get; set; }
        public RelayCommand<object> SaveAsCommand { get; set; }

        public PictureDialogViewModel(IEventAggregator eventAggregator, List<Picture> pictures, int picIndex)
        {
            // in future, maybe use this viewer to navigate through photos
            //_pictures = pictures;
            //_picIndex = picIndex;

            ViewingPicture = pictures[picIndex];
            SetupCanvas(eventAggregator, ViewingPicture);

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<CropAreaDrawnEvent>().Subscribe(OnCropAreaDrawn);

            SaveCommand = new RelayCommand<object>(OnSaveExecute, OnSaveCanExecute);
            SaveAsCommand = new RelayCommand<object>(OnSaveAsExecute, OnSaveCanExecute);
        }

        private void OnSaveAsExecute()
        {
            var croppedImage = BitmapService.CropPreview(ViewingPicture, _cropArea.Value, BitmapEncoderType.JPEG);
            FileService fileService = new FileService();
            fileService.SaveImageAs(croppedImage, BitmapEncoderType.JPEG);
        }

        private void OnSaveExecute()
        {
            var croppedImage = BitmapService.CropPreview(ViewingPicture, _cropArea.Value, BitmapEncoderType.JPEG);
            //1 are you sure you want to overwrite?
            //2 FileService fileService = new FileService();
            //fileService.SaveImageOverwrite(croppedImage, BitmapEncoderType.JPEG);
            //3 notifypicturechange
        }

        private bool OnSaveCanExecute() => _cropArea.HasValue;

        private void SetupCanvas(IEventAggregator eventAggregator, Picture picture)
        {
            Imagesrc = new System.Windows.Controls.Image();
            Imagesrc.Source = picture.BitmapImage;
            CropCanvas = new CropSelectionCanvas(eventAggregator, /*new Point(picture.Width, picture.Height)*/
                new PointBoundaries(new Point(0.0,0.0), new Point(picture.Width, picture.Height)));
            CropCanvas.Children.Add(Imagesrc);
            CropCanvas.Height = Convert.ToDouble(picture.Height);
            CropCanvas.Width = Convert.ToDouble(picture.Width);
        }

        private void OnCropAreaDrawn(Int32Rect rect)
        {
            _cropArea = rect;
            SaveAsCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
        }

        private IEventAggregator _eventAggregator;
        private Int32Rect? _cropArea;
        //private List<Picture> _pictures;
        //private int _picIndex;
    }
}
