namespace WellDataImporter.Models
{
    public class Interval
    {
        public double DepthFrom { get; }
        public double DepthTo { get; }
        public string Rock { get; }
        public double Porosity { get; }

        public Interval(double depthFrom, double depthTo, string rock, double porosity)
        {
            DepthFrom = depthFrom;
            DepthTo = depthTo;
            Rock = rock;
            Porosity = porosity;
        }
    }
}