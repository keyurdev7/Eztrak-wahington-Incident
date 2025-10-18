using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Helpers.File
{
    public interface IFileHelper
    {
        string Save(IFileModel model, string existingUrl = "", string baseFolder = "", string namePrefix = "");
        string SaveFile(IFormFile fileToSave, string _baseFolder = "", string existingUrl = "", string namePrefix = "", string imageUrl = "");
        bool Delete(string url);

        void GetFilePath(string extension, string _baseFolder, string namePrefix, out string subPath, out string uniqueFileName, out string filePath, out string basePath);
    }
    public class FileHelper : IFileHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileHelper> _logger;
        private readonly IHostingEnvironment _env;

        public FileHelper(IConfiguration configuration, ILogger<FileHelper> logger, IHostingEnvironment env)
        {
            _configuration = configuration;
            _logger = logger;
            _env = env;
        }
        public string Save(IFileModel model, string existingUrl = "", string baseFolder = "", string namePrefix = "")
        {
            string imageUrl = "";
            try
            {
                string _baseFolder = string.IsNullOrEmpty(baseFolder) ? model?.GetBaseFolder() ?? "" : baseFolder;
                imageUrl = SaveFile(model.File, _baseFolder, existingUrl, namePrefix, imageUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FileHelper.SaveImage");
            }
            return imageUrl;
        }

        public string SaveFile(IFormFile fileToSave, string _baseFolder = "", string existingUrl = "", string namePrefix = "", string imageUrl = "")
        {
            if (fileToSave != null && IsValidFile(fileToSave))
            {

                var extension = Path.GetExtension(fileToSave.FileName);
                _logger.LogDebug("File Save method, extension trimmed", extension);
                string subPath, filePath, uniqueFileName, basePathStorage;
                GetFilePath(extension, _baseFolder, namePrefix, out subPath, out uniqueFileName, out filePath, out basePathStorage);
                if (!string.IsNullOrEmpty(existingUrl))
                {
                    Delete(existingUrl);
                }
                _logger.LogDebug("File Save method, created filepath", filePath);
                var directoryPath = _configuration.GetValue<string>("DirectoryPath");
                //var absolutePath = //_env.ContentRootPath;
                var savedDirectory = Path.Combine(directoryPath, filePath);
                FileInfo file = new FileInfo(savedDirectory);
                file.Directory.Create(); // If the directory already exists, this method does nothing.
                _logger.LogDebug("File Save method, creating file directory");
                using (var fileStream = new FileStream(savedDirectory, FileMode.Create))
                {
                    _logger.LogDebug("File Save method, copying file");

                    fileToSave.CopyTo(fileStream);
                }
                _logger.LogDebug("File Save method, file saved");

                imageUrl = $"/{basePathStorage}/{subPath}/{_baseFolder}/{uniqueFileName}";
                _logger.LogDebug("File Save method, url created", imageUrl);
            }

            return imageUrl;
        }

        public void GetFilePath(string extension, string _baseFolder, string prefix, out string subPath, out string uniqueFileName, out string filePath, out string basePath)
        {
            string basePathStorage = $"{_configuration.GetValue<string>("UploadBaseStoragePath")}";
            basePath = _configuration.GetValue<string>("UploadBasePath");
            subPath = _configuration.GetValue<string>("UploadSubPath");
            string uploadsFolder = Path.Combine(basePathStorage, subPath, _baseFolder);
            uniqueFileName = prefix + "-" + DateTime.UtcNow.Ticks.ToString() + extension;
            filePath = Path.Combine(uploadsFolder, uniqueFileName);
        }


        public bool Delete(string url)
        {
            try
            {
                string basePath = _configuration.GetValue<string>("UploadBasePath");
                if (!string.IsNullOrEmpty(url))
                {
                    string tempFilePath = basePath + url.Replace("/", "\\");
                    if (System.IO.File.Exists(tempFilePath))
                    {
                        System.IO.File.Delete(tempFilePath);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserManagementAPI.DeleteImage");
            }
            return false;
        }
        public bool IsValidFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            // Check if the file is an image
            //if (!file.ContentType.StartsWith("image/"))
            //{
            //    return false;
            //}

            // Check if the file size is less than 5 MB
            //const int maxFileSize = 5 * 1024 * 1024; // 5 MB
            //if (file.Length > maxFileSize)
            //{
            //    return false;
            //}

            // You can add other checks here, such as file extension, etc.

            return true;
        }
    }
}
