using WellDataImporter.Models;

namespace WellDataImporter.Services.Calculators
{
    public class WellSummaryCalculator : IWellSummaryCalculator
    {
        public List<WellSummary> CalculateSummaries(IEnumerable<Well> wells)
        {
            var summaries = new List<WellSummary>();
            foreach (var well in wells.Where(w => w.Intervals.Any()))
            {
                var intervals = well.Intervals;
                double totalThickness = intervals.Sum(i => i.DepthTo - i.DepthFrom);
                double weightedPorositySum = intervals.Sum(i => i.Porosity * (i.DepthTo - i.DepthFrom));
                double avgPorosity = totalThickness > 0 ? weightedPorositySum / totalThickness : 0;
                string dominantRock = intervals
                    .GroupBy(i => i.Rock)
                    .OrderByDescending(g => g.Sum(i => i.DepthTo - i.DepthFrom))
                    .First().Key;
                summaries.Add(new WellSummary(
                    well.WellId, 
                    well.X, 
                    well.Y,
                    intervals.Max(i => i.DepthTo),
                    intervals.Count,
                    Math.Round(avgPorosity, 4),
                    dominantRock));
            }

            return summaries;
        }
    }
}