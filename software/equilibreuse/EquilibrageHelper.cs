using System;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MathNet.Numerics.LinearAlgebra;

namespace equilibreuse
{
    public class PhaseAnalysis
    {
        public double rMaxTemporal;
        public double rPhaseLockIn;
        public double rFitSinusoid;
        public double rDetectPhase;
        public double rFFT;

    }
    public class FFTData
    {
        public double[] AngleDeg;
        public double[] Frequence;
        public double[] Magnitude;
        public double[] SignalFFTInverse;
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
        public double UnbalanceAngleDeg;
        public double ActualAmplitude;
    }
    public static class EquilibrageHelper
    {
        public static PhaseAnalysis AnalyzeSignal(double[] signal, double sampleRate, double f_rot, ComboBox cbxFFT, CheckBox chkDb, double rpm)
        {
            var res = new PhaseAnalysis();
            res.rPhaseLockIn = EquilibrageHelper.ComputeLockInPhase(signal, f_rot, sampleRate);
            res.rFitSinusoid = EquilibrageHelper.FitSinusoidPhase(signal, f_rot, sampleRate);
            res.rDetectPhase = EquilibrageHelper.DetectPhase(signal, sampleRate, f_rot).phaseDegrees;
            var fft = EquilibrageHelper.CalculateFFT(signal, sampleRate, cbxFFT, chkDb.Checked, rpm, f_rot);
            var fund = EquilibrageHelper.GetFundamentalPhase(fft.Frequence, fft.Magnitude, fft.AngleDeg, f_rot);
            res.rFFT = fund.Angle;
            return res;
        }
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
            double angleInner = (angleDeg + 45) % 360; // +45° → masse intérieure
            double angleOuter = (angleDeg - 45 + 360) % 360; // -45° → masse extérieure

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
                    };
                
                double phaseDiffDeg = Math.Abs((fundaX.Angle - fundaY.Angle));
                if (phaseDiffDeg > 180) phaseDiffDeg = 360 - phaseDiffDeg;

                bool isDynamic = true;// phaseDiffDeg >= (90 - phaseDiffTolDeg) && phaseDiffDeg <= (90 + phaseDiffTolDeg);

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
        public static PolarSeriesResult GetPolarSeries(FFTData data, double sampleRate,double f_rot, int stepDeg)
        {
            var funda = EquilibrageHelper.GetFundamentalPhase(data.Frequence, data.Magnitude,data.AngleDeg, f_rot);
            if (funda == null)
            {
                throw new Exception("Impossible de trouver la fréquence de rotation");
            }

            PolarSeriesResult res = new PolarSeriesResult();
           
            res.MinAmplitude = funda.Magnitude;
            res.UnbalanceAngleDeg = (funda.Angle) % 360;
            res.ActualAmplitude = funda.Magnitude;
            return res;
        }
        public static CalculationResult CompleteSimulation(ListBox lstData, String diagram, FFTData x, FFTData y, FFTData z, FFTData resultante, double sampleRate, double fRot, double correctXAngle, double correctYAngle, int stepDeg = 1)
        {
            CalculationResult cr = new CalculationResult();
            for (int i = 1; i < 6; i++)
            {
                PolarSeriesResult px = new PolarSeriesResult(), py = new PolarSeriesResult(), pz = new PolarSeriesResult(), pr = new PolarSeriesResult();
                try
                {
                    px = GetPolarSeries(x, sampleRate, fRot*i, stepDeg);
                    cr.px[i - 1] = px;
                }
                catch (Exception ex)
                {
                    if(lstData != null)
                        lstData.Items.Add($"{diagram} X : {ex.ToString()}");
                }
                try
                {
                    py = GetPolarSeries(y, sampleRate, fRot * i, stepDeg);
                    cr.py[i - 1] = py;
                }
                catch (Exception ex)
                {
                    if (lstData != null)
                        lstData.Items.Add($"{diagram} Y : {ex.ToString()}");
                }
                try
                {
                    pz = GetPolarSeries(z, sampleRate, fRot * i, stepDeg);
                    cr.pz[i - 1] = pz;
                }
                catch (Exception ex)
                {
                    if (lstData != null)
                        lstData.Items.Add($"{diagram} Z : {ex.ToString()}");
                }
                try
                {
                    pr = GetPolarSeries(resultante, sampleRate, fRot * i, stepDeg);
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
                    lstData.Items.Add($"STATIC {diagram} X : Order {i} Angle determiné balourd {px.UnbalanceAngleDeg} Freq {fRot * i} Amplitude {px.MinAmplitude}");
                    lstData.Items.Add($"STATIC {diagram} Y : Order {i} Angle determiné balourd {py.UnbalanceAngleDeg} Freq {fRot * i} Amplitude {py.MinAmplitude}");
                    lstData.Items.Add($"STATIC {diagram} Z : Order {i} Angle determiné balourd {pz.UnbalanceAngleDeg} Freq {fRot * i} Amplitude {pz.MinAmplitude}");
                    lstData.Items.Add($"STATIC {diagram} Resultante : Order {i} Angle determiné {pr.UnbalanceAngleDeg} Freq {fRot * i} Amplitude {pr.MinAmplitude}");

                    lstData.Items.Add($"Global dynamic imbalance (X and Y) order {i} : {cr.dir[i - 1].ToString()}");
                }
            }
            return cr;
        }
      
