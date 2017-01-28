using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Jw.Vepix.Wpf.Services
{
    public interface IFileService
    {
        Task<Dictionary<string, byte[]>> GetFilesAndBytesFromDirectoryAsync(string path, List<string> fileFilters, SearchOption option);

        Task<Dictionary<string, byte[]>> ReadBytesFromFilesAsync(List<string> files);

        void ChangeFileName(string oldName, string newName);

        bool IsValidFileName(string fileName);

        void DeleteFile(string fileName);
    }
}
