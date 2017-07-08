
namespace JW.Vepix.Core.Models
{
    public class FileBytes
    {
        public string FullFileName { get; }
        public byte[] Bytes { get; }

        public FileBytes(string fullFileName, byte[] bytes)
        {
            FullFileName = fullFileName;
            Bytes = bytes;
        }
    }
}
