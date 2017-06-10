using Jw.Vepix.Core.Models;
using Jw.Vepix.Wpf.ViewModels;
using System.Collections.Generic;

namespace Jw.Vepix.Wpf.Services
{
    interface ICollectionFlyoutService
    {
        void ShowVepixFlyout<T>(List<Picture> pictures) where T : ICollectionViewModel;
        void CloseVepixFlyout();
    }
}
