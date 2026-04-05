using WellDataImporter.Models;

namespace WellDataImporter.Services
{
    public class DataProcessingService : IDataProcessingService
    {
        public DataProcessingService(
            IWellDataReader dataReader,
            IWellSummaryCalculator calculator,
            IWellDataExporter exporter)
        {
            _dataReader = dataReader;
            _calculator = calculator;
            _exporter = exporter;
        }

        public async Task<(List<WellSummary> Summaries, List<ImportError> Errors)> ImportCsvAsync(string filePath)
        {
            var (wells, errors) = await _dataReader.ReadAsync(filePath);
            var summaries = _calculator.CalculateSummaries(wells);
            return (summaries, errors);
        }

        public async Task ExportToJsonAsync(List<WellSummary> summaries, string filePath)
        {
            await _exporter.ExportAsync(summaries, filePath);
        }

        private readonly IWellDataReader _dataReader;
        private readonly IWellSummaryCalculator _calculator;
        private readonly IWellDataExporter _exporter;
    }
}
