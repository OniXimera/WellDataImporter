namespace WellDataImporter.Models
{
    public class WellSummary
    {
        public string WellId { get; }
        public double X { get; }
        public double Y { get; }
        public double TotalDepth { get; }
        public int IntervalCount { get; }
        public double AveragePorosity { get; }
        public string DominantRock { get; }

        public WellSummary(string wellId, double x, double y, double totalDepth, int intervalCount, double averagePorosity, string dominantRock)
        {
            WellId = wellId;
            TotalDepth = totalDepth;
            IntervalCount = intervalCount;
            AveragePorosity = averagePorosity;
            DominantRock = dominantRock;
        }
    }
}