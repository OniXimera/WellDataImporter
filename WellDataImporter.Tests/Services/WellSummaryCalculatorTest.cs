using WellDataImporter.Models;
using WellDataImporter.Services.Calculators;

namespace WellDataImporter.Tests.Services
{
    public class WellSummaryCalculatorTest
    {
        [Fact]
        public void CalculateSummaries_ValidData_CalculatesWeightedAverageAndDominantRock()
        {
            var well = new Well("W-Test", 10, 10);
            well.Intervals.Add(new Interval(0, 5, "Sandstone", 0.2));
            well.Intervals.Add(new Interval(5, 20, "Limestone", 0.1));
            var result = _calculator.CalculateSummaries(new[] { well });
            Assert.Single(result);
            var summary = result[0];
            Assert.Equal(0.125, summary.AveragePorosity, 4);
            Assert.Equal("Limestone", summary.DominantRock);
            Assert.Equal(20, summary.TotalDepth);
        }

        [Fact]
        public void CalculateSummaries_EmptyIntervals_ShouldBeIgnored()
        {
            var emptyWell = new Well("Empty", 0, 0);
            var result = _calculator.CalculateSummaries(new[] { emptyWell });
            Assert.Empty(result);
        }

        private readonly WellSummaryCalculator _calculator = new();
    }
}
