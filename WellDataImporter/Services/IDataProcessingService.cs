using WellDataImporter.Models;

namespace WellDataImporter.Services
{
    public interface IDataProcessingService
    {
        Task ExportToJsonAsync(List<WellSummary> summaries, string filePath);
        Task<(List<WellSummary> Summaries, List<ImportError> Errors)> ImportCsvAsync(string filePath);
    }
}