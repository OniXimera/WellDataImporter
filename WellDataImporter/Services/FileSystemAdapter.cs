using System.IO;

namespace WellDataImporter.Services
{
    public class FileSystemAdapter : IFileSystem
    {
        public Task<string[]> ReadAllLinesAsync(string filePath)
        {
            return File.ReadAllLinesAsync(filePath);
        }

        public Stream Create(string filePath)
        {
            return File.Create(filePath);
        }
    }
}
