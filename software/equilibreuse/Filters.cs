using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace equilibreuse
{
    public class ButterworthHighPass
    {
        private readonly double[] a = new double[3];
        private readonly double[] b = new double[3];
        private double[] z = new double[2]; // mémoire pour filtre IIR (2ème ordre)
        public double[] ZeroPhaseFilter(double[] input)
        {
            // 1. Filtrage avant
            double[] forward = Filter(input);

            // 2. Inverser le signal
            Array.Reverse(forward);

            // 3. Filtrage arrière
            double[] backward = Filter(forward);

            // 4. Ré-inverser pour revenir à l'ordre d'origine
            Array.Reverse(backward);

            return backward;
        }
        public ButterworthHighPass(double cutoffHz, double samplingRateHz)
        {
            // Coefficients du filtre passe-haut Butterworth 2ème ordre
            double wc = Math.Tan(Math.PI * cutoffHz / samplingRateHz);
            double k1 = Math.Sqrt(2) * wc;
            double k2 = wc * wc;
            double norm = 1.0 / (1 + k1 + k2);

            b[0] = 1 * norm;
            b[1] = -2 * norm;
            b[2] = 1 * norm;

            a[0] = 1; // a[0] est toujours 1 dans les IIR standards
            a[1] = 2 * (k2 - 1) * norm;
            a[2] = (1 - k1 + k2) * norm;
        }

        public double[] Filter(double[] input)
        {
            double[] output = new double[input.Length];

            double x1 = 0, x2 = 0;
            double y1 = 0, y2 = 0;

            for (int i = 0; i < input.Length; i++)
            {
                double x0 = input[i];
                double y0 = b[0] * x0 + b[1] * x1 + b[2] * x2
                                      - a[1] * y1 - a[2] * y2;

                output[i] = y0;

                // Décaler les valeurs pour la prochaine itération
                x2 = x1;
                x1 = x0;
                y2 = y1;
                y1 = y0;
            }

            return output;
        }
    }
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
            var window = new MathNet.Filtering.Windowing.HammingWindow { Width = coeffs.Length }.CopyToArray();
            for (int i = 0; i < coeffs.Length; i++)
                coeffs[i] *= window[i];

            var fir = new MathNet.Filtering.FIR.OnlineFirFilter(coeffs);
            for (int i = 0; i < signal.Length; i++)
                signal[i] = fir.ProcessSample(signal[i]);
        }
    }
}
