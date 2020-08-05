using ChatCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatDesktop.Interfaces
{
    public interface IFile
    {
        Task<string> GetFile(string path);
        Task<string> SendFileToApi(string url, FileModel file);
        Task GetFileWithProgress(FileModel file);
        Task GetPhotos(IEnumerable<BaseUserModel> items);
    }
}
