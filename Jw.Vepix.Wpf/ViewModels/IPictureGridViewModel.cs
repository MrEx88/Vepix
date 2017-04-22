using Jw.Vepix.Data;
using System.Collections.Generic;

namespace Jw.Vepix.Wpf.ViewModels
{
    public interface IPictureGridViewModel
    {
        void Load(List<Picture> files);
    }
}
