using JW.Vepix.Core.Models;
using JW.Vepix.Wpf.Utilities;
using JW.Vepix.Wpf.ViewModels;
using MahApps.Metro.Controls;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace JW.Vepix.Wpf.Services
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

        public void ShowVepixFlyout<T>(List<Picture> pictures) where T : IFlyoutViewModel
        {
            var viewModel = ViewModelLocator.Container.Resolve<T>();
            viewModel.Load(pictures);
            _flyout.DataContext = viewModel;

            var binding = new Binding();
            binding.Source = viewModel;
            binding.Path = new PropertyPath("ViewTitle");
            binding.Mode = BindingMode.OneWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(_flyout, Flyout.HeaderProperty, binding);

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