        // Extraction amplitude & phase à f_rot
        public static Fundamentale GetFundamentalPhase(double[] freq, double[] mags, double[] angle, double f_rot, double tolerance = 1, double freqMin = 1.0)
        { 
            // Extrait les index dont freq est proche de f_rot (± tolérence)
            var fundCandidate = freq
                .Select((f, i) => new { Freq = f, Mag = mags[i], Angle=angle[i], Index = i })
                .Where(x => ((x.Freq > freqMin) && (Math.Abs(x.Freq - f_rot) <= tolerance)))
                .OrderBy(x => Math.Abs(x.Freq - f_rot)) //order by freq
              //  .OrderByDescending(x => x.Mag) // order by magnitude
                .FirstOrDefault();
            if (fundCandidate == null) return new Fundamentale() { Freq = 0, Index = 0, Magnitude = 0, Angle = 0};
            return new Fundamentale() { Freq = fundCandidate.Freq, Index = fundCandidate.Index, Magnitude = fundCandidate.Mag , Angle= fundCandidate.Angle};
        }

     
        internal static FFTData CalculateFFT(double[] signal, double sampleRate, ComboBox cbxFFT,bool bdB,double rpm, double f_rot)
        {        
            // Paramètres du zero-padding
            int count = signal.Count();

            int zeroPadFactor = 12;
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
            double omega = 2 * Math.PI * rpm / 60.0;
            for (int i = 0; i < data.Frequence.Length; i++)
            {
                data.Magnitude[i] = (double)Math.Sqrt(re[i] * re[i] + im[i] * im[i]);
               
                data.Magnitude[i] = data.Magnitude[i] / (omega * omega); // ou mag / (rpm * rpm) si tu restes en RPM
                if (bdB)
                    data.Magnitude[i] = 10 * Math.Log10(data.Magnitude[i]) - 10 * Math.Log10(fftSize);
                data.AngleDeg[i]  = (double)(Math.Atan2(im[i], re[i]) * (180.0 / Math.PI) + 360) % 360;
            }
            data.Magnitude[0] = Math.Abs(re[0]) / (omega * omega);

            //calculate FFT inverse on fundamentale
            var fund = EquilibrageHelper.GetFundamentalPhase(data.Frequence, data.Magnitude, data.AngleDeg, f_rot);
            int k = fund.Index;
            // 3. Conserver uniquement ce bin et son symétrique conjugué
            for (int i = 0; i < fftSize; i++)
            {
                if (i != k && i != fftSize - k)
                {
                    re[i] = im[i] = 0;
                }
            }
            new NWaves.Transforms.Fft(fftSize).Inverse(re, im);
            data.SignalFFTInverse = new double[count];
            for (int i = 0; i < count; i++)
                data.SignalFFTInverse[i] = re[i];
            return data;
        }

