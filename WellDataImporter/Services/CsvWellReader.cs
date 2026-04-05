using System.Globalization;
using WellDataImporter.Common;
using WellDataImporter.Models;

namespace WellDataImporter.Services
{
    public class CsvWellReader : IWellDataReader
    {
        public CsvWellReader(IFileSystem fileSystem) 
        {
            _fileSystem = fileSystem;
        }

        public async Task<(List<Well> Wells, List<ImportError> Errors)> ReadAsync(string filePath)
        {
            var errors = new List<ImportError>();
            var wellsDict = new Dictionary<string, Well>();
            var parsedIntervalsMap = new Dictionary<string, List<(int Line, Interval Interval)>>();

            try
            {
                var lines = await _fileSystem.ReadAllLinesAsync(filePath); //OutOfMemory is possible, but it's fine for a test task.
                for (int i = 0; i < lines.Length; i++)
                {
                    ParseLine(lines[i], i + 1, errors, wellsDict, parsedIntervalsMap);
                }

                CheckOverlapsAndBuildWells(wellsDict, parsedIntervalsMap, errors);
            }
            catch (Exception ex)
            {
                errors.Add(new ImportError(0, Resources.Resources.File, string.Format(Resources.Resources.StatusReadError, ex.Message)));
            }

            return (wellsDict.Values.ToList(), errors);
        }

        private void ParseLine(string line, int lineNumber, List<ImportError> errors,
            Dictionary<string, Well> wellsDict, Dictionary<string, List<(int, Interval)>> intervalsMap)
        {
            if (string.IsNullOrWhiteSpace(line) || line[0] == '#')
            {
                return;
            }

            var parts = line.Split(';');
            if (parts[0] == AppConstants.StartCSVCol)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(parts[0]))
            {
                errors.Add(new ImportError(lineNumber, Resources.Resources.Unknown, Resources.Resources.ErrorWellIdNull));
                return;
            }

            if (parts.Length != AppConstants.ExpectedColumnCount)
            {
                errors.Add(new ImportError(lineNumber, Resources.Resources.Unknown, Resources.Resources.ErrorLengthLineFile));
                return;
            }

            string wellId = parts[0];
            if (!double.TryParse(parts[1], CultureInfo.InvariantCulture, out double x) ||
                !double.TryParse(parts[2], CultureInfo.InvariantCulture, out double y) ||
                !double.TryParse(parts[3], CultureInfo.InvariantCulture, out double depthFrom) ||
                !double.TryParse(parts[4], CultureInfo.InvariantCulture, out double depthTo) ||
                !double.TryParse(parts[6], CultureInfo.InvariantCulture, out double porosity))
            {
                errors.Add(new ImportError(lineNumber, wellId, Resources.Resources.ErrorParseNumber));
                return;
            }

            string rock = parts[5].Trim();
            if (depthFrom < 0)
            {
                errors.Add(new ImportError(lineNumber, wellId, Resources.Resources.ErrorDepthFromZero));
                return;
            }

            if (depthFrom >= depthTo)
            {
                errors.Add(new ImportError(lineNumber, wellId, Resources.Resources.ErrorDepthFromDepthTo));
                return;
            }

            if (porosity < 0 || porosity > 1f)
            {
                errors.Add(new ImportError(lineNumber, wellId, Resources.Resources.ErrorPorosity01));
                return;
            }

            if (string.IsNullOrEmpty(rock))
            {
                errors.Add(new ImportError(lineNumber, wellId, Resources.Resources.ErrorNullRock));
                return;
            }

            if (!wellsDict.ContainsKey(wellId))
            {
                wellsDict[wellId] = new Well(wellId, x, 0);
                intervalsMap[wellId] = new List<(int, Interval)>();
            }

            intervalsMap[wellId].Add((lineNumber, new Interval(depthFrom, depthTo, rock, porosity)));
        }

        private void CheckOverlapsAndBuildWells(
            Dictionary<string, Well> wellsDict,
            Dictionary<string, List<(int Line, Interval Interval)>> intervalsMap, 
            List<ImportError> errors)
        {
            foreach (var parsedIntervals in intervalsMap)
            {
                var wellId = parsedIntervals.Key;
                var sortedIntervals = parsedIntervals.Value.OrderBy(x => x.Interval.DepthFrom).ToList();
                bool hasOverlap = false;

                for (int i = 1; i < sortedIntervals.Count; i++)
                {
                    if (sortedIntervals[i].Interval.DepthFrom < sortedIntervals[i - 1].Interval.DepthTo)
                    {
                        errors.Add(new ImportError(sortedIntervals[i].Line, wellId, Resources.Resources.ErrorInterval));
                        hasOverlap = true;
                    }
                }

                if (!hasOverlap)
                {
                    wellsDict[wellId].Intervals.AddRange(sortedIntervals.Select(x => x.Interval));
                }
                else
                {
                    wellsDict.Remove(wellId);
                }
            }
        }

        private readonly IFileSystem _fileSystem;
    }
}
