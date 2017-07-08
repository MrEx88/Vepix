using System.Windows.Media.Imaging;

namespace Jw.Vepix.Core.Services
{
    public enum BitmapEncoderType
    {
        BMP,
        GIF,
        JPEG,
        PNG,
        TIFF,
        WMP
    }

    public static class EncoderService
    {
        public static BitmapEncoder CreateEncoder(BitmapEncoderType encoderType)
        {
            switch (encoderType)
            {
                case BitmapEncoderType.BMP:
                    return new BmpBitmapEncoder();
                case BitmapEncoderType.GIF:
                    return new GifBitmapEncoder();
                case BitmapEncoderType.PNG:
                    return new PngBitmapEncoder();
                case BitmapEncoderType.TIFF:
                    return new TiffBitmapEncoder();
                case BitmapEncoderType.WMP:
                    return new WmpBitmapEncoder();
                default:
                    return new JpegBitmapEncoder();
            }
        }
    }
}
