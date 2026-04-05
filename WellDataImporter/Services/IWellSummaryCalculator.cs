using WellDataImporter.Models;

namespace WellDataImporter.Services
{
    public interface IWellSummaryCalculator
    {
        List<WellSummary> CalculateSummaries(IEnumerable<Well> wells);
    }
}