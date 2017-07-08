using J.Vepix.Core.Models;
using J.Vepix.Wpf.ViewModels;
using System.Collections.Generic;

namespace J.Vepix.Wpf.Services
{
    interface ICollectionFlyoutService
    {
        void ShowVepixFlyout<T>(List<Picture> pictures) where T : ICollectionViewModel;
        void CloseVepixFlyout();
    }
}
