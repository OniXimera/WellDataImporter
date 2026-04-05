using System.IO;

namespace WellDataImporter.Services
{
    public interface IFileSystem
    {
        Task<string[]> ReadAllLinesAsync(string filePath);

        Stream Create(string filePath);
    }
}
