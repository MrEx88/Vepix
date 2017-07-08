using System;

namespace JW.Vepix.Wpf.Payloads
{
    public class PictureNameChangePayload
    {
        public Guid Guid { get; set; }
        public string NewPictureName { get; set; }
    }
}
