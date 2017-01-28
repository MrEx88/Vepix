using Jw.Data;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Jw.Vepix.Wpf.Services
{
    public interface IPictureRepository
    {
        Task<List<Picture>> GetPicturesAsync(string[] files);

        Task<List<Picture>> GetPicturesFromFolderAsync(string filePath, SearchOption option, params string[] fileFilters);

        Task<List<Picture>> GetPicturesFromFolderAsync(string folderPath, SearchOption option);

        Task<List<Picture>> GetPicturesFromCommandLineAsync();

        bool TryChangePictureName(Picture picture, string name);

        bool TryDelete(Picture picture);
    }
}
