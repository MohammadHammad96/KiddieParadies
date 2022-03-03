using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace KiddieParadies.Core.Services
{
    public interface IImagesRepository
    {
        Task<string> Save(IFormFile image, string folderName);
        bool Delete(string imageName, string folderName);
    }
}