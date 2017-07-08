using JW.Vepix.Core.Models;
using JW.Vepix.Wpf.ViewModels;
using System.Collections.Generic;

namespace JW.Vepix.Wpf.Services
{
    interface ICollectionFlyoutService
    {
        void ShowVepixFlyout<T>(List<Picture> pictures) where T : ICollectionViewModel;
        void CloseVepixFlyout();
    }
}
