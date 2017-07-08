using System.Windows.Media.Imaging;

namespace J.Vepix.Core.Services
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
        public static BitmapEncoder CreateEncoder(BitmapEncoderType encoderType, BitmapSource bitmapSource)
        {
            BitmapEncoder bitmapEncoder;
            switch (encoderType)
            {
                case BitmapEncoderType.BMP:
                    bitmapEncoder = new BmpBitmapEncoder();
                    break;
                case BitmapEncoderType.GIF:
                    bitmapEncoder = new GifBitmapEncoder();
                    break;
                case BitmapEncoderType.PNG:
                    bitmapEncoder = new PngBitmapEncoder();
                    break;
                case BitmapEncoderType.TIFF:
                    bitmapEncoder = new TiffBitmapEncoder();
                    break;
                case BitmapEncoderType.WMP:
                    bitmapEncoder = new WmpBitmapEncoder();
                    break;
                default:
                    bitmapEncoder = new JpegBitmapEncoder();
                    break;
            }

            bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            return bitmapEncoder;
        }
    }
}
