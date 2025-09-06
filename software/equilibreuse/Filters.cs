using MathNet.Filtering.FIR;
using MathNet.Filtering.Windowing;
using NWaves.Filters;
using NWaves.Filters.Base;
using NWaves.Filters.BiQuad;
using NWaves.Filters.Fda;
using NWaves.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace equilibreuse
{
   
    public class LowPassFilter
    {
        private double alpha;
        private double previousOutput;
        public double Cutoff;
        public double SampleRate;
        public LowPassFilter(double cutoffFrequency, double sampleRate)
        {
            Cutoff = cutoffFrequency;
            SampleRate = sampleRate;
            double rc = 1.0 / (2 * Math.PI * cutoffFrequency);
            double dt = 1.0 / sampleRate;
            alpha = dt / (rc + dt);
            previousOutput = 0.0;
        }

        public double Process(double input)
        {
            double output = previousOutput + alpha * (input - previousOutput);
            previousOutput = output;
            return output;
        }
        public static double[] ApplyZeroPhase(double[] signal, LowPassFilter filter)
        {
            if (signal == null || signal.Length == 0) return signal;
            // 1) Application vers l'avant
            var forward = new double[signal.Length];
            for (int i = 0; i < signal.Length; i++)
                forward[i] = filter.Process(signal[i]);

            // 2) Réinitialiser le filtre
            filter = new LowPassFilter(filter.Cutoff, filter.SampleRate);

            // 3) Inversion + filtrage
            Array.Reverse(forward);
            var backward = new double[forward.Length];
            for (int i = 0; i < forward.Length; i++)
                backward[i] = filter.Process(forward[i]);

            // 4) Rétablir l'ordre
            Array.Reverse(backward);
            return backward;
        }
        public static void ApplyLowPassFilter(ref double[] signal, double cutoffFrequency, double sampleRate)
        {
            int halfOrder = 50;
            var coeffs = MathNet.Filtering.FIR.FirCoefficients.LowPass(sampleRate, cutoffFrequency, halfOrder);
           // var window = new MathNet.Filtering.Windowing.HammingWindow { Width = coeffs.Length }.CopyToArray();
          //  for (int i = 0; i < coeffs.Length; i++)
          //     coeffs[i] *= window[i];

            var fir = new MathNet.Filtering.FIR.OnlineFirFilter(coeffs);
            for (int i = 0; i < signal.Length; i++)
                signal[i] = fir.ProcessSample(signal[i]);
        }

        public static double[] ApplyLowPassFilterZeroPhase(double[] signal, double cutoffFrequency, double sampleRate, int filterOrder)
        {
            if (signal == null || signal.Length == 0)
                return null;
            int halfOrder = filterOrder;

            // Calcul des coefficients du filtre passe-bas
            var coeffs = FirCoefficients.LowPass(sampleRate, cutoffFrequency, halfOrder);

            // Application d'une fenêtre Hamming pour lisser les bords
          //  var window = new HammingWindow { Width = coeffs.Length }.CopyToArray();
          //  for (int i = 0; i < coeffs.Length; i++)
          //      coeffs[i] *= window[i];

            // Création du filtre FIR
            var fir = new OnlineFirFilter(coeffs);

            // Étape 1 : filtre avant
            double[] forwardFiltered = new double[signal.Length];
            for (int i = 0; i < signal.Length; i++)
                forwardFiltered[i] = fir.ProcessSample(signal[i]);

            // Étape 2 : inversion du signal
            Array.Reverse(forwardFiltered);

            // Réinitialisation du filtre pour deuxième passe
            fir.Reset();

            // Étape 3 : filtre arrière
            double[] backwardFiltered = new double[signal.Length];
            for (int i = 0; i < signal.Length; i++)
                backwardFiltered[i] = fir.ProcessSample(forwardFiltered[i]);

            // Étape 4 : inversion finale
            Array.Reverse(backwardFiltered);

            return backwardFiltered;
        }
        public static double[] ApplyNarrowBandPassFilter(double[] signal, double centerFreq, double sampleRate, int filterOrder, double bandwidth = 0.4)
        {
            if (signal == null || signal.Length == 0)
                return null;

            var coeffs = MathNet.Filtering.FIR.FirCoefficients.BandPass(sampleRate, centerFreq - bandwidth / 2, centerFreq + bandwidth / 2, filterOrder);
            var window = new MathNet.Filtering.Windowing.HammingWindow { Width = coeffs.Length }.CopyToArray();
            for (int i = 0; i < coeffs.Length; i++)
                coeffs[i] *= window[i];

            var filter = new MathNet.Filtering.FIR.OnlineFirFilter(coeffs);
           
            var forward = new double[signal.Length];
            var backward = new double[signal.Length];
            for (int i = 0; i < signal.Length; i++)
                forward[i] = filter.ProcessSample(signal[i]);
            // 2. Backward pass (reverse the signal and filter again)
            filter.Reset();
            Array.Reverse(forward);
            for (int i = 0; i < signal.Length; i++)
                backward[i] = filter.ProcessSample(forward[i]);

            // 3. Reverse back to original time order
            Array.Reverse(backward);
            return backward;
        }
        public static DiscreteSignal ApplySavitzkyGolayFilter(double[] signal, double sampleRate, double f_rot,int filterOrder)
        {
            var filter = new NWaves.Filters.SavitzkyGolayFilter(filterOrder);
            var filtered = filter.ApplyTo(new DiscreteSignal((int)sampleRate, signal.Select(s => (float)s).ToArray()));
            return filtered;

        }
        public static DiscreteSignal ApplyFilter(double[] signal, double sampleRate, double f_rot, int filterOrder, string filterName, double lowpass)
        {
            if (signal == null || signal.Length == 0)
                return new DiscreteSignal(1,0);

            var freq = f_rot / sampleRate;
            double fLow = (f_rot - 0.3) / sampleRate; // normalize frequency onto [0, 0.5] range
            double fHigh = (f_rot + 0.3) / sampleRate; // normalize frequency onto [0, 0.5] range
            
            LtiFilter filter = new NWaves.Filters.BiQuad.PeakFilter(f_rot, 100, 100);
            switch (filterName)
            {
                case "Custom IIR":
                    filter = AnalyzeCustomIirFilter();
                    break;
                case "Custom FIR":
                    // Tes paramètres
                    filterOrder = (int)sampleRate;
                    if (filterOrder % 2 == 0)
                        filterOrder += 1;
                    // Conception du filtre passe-bande FIR à phase linéaire
                    var coeffs = NWaves.Filters.Fda.DesignFilter.FirWinBp(filterOrder, fLow, fHigh);
                    // Création du filtre FIR
                    filter = new NWaves.Filters.Base.FirFilter(coeffs);
                    var filteredSignal2 = filter.ApplyTo(new DiscreteSignal((int)sampleRate, signal.Select(s => (float)s).ToArray()));
                    int delay = coeffs.Length / 2;

                    // Tronquer la sortie pour qu'elle soit de la même taille que l'entrée
                    int start = delay;
                    int end = start + signal.Length;

                    return new DiscreteSignal((int)sampleRate, filteredSignal2.Samples.Skip(start).Take(signal.Length).ToArray());
                case "BiQuad LP":
                case "BiQuad HP":
                case "BiQuad BP":
                case "BiQuad notch":
                case "BiQuad allpass":
                case "BiQuad peaking":
                case "BiQuad lowshelf":
                case "BiQuad highshelf":
                    filter = AnalyzeBiQuadFilter(filterName, fLow, fHigh, freq, lowpass/sampleRate, filterOrder);
                    break;
                case "One-pole LP":
                    filter = new NWaves.Filters.OnePole.LowPassFilter(lowpass);
                    break;
                case "One-pole HP":
                    filter = new NWaves.Filters.OnePole.HighPassFilter(lowpass);
                    break;
                case "Comb feed-forward":
                    filter = new CombFeedforwardFilter(500);
                    break;
                case "Comb feed-back":
                    filter = new CombFeedbackFilter(1800);
                    break;
                case "Moving average":
                    filter = AnalyzeMovingAverageFilter();
                    break;
                case "Moving average recursive":
                    filter = AnalyzeRecursiveMovingAverageFilter();
                    break;
                case "Savitzky-Golay":
                    filter = AnalyzeSavitzkyGolayFilter();
                    break;
                case "Pre-emphasis":
                    filter = AnalyzePreemphasisFilter();
                    break;
                case "De-emphasis":
                    filter = new DeEmphasisFilter();
                    break;
                case "DC removal":
                    filter = new DcRemovalFilter();
                    break;
                case "RASTA":
                    filter = new RastaFilter();
                    break;
                case "Butterworth":
                    filter = AnalyzeButterworthFilter(fLow, fHigh, filterOrder);
                    break;
                case "Elliptic":
                    filter = AnalyzeEllipticFilter(freq, filterOrder);
                    break;
                case "Chebyshev-I":
                    filter = AnalyzeChebyshevIFilter(fLow, fHigh, filterOrder);
                    
                    break;
                case "Chebyshev-II":
                    filter = AnalyzeChebyshevIIFilter(fLow, fHigh, filterOrder);
                    break;
                case "Bessel":
                    filter = AnalyzeBesselFilter(fLow, fHigh, filterOrder);
                    break;
                case "Thiran":
                    filter = AnalyzeThiranFilter();
                    break;
                case "Equiripple LP":
                    filter = AnalyzeEquirippleLpFilter();
                    break;
                case "Equiripple BS":
                    filter = AnalyzeEquirippleBsFilter();
                    break;
            }
                                                       /*var filter = new NWaves.Filters.Butterworth.BandPassFilter(fLow, fHigh, filterOrder);
                                                        var sos = DesignFilter.TfToSos(filter.Tf);
                                                        var filters = new FilterChain(sos);

                                                        // =========== filtering ==============

                                                        var gain = filters.EstimateGain();
                                                        var filteredSignal = filters.ApplyTo(new DiscreteSignal((int)sampleRate, signal.Select(s => (float)s).ToArray()), gain);
                                                        **/
            //var filter = new NWaves.Filters.Elliptic.BandPassFilter(fLow, fHigh, filterOrder);
            //var filter = new NWaves.Filters.BiQuad.PeakFilter(freq, 100, 100);
            var filteredSignal = filter.ApplyTo(new DiscreteSignal((int)sampleRate, signal.Select(s => (float)s).ToArray()));
            //reverse and reapply for zerophase
            filter.Reset();
            filteredSignal.Reverse();
            filteredSignal = filter.ApplyTo(filteredSignal);
            filteredSignal.Reverse();
            /*     var filter2 = new NWaves.Filters.MovingAverageFilter();
                 //SavitzkyGolayFilter(31,0);
                 filteredSignal = filter2.ApplyTo(new DiscreteSignal((int)sampleRate, filteredSignal.Samples), FilteringMethod.Auto);
                 */


            return filteredSignal;

        }
        public static double[] RemoveDCOffset(double[] signal)
        {
            if (signal == null || signal.Length == 0) return signal;

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
            return signal;
        }


        #region filter analysis

        private static LtiFilter AnalyzeCustomIirFilter()
        {
            var b = new List<double>();
            var a = new List<double>();

          b.AddRange(new[] { 1, -0.4, 0.6 });
          a.AddRange(new[] { 1, 0.4, 0.2 });
          
            // lose some precision:

            return new IirFilter(b, a);
        }

        

        private static LtiFilter AnalyzeBiQuadFilter(string filterType, double f_low, double f_high, double f_rot, double f_lowpassfilter, double q)
        {    
        
            var gain = 100.0;
            
         
            LtiFilter _filter = new NWaves.Filters.BiQuad.LowPassFilter(f_lowpassfilter, q);

            switch (filterType)
            {
                case "BiQuad LP":
                    _filter = new NWaves.Filters.BiQuad.LowPassFilter(f_lowpassfilter, q);
                    break;
                case "BiQuad HP":
                    _filter = new HighPassFilter(f_lowpassfilter, q);
                    break;
                case "BiQuad BP":
                    _filter = new NWaves.Filters.BiQuad.BandPassFilter(f_rot, q);
                    break;
                case "BiQuad notch":
                    _filter = new NotchFilter(f_rot, q);
                    break;
                case "BiQuad allpass":
                    _filter = new AllPassFilter(f_rot, q);
                    break;
                case "BiQuad peaking":
                    _filter = new PeakFilter(f_rot, q, gain);
                    break;
                case "BiQuad lowshelf":
                    _filter = new LowShelfFilter(f_lowpassfilter, q, gain);
                
                    break;
                case "BiQuad highshelf":
                    _filter = new HighShelfFilter(f_lowpassfilter, q, gain);
                
                    break;
            }

            return _filter;
        }

        private static LtiFilter AnalyzeMovingAverageFilter()
        {
            var size = 5;
          
            return new MovingAverageFilter(size);

            
        }

        private static LtiFilter AnalyzeRecursiveMovingAverageFilter()
        {
            var size = 5;
            

            return new MovingAverageRecursiveFilter(size);
            
        }

        private static LtiFilter AnalyzeSavitzkyGolayFilter()
        {
            var size = 5;
            return new SavitzkyGolayFilter(size);
            
        }

        private static LtiFilter AnalyzePreemphasisFilter()
        {
            var pre = 0.95;
           return new PreEmphasisFilter(pre);

    
        }

        private static LtiFilter AnalyzeButterworthFilter(double f_low,double f_high, int orderfilter)
        {
    
            var bw = new NWaves.Filters.Butterworth.BandPassFilter(f_low, f_high, orderfilter);
            //var df = DesignFilter.TfToSos(bw.Tf);
            return new IirFilter(bw.Tf);

            // =========== filtering ==============           
        }

        private static LtiFilter AnalyzeEllipticFilter(double f_rot, int orderfilter)
        {
            
            // example how to convert linear scale specifications to decibel scale:

            var deltaPass = 0.96;
            var deltaStop = 0.04;

            var ripplePassDb = NWaves.Utils.Scale.ToDecibel(1 / deltaPass);
            var attenuateDb = NWaves.Utils.Scale.ToDecibel(1 / deltaStop);

            return new NWaves.Filters.Elliptic.LowPassFilter(f_rot, orderfilter, ripplePassDb, attenuateDb);

        }

        private static LtiFilter AnalyzeChebyshevIFilter(double f_low, double f_high, int order)
        {

            var filter = new NWaves.Filters.ChebyshevI.BandPassFilter(f_low, f_high, order);
            return new IirFilter(filter.Tf);
        }

        private static LtiFilter AnalyzeChebyshevIIFilter(double f_low, double f_high, int order)
        {
            
            var filter = new NWaves.Filters.ChebyshevII.BandPassFilter(f_low, f_high, order);
            return new IirFilter(filter.Tf);
        }

        private static LtiFilter AnalyzeBesselFilter(double f_low, double f_high, int order)
        {
            
            return new NWaves.Filters.Bessel.BandPassFilter(f_low, f_high, order);
            
        }

        private static LtiFilter AnalyzeThiranFilter()
        {
            var order = 10;
            var delta = 10.3;

            return new ThiranFilter(order, order + delta);

        }

        private static LtiFilter AnalyzeEquirippleLpFilter()
        {
            var order = 47;
            var fp = 0.15;
            var fa = 0.18;
            var ripplePass = 1.0;   // dB
            var rippleStop = 42.0;  // dB

         
            var wp = Remez.DbToPassbandWeight(ripplePass);
            var wa = Remez.DbToStopbandWeight(rippleStop);

            return new FirFilter(DesignFilter.FirEquirippleLp(order, fp, fa, wp, wa));

        }

        private static LtiFilter AnalyzeEquirippleBsFilter()
        {
            var order = 51;
            var fp1 = 0.19;
            var fa1 = 0.21;
            var fa2 = 0.39;
            var fp2 = 0.41;
            var ripplePass1 = 1.0;
            var rippleStop = 24.0;
            var ripplePass2 = 3.0;

           
            var freqs = new[] { 0, fp1, fa1, fa2, fp2, 0.5 };

            var weights = new[]
            {
                Remez.DbToPassbandWeight(ripplePass1),
                Remez.DbToStopbandWeight(rippleStop),
                Remez.DbToPassbandWeight(ripplePass2),
            };

            var remez = new Remez(order, freqs, new double[] { 1, 0, 1 }, weights);

            return new FirFilter(remez.Design());
            
        }

        
        #endregion
       
        public static double[] ComputePhaseIQ(
            double[] signal,
            double sampleRate,
            double targetFreq,
            double smoothingSec = 0.1) // largeur moyenne glissante en secondes
        {
            if (signal.Length == 0) return signal;
            int len = signal.Length;
            double[] cosRef = new double[len];
            double[] sinRef = new double[len];
            double dt = 1.0 / sampleRate;
            //smoothingSec = 1.0 / targetFreq/2;
            // Génération des références sinusoïdales
            for (int i = 0; i < len; i++)
            {
                double t = i * dt;
                cosRef[i] = Math.Cos(2 * Math.PI * targetFreq * t);
                sinRef[i] = Math.Sin(2 * Math.PI * targetFreq * t);
            }

            // Multiplication (I et Q)
            double[] I = new double[len];
            double[] Q = new double[len];
            for (int i = 0; i < len; i++)
            {
                I[i] = signal[i] * cosRef[i];
                Q[i] = signal[i] * sinRef[i];
            }

            // Moyenne glissante (filtrage passe-bas)
            int win = (int)(smoothingSec * sampleRate);
            double[] I_filt = MovingAverage(I, win);
            double[] Q_filt = MovingAverage(Q, win);

            // Calcul de la phase instantanée
            double[] phase = new double[len];
            for (int i = 0; i < len; i++)
            {
                phase[i] = Math.Atan2(Q_filt[i], I_filt[i]);
            }

            // Unwrap phase
            return UnwrapPhase(phase).Select(p => ((p * 180.0 / Math.PI) % 360 + 360) % 360) // Convertit en deg, puis modulo 360 en positif
    .ToArray(); 
        }

        private static double[] MovingAverage(double[] data, int window)
        {
            double[] result = new double[data.Length];
            double sum = 0.0;

            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i];
                if (i >= window)
                    sum -= data[i - window];

                int currentWindowSize = Math.Min(i + 1, window);
                result[i] = sum / currentWindowSize;
            }
            return result;
        }

        private static double[] UnwrapPhase(double[] phase)
        {
            double[] unwrapped = new double[phase.Length];
            unwrapped[0] = phase[0];
            double offset = 0;

            for (int i = 1; i < phase.Length; i++)
            {
                double delta = phase[i] - phase[i - 1];
                if (delta > Math.PI)
                    offset -= 2 * Math.PI;
                else if (delta < -Math.PI)
                    offset += 2 * Math.PI;

                unwrapped[i] = phase[i] + offset;
            }

            return unwrapped;
        }

    }
}
