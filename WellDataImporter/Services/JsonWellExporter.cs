using System.Text.Json;
using WellDataImporter.Models;

namespace WellDataImporter.Services
{
    public class JsonWellExporter : IWellDataExporter
    {
        public JsonWellExporter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public async Task ExportAsync(IEnumerable<WellSummary> summaries, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            using var stream = _fileSystem.Create(filePath);
            await JsonSerializer.SerializeAsync(stream, summaries, options);
        }

        private readonly IFileSystem _fileSystem;
    }
}
