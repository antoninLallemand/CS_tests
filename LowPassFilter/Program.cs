using System;
using System.Globalization;
using System.Text;
using System.IO;


namespace test{
    class Programm{
        public static void Main(string[] args){
            
            ushort sampleRate = 1000;
            ushort signalDuration = 1;
            int signalLength = sampleRate*signalDuration;

            double[] signal = new double[signalLength];
            double[] filteredSignal = new double[signalLength];
            double[] t = new double[signalLength];

            for (int i = 0; i < signalLength; i++){
                t[i] = i / (double)sampleRate;
                signal[i] = Math.Sin(2 * Math.PI * 20 * t[i]) + 0.5 * Math.Sin(2 * Math.PI * 200 * t[i]);
            }

            ushort finalSampleRate = 200;
            filteredSignal = ApplyFIRFilter(signal, GenerateLowPassCoefficients(finalSampleRate/2, sampleRate, 51));
            byte decimationFactor = (byte)(sampleRate/finalSampleRate);
            double[] decimatedSignal = new double[(int)(signalLength/decimationFactor)];
            int decimatedIndex = 0;
            for(int i=0; i<signalLength; i+=decimationFactor){
                decimatedSignal[decimatedIndex] = filteredSignal[i];
                decimatedIndex++;
            }

            // SAVE IN CSV

            StringBuilder csv = new StringBuilder();
            for (int i = 0; i < signalLength; i++)
            {
                csv.Append(t[i].ToString(CultureInfo.InvariantCulture));
                csv.Append(",");
                csv.Append(signal[i].ToString(CultureInfo.InvariantCulture));
                csv.Append(",");
                csv.Append(filteredSignal[i].ToString(CultureInfo.InvariantCulture));
                csv.AppendLine();
            }
            string filePath = "filter.csv";
            File.WriteAllText(filePath, csv.ToString());

            StringBuilder csv2 = new StringBuilder();
            decimatedIndex = 0;
            for (int i = 0; i < signalLength; i+=decimationFactor)
            {
                csv2.Append(t[i].ToString(CultureInfo.InvariantCulture));
                csv2.Append(",");
                csv2.Append(decimatedSignal[decimatedIndex].ToString(CultureInfo.InvariantCulture));
                csv2.AppendLine();
                decimatedIndex++;
            }
            string secondFilePath = "downsampled.csv";
            File.WriteAllText(secondFilePath, csv2.ToString());

        }
        public static double[] GenerateLowPassCoefficients(double cutoffFrequency, int samplingRate, int length)
        {
            double[] coefficients = new double[length];
            double normalizedCutoff = 2 * cutoffFrequency / samplingRate;
            double sum = 0.0;

            for (int i = 0; i < length; i++)
            {
                int m = i - (length - 1) / 2;
                if (m == 0)
                {
                    coefficients[i] = normalizedCutoff;
                }
                else
                {
                    coefficients[i] = Math.Sin(Math.PI * m * normalizedCutoff) / (Math.PI * m);
                    coefficients[i] *= 0.54 - 0.46 * Math.Cos(2 * Math.PI * i / (length - 1)); // Apply Hamming window
                }
                sum += coefficients[i];
            }

            // Normalize coefficients
            for (int i = 0; i < length; i++)
            {
                coefficients[i] /= sum;
            }

            return coefficients;
        }

        static double[] ApplyFIRFilter(double[] input, double[] coefficients)
        {
            int filterOrder = coefficients.Length;
            int signalLength = input.Length;
            double[] output = new double[signalLength];

            for (int n = 0; n < signalLength; n++)
            {
                output[n] = 0.0;
                for (int k = 0; k < filterOrder; k++)
                {
                    if (n - k >= 0)
                    {
                        output[n] += coefficients[k] * input[n - k];
                    }
                }
            }
            return output;
        }

    }
}