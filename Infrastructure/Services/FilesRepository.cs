using KiddieParadies.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KiddieParadies.Infrastructure.Services
{
    public class FilesRepository : IFilesRepository
    {
        private readonly IWebHostEnvironment _host;

        public FilesRepository(IWebHostEnvironment host)
        {
            _host = host;
        }

        public async Task<string> Save(IFormFile file, string mainFolderName, string subFolderName)
        {
            try
            {
                var fullPath = Path.Combine(_host.ContentRootPath + "\\wwwroot", mainFolderName, subFolderName);
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var fullFilePath = Path.Combine(fullPath, fileName);

                await using var stream = new FileStream(fullFilePath, FileMode.Create);
                await file.CopyToAsync(stream);

                return fileName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public bool Delete(string fileName, string mainFolderName, string subFolderName)
        {
            try
            {
                var fullFilePath = Path
                    .Combine(_host.ContentRootPath + "\\wwwroot", mainFolderName, subFolderName, fileName);
                File.Delete(fullFilePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}