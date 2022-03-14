using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace KiddieParadies.Core.Services
{
    public interface IFilesRepository
    {
        Task<string> Save(IFormFile file, string mainFolderName, string subFolderName);
        bool Delete(string fileName, string mainFolderName, string subFolderName);
    }
}