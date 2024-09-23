using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    class ProcessFacade
    {
        private static ProcessFacade instance = null;
        private ProcessFacade()
        {

        }

        public static ProcessFacade GetInstance()
        {
            if (instance == null)
            {
                instance = new ProcessFacade();
            }
            return instance;
        }

        public async Task<byte[]> RetrieveFile(string fileName)
        {
            string serverStorage = @"C:\MattFileSystem\ServerStorage";
            string filePath = Path.GetFullPath(fileName);

            if (filePath.StartsWith(serverStorage) && File.Exists(filePath))
            {
                return await File.ReadAllBytesAsync(filePath);
            }
            else if (File.Exists(filePath))
            {
                throw new UnauthorizedAccessException("You do not have access to that file");
            }
            else
            {
                throw new FileNotFoundException("File not found", fileName);
            }
        }

        public async Task<string> UploadFile(string fileName, byte[] fileData)
        {
            await File.WriteAllBytesAsync(fileName, fileData);
            return $"File uploaded sucessfully: {Path.GetFileName(fileName)}";
        }

        public async Task<string[]> ListFiles()
        {
            return await Task.Run(() =>
                Directory.GetFiles(@"C:\MattFileSystem\ServerStorage")
                         .Select(file => Path.GetFileName(file))
                         .ToArray()
            );
        }
    }
}
