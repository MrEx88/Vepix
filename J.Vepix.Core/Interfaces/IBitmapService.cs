using J.Vepix.Core.Services;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace J.Vepix.Core.Interfaces
{
    public interface IBitmapService
    {
        /// <summary>
        /// Creates a BitmapImage from byte array.
        /// </summary>
        /// <param name="bytes">byte array of image</param>
        /// <returns>Instance of BitmapImage</returns>
        BitmapImage CreateBitmapImage(byte[] bytes);

        /// <summary>
        /// Converts byte array to a BitmapImage.
        /// </summary>
        /// <param name="fileName">The image file name</param>
        /// <returns>Instance of BitmapImage</returns>
        BitmapImage CreateBitmapImage(string fileName);

        /// <summary>
        /// Crops a picture to the specified size and encoder.
        /// </summary>
        /// <param name="bitmapImage">The image to crop</param>
        /// <param name="rect">The size to crop the image to</param>
        /// <param name="encoderType">The type to encode to</param>
        /// <returns>The cropped Image</returns>
        BitmapImage Crop(BitmapImage bitmapImage, Int32Rect rect, BitmapEncoderType encoderType);

        /// <summary>
        /// Converts BitmapImage to Bitmap.
        /// </summary>
        /// <param name="bitmapImage">The bitmap to convert</param>
        /// <param name="encoderType">The type to encode to</param>
        /// <returns>A bitmap</returns>
        Bitmap ConvertBitmapImageToBitmap(BitmapImage bitmapImage, BitmapEncoderType encoderType);
    }
}
