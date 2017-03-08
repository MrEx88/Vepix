using System;
using System.Collections.Generic;
using Jw.Data;

namespace Jw.Vepix.Wpf.ViewModels
{
    public class PictureViewModel
    {
        public Picture ViewingPicture { get; set; }

        public PictureViewModel(List<Picture> pictures, int picIndex)
        {
            _pictures = pictures;
            _picIndex = picIndex;

            ViewingPicture = pictures[picIndex];
        }


        private int _picIndex;
        private List<Picture> _pictures;
    }
}
