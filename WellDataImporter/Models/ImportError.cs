namespace WellDataImporter.Models
{
    public class ImportError
    {
        public int LineNumber { get; }
        public string WellId { get; }
        public string ErrorMessage { get; }

        public ImportError(int lineNumber, string wellId, string errorMessage)
        {
            LineNumber = lineNumber;
            WellId = wellId;
            ErrorMessage = errorMessage;
        }
    }
}