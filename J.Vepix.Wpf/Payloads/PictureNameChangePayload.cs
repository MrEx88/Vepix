using System;

namespace J.Vepix.Wpf.Payloads
{
    public class PictureNameChangePayload
    {
        public Guid Guid { get; set; }
        public string NewPictureName { get; set; }
    }
}
