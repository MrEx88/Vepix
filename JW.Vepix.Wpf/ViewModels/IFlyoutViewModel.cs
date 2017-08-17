using JW.Vepix.Core.Models;
using System.Collections.Generic;

namespace JW.Vepix.Wpf.ViewModels
{
    public interface IFlyoutViewModel
    {
        string ViewTitle { get; }
        void Load(List<Picture> pictures);
    }
}
