using JW.Vepix.Core.Models;
using System.Collections.Generic;

namespace JW.Vepix.Wpf.Payloads
{
    public class MovingPicturesPayload
    {
        public List<Picture> Pictures { get; set; }
        public string NewFolderPath { get; set; }
    }
}
