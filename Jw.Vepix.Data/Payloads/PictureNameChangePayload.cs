using System;

namespace Jw.Vepix.Data.Payloads
{
    public class PictureNameChangePayload
    {
        public Guid Guid { get; set; }
        public string PictureName { get; set; }
    }
}