        public static (double amplitude, double phaseDegrees) DetectPhase(double[] signal, double sampleRateHz, double targetFreqHz)
        {
            // 1. Préparation des signaux de référence
            double[] t = Enumerable.Range(0, signal.Length)
                                 .Select(i => i / sampleRateHz)
                                 .ToArray();

            // 2. Génération des références sin/cos
            var referenceSin = t.Select(x => Math.Sin(2 * Math.PI * targetFreqHz * x)).ToArray();
            var referenceCos = t.Select(x => Math.Cos(2 * Math.PI * targetFreqHz * x)).ToArray();

            // 3. Calcul des composantes I (In-Phase) et Q (Quadrature)
            double I = 0, Q = 0;
            for (int i = 0; i < signal.Length; i++)
            {
                I += signal[i] * referenceSin[i];
                Q += signal[i] * referenceCos[i];
            }
            I *= 2.0 / signal.Length; // Normalisation
            Q *= 2.0 / signal.Length;

            // 4. Calcul amplitude et phase
            double amplitude = Math.Sqrt(I * I + Q * Q);
            double phaseRad = Math.Atan2(Q, I); // [-π, π]
            double phaseDeg = ((phaseRad * 180 / Math.PI) + 360) % 360;

            return (amplitude, phaseDeg);
        }
        public static (double amplitude, double phaseDegrees) DetectPhaseWithSweep(
                double[] signal,
                double sampleRateHz,
                double centerFreqHz,
                double sweepRangeHz = 0, // ±1 Hz par défaut
                int steps = 1)            // Nombre de points de scan
        {
            double maxAmplitude = 0;
            double bestPhaseDeg = 0;
            double bestFreqHz = centerFreqHz;

            // 1. Balayage linéaire autour de la fréquence cible
            for (int i = 0; i <= steps; i++)
            {
                double currentFreqHz = centerFreqHz - sweepRangeHz + (2 * sweepRangeHz * i / steps);

                var f = DetectPhase(signal, sampleRateHz, currentFreqHz);
                double amplitude = f.amplitude;
                // 3. Mise à jour du meilleur résultat
                if (amplitude > maxAmplitude)
                {
                    maxAmplitude = amplitude;
                    bestPhaseDeg = f.phaseDegrees;
                    bestFreqHz = currentFreqHz;
                }
            }

           // Console.WriteLine($"Fréquence optimale trouvée : {bestFreqHz:F3} Hz");
            return (maxAmplitude, bestPhaseDeg);
        }
        public static double[] ComputePhaseHilbert(double[] signal, double samplingRate)
        {
            // 1. Calcul de la FFT
            Complex32[] fft = new Complex32[signal.Length];
            for (int i = 0; i < signal.Length; i++)
                fft[i] = new Complex32((float)signal[i], 0);

            Fourier.Forward(fft, FourierOptions.Default);

            // 2. Application de la transformée de Hilbert (déphasage de 90°)
            for (int i = 1; i < fft.Length / 2; i++)
                fft[i] *= Complex32.FromPolarCoordinates(1, (float)(-Math.PI / 2)); // -90°

            // 3. FFT inverse
            Fourier.Inverse(fft, FourierOptions.Default);

            // 4. Extraction de la phase
            double[] phase = new double[signal.Length];
            for (int i = 0; i < signal.Length; i++)
                phase[i] = Math.Atan2(fft[i].Imaginary, signal[i]) * 180 / Math.PI; // en degrés

            return phase;
        }


