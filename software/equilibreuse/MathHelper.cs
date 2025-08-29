using ScottPlot;
using ScottPlot.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace equilibreuse
{
    public static class MathHelper
    {
        public static PhaseAnalysis CalculateMeanPhaseAnalysis(List<PhaseAnalysis> phaseAnalyses)
        {
            var pa = new PhaseAnalysis();
            double[] rMaxTemporalAngles = phaseAnalyses.Select(p => p.rMaxTemporal).ToArray();
            double[] rPhaseLockInAngles = phaseAnalyses.Select(p => p.rPhaseLockIn).ToArray();
            double[] rFitSinusoidAngles = phaseAnalyses.Select(p => p.rFitSinusoid).ToArray();
            double[] rDetectPhaseAngles = phaseAnalyses.Select(p => p.rDetectPhase).ToArray();
            double[] rFFTAngles = phaseAnalyses.Select(p => p.rFFT).ToArray();

            pa.rMaxTemporal = CalculateMeanAngle(rMaxTemporalAngles);
            pa.rPhaseLockIn = CalculateMeanAngle(rPhaseLockInAngles);
            pa.rFitSinusoid = CalculateMeanAngle(rFitSinusoidAngles);
            pa.rDetectPhase = CalculateMeanAngle(rDetectPhaseAngles);
            pa.rFFT = CalculateMeanAngle(rFFTAngles);
            return pa;
        }
        public static double CalculateMeanAngle(double[] angles)
        {
            int totalOccurrences = angles.Length;

            double sommeCos = 0;
            double sommeSin = 0;

            for (int i = 0; i < totalOccurrences; i++)
            {
                double angle = angles[i];
                double radian = angle * (Math.PI / 180); // Conversion en radians
                sommeCos += Math.Cos(radian);
                sommeSin += Math.Sin(radian);
            }

            double moyenneRad = Math.Atan2(sommeSin, sommeCos); // Calcul de l'angle moyen en radians
            var mean = moyenneRad * (180 / Math.PI); // Conversion en degrés
            mean = (mean + 360) % 360;
            return mean;
        }
        public static double CalcAngle(double ang)
        {
            if (ang > 1.0)
            {
                ang = 1.0;
            }
            else if (ang < -1.0)
            {
                ang = -1.0;
            }
            return (double)((Math.Asin(ang)) * 57.296);
        }
        public static void CalculateStatistics(string data, int i, Dictionary<int, int> lstData, ref double mean, ref double coeffVariation, ref double variance, ref double standardDeviation, ListBox lstSimulationTurnByTurn)
        {
            int totalOccurrences = lstData.Values.Sum();

            double sommeCos = 0;
            double sommeSin = 0;

            foreach (var t in lstData)
            {
                for (int j = 0; j < t.Value; j++)
                {
                    double angle = t.Key;
                    double radian = angle * (Math.PI / 180); // Conversion en radians
                    sommeCos += Math.Cos(radian);
                    sommeSin += Math.Sin(radian);
                }
            }

            double moyenneRad = Math.Atan2(sommeSin, sommeCos); // Calcul de l'angle moyen en radians
            mean = moyenneRad * (180 / Math.PI); // Conversion en degrés
            mean = (mean + 360) % 360;
            double mean2 = mean;
            // Variance pondérée
            variance = lstData.Sum(kv => kv.Value * Math.Pow(kv.Key - mean2, 2)) / totalOccurrences;

            // Écart-type pondéré
            standardDeviation = Math.Sqrt(variance);

            //codef variation
            coeffVariation = standardDeviation / mean;
            lstSimulationTurnByTurn.Items.Add($"Order {i} Statistic {data} : AVG Angle {mean:F2} Variation {coeffVariation:F2} Variance {variance:F2} Ecart-type {standardDeviation:F2}");
        }
        public static double CalcAngleDistance(double a1, double a2)
        {
            double diff = Math.Abs(a1 - a2) % 360;
            return diff > 180 ? 360 - diff : diff;
        }

        public static double Interpolate(double t0, double x0, double t1, double x1, double t)
        {
            return x0 + (x1 - x0) * ((t - t0) / (t1 - t0));
        }
        public static double[] InterpolateNaNsCircular(double[] values)
        {
            int N = values.Length;
            double[] result = new double[N];
            Array.Copy(values, result, N);

            for (int i = 0; i < N; i++)
            {
                if (double.IsNaN(result[i]))
                {
                    Console.WriteLine("found NAN");
                    // trouver précédent et suivant valides
                    int prev = (i - 1 + N) % N;
                    while (prev != i && double.IsNaN(result[prev]))
                        prev = (prev - 1 + N) % N;
                    int next = (i + 1) % N;
                    while (next != i && double.IsNaN(result[next]))
                        next = (next + 1) % N;

                    if (prev != i && next != i)
                    {
                        double v0 = result[prev], v1 = result[next];
                        int d = (next - prev + N) % N;
                        int idx = (i - prev + N) % N;
                        result[i] = v0 + (v1 - v0) * idx / d;
                    }
                }
            }
            return result;
        }

        // Rééchantillonne une section en N points angulaires pour X,Y,Z
        public static void ResampleSectionAngularXYZ(List<xyz> records, int N, double dt,
            double[] outX, double[] outY, double[] outZ, int offset)
        {
            int raw = records.Count;
            double T = raw * dt;

            for (int j = 0; j < N; j++)
            {
                double tTarget = j * T / (N - 1);
                int i = Math.Min((int)(tTarget / dt), raw - 2);
                double t0 = i * dt, t1 = (i + 1) * dt;

                var r0 = records[i];
                var r1 = records[i + 1];

                outX[offset + j] = Interpolate(t0, r0.x, t1, r1.x, tTarget);
                outY[offset + j] = Interpolate(t0, r0.y, t1, r1.y, tTarget);
                outZ[offset + j] = Interpolate(t0, r0.z, t1, r1.z, tTarget);
            }
        }
        // Rééchantillonne une section en N points angulaires pour X,Y,Z
        public static void ResampleSectionAngularXYZ(double[] inX, double[] inY, double[] inZ, int N, double dt,
            double[] outX, double[] outY, double[] outZ, int offset)
        {
            int raw = inX.Length;
            double T = raw * dt;

            for (int j = 0; j < N; j++)
            {
                double tTarget = j * T / (N - 1);
                int i = Math.Min((int)(tTarget / dt), raw - 2);
                double t0 = i * dt, t1 = (i + 1) * dt;

                outX[offset + j] = Interpolate(t0, inX[i], t1, inX[i + 1], tTarget);
                outY[offset + j] = Interpolate(t0, inY[i], t1, inY[i + 1], tTarget);
                outZ[offset + j] = Interpolate(t0, inZ[i], t1, inZ[i + 1], tTarget);
            }
        }
        // Rééchantillonne une section en N points angulaires pour X,Y,Z
        public static void ResampleSectionGyroXYZ(List<xyz> records, int N, double dt,
            double[] outX, double[] outY, double[] outZ, int offset)
        {
            int raw = records.Count;
            double T = raw * dt;

            for (int j = 0; j < N; j++)
            {
                double tTarget = j * T / (N - 1);
                int i = Math.Min((int)(tTarget / dt), raw - 2);
                double t0 = i * dt, t1 = (i + 1) * dt;

                var r0 = records[i];
                var r1 = records[i + 1];

                outX[offset + j] = Interpolate(t0, r0.gx, t1, r1.gx, tTarget);
                outY[offset + j] = Interpolate(t0, r0.gy, t1, r1.gy, tTarget);
                outZ[offset + j] = Interpolate(t0, r0.gz, t1, r1.gz, tTarget);
            }
        }
        public static void AnalyzeAxis(string name, FFTData cmp, double sampleRate, ListBox targetList, ScottPlot.Color plotColor, Plot plt, double f_rot, double fftLimit)
        {
            try
            {
                double filterFFT = fftLimit;
                // Filtrer pour ne garder que les fréquences < filterFFT Hz
                var filtered = cmp.Frequence
                    .Select((f, i) => new { Freq = f, Mag = cmp.Magnitude[i], Index = i, Angle = cmp.AngleDeg[i] })
                    .Where(x => x.Freq < filterFFT)
                    .ToArray();

                // Extraction des fréquences et magnitudes filtrées
                var filteredFreqs = filtered.Select(x => x.Freq).ToArray();
                var filteredMags = filtered.Select(x => x.Mag).ToArray();
                var filteredAngle = filtered.Select(x => x.Angle).ToArray();
                var scatter = plt.Add.Scatter(filteredFreqs, filteredMags, color: plotColor);
                scatter.LegendText = name;
                var scatter2 = plt.Add.Scatter(filteredFreqs, filteredAngle, color: plotColor);
                scatter2.LegendText = "ANGLE" + name;
                scatter2.IsVisible = false;
                //find and draw first 5 harmonics
                for (int i = 0; i < 5; i++)
                {
                    Fundamentale fundCandidate = EquilibrageHelper.GetFundamentalPhase(filteredFreqs, filteredMags, filteredAngle, f_rot * (i + 1));

                    if (fundCandidate != null)
                    {
                        int idxFund = fundCandidate.Index;
                        double f = fundCandidate.Freq;
                        double m = fundCandidate.Magnitude;
                        double angleOffset = fundCandidate.Angle;
                        plt.Add.Marker(f, m, color: Colors.Magenta);
                        plt.Add.Text($"{name}: {f:F2}Hz\n{angleOffset:F0}°", f, m).Color = Colors.DarkGreen;
                        if (targetList != null)
                            targetList.Items.Add($"{name} ordre {i + 1} à {f:F2} Hz → angle ≈ {angleOffset:F0}° (mag {m:F3})");
                    }
                    else
                    {
                        if (targetList != null)
                            targetList.Items.Add($"{name} ordre {i + 1} (~{f_rot:F2} Hz) non trouvé");
                    }
                }
                //find max peaks
             
                //   plt.Plot.AddSignal(cmp.SignalFFTInverse, sampleRate, Color.Black);
                plt.Axes.AutoScale();
            }
            catch
            {

            }
        }

        public static int[] GetPeakPerTurn(double[] data)
        {

            double threshold = data.Average() * 0.02;
            //find peak in each segment to display average of each peak in the listbox
            return FindPeaks(data, 7, threshold)
                .OrderByDescending(i => data[i])
                .Take(5).ToArray();
        }
        public static void DisplayPeaksTemporal(double[] data, double[] angle, string axis, Plot plt, ListBox lst)
        {
            int range = 7;
            double threshold = data.Average() * 0.02;
            var peaks = FindPeaks(data, range, threshold)
                         .OrderByDescending(i => data[i])
                         .Take(5);

            foreach (int idx in peaks)
            {
                plt.Add.Marker(angle[idx], data[idx], color: Colors.Magenta, size: 8);

                plt.Add.Text($"Force {data[idx]}", angle[idx], data[idx]).Color = Colors.DarkGreen;
                if (lst != null)
                    lst.Items.Add($"[{axis}] : Force {data[idx]}");

            }
        }
        public static float FirstPeakPosition(float[] spectrum, float[] frequencies)
        {
            for (var i = 2; i < spectrum.Length - 2; i++)
            {
                if (spectrum[i] > spectrum[i - 2] && spectrum[i] > spectrum[i - 1] &&
                    spectrum[i] > spectrum[i + 2] && spectrum[i] > spectrum[i + 1])
                {
                    return (float)i / spectrum.Length;
                }
            }
            return 0;
        }
        
        public static List<int> FindPeaks(IList<double> values, int range, double threshold)
        {
            var peaks = new List<int>();
            int half = range / 2;
            for (int i = half; i < values.Count - half; i++)
            {
                double current = values[i];
                var window = values.Skip(i - half).Take(range);
                if (window.Max() == current && current - window.Min() > threshold)
                    peaks.Add(i);
            }
            return peaks;
        }
        public static List<PeakInfo> GetTopCommonPeaksWithAmplitude(int[][] samples,double[] data,double tol = 10.0,int minSamples = 2,int topN = 5)
        {
            var all = samples.SelectMany(s => s).Distinct().OrderBy(v => v).ToList();

            // Clusterisation angulaire
            var clusters = new List<List<double>>();
            foreach (var v in all)
            {
                var cluster = clusters.FirstOrDefault(c => c.Any(u => Math.Abs(u - v) <= tol));
                if (cluster != null) cluster.Add(v);
                else clusters.Add(new List<double> { v });
            }

            var peaks = new List<PeakInfo>();

            foreach (var c in clusters)
            {
                int freq = 0;
                double sumAmp = 0;

                for (int t = 0; t < samples.Length; t++)
                {
                    foreach (int x in samples[t])
                    {
                        if (c.Any(v => Math.Abs(x - v) <= tol))
                        {
                            int index = x + t * 360;
                            if (index < data.Length)
                            {
                                sumAmp += Math.Abs(data[index]);
                                freq++;
                            }
                        }
                    }
                }

                if (freq >= minSamples)
                {
                    peaks.Add(new PeakInfo
                    {
                        Mean = c.Average(),
                        Freq = freq,
                        SumAmplitude = sumAmp
                    });
                }
            }
            if (topN > peaks.Count)
                return peaks
                .OrderByDescending(p => p.SumAmplitude)
                .ThenBy(p => p.Mean)
                .ToList();
            return peaks
                .OrderByDescending(p => p.SumAmplitude)
                .ThenBy(p => p.Mean)
                .Take(topN)
                .ToList();
        }
        
        public static (List<double[]> dataSegments, List<double[]> whiteLineSegments) GetSegments(double[] data, double[] whiteLine)
        {
            // Étape 1 : Trouver les indices de début de segment (valeur == 10 dans whiteLine)
            var segmentStartIndices = whiteLine
                .Select((value, index) => new { value, index })
                .Where(x => x.value == 10)
                .Select(x => x.index)
                .ToList();


            var dataSegments = new List<double[]>();
            var whiteLineSegments = new List<double[]>();

            for (int i = 0; i < segmentStartIndices.Count - 1; i++)
            {
                int start = segmentStartIndices[i];
                int end = segmentStartIndices[i + 1];

                var dataSegment = data.Skip(start).Take(end - start).ToArray();
                var whiteLineSegment = whiteLine.Skip(start).Take(end - start).ToArray();

                dataSegments.Add(dataSegment);
                whiteLineSegments.Add(whiteLineSegment);
            }
            //last data
            dataSegments.Add(data.Skip(segmentStartIndices.Last()).ToArray());
            whiteLineSegments.Add(whiteLine.Skip(segmentStartIndices.Last()).ToArray());

            return (dataSegments, whiteLineSegments);
        }
        public static void ShowPeakHistogram(List<PeakInfo> peaks, FormsPlot formsPlot)
        {
            if (peaks == null || peaks.Count == 0)
                return;

            // Tri par angle croissant
            var ordered = peaks.OrderBy(p => p.Mean).ToList();

            // Récupération des données
            double[] positions = ordered.Select(p => p.Mean).ToArray();
            double[] heights = ordered.Select(p => p.AverageAmplitude).ToArray();
            string[] labels = ordered.Select(p => p.Freq.ToString()).ToArray();

            // Nettoyer le graphe
            formsPlot.Plot.Clear();

            // Tracer le bar plot
            var bars = formsPlot.Plot.Add.Bars(positions, heights);
            bars.Bars.ForEach(b => b.LineWidth = 8);
            bars.Color = Colors.SteelBlue;

            // Ajouter les labels de fréquence au-dessus de chaque barre
            for (int i = 0; i < positions.Length; i++)
            {
                double x = positions[i];
                double y = heights[i];
                string label = labels[i];
                formsPlot.Plot.Add.Text(label, x, y + 0.01);
            }

            // Ajuster les axes
            formsPlot.Plot.Title("Histogramme des pics groupés par angle");
            formsPlot.Plot.XLabel("Angle (degrés)");
            formsPlot.Plot.YLabel("Amplitude moyenne");
            formsPlot.Plot.Axes.SetLimitsX(0, 360); // pour rester entre 0 et 360°
            formsPlot.Plot.Axes.AutoScaleY();

            formsPlot.Refresh();
        }

        public static void DisplayTurnByTurnGraph(Dictionary<int, int> lstBestAngleInner, Dictionary<int, int> lstBestAngleOuter, Dictionary<int, int> lstBestAngleX, Dictionary<int, int> lstBestAngleY, FormsPlot frmInner, FormsPlot frmOuter, FormsPlot frmX, FormsPlot frmY)
        {
            try
            {
                double[] xs = lstBestAngleInner.Keys.Select(x => (double)x).ToArray();
                double[] ys = lstBestAngleInner.Values.Select(y => (double)y).ToArray();
                if (xs.Length > 0)
                {
                    var b = frmInner.Plot.Add.Bars(xs, ys);
                    b.Bars.ForEach(ba => { ba.LineWidth = 0.5f; ba.FillColor = ba.LineColor = Colors.Blue; });
                }
                xs = lstBestAngleOuter.Keys.Select(x => (double)x).ToArray();
                ys = lstBestAngleOuter.Values.Select(y => (double)y).ToArray();
                if (xs.Length > 0)
                {
                    var b = frmOuter.Plot.Add.Bars(xs, ys);
                    b.Bars.ForEach(ba => { ba.LineWidth = 0.5f; ba.FillColor = ba.LineColor = Colors.Blue; });

                }
                xs = lstBestAngleX.Keys.Select(x => (double)x).ToArray();
                ys = lstBestAngleX.Values.Select(y => (double)y).ToArray();
                if (xs.Length > 0)
                {
                    //frmX.Plot.PlotBar(xs, ys, barWidth: 0.5, fillColor: Color.Blue, outlineColor: Color.Blue);
                    var b = frmX.Plot.Add.Bars(xs, ys);
                    b.Bars.ForEach(ba => { ba.LineWidth = 0.5f; ba.FillColor = ba.LineColor = Colors.Blue; });

                }
                xs = lstBestAngleY.Keys.Select(x => (double)x).ToArray();
                ys = lstBestAngleY.Values.Select(y => (double)y).ToArray();
                if (xs.Length > 0)
                {
                    //   frmY.Plot.PlotBar(xs, ys, barWidth: 0.5, fillColor: Color.Blue, outlineColor: Color.Blue);
                    var b = frmY.Plot.Add.Bars(xs, ys);
                    b.Bars.ForEach(ba => { ba.LineWidth = 0.5f; ba.FillColor = ba.LineColor = Colors.Blue; });
                }
            }
            catch
            { }
        }

        public static void CalculateGyroAngles(ref double[] analyzedX, ref double[] analyzedY, ref double[] analyzedZ, ref double[] resultante, ref double[] angleX, ref double[] angleY, ref double[] angleZ, ref double[] pitch, ref double[] roll)
        {
            int count = analyzedX.Count();
            for (int i = 0; i < count; i++)
            {
                resultante[i] = Math.Sqrt(Math.Pow(analyzedX[i], 2)
                                            + Math.Pow(analyzedY[i], 2)
                                            + Math.Pow(analyzedZ[i], 2));
                angleX[i] = CalcAngle(analyzedX[i]);
                angleY[i] = CalcAngle(analyzedY[i]);
                angleZ[i] = CalcAngle(analyzedZ[i]);
                pitch[i] = (Math.Atan2(-angleX[i], Math.Sqrt(Math.Abs((angleY[i] * angleY[i] + angleZ[i] * angleZ[i])))) * 180.0) / Math.PI;
                roll[i] = (Math.Atan2(angleY[i], angleZ[i]) * 180.0) / Math.PI;
            }
        }

        internal static void AnalyzeAxisTemporal(string axisName, double[] data, double[] angle,double sampleRate, ListBox lstPeak, Color c, Plot plotTemporal, double f_rot)
        {
            var peaks = MathHelper.GetPeakPerTurn(data);
            foreach (var item in peaks)
                lstPeak.Items.Add($"Peak: Angle {angle[item]} – Force {data[item]}");
            double[] temporal = Enumerable.Range(0, data.Length)
                                    .Select(i => (double)i)
                                    .ToArray();
            var sp = plotTemporal.Add.Scatter(temporal, data, c);
            sp.MarkerShape = MarkerShape.None;
            sp.LegendText = axisName;
            MathHelper.DisplayPeaksTemporal(data, temporal, "Top Peak "+axisName, plotTemporal,null);
        }
    }
}
