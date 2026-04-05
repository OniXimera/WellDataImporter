using WellDataImporter.Models;

namespace WellDataImporter.Services
{
    public interface IWellDataReader
    {
        Task<(List<Well> Wells, List<ImportError> Errors)> ReadAsync(string filePath);
    }
}