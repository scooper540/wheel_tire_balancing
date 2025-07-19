using System;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace equilibreuse
{
    public class FFTData
    {
        public double[] AngleDeg;
        public double[] Frequence;
        public double[] Magnitude;
    }
    public class Fundamentale
    {
        public double Freq { get; set; }
        public double Magnitude { get; set; }
        public double Angle { get; set; }
        public int Index { get; set; }
    }
    public class DynamicImbalanceResult
    {
        public bool IsDynamic { get; set; }
        public double FrequencyHzX { get; set; }
        public double FrequencyHzY { get; set; }
        public double MagnitudeX { get; set; }
        public double MagnitudeY { get; set; }
        public double PhaseDifferenceDeg { get; set; }
        public DynamicCorrectionResult correction { get; set; }
        public override string ToString()
        {
            return (IsDynamic ? $"Imbalance found: FX {FrequencyHzX} FY {FrequencyHzY} {correction.ToString()} MagnX {MagnitudeX} MagnY {MagnitudeY} Diff Phase {PhaseDifferenceDeg}" : "Imbalance not found");
        }
    }
    public class CalculationResult
    {
        public PolarSeriesResult[] px = new PolarSeriesResult[5], py = new PolarSeriesResult[5], pz = new PolarSeriesResult[5], pResultante = new PolarSeriesResult[5];

        public DynamicImbalanceResult[] dir = new DynamicImbalanceResult[5];
    }
    public class DynamicCorrectionResult
    {
        public double AngleInnerDeg;   // Flasque intérieur
        public double AngleOuterDeg;   // Flasque extérieur
        public double Magnitude;       // Amplitude de correction (relative)
        public override string ToString()
        {
            return $"Angle intérieur: {AngleInnerDeg}° Angle exterieur {AngleOuterDeg}°";
        }
    }
    public class PolarSeriesResult
    {
        public double MinAmplitude;
        public double BestAngleDeg;
        public double ActualAmplitude;
    }
    public static class EquilibrageHelper
    {
        public static DynamicCorrectionResult EstimateDynamicImbalanceCorrection(double phaseX, double magX,double phaseY, double magY)
        {
            // 1. Décomposition en vecteurs
            double angleDegSigned = phaseX > 180.0 ? phaseX - 360.0 : phaseX;
            double phaseXRad = angleDegSigned * (Math.PI / 180.0);

            angleDegSigned = phaseY > 180.0 ? phaseY - 360.0 : phaseY;
            double phaseYRad = angleDegSigned * (Math.PI / 180.0);

            double vx = magX * Math.Cos(phaseXRad);
            double vy = magX * Math.Sin(phaseXRad);

            double wx = magY * Math.Cos(phaseYRad);
            double wy = magY * Math.Sin(phaseYRad);

            // 2. Calcul du couple dynamique (vecteur rotationnel entre X et Y)
            //    On prend la différence des deux vecteurs (sens dynamique)
            double dx = vx - wx;
            double dy = vy - wy;

            // 3. Angle moyen pour placer les 2 masses (± 90° de différence)
            double angleRad = Math.Atan2(dy, dx); // angle en radians
            double angleDeg = (angleRad * 180.0 / Math.PI + 360) % 360;

            // 4. Définir les deux angles de correction (opposés de l'effort dynamique)
            double angleInner = (angleDeg + 90) % 360; // +90° → masse intérieure
            double angleOuter = (angleDeg - 90 + 360) % 360; // -90° → masse extérieure

            // 5. Estimation simple de la "force" à corriger
            double magnitude = Math.Sqrt(dx * dx + dy * dy);

            return new DynamicCorrectionResult
            {
                AngleInnerDeg = angleInner,
                AngleOuterDeg = angleOuter,
                Magnitude = magnitude
            };
        }
        public static DynamicImbalanceResult HasDynamicImbalance(FFTData x, FFTData y, double sampleRate, double f_rot,double minMagnitude = 0.05,double minFreq = 1.0,double maxFreq = 20.0,double toleranceHz = 0.3,double phaseDiffTolDeg = 30.0)
        {
            
            var fundaX = EquilibrageHelper.GetFundamentalPhase(x.Frequence, x.Magnitude, x.AngleDeg, f_rot);
            var fundaY = EquilibrageHelper.GetFundamentalPhase(y.Frequence, y.Magnitude, y.AngleDeg, f_rot);
           
            
            double magX = fundaX.Magnitude;
            double magY = fundaY.Magnitude;

            if (magX >= minMagnitude && magY >= minMagnitude)
            {
            if (Math.Abs(fundaX.Freq - fundaY.Freq) > 3.0)
                return new DynamicImbalanceResult
                {
                    IsDynamic = false,
                    FrequencyHzX = 0,
                    FrequencyHzY = 0,
                            MagnitudeX = 0,
                            MagnitudeY = 0,
                            PhaseDifferenceDeg = 0
                        }; ;
                
                double phaseDiffDeg = Math.Abs((fundaX.Angle - fundaY.Angle));
                if (phaseDiffDeg > 180) phaseDiffDeg = 360 - phaseDiffDeg;

                bool isDynamic = phaseDiffDeg >= (90 - phaseDiffTolDeg) && phaseDiffDeg <= (90 + phaseDiffTolDeg);

                return new DynamicImbalanceResult
                {
                    IsDynamic = isDynamic,
                    FrequencyHzX = fundaX.Freq,
                    FrequencyHzY = fundaY.Freq,
                    MagnitudeX = magX,
                    MagnitudeY = magY,
                    PhaseDifferenceDeg = phaseDiffDeg,
                    correction = EstimateDynamicImbalanceCorrection(fundaX.Angle, fundaX.Magnitude, fundaY.Angle, fundaY.Magnitude)
                };
            }

            // Aucun déséquilibre dynamique trouvé
            return new DynamicImbalanceResult
            {
                IsDynamic = false,
                FrequencyHzX = 0,
                FrequencyHzY = 0,
                MagnitudeX = 0,
                MagnitudeY = 0,
                PhaseDifferenceDeg = 0
            };

        }
        public static double SimulateCorrectionAngle(double[] signal, double sampleRate, double fRot, double gain = 1.0, double[] window = null, int angleStepDeg = 10)
        {
            int n = signal.Length;
            Complex[] fftInput = new Complex[n];

            for (int i = 0; i < n; i++)
            {
                double value = signal[i];
                if (window != null)
                    value *= window[i];
                fftInput[i] = new Complex(value, 0);
            }

            // FFT réelle
            Fourier.Forward(fftInput, FourierOptions.Matlab);

            // Recherche du pic à la fréquence de rotation
            double binSize = sampleRate / n;
            int peakIndex = (int)Math.Round(fRot / binSize);

            if (peakIndex <= 0 || peakIndex >= n)
                return -1; // erreur ou signal non exploitable

            Complex fundamental = fftInput[peakIndex];
            double originalAmplitude = fundamental.Magnitude;

            double bestReduction = double.MaxValue;
            double bestAngle = 0;

            for (int angleDeg = 0; angleDeg < 360; angleDeg += angleStepDeg)
            {
                double angleRad = angleDeg * Math.PI / 180.0;

                // Ajout d’une contre-masse virtuelle opposée
                double correctedReal = fundamental.Real + Math.Cos(angleRad) * gain;
                double correctedImag = fundamental.Imaginary + Math.Sin(angleRad) * gain;
                double correctedAmplitude = Math.Sqrt(correctedReal * correctedReal + correctedImag * correctedImag);

                if (correctedAmplitude < bestReduction)
                {
                    bestReduction = correctedAmplitude;
                    bestAngle = angleDeg;
                }
            }

            return bestAngle;
        }
        public static PolarSeriesResult GetPolarSeries(FFTData data, double sampleRate,double f_rot, double gain,int stepDeg)
        {
            var funda = EquilibrageHelper.GetFundamentalPhase(data.Frequence, data.Magnitude,data.AngleDeg, f_rot);
            if (funda == null)
            {
                throw new Exception("Impossible de trouver la fréquence de rotation");
            }

            PolarSeriesResult res = new PolarSeriesResult();
           
            res.MinAmplitude = funda.Magnitude;
            res.BestAngleDeg = (funda.Angle + 180) % 360;
            res.ActualAmplitude = funda.Magnitude;
            return res;
        }
        public static CalculationResult CompleteSimulation(ListBox lstData, String diagram, FFTData x, FFTData y, FFTData z, FFTData resultante, double sampleRate, double fRot, double gain = 1.0, int stepDeg = 1)
        {
            CalculationResult cr = new CalculationResult();
            for (int i = 1; i < 6; i++)
            {
                PolarSeriesResult px = new PolarSeriesResult(), py = new PolarSeriesResult(), pz = new PolarSeriesResult(), pr = new PolarSeriesResult();
                try
                {
                    px = GetPolarSeries(x, sampleRate, fRot*i, gain, stepDeg);
                    cr.px[i - 1] = px;
                }
                catch (Exception ex)
                {
                    if(lstData != null)
                        lstData.Items.Add($"{diagram} X : {ex.ToString()}");
                }
                try
                {
                    py = GetPolarSeries(y, sampleRate, fRot * i, gain, stepDeg);
                    cr.py[i - 1] = py;
                }
                catch (Exception ex)
                {
                    if (lstData != null)
                        lstData.Items.Add($"{diagram} Y : {ex.ToString()}");
                }
                try
                {
                    pz = GetPolarSeries(z, sampleRate, fRot * i, gain, stepDeg);
                    cr.pz[i - 1] = pz;
                }
                catch (Exception ex)
                {
                    if (lstData != null)
                        lstData.Items.Add($"{diagram} Z : {ex.ToString()}");
                }
                try
                {
                    pr = GetPolarSeries(resultante, sampleRate, fRot * i, gain, stepDeg);
                    cr.pResultante[i - 1] = pr;
                }
                catch (Exception ex)
                {
                    if (lstData != null)
                        lstData.Items.Add($"{diagram} Resultante : {ex.ToString()}");
                }
                cr.dir[i - 1] = EquilibrageHelper.HasDynamicImbalance(x, y, sampleRate, fRot * i);
                if (lstData != null)
                {
                    lstData.Items.Add($"STATIC {diagram} X : Order {i} Angle determiné {px.BestAngleDeg} Freq {fRot * i} Amplitude {px.MinAmplitude}");
                    lstData.Items.Add($"STATIC {diagram} Y : Order {i} Angle determiné {py.BestAngleDeg} Freq {fRot * i} Amplitude {py.MinAmplitude}");
                    lstData.Items.Add($"STATIC {diagram} Z : Order {i}Angle determiné {pz.BestAngleDeg} Freq {fRot * i} Amplitude {pz.MinAmplitude}");
                    lstData.Items.Add($"STATIC {diagram} Resultante : Order {i} Angle determiné {pr.BestAngleDeg} Freq {fRot * i} Amplitude {pr.MinAmplitude}");

                    lstData.Items.Add($"Global dynamic imbalance (X and Y) order {i} : {cr.dir[i - 1].ToString()}");
                }
            }
            return cr;
        }
        public static void RemoveDCOffset(double[] signal)
        {
            if (signal == null || signal.Length == 0) return;

            // Calcul de la moyenne (composante DC)
            double mean = 0;
            for (int i = 0; i < signal.Length; i++)
            {
                mean += signal[i];
            }
            mean /= signal.Length;

            // Soustraction de la moyenne à chaque point
            for (int i = 0; i < signal.Length; i++)
            {
                signal[i] -= mean;
            }
        }
        // Extraction amplitude & phase à f_rot
        public static Fundamentale GetFundamentalPhase(double[] freq, double[] mags, double[] angle, double f_rot, double tolerance = 1, double freqMin = 1.0)
        { 
            // Extrait les index dont freq est proche de f_rot (± tolérence)
            var fundCandidate = freq
                .Select((f, i) => new { Freq = f, Mag = mags[i], Angle=angle[i], Index = i })
                .Where(x => ((x.Freq > freqMin) && (Math.Abs(x.Freq - f_rot) <= tolerance)))
                .OrderBy(x => Math.Abs(x.Freq - f_rot)) //order by freq
                //.OrderByDescending(x => x.Mag) // order by magnitude
                .FirstOrDefault();
            if (fundCandidate == null) return new Fundamentale() { Freq = 0, Index = 0, Magnitude = 0, Angle = 0};
            return new Fundamentale() { Freq = fundCandidate.Freq, Index = fundCandidate.Index, Magnitude = fundCandidate.Mag , Angle= fundCandidate.Angle};
        }

     
        internal static FFTData CalculateFFT(double[] signal, double sampleRate, ComboBox cbxFFT, bool bRemoveDC)
        {        
            // Paramètres du zero-padding
            int count = signal.Count(); //should be 360
            if (bRemoveDC)
                RemoveDCOffset(signal);

            int zeroPadFactor = 8;
            int fftSize = count * zeroPadFactor;
            fftSize = NextPowerOfTwo(fftSize);
            // Application de la fenêtre si sélectionnée
            double[] window = null;
            if (cbxFFT.SelectedItem != null)
            {
                string windowType = cbxFFT.SelectedItem.ToString();

                switch (windowType)
                {
                    case "None": break;
                    case "Hann": window = Window.Hann(count); break;
                    case "HannPeriodic": window = Window.HannPeriodic(count); break;
                    case "Hamming": window = Window.Hamming(count); break;
                    case "HammingPeriodic": window = Window.HammingPeriodic(count); break;
                    case "Blackman": window = Window.Blackman(count); break;
                    case "BlackmanHarris": window = Window.BlackmanHarris(count); break;
                    case "BlackmanNuttal": window = Window.BlackmanNuttall(count); break;
                    case "FlatTop": window = Window.FlatTop(count); break;
                    default: break;
                };
            }
            
            float[] re = new float[fftSize];
            float[] im = new float[fftSize];

            for (int i = 0; i < count; i++)
                re[i] = (float)signal[i];
            if (window != null)
            {
                for (int i = 0; i < count; i++)
                {
                    re[i] *= (float)window[i];
                }
            }
            new NWaves.Transforms.Fft(fftSize).Direct(re, im);
            FFTData data = new FFTData();
            var resolution = (double)sampleRate / fftSize;
            var frequencies = Enumerable.Range(0, fftSize / 2 + 1)
                                        .Select(f => f * resolution)
                                        .ToArray();
            data.Frequence = frequencies;
            data.AngleDeg = new double[data.Frequence.Length];
            data.Magnitude = new double[data.Frequence.Length];
            for (int i = 0; i < data.Frequence.Length; i++)
            {
                data.Magnitude[i] = (double)Math.Sqrt(re[i] * re[i] + im[i] * im[i]);
                data.AngleDeg[i]  = (double)(Math.Atan2(im[i], re[i]) * (180.0 / Math.PI) + 360) % 360;
            }
            data.Magnitude[0] = Math.Abs(re[0]);
            return data;
        }
        internal static Complex[] CalculateFFT2(double[] signal, ComboBox cbxFFT,bool bRemoveDC)
        {
            // Paramètres du zero-padding
            int count = signal.Count(); //should be 360
            if(bRemoveDC)
                RemoveDCOffset(signal);
         
            int zeroPadFactor = 8;
            int fftSize = count * zeroPadFactor;
            fftSize = NextPowerOfTwo(fftSize);
            // Application de la fenêtre si sélectionnée
            double[] window = null;
            if (cbxFFT.SelectedItem != null)
            {
                string windowType = cbxFFT.SelectedItem.ToString();

                switch (windowType)
                {
                    case "None": break;
                    case "Hann": window = Window.Hann(count); break;
                    case "HannPeriodic": window = Window.HannPeriodic(count); break;
                    case "Hamming": window = Window.Hamming(count); break;
                    case "HammingPeriodic": window = Window.HammingPeriodic(count); break;
                    case "Blackman": window = Window.Blackman(count); break;
                    case "BlackmanHarris": window = Window.BlackmanHarris(count); break;
                    case "BlackmanNuttal": window = Window.BlackmanNuttall(count); break;
                    case "FlatTop": window = Window.FlatTop(count); break;
                    default: break;
                };
            }
            Complex[] cmp = new Complex[fftSize];

            for (int i = 0; i < count; i++)
                cmp[i] = new Complex((signal[i]), 0);
            for (int i = count; i < fftSize; i++)
                cmp[i] = Complex.Zero;
            if (window != null)
            {
                for (int i = 0; i < count; i++)
                {
                    cmp[i] *= window[i];
                }
            }
            Fourier.Forward(cmp, FourierOptions.Matlab);
            return cmp;
        }

        public static int NextPowerOfTwo(int value)
        {
            if (value < 1)
                throw new ArgumentException("Value must be greater than 0.");

            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }
        // Affiche une boîte de résumé (optionnel)
        public static void ShowCorrectionMessage(double amp, double phase, double correction)
        {
            MessageBox.Show(
                $"Amplitude: {amp:F2} g\nPhase mesurée: {phase:F1}°\n=> Ajouter masse à: {correction:F1}°",
                "Équilibrage dynamique",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        public static double GainToGrams(double accelerationG, double radiusMeters, double rpm)
        {
            // 1 g ≈ 9.80665 m/s²
            double acceleration = accelerationG * 9.80665;

            // Convert RPM to rad/s
            double omega = 2.0 * Math.PI * rpm / 60.0;

            // Force centrifuge : F = m * r * ω² => m = a / (r * ω²)
            double denominator = radiusMeters * omega * omega;
            if (denominator == 0) return 0;

            double massKg = acceleration / denominator;

            // Convert kg to grams
            return massKg * 1000.0;
        }
        public static void SaveWav(string filePath, double[] samples, int sampleRate)
        {
            int bitsPerSample = 16;
            short numChannels = 1;
            short blockAlign = (short)(numChannels * bitsPerSample / 8);
            int byteRate = sampleRate * blockAlign;
            int dataSize = samples.Length * blockAlign;

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    // === RIFF HEADER ===
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                    writer.Write(36 + dataSize); // total file size - 8 bytes
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

                    // === fmt subchunk ===
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                    writer.Write(16);                      // Subchunk1Size for PCM
                    writer.Write((short)1);                // AudioFormat = PCM
                    writer.Write(numChannels);
                    writer.Write(sampleRate);
                    writer.Write(byteRate);
                    writer.Write(blockAlign);
                    writer.Write((short)bitsPerSample);

                    // === data subchunk ===
                    writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                    writer.Write(dataSize);

                    // === Write samples ===
                    foreach (var sample in samples)
                    {
                        // Clamp entre -1.0 et 1.0
                        double clamped = Math.Max(-1.0, Math.Min(1.0, sample));
                        short intSample = (short)(clamped * short.MaxValue);
                        writer.Write(intSample);
                    }
                }
            }
        }
    }
}