        public static double ComputeLockInPhase(double[] signal, double freqFundamental, double samplingRate)
        {
            double[] referenceSin = GenerateSineWave(freqFundamental, samplingRate, signal.Length, 0);
            double[] referenceCos = GenerateSineWave(freqFundamental, samplingRate, signal.Length, 90);

            // Multiplier et intégrer (moyenne)
            double I = 0, Q = 0;
            for (int i = 0; i < signal.Length; i++)
            {
                I += signal[i] * referenceSin[i];
                Q += signal[i] * referenceCos[i];
            }
            I /= signal.Length;
            Q /= signal.Length;

            return ((Math.Atan2(Q, I) * 180 / Math.PI)+360)%360; // Phase en degrés
        }

        private static double[] GenerateSineWave(double freq, double samplingRate, int length, double phaseDeg)
        {
            double[] wave = new double[length];
            double phaseRad = phaseDeg * Math.PI / 180;
            for (int i = 0; i < length; i++)
                wave[i] = Math.Sin(2 * Math.PI * freq * i / samplingRate + phaseRad);
            return wave;
        }




    public static double FitSinusoidPhase(double[] signal, double freqFundamental, double samplingRate)
    {
        // Construction de la matrice A = [sin(2πft), cos(2πft)]
        var A = Matrix<double>.Build.Dense(signal.Length, 2);
        for (int i = 0; i < signal.Length; i++)
        {
            double t = i / samplingRate;
            A[i, 0] = Math.Sin(2 * Math.PI * freqFundamental * t);
            A[i, 1] = Math.Cos(2 * Math.PI * freqFundamental * t);
        }

        // Résolution Ax = b (b = signal)
        var b = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(signal);
        var x = A.TransposeThisAndMultiply(A).Inverse() * A.TransposeThisAndMultiply(b);

        // Phase = atan2(cos_coeff, sin_coeff)
        return ((Math.Atan2(x[1], x[0]) * 180 / Math.PI)+360)%360;
    }
    public static string GetStatisticsFundamental(string sTitle, double totalSamples, double sampleRate, double magnitude, double numberoftours)
        {

            // 1. Durée du signal (s)
            double duration = totalSamples / sampleRate;

            // 2. Magnitude normalisée par durée (amplitude moyenne par seconde)
            double magnitudePerSecond = magnitude / duration;

            // 3. Magnitude normalisée par tour
            double magnitudePerRevolution = magnitude / numberoftours;

  

            // 5. PSD (densité spectrale de puissance)
            // PSD = |X(f)|^2 / (fs * N) ou / (fs * durée)
            double psd = (magnitude * magnitude) / (totalSamples);
            return $"{sTitle}: {magnitudePerSecond:F4}\r\n{magnitudePerRevolution:F4} {psd:F4}";
        }

