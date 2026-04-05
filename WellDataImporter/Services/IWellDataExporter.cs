using WellDataImporter.Models;

namespace WellDataImporter.Services
{
    public interface IWellDataExporter
    {
        Task ExportAsync(IEnumerable<WellSummary> summaries, string filePath);
    }
}