using System;
using System.Collections.Generic;
using System.Linq;

namespace equilibreuse
{


    public class StabilityAnalysisResult
    {
        public string Variable { get; set; }
        public int BestExclusionIndex { get; set; }
        public double OriginalRange { get; set; }
        public double RangeAfterExclusion { get; set; }
        public List<int> ZScoreOutliers { get; set; }
    }

    public static class StabilityAnalyzer
    {
        public static List<StabilityAnalysisResult> AnalyzeStability(
            List<double> rms,
            List<double> lockin,
            List<double> pkpk,
            List<double> fftmag)
        {
            var results = new List<StabilityAnalysisResult>();

            results.Add(AnalyzeVariable("RMS", rms));
            results.Add(AnalyzeVariable("Lockin", lockin));
            results.Add(AnalyzeVariable("PkPk", pkpk));
            results.Add(AnalyzeVariable("FFTMag", fftmag));

            return results;
        }

        private static StabilityAnalysisResult AnalyzeVariable(string name, List<double> values)
        {
            double mean = values.Average();
            double std = Math.Sqrt(values.Select(v => Math.Pow(v - mean, 2)).Sum() / values.Count);

            // Z-score outlier detection
            var outliers = DetectOutliersRobust(values,2.5);

            // Range original
            double originalRange = values.Max() - values.Min();

            // Find best exclusion index
            int bestIndex = -1;
            double minRange = double.MaxValue;

            for (int i = 0; i < values.Count; i++)
            {
                var temp = values.Where((_, idx) => idx != i).ToList();
                if (temp.Count == 0)
                    continue;
                double tempRange = temp.Max() - temp.Min();

                if (tempRange < minRange)
                {
                    minRange = tempRange;
                    bestIndex = i;
                }
            }

            return new StabilityAnalysisResult
            {
                Variable = name,
                BestExclusionIndex = bestIndex,
                OriginalRange = originalRange,
                RangeAfterExclusion = minRange,
                ZScoreOutliers = outliers
            };
        }
        public static List<int> DetectOutliersRobust(IList<double> values, double zThreshold = 2.5)
        {
            var median = GetMedian(values);
            var deviations = values.Select(v => Math.Abs(v - median)).ToList();
            var mad = GetMedian(deviations);

            if (mad == 0)
                return new List<int>(); // Pas de variation → pas d'outliers

            var outliers = new List<int>();

            for (int i = 0; i < values.Count; i++)
            {
                double modifiedZ = 0.6745 * (values[i] - median) / mad;
                if (Math.Abs(modifiedZ) > zThreshold)
                    outliers.Add(i);
            }

            return outliers;
        }

        private static double GetMedian(IList<double> list)
        {
            var sorted = list.OrderBy(x => x).ToList();
            int n = sorted.Count;
            if (n % 2 == 1)
                return sorted[n / 2];
            else
                return (sorted[(n - 1) / 2] + sorted[n / 2]) / 2.0;
        }
    }

}