        /// <summary>
        /// Calcule la moyenne vectorielle complexe de plusieurs composantes FFT (magnitude + phase)
        /// </summary>
        public static Complex ComputeVectorAverage(List<(double magnitude, double phaseRadians)> components)
        {
            Complex sum = Complex.Zero;
            foreach (var (magnitude, phase) in components)
            {
                Complex z = Complex.FromPolarCoordinates(magnitude, phase);
                sum += z;
            }

            return sum / components.Count;
        }
        internal static Complex[] CalculateFFT2(double[] signal, ComboBox cbxFFT,bool bRemoveDC)
        {
            // Paramètres du zero-padding
            int count = signal.Count(); //should be 360

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
        public static double CalculerMasseCorrection(double acceleration, double wheelSize, double wheelWidth, double rpm)
        {
            //return acceleration * 5.0;
            // Conversion de la taille et de la largeur de la jante de pouces à mètres (1 pouce = 0.0254 m)
            double tailleJanteM = wheelSize  * 0.0254;
            double largeurJanteM = wheelWidth * 0.0254;

            // Calcul du rayon
            double rayon = tailleJanteM / 2;

            // Conversion de la vitesse en radians par seconde
            double vitesseAngular = rpm * 2 * Math.PI / 60;

            // Force centripète pour équilibrage (F = m * a)
            // Ici, on considère également la largeur de la jante pour ajuster la force
            double coefficientLargeur = 1.0 + (largeurJanteM / 1000.0); // Ajustement proportionnel selon la largeur
            double forceCentrifuge = coefficientLargeur * acceleration * rayon;

            // Calcul de la masse de correction (F = m * a, donc m = F / a)
            double masseCorrection = forceCentrifuge / 9.81;

            masseCorrection = ((acceleration * 20.0 / 9.81) / ((vitesseAngular * vitesseAngular) * rayon));
            return masseCorrection * 1000;
        }
        /// <summary>
        /// Calcule la constante d’atténuation exponentielle k à partir d’un étalonnage.
        /// </summary>
        public static double CalculateAttenuationConstant(double initialMagnitude, double finalMagnitude, double mass)
        {
            if (initialMagnitude <= 0 || finalMagnitude <= 0 || mass <= 0)
                return 0;
            
            double max = Math.Max(initialMagnitude, finalMagnitude);
            double min = Math.Min(initialMagnitude, finalMagnitude);
            return (1.0 / mass) * Math.Log(max / min);
        }

        /// <summary>
        /// Calcule la masse nécessaire pour réduire une magnitude actuelle à une cible, avec un k donné.
        /// </summary>
        public static double CalculateRequiredMass(double k, double currentMagnitude, double targetMagnitude)
        {
            if (currentMagnitude <= 0 || targetMagnitude <= 0)
                return 0;
            if (targetMagnitude >= currentMagnitude)
                return 0;

            return (1.0 / k) * Math.Log(currentMagnitude / targetMagnitude);
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
        //new methode
        /// <summary>
        /// Estime les masses et angles de correction pour les plans intérieur et extérieur.
        /// </summary>
        public static MassCorrectionResult EstimateMassCorrection(
            double magX, double phaseXDeg,
            double magY, double phaseYDeg,
            double kExtX, double kExtY,
            double kIntX, double kIntY)
        {
            // Convertir les phases en radians
            double phaseXRad = phaseXDeg * Math.PI / 180.0;
            double phaseYRad = phaseYDeg * Math.PI / 180.0;

            // Vecteur de vibration mesuré
            double Rx = magX * Math.Cos(phaseXRad) + magY * Math.Cos(phaseYRad);
            double Ry = magX * Math.Sin(phaseXRad) + magY * Math.Sin(phaseYRad);

            // Système à résoudre : R = Kext * Mext + Kint * Mint
            // --> [KextX  KintX]   [Mext]   = [Rx]
            //     [KextY  KintY]   [Mint]     [Ry]

            double det = kExtX * kIntY - kIntX * kExtY;
            if (Math.Abs(det) < 1e-6)
                throw new Exception("Système non inversible (les vecteurs K sont colinéaires).");

            double invDet = 1.0 / det;

            // Inversion 2x2
            double mExt = invDet * (kIntY * Rx - kIntX * Ry);
            double mInt = invDet * (-kExtY * Rx + kExtX * Ry);

            // Calcul des angles pour poser les masses (à l'opposé du vecteur produit)
            double angleExt = Math.Atan2(kExtY, kExtX) * 180.0 / Math.PI;
            double angleInt = Math.Atan2(kIntY, kIntX) * 180.0 / Math.PI;

            // Inverser de 180° pour corriger le balourd
            angleExt = (angleExt + 180.0 + 360.0) % 360.0;
            angleInt = (angleInt + 180.0 + 360.0) % 360.0;

            return new MassCorrectionResult
            {
                MassExt = mExt,
                AngleExtDeg = angleExt,
                MassInt = mInt,
                AngleIntDeg = angleInt
            };
        }

   
        /// <summary>
        /// Calcule les coefficients d'atténuation Kext et Kint pour les axes X, Y et la magnitude totale.
        /// </summary>
        public static AttenuationConstants CalculateAttenuationConstantsXY(
            double massExt, double xBefore, double yBefore, double xAfterExt, double yAfterExt,
            double massInt, double xAfterInt, double yAfterInt)
        {
            if (massExt <= 0 || massInt <= 0)
                throw new ArgumentException("Les masses doivent être positives.");

            // Magnitudes vectorielles
            double magBefore = Math.Sqrt(xBefore * xBefore + yBefore * yBefore);
            double magAfterExt = Math.Sqrt(xAfterExt * xAfterExt + yAfterExt * yAfterExt);
            double magAfterInt = Math.Sqrt(xAfterInt * xAfterInt + yAfterInt * yAfterInt);

            if (magBefore <= 0 || magAfterExt <= 0 || magAfterInt <= 0)
                throw new ArgumentException("Les magnitudes doivent être strictement positives.");
/*
            return new AttenuationConstants
            {
                KextX = (xAfterExt - xBefore) / massExt,
                KextY = (yAfterExt - yBefore) / massExt,
                KextTotal = (Math.Sqrt(Math.Pow(xAfterExt, 2) + Math.Pow(yAfterExt, 2)) -
                     Math.Sqrt(Math.Pow(xBefore, 2) + Math.Pow(yBefore, 2))) / massExt,

                KintX = (xAfterInt - xBefore) / massInt,
                KintY = (yAfterInt - yBefore) / massInt,
                KintTotal = (Math.Sqrt(Math.Pow(xAfterInt, 2) + Math.Pow(yAfterInt, 2)) -
                      Math.Sqrt(Math.Pow(xBefore, 2) + Math.Pow(yBefore, 2))) / massInt
            };

    */
            var result = new AttenuationConstants
            {
                // K_ext
                KextX = (1.0 / massExt) * Math.Log(xAfterExt / xBefore),
                KextY = (1.0 / massExt) * Math.Log(yAfterExt / yBefore),
                KextTotal = (1.0 / massExt) * Math.Log(magAfterExt / magBefore),

                // K_int
                KintX = (1.0 / massInt) * Math.Log(xAfterInt / xBefore),
                KintY = (1.0 / massInt) * Math.Log(yAfterInt / yBefore),
                KintTotal = (1.0 / massInt) * Math.Log(magAfterInt / magBefore)
            };

            return result;
        }
      
        public static DynamicCorrectionResult2 EstimateDynamicBalancing(
            double magX, double phaseXDeg,
            double magY, double phaseYDeg,
            double kExt, double kInt)
        {
            // 1. Convertir les phases en radians
            double phaseXRad = phaseXDeg * Math.PI / 180.0;
            double phaseYRad = phaseYDeg * Math.PI / 180.0;

            // 2. Calcul du vecteur total de balourd (somme des deux composantes)
            double fx = magX * Math.Cos(phaseXRad) + magY * Math.Cos(phaseYRad);
            double fy = magX * Math.Sin(phaseXRad) + magY * Math.Sin(phaseYRad);

            // 3. Angle du balourd (en degrés, dans [0-360[)
            double balourdAngle = Math.Atan2(fy, fx) * 180.0 / Math.PI;
            balourdAngle = (balourdAngle + 360.0) % 360.0;

            // 4. Placer les masses à ±90° du balourd
            double angleOuter = (balourdAngle - 45 + 360) % 360.0;
            double angleInner = (balourdAngle + 45 + 360) % 360.0;

            // 5. Résolution du système : F = K_ext * m_ext + K_int * m_int
            // Les vecteurs de placement :
            double kExtX = kExt * Math.Cos(angleOuter * Math.PI / 180.0);
            double kExtY = kExt * Math.Sin(angleOuter * Math.PI / 180.0);

            double kIntX = kInt * Math.Cos(angleInner * Math.PI / 180.0);
            double kIntY = kInt * Math.Sin(angleInner * Math.PI / 180.0);

            // Résultat mesuré (Fx, Fy)
            double Fx = fx;
            double Fy = fy;

            // Résolution du système linéaire 2x2
            double det = kExtX * kIntY - kIntX * kExtY;
            if (Math.Abs(det) < 1e-6)
                throw new Exception("Système non inversible : K_ext et K_int sont colinéaires.");

            double invDet = 1.0 / det;

            double mExt = invDet * (kIntY * Fx - kIntX * Fy);
            double mInt = invDet * (-kExtY * Fx + kExtX * Fy);

            return new DynamicCorrectionResult2
            {
                AngleInnerDeg = angleInner,
                AngleOuterDeg = angleOuter,
                MassInner = mInt,
                MassOuter = mExt
            };
        }
  

        public static MassCorrectionResult EstimateDynamicBalancing(
            double magX, double phaseXDeg,
            double magY, double phaseYDeg,
            double kExtX, double kExtY,
            double kIntX, double kIntY)
        {
            // Convertir phases en radians
            //double phaseXRad = (phaseXDeg > 180 ? phaseXDeg - 360 : phaseXDeg) * Math.PI / 180.0;
            //double phaseYRad = (phaseYDeg > 180 ? phaseYDeg - 360 : phaseYDeg) * Math.PI / 180.0;

            double phaseXRad = phaseXDeg * Math.PI / 180.0;
            double phaseYRad = phaseYDeg * Math.PI / 180.0;
            // Conversion phase depuis repère à 0° = haut, sens horaire
         

            // Vecteur vibration mesuré dans le plan XY
            double Rx = magX * Math.Cos(phaseXRad);
            double Ry = magX * Math.Sin(phaseXRad);
            double Wx = magY * Math.Cos(phaseYRad);
            double Wy = magY * Math.Sin(phaseYRad);

            // Différence vecteurs X et Y -> effort dynamique à corriger
            double dx = Rx - Wx;
            double dy = Ry - Wy;

            // Résolution du système :
            // [kExtX  kIntX] [Mext] = [dx]
            // [kExtY  kIntY] [Mint]   [dy]
            double det = kExtX * kIntY - kIntX * kExtY;
            if (Math.Abs(det) < 1e-12)
                throw new Exception("Système non inversible (vecteurs k colinéaires)");

            double invDet = 1.0 / det;

            // Résolution du système avec attention aux plans


            // Calcul masses extérieure et intérieure
            double massExt = invDet * (kIntY * dx - kIntX * dy);
            double massInt = invDet * (-kExtY * dx + kExtX * dy);
            massInt = invDet * (kExtY * Rx - kExtX * Ry);
            massExt = invDet * (-kIntY * Rx + kIntX * Ry);
            // Calcul vecteurs forces corrélées aux masses (utile pour angles)
            double forceExtX = massExt * kExtX;
            double forceExtY = massExt * kExtY;
            double forceIntX = massInt * kIntX;
            double forceIntY = massInt * kIntY;

            // Angle du vecteur dynamique
            double angleEffortRad = Math.Atan2(dy, dx);
            double angleEffortDeg = (angleEffortRad * 180.0 / Math.PI + 360.0) % 360.0;

            // Angles des masses à corriger ±90°
            double angleExtDeg = (angleEffortDeg - 45.0 + 360.0) % 360.0;
            double angleIntDeg = (angleEffortDeg + 45.0 + 360.0) % 360.0;

            return new MassCorrectionResult
            {
                MassExt = massExt,
                AngleExtDeg = angleExtDeg,
                MassInt = massInt,
                AngleIntDeg = angleIntDeg
            };
        }


    }

}
public class AttenuationConstants
{
    public double KextX;
    public double KextY;
    public double KextTotal;

    public double KintX;
    public double KintY;
    public double KintTotal;
}
public class MassCorrectionResult
    {
        public double MassExt;
        public double AngleExtDeg;
        public double MassInt;
        public double AngleIntDeg;
}
public class DynamicCorrectionResult2
{
    public double AngleInnerDeg { get; set; }
    public double AngleOuterDeg { get; set; }
    public double MassInner { get; set; }
    public double MassOuter { get; set; }
}

