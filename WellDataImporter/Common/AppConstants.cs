namespace WellDataImporter.Common
{
    internal static class AppConstants
    {
        public const string CsvFilter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
        public const string JsonFilter = "JSON files (*.json)|*.json";
        public const string JsonFileName = "WellSummaries.json";
        public const string StartCSVCol = "WellId";
        public const int ExpectedColumnCount = 7;
    }
}
