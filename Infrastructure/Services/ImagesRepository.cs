using KiddieParadies.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KiddieParadies.Infrastructure.Services
{
    public class ImagesRepository : IImagesRepository
    {
        private const string ImagesFolderName = "images";
        private readonly IWebHostEnvironment _host;

        public ImagesRepository(IWebHostEnvironment host)
        {
            _host = host;
        }

        public async Task<string> Save(IFormFile image, string folderName)
        {
            try
            {
                var fullPath = Path.Combine(_host.ContentRootPath + "\\wwwroot", ImagesFolderName, folderName);
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var fullImagePath = Path.Combine(fullPath, fileName);

                await using var stream = new FileStream(fullImagePath, FileMode.Create);
                await image.CopyToAsync(stream);

                return fileName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public bool Delete(string imageName, string folderName)
        {
            try
            {
                var fullImagePath = Path
                    .Combine(_host.ContentRootPath + "\\wwwroot", ImagesFolderName, folderName, imageName);
                File.Delete(fullImagePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}