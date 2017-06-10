using Jw.Vepix.Core.Models;
using Jw.Vepix.Wpf.ViewModels;
using MahApps.Metro.Controls;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Jw.Vepix.Wpf.Services
{
    public enum FlyoutViewType
    {
        Viewer,
        EditNames
    }

    public class PicturesFyloutService : ICollectionFlyoutService
    {
        public PicturesFyloutService(FlyoutViewType flyoutViewType)
        {
            var parent = Window.GetWindow(App.Current.MainWindow);
            _flyout = (Flyout)parent.FindName(GetFlyoutName(flyoutViewType));
        }

        public void CloseVepixFlyout()
        {
            _flyout.IsOpen = false;
        }

        public void ShowVepixFlyout<T>(List<Picture> pictures) where T : ICollectionViewModel
        {
            var viewModel = Utilities.ViewModelLocator.Container.Resolve<T>();
            viewModel.Load(pictures);
            _flyout.DataContext = viewModel;
            
            _flyout.IsOpen = true;
        }

        private string GetFlyoutName(FlyoutViewType flyoutViewType)
        {
            switch (flyoutViewType)
            {
                case FlyoutViewType.Viewer:
                    return "picturesViewer";
                case FlyoutViewType.EditNames:
                    return "editNames";
                default:
                    throw new ArgumentException($"Invalid argument: {flyoutViewType}");
            }
        }

        private Flyout _flyout;
    }
}
