using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.Interpolation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ScottPlot;
using NWaves.Signals;
using NWaves.Features;
using NWaves.FeatureExtractors.Options;
using NWaves.FeatureExtractors.Multi;

namespace equilibreuse
{

    public partial class Form1 : Form
    {
        const double G = 9.80665;
        SerialPort sp;
        volatile bool keepReading = true;
        int minRPM, maxRPM;
        volatile bool saveData = false;
        public List<section> dataToSave = new List<section>();
        Thread readThread;
        volatile float rpm = 0;
        public xyz avg = new xyz();
        public double fDataPerMS = 0;
        List<section> loadedSections = new List<section>(200);
        List<section> selectedSections = new List<section>();
        private bool bUseMPU9250Format = false;
        private String sLastCSV;
        private int iTotalRecordsInCSV;
        public System.Windows.Forms.Timer t1 = new System.Windows.Forms.Timer();

        public Form1()
        {
            InitializeComponent();
            t1.Tick += T1_Tick;
            t1.Interval = 1000;
            cbxFFT.SelectedItem = "Hann";
            cbxSensor.SelectedIndex = 1; //lsm6ds3 per default
            Help.FillHelp(richTextBox1);
        }

        private void T1_Tick(object sender, EventArgs e)
        {
            txtRPM.Text = rpm.ToString("F2");
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (sp != null && sp.IsOpen)
                    sp.Close();
                sp = new SerialPort(txtCom.Text, 2000000);
                sp.ReadBufferSize *= 4;
                sp.Open();
                sp.DtrEnable = true;
                Thread.Sleep(2000); //givr 1 sec to arduino to boot
                txtStatus.Text = "COM Port open\r\nDon't move the accelerometer, calibration in progress";
                Application.DoEvents();
                Stopwatch sw = new Stopwatch();

                int count = 0;

                byte[] buffer = new byte[sp.ReadBufferSize * 4];
                sp.ReadTimeout = 1;
                xyz avgTemp = new xyz();
                double lastX = 0, lastY = 0, lastZ = 0; //remove duplicates
                byte[] bufDataRemaining = new byte[sp.ReadBufferSize * 4];
                int dataRemaining = 0;
                if (cbxSensor.SelectedIndex == 0) //mpu format
                    bUseMPU9250Format = true;
                else //lsm6ds3 format
                    bUseMPU9250Format = false;
                sw.Start();
                do
                {
                    int len = sp.Read(buffer, 0, sp.BytesToRead);
                    Array.Copy(buffer, 0, bufDataRemaining, dataRemaining, len);
                    len += dataRemaining;
                    if (len > 0)
                    {
                        int pos = 0;
                        do
                        {
                            byte whiteline = bufDataRemaining[pos++];
                            if (pos + 14 > len)
                            {
                                pos--; // data are remaining
                                dataRemaining = len - pos;
                                Array.Copy(bufDataRemaining, pos, bufDataRemaining, 0, dataRemaining);
                                break;
                            }
                            if ((whiteline == 0xFF || whiteline == 0xFE) && bufDataRemaining[pos + 14] == 0x0A)
                            {
                                avgTemp = calcXYZ(ref bufDataRemaining, pos);
                                pos += 13;
                                if (avgTemp.x == lastX && avgTemp.y == lastY && avgTemp.z == lastZ) //duplicate, skip
                                {
                                   //Console.WriteLine("duplicate");
                                }
                                else
                                {
                                    lastX = avgTemp.x;
                                    lastY = avgTemp.y;
                                    lastZ = avgTemp.z;
                                    avg.x += avgTemp.x;
                                    avg.y += avgTemp.y;
                                    avg.z += avgTemp.z;
                                    count++;
                                }

                            }
                        } while (len > pos);
                    }
                } while (sw.ElapsedMilliseconds < 5000);
                sw.Stop();
                fDataPerMS = (double)((double)count / (double)sw.ElapsedMilliseconds);
                avg.x = avg.x / count;
                avg.y = avg.y / count;
                avg.z = avg.z / count;
                avg.gx = avg.gx / count;
                avg.gy = avg.gy / count;
                avg.gz = avg.gz / count;
                txtStatus.Text = "number of lines read during 5s: " + count +
                        "\r\n AVG X: " + avg.x.ToString("F4") + " AVG Y: " + avg.y.ToString("F4") + " AVG Z: " + avg.z.ToString("F4");
                txtSampleRate.Text = (count / 5).ToString(); ;
                keepReading = true;
                readThread = new Thread(ReadData);
                readThread.Start();
                t1.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot open com port: " + ex.ToString());
            }
        }

        private void ReadData()
        {
            sp.ReadTimeout = 1;
            byte[] buffer = new byte[sp.ReadBufferSize * 4];
            xyz avgTemp = new xyz();
            byte[] bufDataRemaining = new byte[sp.ReadBufferSize * 4];
            int dataRemaining = 0;
            bool isWhite = false;
            section s = null;
            int count = 0;
            double sampleRate = Convert.ToDouble(txtSampleRate.Text);
            double lastX = 0, lastY = 0, lastZ = 0; //remove duplicates
            sp.ReadExisting(); //clear input buffer
            try
            {
                while (keepReading)
                {
                    int len = sp.Read(buffer, 0, sp.BytesToRead);
                    Array.Copy(buffer, 0, bufDataRemaining, dataRemaining, len);
                    len += dataRemaining;
                    if (len > 0)
                    {
                        int pos = 0;
                        do
                        {
                            byte whiteline = bufDataRemaining[pos++];
                            if (pos + 14 > len)
                            {
                                pos--; // data are remaining
                                dataRemaining = len - pos;
                                Array.Copy(bufDataRemaining, pos, bufDataRemaining, 0, dataRemaining);
                                break;
                            }
                            if ((whiteline == 0xFF || whiteline == 0xFE) && bufDataRemaining[pos + 14] == 0x0A)
                            {
                                count++;
                                bool bNewSection = false;
                                if (whiteline == 0xFF)
                                {
                                    if (!isWhite)
                                    {
                                        //count RPM
                                        rpm = (float)(60.0 * sampleRate / count);
                                        bNewSection = true;
                                        count = 0;
                                    }
                                    isWhite = true;
                                }
                                else
                                    isWhite = false;
                                avgTemp = calcXYZ(ref bufDataRemaining, pos);
                                pos += 15;
                                if (!isWhite && (avgTemp.x == lastX && avgTemp.y == lastY && avgTemp.z == lastZ)) //duplicate, skip only if not white line
                                {
                                    count--;
                                }
                                else
                                {

                                    lastX = avgTemp.x;
                                    lastY = avgTemp.y;
                                    lastZ = avgTemp.z;

                                    avgTemp.x -= avg.x;
                                    avgTemp.y -= avg.y;
                                    avgTemp.z -= avg.z;
                                    avgTemp.isWhite = isWhite;
                                    if (saveData && (rpm > minRPM && rpm < maxRPM))
                                    {
                                        if (bNewSection)
                                        {
                                            s = new section();
                                            s.records = new List<xyz>(360);
                                            dataToSave.Add(s);
                                        }
                                        if (s != null)
                                            s.records.Add(avgTemp);
                                    }
                                }
                            }
                        } while (len > pos);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in reading thread: " + ex.ToString());
            }
        }
        private xyz calcXYZ(ref byte[] buff, int pos)
        {
            UInt16 timestamp = (UInt16)((buff[pos+1] << 8) | (buff[pos]));
            if (!bUseMPU9250Format)
            {
                short x = (short)((buff[pos + 3] << 8) | (buff[pos + 2]));
                short y = (short)((buff[pos + 5] << 8) | (buff[pos + 4]));
                short z = (short)((buff[pos + 7] << 8) | (buff[pos + 6]));
                short gx = (short)((buff[pos + 9] << 8) | (buff[pos + 8]));
                short gy = (short)((buff[pos + 11] << 8) | (buff[pos + 10]));
                short gz = (short)((buff[pos + 13] << 8) | (buff[pos + 12]));
                xyz data = new xyz();

                //lsm6ds3 : *0.061 * (settings.accelRange >> 1) / 1000;
                data.ms = timestamp; //timestamp is encoded in 0.064ms
                data.x = (double)((x) * 0.061 * G / 1000.0);
                data.y = (double)((y) * 0.061 * G / 1000.0);
                data.z = (double)((z) * 0.061 * G / 1000.0);

                data.gx = (double)((gx) * 4.375 / 1000.0);
                data.gy = (double)((gy) * 4.375 / 1000.0);
                data.gz = (double)((gz) * 4.375 / 1000.0);

             
                data.isWhite = false;
                return data;
            }
            else
            {
                short x = (short)((buff[pos + 2] << 8) | (buff[pos + 3]));
                short y = (short)((buff[pos + 4] << 8) | (buff[pos + 5]));
                short z = (short)((buff[pos + 6] << 8) | (buff[pos + 7]));
                short gx = (short)((buff[pos + 8] << 8) | (buff[pos + 9]));
                short gy = (short)((buff[pos + 10] << 8) | (buff[pos + 11]));
                short gz = (short)((buff[pos + 12] << 8) | (buff[pos + 13]));
                xyz data = new xyz();
                data.ms = timestamp; //timestamp is encoded in 0.064ms
                // Sensitivity Scale Factor (MPU datasheet page 9)
                //16384 for 2G
                //8192 for 4G
                data.x = (double)((x) * G / 16384.0);
                 data.y = (double)((y) * G / 16384.0);
                 data.z = (double)((z) * G / 16384.0);
                 data.gx = (double)((gx) / 131.0);
                 data.gy = (double)((gy) / 131.0);
                 data.gz = (double)((gz) / 131.0);
                data.isWhite = false;
                return data;

                /*data.x = (double)((x) / 1023.0 * 3333.0 / 300.0); //adxl
                data.y = (double)((y) / 1023.0 * 3333.0 / 300.0 * G);
                data.z = (double)((z) / 1023.0 / 0.300 * G);*/

            }

        }
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            t1.Stop();
            keepReading = false;
            if (readThread != null && readThread.ThreadState == System.Threading.ThreadState.Running)
                readThread.Join();
            if (sp != null && sp.IsOpen)
                sp.Close();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnDisconnect_Click(null, EventArgs.Empty);
        }

        private void btnEndCapture_Click(object sender, EventArgs e)
        {
            saveData = false;
            btnDisconnect_Click(null, EventArgs.Empty);
            String executablePath = Assembly.GetExecutingAssembly().Location;
            String csvFile = Path.Combine(Path.GetDirectoryName(executablePath), DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".csv");
            using (var writer = new StreamWriter(csvFile))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    writer.WriteLine(txtSampleRate.Text);
                    csv.WriteHeader<xyz>();
                    csv.NextRecord();
                    foreach (var s in dataToSave)
                    {
                        foreach (xyz r in s.records)
                        {
                            csv.WriteRecord<xyz>(r);
                            csv.NextRecord();
                        }
                    }
                }
            }
            AnalyzeCSV(csvFile);
        }


        // analysis of all segments in one graph aligned on the max size of segment
        private void RefreshAnalysisCompiled()
        {
            if (String.IsNullOrEmpty(sLastCSV)) return;

            int alignedCount = selectedSections.Select(x => x.records.Count).Max();
            //get avg samplerate
            double sampleRate = selectedSections.Select(x => x.SamplingRate).Average();

            double[] analyzedX = new double[alignedCount];
            double[] analyzedY = new double[alignedCount];
            double[] analyzedZ = new double[alignedCount];
            double[] resultante = new double[alignedCount];
            int[] countAngle = new int[alignedCount];
            //apply options on signal
            //find all selected sections

            foreach (section se in selectedSections)
            {
                int count = se.records.Count;
                double anglePerRecord = ((double)((double)alignedCount / count));
                for (int i = 0; i < count; i++)
                {
                    int angle = ((int)(i * anglePerRecord));
                    if (chkAbsolute.Checked)
                    {
                        analyzedX[angle] += Math.Abs(se.records[i].x);
                        analyzedY[angle] += Math.Abs(se.records[i].y);
                        analyzedZ[angle] += Math.Abs(se.records[i].z);
                    }
                    else
                    {
                        analyzedX[angle] += se.records[i].x;
                        analyzedY[angle] += se.records[i].y;
                        analyzedZ[angle] += se.records[i].z;
                    }
                    countAngle[angle] += 1;
                }
            }

            if (!chkSum.Checked)
            {
                for (int i = 0; i < alignedCount; i++)
                {
                    if (countAngle[i] > 0)
                    {
                        analyzedX[i] = analyzedX[i] / countAngle[i];
                        analyzedY[i] = analyzedY[i] / countAngle[i];
                        analyzedZ[i] = analyzedZ[i] / countAngle[i];
                    }
                }
            }
            ApplyLowPassFilter(ref analyzedX, sampleRate);
            ApplyLowPassFilter(ref analyzedY, sampleRate);
            ApplyLowPassFilter(ref analyzedZ, sampleRate);
            int countX = analyzedX.Count();
            for (int i = 0; i < countX; i++)
            {
                resultante[i] = Math.Sqrt(Math.Pow(analyzedX[i], 2)
                                            + Math.Pow(analyzedY[i], 2)
                                            );
            }
            lblPeak.Text = $"X Pk-Pk (average) : {analyzedX.Max() - analyzedX.Min()}\r\nY Pk-Pk (average) : {analyzedY.Max() - analyzedY.Min()}\r\nZ Pk-Pk (average) : {analyzedZ.Max() - analyzedZ.Min()}";

            formsPlotAnalysis.Plot.Clear();
            if (!chkFFT.Checked)
            {
                //display the graph on 360° basis
                double[] angle = new double[alignedCount];
                double anglePerCount = 360.0 / alignedCount;
                for (int i = 0; i < alignedCount; i++)
                    angle[i] = i*anglePerCount;

                int[][] peakX = new int[1][]; peakX[0] = GetPeakPerTurn(analyzedX);
                int[][] peakY = new int[1][]; peakY[0] = GetPeakPerTurn(analyzedY); 
                int[][] peakZ = new int[1][]; peakZ[0] = GetPeakPerTurn(analyzedZ); 
                int[][] peakResultante = new int[1][]; peakResultante[0] = GetPeakPerTurn(resultante); 

                lstPeakXCompiled.Items.Clear();
                lstPeakYCompiled.Items.Clear();
                lstPeakZCompiled.Items.Clear();
                lstPeakResultanteCompiled.Items.Clear();

                var top5 = GetTopCommonPeaksWithAmplitude(peakX, analyzedX, tol: 10, minSamples: 2, topN: 5);
                foreach (var item in top5)
                    lstPeakXCompiled.Items.Add($"PeakX: Angle {item.Mean} – Fréquence {item.Freq} - Force {item.AverageAmplitude}");
                top5 = GetTopCommonPeaksWithAmplitude(peakY, analyzedY, tol: 10, minSamples: 2, topN: 5);
                foreach (var item in top5)
                    lstPeakYCompiled.Items.Add($"PeakY: Angle {item.Mean} – Fréquence {item.Freq} - Force {item.AverageAmplitude}");
                top5 = GetTopCommonPeaksWithAmplitude(peakZ, analyzedZ, tol: 10, minSamples: 2, topN: 5);
                foreach (var item in top5)
                    lstPeakZCompiled.Items.Add($"PeakZ: Angle {item.Mean} – Fréquence {item.Freq} - Force {item.AverageAmplitude}");
                top5 = GetTopCommonPeaksWithAmplitude(peakResultante, resultante, tol: 10, minSamples: 2, topN: 5);
                foreach (var item in top5)
                    lstPeakResultanteCompiled.Items.Add($"Resultante: Angle {item.Mean} – Fréquence {item.Freq} - Force {item.AverageAmplitude}");

                if (chkShowX.Checked)
                {
                    formsPlotAnalysis.Plot.PlotScatter(angle, analyzedX, Color.Blue, 1, 1, "X");
                    DisplayPeaksTemporal(analyzedX, angle, "Top Peak X", formsPlotAnalysis, lstPeakXCompiled);
                }
                if (chkShowY.Checked)
                {
                    formsPlotAnalysis.Plot.PlotScatter(angle, analyzedY, Color.Red, 1, 1, "Y");
                    DisplayPeaksTemporal(analyzedY, angle, "Top Peak Y", formsPlotAnalysis, lstPeakYCompiled);
                }
                if (chkShowZ.Checked)
                {
                    formsPlotAnalysis.Plot.PlotScatter(angle, analyzedZ, Color.Yellow, 1, 1, "Z");
                    DisplayPeaksTemporal(analyzedX, angle, "Top Peak Z", formsPlotAnalysis, lstPeakZCompiled);
                }
                if (chkShowResultante.Checked)
                {
                    formsPlotAnalysis.Plot.PlotScatter(angle, resultante, Color.Black, 1, 1, "Resultante");
                    DisplayPeaksTemporal(analyzedX, angle, "Top Peak Resultante", formsPlotAnalysis, lstPeakResultanteCompiled);
                }
                //formsPlotAnalysis.Plot.Axis(0, 360, -1, 1);

                formsPlotAnalysis.Plot.SetAxisLimitsX(0.0, 360.0);
                formsPlotAnalysis.Plot.AxisAutoY();
                formsPlotAnalysis.Plot.Legend(true);
            }
            else
            {
                FFTData dataX = EquilibrageHelper.CalculateFFT(analyzedX, sampleRate, cbxFFT,chkRemoveDC.Checked);
                FFTData dataY = EquilibrageHelper.CalculateFFT(analyzedY, sampleRate, cbxFFT, chkRemoveDC.Checked);
                FFTData dataZ = EquilibrageHelper.CalculateFFT(analyzedZ, sampleRate, cbxFFT, chkRemoveDC.Checked);
                FFTData dataResultante = EquilibrageHelper.CalculateFFT(resultante, sampleRate, cbxFFT, chkRemoveDC.Checked);
                
                
                double avgTourTime = alignedCount / sampleRate;
                double f_rot = 1.0 / avgTourTime;
                formsPlotAnalysis.Plot.Clear();
                lstPeakZCompiled.Items.Clear();
                lstPeakXCompiled.Items.Clear();
                lstPeakYCompiled.Items.Clear();
                lstPeakResultanteCompiled.Items.Clear();
                if (chkShowX.Checked)
                {
                    AnalyzeAxis("X", dataX, sampleRate, lstPeakXCompiled, Color.Blue, formsPlotAnalysis, f_rot);
                }
                if (chkShowY.Checked)
                {
                    AnalyzeAxis("Y", dataY, sampleRate, lstPeakYCompiled, Color.Red, formsPlotAnalysis, f_rot);
                }
                if (chkShowZ.Checked)
                {
                    AnalyzeAxis("Z", dataZ, sampleRate, lstPeakZCompiled, Color.Yellow, formsPlotAnalysis, f_rot);
                }
                if (chkShowResultante.Checked)
                {
                    AnalyzeAxis("Resultante", dataResultante, sampleRate, lstPeakResultanteCompiled, Color.DeepPink, formsPlotAnalysis, f_rot);
                }
                String sText = $"Fundamental: {f_rot}Hz\r\n1er order: {f_rot * 2}\r\n2eme order: {f_rot * 3}\r\n3eme order: {f_rot * 4}\r\n4er order: {f_rot * 5}\r\n5er order: {f_rot * 6}\r\n";
                formsPlotAnalysis.Plot.AddAnnotation(sText);
                lstSimulationCompiled.Items.Clear();
                double gain = 1.0;
                var calcResult = EquilibrageHelper.CompleteSimulation(lstSimulationCompiled, "Compiled", dataX, dataY, dataZ, dataResultante, sampleRate, f_rot);
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT1I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Red, width: 3);
                            formsPlotT1O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Red, width: 3);
                        }
                        formsPlotT1X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Red, width: 3);
                        formsPlotT1Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Red, width: 3);
                    }
                    else if (i == 1)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT2I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Red, width: 3);
                            formsPlotT2O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Red, width: 3);
                        }
                        formsPlotT2X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Red, width: 3);
                        formsPlotT2Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Red, width: 3);
                    }
                    else if (i == 2)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT3I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Red, width: 3);
                            formsPlotT3O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Red, width: 3);
                        }
                        formsPlotT3X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Red, width: 3);
                        formsPlotT3Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Red, width: 3);
                    }
                    else if (i == 3)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT4I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Red, width: 3);
                            formsPlotT4O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Red, width: 3);
                        }
                        formsPlotT4X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Red, width: 3);
                        formsPlotT4Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Red, width: 3);
                    }
                    else if (i == 4)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT5I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Red, width: 3);
                            formsPlotT5O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Red, width: 3);
                        }
                        formsPlotT5X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Red, width: 3);
                        formsPlotT5Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Red, width: 3);
                    }
                }
            }
            formsPlotAnalysis.Render();
            if (selectedSections.Count > 0)
            {
                lblRecordNumber.Text = "0";
                RefreshXYZ(selectedSections[0]);
            }
        }
        private void CalculateXYZ()
        {
            List<CalculationResult> lstCR = new List<CalculationResult>();
            foreach (var s in selectedSections)
            {
                double sampleRate = s.SamplingRate;
                int count = s.records.Count;
                double[] x = new double[count];
                double[] y = new double[count];
                double[] z = new double[count];
                double[] resultante = new double[count];
                for (int i = 0; i < count; i++)
                {
                    x[i] = s.records[i].x;
                    y[i] = s.records[i].y;
                    z[i] = s.records[i].z;
                }
                ApplyLowPassFilter(ref x, sampleRate);
                ApplyLowPassFilter(ref y, sampleRate);
                ApplyLowPassFilter(ref z, sampleRate);
                for (int i = 0; i < count; i++)
                {
                    resultante[i] = Math.Sqrt(Math.Pow(x[i], 2)
                                                + Math.Pow(y[i], 2)
                                                );
                }
              
                FFTData cmpX = EquilibrageHelper.CalculateFFT(x, sampleRate, cbxFFT, chkRemoveDC.Checked);
                FFTData cmpY = EquilibrageHelper.CalculateFFT(y, sampleRate, cbxFFT, chkRemoveDC.Checked);
                FFTData cmpZ = EquilibrageHelper.CalculateFFT(z, sampleRate, cbxFFT, chkRemoveDC.Checked);
                FFTData cmpResultante = EquilibrageHelper.CalculateFFT(resultante, sampleRate, cbxFFT, chkRemoveDC.Checked);
                
                double avgTourTime = count / sampleRate;  // en secondes
                double f_rot = 1.0 / avgTourTime;
                lstCR.Add(EquilibrageHelper.CompleteSimulation(null, "turn by turn", cmpX, cmpY, cmpZ, cmpResultante, sampleRate, f_rot));
            }
            //for the 5 orders, calculate the average and most possible value =
           
            for (int i = 0; i < 5; i++)
            {
                Dictionary<int,int> lstBestAngleX = new Dictionary<int, int>(), lstBestAngleY = new Dictionary<int, int>(), lstBestAngleZ = new Dictionary<int, int>(), lstBestAngleRes = new Dictionary<int, int>(), lstBestAngleInner = new Dictionary<int, int>(), lstBestAngleOuter = new Dictionary<int, int>();
                List<Tuple<double, double>> lstMagnitudeAngles = new List<Tuple<double, double>>();
                List<Tuple<double, double>> lstMagnitudeX = new List<Tuple<double, double>>();
                List<Tuple<double, double>> lstMagnitudeY = new List<Tuple<double, double>>();
                List<Tuple<double, double>> lstMagnitudeZ = new List<Tuple<double, double>>();
                List<Tuple<double, double>> lstMagnitudeRes = new List<Tuple<double, double>>();
                foreach (var cr in lstCR)
                {
                    int key = (int)cr.px[i].BestAngleDeg;
                    if (lstBestAngleX.ContainsKey(key))
                        lstBestAngleX[key]++;
                    else
                        lstBestAngleX.Add((int)cr.px[i].BestAngleDeg, 1);
                    key = (int)cr.py[i].BestAngleDeg;
                    if (lstBestAngleY.ContainsKey(key))
                        lstBestAngleY[key]++;
                    else
                        lstBestAngleY.Add((int)cr.py[i].BestAngleDeg, 1);
                    key = (int)cr.pz[i].BestAngleDeg;
                    if (lstBestAngleZ.ContainsKey(key))
                        lstBestAngleZ[key]++;
                    else
                        lstBestAngleZ.Add((int)cr.pz[i].BestAngleDeg, 1);
                    key = (int)cr.pResultante[i].BestAngleDeg;
                    if (lstBestAngleRes.ContainsKey(key))
                        lstBestAngleRes[key]++;
                    else
                        lstBestAngleRes.Add((int)cr.pResultante[i].BestAngleDeg, 1);

                    if (cr.dir[i].IsDynamic)
                    {
                        key = (int)cr.dir[i].correction.AngleInnerDeg;
                        if (lstBestAngleInner.ContainsKey(key))
                            lstBestAngleInner[key]++;
                        else
                            lstBestAngleInner.Add(key, 1);

                        key = (int)cr.dir[i].correction.AngleOuterDeg;
                        if (lstBestAngleOuter.ContainsKey(key))
                            lstBestAngleOuter[key]++;
                        else
                            lstBestAngleOuter.Add(key, 1);
                    }
                    lstMagnitudeAngles.Add(new Tuple<double, double>(cr.dir[i].MagnitudeX, cr.dir[i].MagnitudeY));
                    lstMagnitudeX.Add(new Tuple<double, double>(cr.px[i].MinAmplitude, cr.px[i].ActualAmplitude));
                    lstMagnitudeY.Add(new Tuple<double, double>(cr.py[i].MinAmplitude, cr.py[i].ActualAmplitude));
                    lstMagnitudeZ.Add(new Tuple<double, double>(cr.pz[i].MinAmplitude, cr.pz[i].ActualAmplitude));
                    lstMagnitudeRes.Add(new Tuple<double, double>(cr.pResultante[i].MinAmplitude, cr.pResultante[i].ActualAmplitude));

                }
                //display info about the data
                lstSimulationTurnByTurn.Items.Add($"Order {i} DYNAMIC AVG MagX {lstMagnitudeAngles.Average(t => t.Item1)} AVG MagY { lstMagnitudeAngles.Average(t => t.Item2)}");
                lstSimulationTurnByTurn.Items.Add($"Order {i} AVG Magnitude X {lstMagnitudeX.Average(t => t.Item2)} Magnitude Y {lstMagnitudeY.Average(t => t.Item2)} Magnitude Z {lstMagnitudeZ.Average(t => t.Item2)}");
                lstSimulationTurnByTurn.Items.Add($"Order {i} AVG Magnitude Resultante {lstMagnitudeRes.Average(t => t.Item2)}");
                CalculateStatistics("Angle Inner", i, lstBestAngleInner);
                CalculateStatistics("Angle Outer", i, lstBestAngleOuter);
                CalculateStatistics("X", i, lstBestAngleX);
                CalculateStatistics("Y", i, lstBestAngleY);
                CalculateStatistics("Z", i, lstBestAngleZ);
                CalculateStatistics("Resultante", i, lstBestAngleRes);

                if (i == 0)
                {
                    DisplayTurnByTurnGraph(lstBestAngleInner, lstBestAngleOuter, lstBestAngleX, lstBestAngleY, formsPlotT1I, formsPlotT1O, formsPlotT1X, formsPlotT1Y);
                    lblTotalSelected.Text = $"Found Dynamic unbalance on {lstBestAngleInner.Count} turn of {lstSectionSelector.CheckedItems.Count} selected";
                }
                else if (i == 1)
                    DisplayTurnByTurnGraph(lstBestAngleInner, lstBestAngleOuter, lstBestAngleX, lstBestAngleY, formsPlotT2I, formsPlotT2O, formsPlotT2X, formsPlotT2Y);
                else if (i == 2)
                    DisplayTurnByTurnGraph(lstBestAngleInner, lstBestAngleOuter, lstBestAngleX, lstBestAngleY, formsPlotT3I, formsPlotT3O, formsPlotT3X, formsPlotT3Y);
                else if (i == 3)
                    DisplayTurnByTurnGraph(lstBestAngleInner, lstBestAngleOuter, lstBestAngleX, lstBestAngleY, formsPlotT4I, formsPlotT4O, formsPlotT4X, formsPlotT4Y);
                else if (i == 4)
                    DisplayTurnByTurnGraph(lstBestAngleInner, lstBestAngleOuter, lstBestAngleX, lstBestAngleY, formsPlotT5I, formsPlotT5O, formsPlotT5X, formsPlotT5Y);

                

            }
        }

        private void CalculateStatistics(string data, int i, Dictionary<int, int> lstData)
        {
            int totalOccurrences = lstData.Values.Sum();

            // Moyenne pondérée
            double weightedSum = lstData.Sum(kv => kv.Key * kv.Value);
            double mean = weightedSum / totalOccurrences;

            // Variance pondérée
            double variance = lstData.Sum(kv => kv.Value * Math.Pow(kv.Key - mean, 2)) / totalOccurrences;

            // Écart-type pondéré
            double standardDeviation = Math.Sqrt(variance);

            //codef variation
            double coeffVariation = standardDeviation / mean;
            lstSimulationTurnByTurn.Items.Add($"Order {i} Statistic {data} : AVG Angle {mean:F2} Variation {coeffVariation:F2} Variance {variance:F2} Ecart-type {standardDeviation:F2}");
        }

        private void DisplayTurnByTurnGraph(Dictionary<int, int> lstBestAngleInner, Dictionary<int, int> lstBestAngleOuter, Dictionary<int, int> lstBestAngleX, Dictionary<int, int> lstBestAngleY, FormsPlot frmInner, FormsPlot frmOuter, FormsPlot frmX, FormsPlot frmY)
        {
            try
            {
                double[] xs = lstBestAngleInner.Keys.Select(x => (double)x).ToArray();
                double[] ys = lstBestAngleInner.Values.Select(y => (double)y).ToArray();
                if (xs.Length > 0)
                {
                    frmInner.Plot.PlotBar(xs, ys, barWidth: 0.5, fillColor: Color.Blue, outlineColor: Color.Blue);
    
                }
                xs = lstBestAngleOuter.Keys.Select(x => (double)x).ToArray();
                ys = lstBestAngleOuter.Values.Select(y => (double)y).ToArray();
                if (xs.Length > 0)
                {
                    frmOuter.Plot.PlotBar(xs, ys, barWidth: 0.5, fillColor: Color.Blue, outlineColor: Color.Blue);
              
                }
                xs = lstBestAngleX.Keys.Select(x => (double)x).ToArray();
                ys = lstBestAngleX.Values.Select(y => (double)y).ToArray();
                if (xs.Length > 0)
                {
                    frmX.Plot.PlotBar(xs, ys, barWidth: 0.5, fillColor: Color.Blue, outlineColor: Color.Blue);
   
                }
                xs = lstBestAngleY.Keys.Select(x => (double)x).ToArray();
                ys = lstBestAngleY.Values.Select(y => (double)y).ToArray();
                if (xs.Length > 0)
                {
                    frmY.Plot.PlotBar(xs, ys, barWidth: 0.5, fillColor: Color.Blue, outlineColor: Color.Blue);
                }
            }
            catch
            { }
        }

        private void RefreshXYZ(section s)
        {
            int count = s.records.Count;
            double[] x = new double[count];
            double[] y = new double[count];
            double[] z = new double[count];
            double[] axis = new double[count];
            double anglePerRecord = 360.0 / count;
            double sampleRate = s.SamplingRate;
            for (int i = 0; i < count; i++)
            {
                x[i] = s.records[i].x;
                y[i] = s.records[i].y;
                z[i] = s.records[i].z;
                axis[i] = i * anglePerRecord;
            }
            ApplyLowPassFilter(ref x, sampleRate);
            ApplyLowPassFilter(ref y, sampleRate);
            ApplyLowPassFilter(ref z, sampleRate);
            formsPlotX.Plot.Clear();
            lstPeakX.Items.Clear();
            formsPlotY.Plot.Clear();
            lstPeakY.Items.Clear();
            formsPlotZ.Plot.Clear();
            lstPeakZ.Items.Clear();
            if (!chkFFT.Checked)
            {
             
                formsPlotX.Plot.PlotScatter(axis, x, null, 1, 1);
 
                formsPlotY.Plot.PlotScatter(axis, y, null, 1, 1);

                formsPlotZ.Plot.PlotScatter(axis, z, null, 1, 1);
                formsPlotX.Plot.AxisAuto();
                formsPlotY.Plot.AxisAuto();
                formsPlotZ.Plot.AxisAuto();
                formsPlotX.Render();
                formsPlotY.Render();
                formsPlotZ.Render();
                return;
            }
            else
            {
               
                FFTData cmpX = EquilibrageHelper.CalculateFFT(x, sampleRate, cbxFFT, chkRemoveDC.Checked);
                FFTData cmpY = EquilibrageHelper.CalculateFFT(y, sampleRate, cbxFFT, chkRemoveDC.Checked);
                FFTData cmpZ = EquilibrageHelper.CalculateFFT(z, sampleRate, cbxFFT, chkRemoveDC.Checked);
              
                double avgTourTime = count / sampleRate;  // en secondes
                double f_rot = 1.0 / avgTourTime;
         
                AnalyzeAxis("X", cmpX, sampleRate, lstPeakX, Color.Blue, formsPlotX, f_rot);
                AnalyzeAxis("Y", cmpY, sampleRate, lstPeakY, Color.Blue, formsPlotY, f_rot);
                AnalyzeAxis("Z", cmpZ, sampleRate, lstPeakZ, Color.Blue, formsPlotZ, f_rot);
                String sText = $"Fundamental: {f_rot}Hz\r\n1er order: {f_rot * 2}\r\n2eme order: {f_rot * 3}\r\n3eme order: {f_rot * 4}\r\n4er order: {f_rot * 5}\r\n5er order: {f_rot * 6}\r\n";
                formsPlotX.Plot.AddAnnotation(sText);
                
                formsPlotX.Render();
                formsPlotY.Render();
                formsPlotZ.Render();
            }
        }


        // analysis of all consecutives segments
        private void RefreshAnalysisGlobal()
        {
            if (String.IsNullOrEmpty(sLastCSV)) return;

            int countTotal = iTotalRecordsInCSV; //number of total records for selected sections in the dataset
            int alignedCount = selectedSections.Select(x => x.records.Count).Max();
            //get avg samplerate
            double sampleRate = selectedSections.Select(x => x.SamplingRate).Average();
            if (chkOrderTracking.Checked) //resample on 360 points per sections
                countTotal = selectedSections.Count * alignedCount; //made a count total based on the max number of data

            double[] analyzedX = new double[countTotal];
            double[] analyzedY = new double[countTotal];
            double[] analyzedZ = new double[countTotal];
            double[] whiteLine = new double[countTotal];
            double[] resultante = new double[countTotal];
            //display peaks
            //double[] angle = new double[countTotal];
            int[][] peakX = new int[selectedSections.Count][];
            int[][] peakY = new int[selectedSections.Count][];
            int[][] peakZ = new int[selectedSections.Count][];
            
            int iCount = 0, tourNumber = 0;
            foreach (section se in selectedSections)
            {
                int startCount = iCount;
                whiteLine[iCount] = 10;
                int count = se.records.Count;
                double angleIncrement = 360.0 / count;
                if (!chkOrderTracking.Checked)
                {
                   
                    for (int i = 0; i < count; i++)
                    {
                     //   angle[iCount] = i*angleIncrement;
                        if (chkAbsolute.Checked)
                        {
                            analyzedX[iCount] = Math.Abs(se.records[i].x);
                            analyzedY[iCount] = Math.Abs(se.records[i].y);
                            analyzedZ[iCount] = Math.Abs(se.records[i].z);
                        }
                        else
                        {
                            analyzedX[iCount] = se.records[i].x;
                            analyzedY[iCount] = se.records[i].y;
                            analyzedZ[iCount] = se.records[i].z;
                        }
                        iCount++;
                    }
                }
                else
                {
                    angleIncrement = 360.0 / alignedCount;
                    //for (int i = 0; i < alignedCount; i++)
                    //    angle[i+ (tourNumber* alignedCount)] = i * angleIncrement;
                    iCount += alignedCount;

                    ResampleSectionAngularXYZ(se.records, alignedCount, 1.0 / sampleRate, analyzedX, analyzedY, analyzedZ, tourNumber * alignedCount);
                   
                }

                //convert the return x axis (0 to number of sample for 1 turn) to 360°
                peakX[tourNumber] = GetPeakPerTurn(analyzedX.Skip(startCount).Take(iCount-startCount).ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();
                peakY[tourNumber] = GetPeakPerTurn(analyzedY.Skip(startCount).Take(iCount - startCount).ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();
                peakZ[tourNumber] = GetPeakPerTurn(analyzedZ.Skip(startCount).Take(iCount - startCount).ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();


                tourNumber++;

            }

            ApplyLowPassFilter(ref analyzedX, sampleRate);
            ApplyLowPassFilter(ref analyzedY, sampleRate);
            ApplyLowPassFilter(ref analyzedZ, sampleRate);
            int countX = analyzedX.Count();
            for (int i = 0; i < countX; i++)
            {
                resultante[i] = Math.Sqrt(Math.Pow(analyzedX[i], 2)
                                            + Math.Pow(analyzedY[i], 2)
                                            );
            }
            lblPeak.Text += $"\r\nX Pk-Pk (global) : {analyzedX.Max() - analyzedX.Min()}\r\nY Pk-Pk (global) : {analyzedY.Max() - analyzedY.Min()}\r\nZ Pk-Pk (global) : {analyzedZ.Max() - analyzedZ.Min()}";


            var tpx = GetTopCommonPeaksWithAmplitude(peakX, analyzedX, 10, 2, peakX.Count());
            ShowPeakHistogram(tpx, formsPlotAnalysisTemporalX);
            var tpy = GetTopCommonPeaksWithAmplitude(peakY, analyzedY, 10, 2, peakY.Count());
            ShowPeakHistogram(tpy, formsPlotAnalysisTemporalY);
            var tpz = GetTopCommonPeaksWithAmplitude(peakZ, analyzedZ, 10, 2, peakZ.Count());
            ShowPeakHistogram(tpz, formsPlotAnalysisTemporalZ);
            

            formsPlotGlobal.Plot.Clear();

            if (!chkFFT.Checked)
            {
               

                lstPeakGlobalX.Items.Clear();
                lstPeakGlobalY.Items.Clear();
                lstPeakGlobalZ.Items.Clear();
                lstPeakResultanteGlobal.Items.Clear();

             
                foreach (var item in tpx)
                    lstPeakGlobalX.Items.Add($"Average PeakX: Angle {item.Mean} – Fréquence {item.Freq} - Force {item.AverageAmplitude}");
                
                foreach (var item in tpy)
                    lstPeakGlobalY.Items.Add($"Average PeakY: Angle {item.Mean} – Fréquence {item.Freq} - Force {item.AverageAmplitude}");
                
                foreach (var item in tpz)
                    lstPeakGlobalZ.Items.Add($"Average PeakZ: Angle {item.Mean} – Fréquence {item.Freq} - Force {item.AverageAmplitude}");
                double[] temporal = Enumerable.Range(0, countTotal)
                                     .Select(i => (double)i)
                                     .ToArray();
                double max = 0;
                if (chkShowX.Checked)
                {
                    formsPlotGlobal.Plot.PlotScatter(temporal, analyzedX, Color.Blue, 1, 1, "X");
                    DisplayPeaksTemporal(analyzedX, temporal, "Top Peak X", formsPlotGlobal, lstPeakGlobalX);
                    max = analyzedX.Max();
                }
                if (chkShowY.Checked)
                {
                    formsPlotGlobal.Plot.PlotScatter(temporal, analyzedY, Color.Red, 1, 1, "Y");
                    DisplayPeaksTemporal(analyzedY, temporal, "Top Peak Y", formsPlotGlobal, lstPeakGlobalY);
                    max = Math.Max(max, analyzedY.Max());
                }
                if (chkShowZ.Checked)
                {
                    formsPlotGlobal.Plot.PlotScatter(temporal, analyzedZ, Color.Yellow, 1, 1, "Z");
                    DisplayPeaksTemporal(analyzedZ, temporal, "Top Peak Z", formsPlotGlobal, lstPeakGlobalZ);
                    max = Math.Max(max, analyzedZ.Max());
                }
                if (chkShowResultante.Checked)
                {
                    formsPlotGlobal.Plot.PlotScatter(temporal, resultante, Color.DeepPink, 1, 1, "Resultante");
                    DisplayPeaksTemporal(resultante, temporal, "Resultante", formsPlotGlobal, lstPeakResultanteGlobal);
                    max = Math.Max(max, resultante.Max());
                }
                max += 5;
                for (int i = 0; i < iCount; i++)
                {
                    if(whiteLine[i] == 10)
                        whiteLine[i] = max;
                }
                formsPlotGlobal.Plot.PlotScatter(temporal, whiteLine, Color.Black, 1, 3, "WhiteLine");
                //formsPlotAnalysis.Plot.Axis(0, 360, -1, 1);
                formsPlotGlobal.Plot.SetAxisLimitsX(0, countTotal);
                formsPlotGlobal.Plot.AxisAutoY();
                formsPlotGlobal.Plot.Legend(true);
            }
            else
            {

                FFTData cmpX = EquilibrageHelper.CalculateFFT(analyzedX, sampleRate, cbxFFT, chkRemoveDC.Checked);
                FFTData cmpY = EquilibrageHelper.CalculateFFT(analyzedY, sampleRate, cbxFFT, chkRemoveDC.Checked);
                FFTData cmpZ = EquilibrageHelper.CalculateFFT(analyzedZ, sampleRate, cbxFFT, chkRemoveDC.Checked);
                FFTData cmpResultante = EquilibrageHelper.CalculateFFT(resultante, sampleRate, cbxFFT, chkRemoveDC.Checked);
                double avgTourTime = countTotal / sampleRate / selectedSections.Count;  // en secondes  
                double f_rot = 1.0 / avgTourTime;
                
                formsPlotGlobal.Plot.Clear();
                lstPeakGlobalX.Items.Clear();
                lstPeakGlobalY.Items.Clear();
                lstPeakGlobalZ.Items.Clear();
                lstPeakResultanteGlobal.Items.Clear();
                if (chkShowX.Checked)
                {
                    AnalyzeAxis("X", cmpX, sampleRate, lstPeakGlobalX, Color.Blue, formsPlotGlobal, f_rot);
                }
                if (chkShowY.Checked)
                {
                    AnalyzeAxis("Y", cmpY, sampleRate, lstPeakGlobalY, Color.Red, formsPlotGlobal, f_rot);
                }
                if (chkShowZ.Checked)
                {
                    AnalyzeAxis("Z", cmpZ, sampleRate, lstPeakGlobalZ, Color.Yellow, formsPlotGlobal, f_rot);
                }
                if (chkShowResultante.Checked)
                {
                    AnalyzeAxis("Resultante", cmpResultante, sampleRate, lstPeakResultanteGlobal, Color.DeepPink, formsPlotGlobal, f_rot);
                }
                String sText = $"Fundamental: {f_rot}Hz\r\n1er order: {f_rot * 2}\r\n2eme order: {f_rot * 3}\r\n3eme order: {f_rot * 4}\r\n4er order: {f_rot * 5}\r\n5er order: {f_rot * 6}\r\n";
                formsPlotGlobal.Plot.AddAnnotation(sText);
                lstSimulationGlobal.Items.Clear();
                double gain = 1.0;
                var calcResult = EquilibrageHelper.CompleteSimulation(lstSimulationGlobal, "Global", cmpX, cmpY, cmpZ, cmpResultante, sampleRate, f_rot, gain, 1);
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT1I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Green, width: 3);
                            formsPlotT1O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Green, width: 3);
                        }
                        formsPlotT1X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Green, width: 3);
                        formsPlotT1Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Green, width: 3);
                    }
                    else if (i == 1)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT2I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Green, width: 3);
                            formsPlotT2O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Green, width: 3);
                        }
                        formsPlotT2X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Green, width: 3);
                        formsPlotT2Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Green, width: 3);
                    }
                    else if (i == 2)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT3I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Green, width: 3);
                            formsPlotT3O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Green, width: 3);
                        }
                        formsPlotT3X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Green, width: 3);
                        formsPlotT3Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Green, width: 3);
                    }
                    else if (i == 3)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT4I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Green, width: 3);
                            formsPlotT4O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Green, width: 3);
                        }
                        formsPlotT4X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Green, width: 3);
                        formsPlotT4Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Green, width: 3);
                    }
                    else if (i == 4)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT5I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Green, width: 3);
                            formsPlotT5O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Green, width: 3);
                        }
                        formsPlotT5X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Green, width: 3);
                        formsPlotT5Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Green, width: 3);
                    }
                }
            }
            formsPlotGlobal.Render();
        }
        private void RefreshGyro()
        {
            if (String.IsNullOrEmpty(sLastCSV)) return;

            int countTotal = iTotalRecordsInCSV; //number of total records for selected sections in the dataset
            int alignedCount = selectedSections.Select(x => x.records.Count).Max();
            double sampleRate = selectedSections.Select(x => x.SamplingRate).Average();

            if (chkOrderTracking.Checked) //resample on 360 points per sections
                countTotal = selectedSections.Count * alignedCount; //made a count total based on the max number of data
            double[] analyzedX = new double[countTotal];
            double[] analyzedY = new double[countTotal];
            double[] analyzedZ = new double[countTotal];
            double[] whiteLine = new double[countTotal];
            double[] resultante = new double[countTotal];
            double[] angleX = new double[countTotal];
            double[] angleY = new double[countTotal];
            double[] angleZ = new double[countTotal];
            double[] pitch = new double[countTotal];
            double[] roll = new double[countTotal];

            double[] angle = new double[countTotal];
            int[][] peakX = new int[selectedSections.Count][];
            int[][] peakY = new int[selectedSections.Count][];
            int[][] peakZ = new int[selectedSections.Count][];


            
            int iCount = 0, tourNumber = 0;
            foreach (section se in selectedSections)
            {
                int startCount = iCount;
                whiteLine[iCount] = 10;
                int count = se.records.Count;
                double angleIncrement = 360.0 / count;
                if (!chkOrderTracking.Checked)
                {

                    for (int i = 0; i < count; i++)
                    {
                        angle[iCount] = i * angleIncrement;
                        if (chkAbsolute.Checked)
                        {
                            analyzedX[iCount] = Math.Abs(se.records[i].gx);
                            analyzedY[iCount] = Math.Abs(se.records[i].gy);
                            analyzedZ[iCount] = Math.Abs(se.records[i].gz);
                        }
                        else
                        {
                            analyzedX[iCount] = se.records[i].gx;
                            analyzedY[iCount] = se.records[i].gy;
                            analyzedZ[iCount] = se.records[i].gz;
                        }
                        iCount++;
                    }
                }
                else
                {
                    angleIncrement = 360.0 / alignedCount;
                    for (int i = 0; i < alignedCount; i++)
                        angle[i+ (tourNumber* alignedCount)] = i * angleIncrement;
                    iCount += alignedCount;

                    ResampleSectionGyroXYZ(se.records, alignedCount, 1.0 / sampleRate, analyzedX, analyzedY, analyzedZ, tourNumber * alignedCount);
                   
                }

                //convert the return x axis (0 to number of sample for 1 turn) to 360°
                peakX[tourNumber] = GetPeakPerTurn(analyzedX.Skip(startCount).Take(iCount - startCount).ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();
                peakY[tourNumber] = GetPeakPerTurn(analyzedY.Skip(startCount).Take(iCount - startCount).ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();
                peakZ[tourNumber] = GetPeakPerTurn(analyzedZ.Skip(startCount).Take(iCount - startCount).ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();


                tourNumber++;

            }
            ApplyLowPassFilter(ref analyzedX, sampleRate);
            ApplyLowPassFilter(ref analyzedY, sampleRate);
            ApplyLowPassFilter(ref analyzedZ, sampleRate);
            int countX = analyzedX.Count();
            for (int i = 0; i < countX; i++)
            {
                resultante[i] = Math.Sqrt(Math.Pow(analyzedX[i], 2)
                                            + Math.Pow(analyzedY[i], 2)
                                            );
            }
            CalculateGyroAngles(ref analyzedX, ref analyzedY, ref analyzedZ, ref resultante, ref angleX, ref angleY, ref angleZ, ref pitch, ref roll);

            lblPeak.Text += $"\r\nGyro X Pk-Pk (global) : {analyzedX.Max() - analyzedX.Min()}\r\nGyro Y Pk-Pk (global) : {analyzedY.Max() - analyzedY.Min()}\r\nGyro Z Pk-Pk (global) : {analyzedZ.Max() - analyzedZ.Min()}";

            formsPlotGyro.Plot.Clear();
            if (!chkFFT.Checked)
            {
                lstPeakResultanteGyro.Items.Clear();
                lstPeakGyroX.Items.Clear();
                lstPeakGyroY.Items.Clear();
                lstPeakGyroZ.Items.Clear();

                double[] temporal = Enumerable.Range(0, countTotal)
                                    .Select(i => (double)i)
                                    .ToArray();

                var top5 = GetTopCommonPeaksWithAmplitude(peakX, analyzedX, tol: 10, minSamples: 2, topN: 5);
                foreach (var item in top5)
                    lstPeakGyroX.Items.Add($"Average PeakX: Angle {item.Mean} – Fréquence {item.Freq} - Force {item.AverageAmplitude}");
                top5 = GetTopCommonPeaksWithAmplitude(peakY, analyzedY, tol: 10, minSamples: 2, topN: 5);
                foreach (var item in top5)
                    lstPeakGyroY.Items.Add($"Average PeakY: Angle {item.Mean} – Fréquence {item.Freq} - Force {item.AverageAmplitude}");
                top5 = GetTopCommonPeaksWithAmplitude(peakZ, analyzedZ, tol: 10, minSamples: 2, topN: 5);
                foreach (var item in top5)
                    lstPeakGyroZ.Items.Add($"Average PeakZ: Angle {item.Mean} – Fréquence {item.Freq} - Force {item.AverageAmplitude}");

                double max = 0;
                if (chkShowX.Checked)
                {
                    formsPlotGyro.Plot.PlotScatter(temporal, analyzedX, Color.Blue, 1, 1, "X");
                    DisplayPeaksTemporal(analyzedX, temporal, "Top Peak X", formsPlotGyro, lstPeakGyroX);
                    max = analyzedX.Max();
                }
                if (chkShowY.Checked)
                {
                    formsPlotGyro.Plot.PlotScatter(temporal, analyzedY, Color.Red, 1, 1, "Y");
                    DisplayPeaksTemporal(analyzedY, temporal, "Top Peak Y", formsPlotGyro, lstPeakGyroY);
                    max = Math.Max(max, analyzedY.Max());
                }
                if (chkShowZ.Checked)
                {
                    formsPlotGyro.Plot.PlotScatter(temporal, analyzedZ, Color.Yellow, 1, 1, "Z");
                    DisplayPeaksTemporal(analyzedZ, temporal, "Top Peak Z", formsPlotGyro, lstPeakGyroZ);
                    max = Math.Max(max, analyzedZ.Max());
                }
                if (chkShowResultante.Checked)
                {
                    formsPlotGyro.Plot.PlotScatter(temporal, resultante, Color.DeepPink, 1, 1, "Resultante");
                    DisplayPeaksTemporal(resultante, temporal, "Top Peak Resultante", formsPlotGyro, lstPeakResultanteGyro);
                    max = Math.Max(max, resultante.Max());
                }
                max += 5;
                for (int i = 0; i < iCount; i++)
                {
                    if (whiteLine[i] == 10)
                        whiteLine[i] = max;
                }
                formsPlotGyro.Plot.PlotScatter(temporal, whiteLine, Color.Black, 1, 3, "WhiteLine");
                //formsPlotAnalysis.Plot.Axis(0, 360, -1, 1);
                formsPlotGyro.Plot.SetAxisLimitsX(0, countTotal);
                formsPlotGyro.Plot.AxisAutoY();
                formsPlotGyro.Plot.Legend(true);
            }
            else
            {
                FFTData cmpX = EquilibrageHelper.CalculateFFT(analyzedX, sampleRate, cbxFFT, chkRemoveDC.Checked);
                FFTData cmpY = EquilibrageHelper.CalculateFFT(analyzedY, sampleRate, cbxFFT, chkRemoveDC.Checked);
                FFTData cmpZ = EquilibrageHelper.CalculateFFT(analyzedZ, sampleRate, cbxFFT, chkRemoveDC.Checked);
                FFTData cmpResultante = EquilibrageHelper.CalculateFFT(resultante, sampleRate, cbxFFT, chkRemoveDC.Checked);
                double avgTourTime = countTotal / sampleRate / selectedSections.Count;  // en secondes  
                double f_rot = 1.0 / avgTourTime;

                formsPlotGyro.Plot.Clear();
                lstPeakGyroX.Items.Clear();
                lstPeakGyroY.Items.Clear();
                lstPeakGyroZ.Items.Clear();
                lstPeakResultanteGyro.Items.Clear();
                if (chkShowX.Checked)
                {
                    AnalyzeAxis("X", cmpX, sampleRate, lstPeakGyroX, Color.Blue, formsPlotGyro, f_rot);
                }
                if (chkShowY.Checked)
                {
                    AnalyzeAxis("Y", cmpY, sampleRate, lstPeakGyroY, Color.Red, formsPlotGyro, f_rot);
                }
                if (chkShowZ.Checked)
                {
                    AnalyzeAxis("Z", cmpZ, sampleRate, lstPeakGyroZ, Color.Yellow, formsPlotGyro, f_rot);
                }
                if (chkShowResultante.Checked)
                {
                    AnalyzeAxis("Resultante", cmpResultante,sampleRate , lstPeakResultanteGyro, Color.DeepPink, formsPlotGyro, f_rot);
                }
                String sText = $"Fundamental: {f_rot}Hz\r\n1er order: {f_rot * 2}\r\n2eme order: {f_rot * 3}\r\n3eme order: {f_rot * 4}\r\n4er order: {f_rot * 5}\r\n5er order: {f_rot * 6}\r\n";
                formsPlotGyro.Plot.AddAnnotation(sText);

                lstSimulationGyro.Items.Clear();
                double gain = 1.0;
                var calcResult = EquilibrageHelper.CompleteSimulation(lstSimulationGyro, "Gyroscope", cmpX, cmpY, cmpZ, cmpResultante, sampleRate, f_rot, gain, 1);
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT1I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Yellow, width: 3);
                            formsPlotT1O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Yellow, width: 3);
                        }
                        formsPlotT1X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Yellow, width: 3);
                        formsPlotT1Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Yellow, width: 3);
                    }
                    else if (i == 1)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT2I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Yellow, width: 3);
                            formsPlotT2O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Yellow, width: 3);
                        }
                        formsPlotT2X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Yellow, width: 3);
                        formsPlotT2Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Yellow, width: 3);
                    }
                    else if (i == 2)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT3I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Yellow, width: 3);
                            formsPlotT3O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Yellow, width: 3);
                        }
                        formsPlotT3X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Yellow, width: 3);
                        formsPlotT3Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Yellow, width: 3);
                    }
                    else if (i == 3)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT4I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Yellow, width: 3);
                            formsPlotT4O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Yellow, width: 3);
                        }
                        formsPlotT4X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Yellow, width: 3);
                        formsPlotT4Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Yellow, width: 3);
                    }
                    else if (i == 4)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT5I.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleInnerDeg, Color.Yellow, width: 3);
                            formsPlotT5O.Plot.AddVerticalLine(calcResult.dir[i].correction.AngleOuterDeg, Color.Yellow, width: 3);
                        }
                        formsPlotT5X.Plot.AddVerticalLine(calcResult.px[i].BestAngleDeg, Color.Yellow, width: 3);
                        formsPlotT5Y.Plot.AddVerticalLine(calcResult.py[i].BestAngleDeg, Color.Yellow, width: 3);
                    }
                }
            }
            formsPlotGyro.Render();
        }
        public static void ShowPeakHistogram(List<PeakInfo> peaks, ScottPlot.FormsPlot formsPlot)
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
            var bars = formsPlot.Plot.PlotBar(positions, heights, barWidth: 8, fillColor: System.Drawing.Color.SteelBlue);

            // Ajouter les labels de fréquence au-dessus de chaque barre
            for (int i = 0; i < positions.Length; i++)
            {
                double x = positions[i];
                double y = heights[i];
                string label = labels[i];
                formsPlot.Plot.PlotText(label, x, y + 0.01, color: System.Drawing.Color.Black, fontSize: 10, alignment: Alignment.LowerCenter);
            }

            // Ajuster les axes
            formsPlot.Plot.Title("Histogramme des pics groupés par angle");
            formsPlot.Plot.XLabel("Angle (degrés)");
            formsPlot.Plot.YLabel("Amplitude moyenne");
            formsPlot.Plot.SetAxisLimitsX(0, 360); // pour rester entre 0 et 360°
            formsPlot.Plot.AxisAutoY();

            formsPlot.Render();
        }

      
        private double CalcAngle(double ang)
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
        private double Interpolate(double t0, double x0, double t1, double x1, double t)
        {
            return x0 + (x1 - x0) * ((t - t0) / (t1 - t0));
        }

        // Rééchantillonne une section en N points angulaires pour X,Y,Z
        void ResampleSectionAngularXYZ(List<xyz> records, int N, double dt,
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
        void ResampleSectionGyroXYZ(List<xyz> records, int N, double dt,
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

        private void AnalyzeAxis(string name, FFTData cmp, double sampleRate, ListBox targetList, Color plotColor, ScottPlot.FormsPlot plt, double f_rot)
        {
         
            double filterFFT = Convert.ToDouble(txtFFTLimit.Text);
            // Filtrer pour ne garder que les fréquences < filterFFT Hz
            var filtered = cmp.Frequence
                .Select((f, i) => new { Freq = f, Mag = cmp.Magnitude[i], Index= i, Angle = cmp.AngleDeg[i]})
                .Where(x => x.Freq < filterFFT)
                .ToArray();
            
            // Extraction des fréquences et magnitudes filtrées
            var filteredFreqs = filtered.Select(x => x.Freq).ToArray();
            var filteredMags = filtered.Select(x => x.Mag).ToArray();
            var filteredAngle = filtered.Select(x => x.Angle).ToArray();
            var scatter = plt.Plot.AddScatter(filteredFreqs, filteredMags, color: plotColor, label: name);
            scatter = plt.Plot.AddScatter(filteredFreqs, filteredAngle, color: plotColor, label: "ANGLE"+name);
            scatter.IsVisible = false;
            //find and draw first 5 harmonics
            for (int i = 0; i < 5; i++)
            {
                Fundamentale fundCandidate = EquilibrageHelper.GetFundamentalPhase(filteredFreqs, filteredMags, filteredAngle, f_rot *(i+1));

                if (fundCandidate != null)
                {
                    int idxFund = fundCandidate.Index;
                    double f = fundCandidate.Freq;
                    double m = fundCandidate.Magnitude;
                    double angleOffset = fundCandidate.Angle;
                    plt.Plot.AddPoint(f, m, Color.Magenta, 8);
                    plt.Plot.AddText($"{name}: {f:F2}Hz\n{angleOffset:F0}°", f, m, color: Color.DarkGreen);
                    if (targetList != null)
                        targetList.Items.Add($"{name} ordre {i+1} à {f:F2} Hz → angle ≈ {angleOffset:F0}° (mag {m:F3})");
                }
                else
                {
                    if (targetList != null)
                        targetList.Items.Add($"{name} ordre {i+1} (~{f_rot:F2} Hz) non trouvé");
                }
            }
            //find max peaks
            int range = 7;
            double threshold = filteredMags.Average() * 0.02;
            var peaks = FindPeaks(filteredMags, range, threshold)
                         .OrderByDescending(i => filteredMags[i])
                         .Take(5);

            foreach (int idx in peaks)
            {
                double f = filteredFreqs[idx];
                double m = filteredMags[idx];
                double angleOffset = filteredAngle[idx];
                //targetList.Items.Add($"{name} {f:F2} Hz → a={a:F3} m/s² → v_peak≈{vPeak:F1} mm/s, v_rms≈{vRms:F1} mm/s");
               // plt.Plot.PlotPoint(f, m, Color.Magenta, 8);
                //plt.Plot.PlotText($"{name}: {f:F2}Hz\n{angleOffset:F0}°",f, m, color: Color.DarkGreen);
                if (targetList != null)
                    targetList.Items.Add($"{name} Peak at {f:F2} Hz → {angleOffset:F0}° (mag {m:F3})");
            }
           

            plt.Plot.AxisAuto();
        }
        private int[] GetPeakPerTurn(double[] data)
        {
           
            double threshold = data.Average() * 0.02;
            //find peak in each segment to display average of each peak in the listbox
            return FindPeaks(data, 7, threshold)
                .OrderByDescending(i => data[i])
                .Take(5).ToArray();
        }

        private void DisplayPeaksTemporal(double[] data, double[] angle, string axis, ScottPlot.FormsPlot plt, ListBox lst)
        {
            int range = 7;
            double threshold = data.Average() * 0.02;
            var peaks = FindPeaks(data, range, threshold)
                         .OrderByDescending(i => data[i])
                         .Take(5);

            foreach (int idx in peaks)
            {
                plt.Plot.AddPoint(angle[idx], data[idx], Color.Magenta, 8);

                plt.Plot.AddText($"Force {data[idx]}",
                   angle[idx], data[idx], color: Color.DarkGreen
                );
                if (lst != null)
                    lst.Items.Add($"[{axis}] : Force {data[idx]}");

            }
        }
        private void AnalyzeCSV(string csvFile)
        {
            this.Text = "Equilibreuse - " + csvFile;
            sLastCSV = csvFile;
            //generate sections
            loadedSections = new List<section>(200);
            section s = null;
            bool bPreviousWhite = false;
            iTotalRecordsInCSV = 0;
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null, // Ignore missing columns
                HeaderValidated = null
            };
            using (var reader = new StreamReader(csvFile))
            {
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    txtSampleRate.Text = reader.ReadLine();
                    //csv.ReadHeader();
                    var records = csv.GetRecords<xyz>();

                    foreach (var r in records)
                    {
                        //ignore out of range values
                       // if (Math.Abs(r.x) > 15 || Math.Abs(r.y) > 15 || Math.Abs(r.z) > 15 || Math.Abs(r.gx) > 15 || Math.Abs(r.gy) > 15 || Math.Abs(r.gz) > 15)
                       //     continue;
                        if (r.isWhite && !bPreviousWhite)
                        {
                            s = new section();
                            s.records = new List<xyz>(360);
                            loadedSections.Add(s);
                        }
                        if (s != null)
                            s.records.Add(r);
                        bPreviousWhite = r.isWhite;
                        iTotalRecordsInCSV++;
                    }

                }
            }
            double sampleRate = Convert.ToDouble(txtSampleRate.Text);
            int i = 0;
            lstSectionSelector.Items.Clear();
            foreach (section se in loadedSections)
            {

                lstSectionSelector.Items.Add(new SectionInfo() { sampleRate = se.SamplingRate,  rpm = ((float)(60.0 * se.SamplingRate / se.Size)), index = i });
                i++;
            }
            lstSimulationCompiled.Items.Clear();
            selectedSections.Clear();
        }
        private void Analyze(string csvFile)
        {
            if (String.IsNullOrEmpty(csvFile))
                return;

            lstSimulationTurnByTurn.Items.Clear();
            selectedSections.Clear();
            iTotalRecordsInCSV = 0;
            foreach (int s in lstSectionSelector.CheckedIndices)
            {
                selectedSections.Add(loadedSections[s]);
                iTotalRecordsInCSV += loadedSections[s].records.Count;
            }
            formsPlotT1X.Plot.Clear(); formsPlotT1Y.Plot.Clear(); formsPlotT1O.Plot.Clear(); formsPlotT1I.Plot.Clear();
            formsPlotT2X.Plot.Clear(); formsPlotT2Y.Plot.Clear(); formsPlotT2O.Plot.Clear(); formsPlotT2I.Plot.Clear();
            formsPlotT3X.Plot.Clear(); formsPlotT3Y.Plot.Clear(); formsPlotT3O.Plot.Clear(); formsPlotT3I.Plot.Clear();
            formsPlotT4X.Plot.Clear(); formsPlotT4Y.Plot.Clear(); formsPlotT4O.Plot.Clear(); formsPlotT4I.Plot.Clear();
            formsPlotT5X.Plot.Clear(); formsPlotT5Y.Plot.Clear(); formsPlotT5O.Plot.Clear(); formsPlotT5I.Plot.Clear();

            if (selectedSections.Count > 0)
            {
                CalculateXYZ();
                RefreshAnalysisCompiled();
                RefreshAnalysisGlobal();
                RefreshGyro();
            }
            formsPlotT1X.Plot.SetAxisLimitsX(0,360); formsPlotT1Y.Plot.SetAxisLimitsX(0, 360); formsPlotT1O.Plot.SetAxisLimitsX(0, 360); formsPlotT1I.Plot.SetAxisLimitsX(0, 360);
            formsPlotT2X.Plot.SetAxisLimitsX(0, 360); formsPlotT2Y.Plot.SetAxisLimitsX(0, 360); formsPlotT2O.Plot.SetAxisLimitsX(0, 360); formsPlotT2I.Plot.SetAxisLimitsX(0, 360);
            formsPlotT3X.Plot.SetAxisLimitsX(0, 360); formsPlotT3Y.Plot.SetAxisLimitsX(0, 360); formsPlotT3O.Plot.SetAxisLimitsX(0, 360); formsPlotT3I.Plot.SetAxisLimitsX(0, 360);
            formsPlotT4X.Plot.SetAxisLimitsX(0, 360); formsPlotT4Y.Plot.SetAxisLimitsX(0, 360); formsPlotT4O.Plot.SetAxisLimitsX(0, 360); formsPlotT4I.Plot.SetAxisLimitsX(0, 360);
            formsPlotT5X.Plot.SetAxisLimitsX(0, 360); formsPlotT5Y.Plot.SetAxisLimitsX(0, 360); formsPlotT5O.Plot.SetAxisLimitsX(0, 360); formsPlotT5I.Plot.SetAxisLimitsX(0, 360);
            formsPlotT1X.Render(); formsPlotT1Y.Render(); formsPlotT1O.Render(); formsPlotT1I.Render();
            formsPlotT2X.Render(); formsPlotT2Y.Render(); formsPlotT2O.Render(); formsPlotT2I.Render();
            formsPlotT3X.Render(); formsPlotT3Y.Render(); formsPlotT3O.Render(); formsPlotT3I.Render();
            formsPlotT4X.Render(); formsPlotT4Y.Render(); formsPlotT4O.Render(); formsPlotT4I.Render();
            formsPlotT5X.Render(); formsPlotT5Y.Render(); formsPlotT5O.Render(); formsPlotT5I.Render();
        }

        public float FirstPeakPosition(float[] spectrum, float[] frequencies)
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
        private void CalculateGyroAngles(ref double[] analyzedX, ref double[] analyzedY, ref double[] analyzedZ, ref double[] resultante, ref double[] angleX, ref double[] angleY, ref double[] angleZ, ref double[] pitch, ref double[] roll)
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


        private void ApplyLowPassFilter(ref double[] data, double sampleRate)
        {
            // Appliquer le filtrage passe-bas FIR avant la FFT
            if (chkLowPassFilter.Checked)
            {
                int count = data.Count();
                LowPassFilter filter = new LowPassFilter(Convert.ToDouble(txtFilter.Text), sampleRate); // 100 Hz cutoff, 1000 Hz sample rate
                if (chkZeroPhase.Checked)
                    data = LowPassFilter.ApplyZeroPhase(data, filter);
                else
                {
                    for (int i = 0; i < count; i++)
                        data[i] = filter.Process(data[i]);
                }
            }
        }


        

        private double[] InterpolateNaNsCircular(double[] values)
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
        private static List<int> FindPeaks(IList<double> values, int range, double threshold)
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
        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Title = "Select CSV file";
                dlg.CheckFileExists = true;
                dlg.DefaultExt = "*.csv";
                dlg.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    AnalyzeCSV(dlg.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedSections != null)
                {
                    int nIndex = Convert.ToInt32(lblRecordNumber.Text);
                    var section = selectedSections[nIndex - 1];
                    if (section != null)
                    {
                        RefreshXYZ(section);
                        lblRecordNumber.Text = (nIndex - 1).ToString();
                    }
                }
            }
            catch
            { }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedSections != null)
                {
                    int nIndex = Convert.ToInt32(lblRecordNumber.Text);
                    var section = selectedSections[nIndex + 1];
                    if (section != null)
                    {
                        RefreshXYZ(section);
                        lblRecordNumber.Text = (nIndex + 1).ToString();
                    }
                }
            }
            catch
            { }
        }
        private void chkShowResultante_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateAnalysis();
            }
            catch
            { }
        }

        private void chkShowX_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateAnalysis();
            }
            catch
            { }
        }

        private void chkShowY_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateAnalysis();
            }
            catch
            { }
        }

        private void chkShowZ_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateAnalysis();
            }
            catch
            { }
        }



        private void chkFFT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateAnalysis();
            }
            catch
            { }
        }

        private void cbxFFT_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateAnalysis();
            }
            catch
            { }
        }

        private void chkAbsolute_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateAnalysis();
            }
            catch
            { }
        }
        private void chkRemoveDC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateAnalysis();
            }
            catch
            { }
        }

        private void chkSum_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateAnalysis();
            }
            catch
            { }
        }

        private void chkLowPassFilter_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateAnalysis();
            }
            catch
            { }
        }

        private void chkZeroPhase_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateAnalysis();
            }
            catch
            { }
        }

        private void chkOrderTracking_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateAnalysis();
            }
            catch
            { }
        }

        private void UpdateAnalysis()
        {
            if (String.IsNullOrEmpty(sLastCSV)) return;
            int nIndex = Convert.ToInt32(lblRecordNumber.Text);
            Analyze(sLastCSV);

            var section = loadedSections[nIndex];
            if (section != null)
            {
                lblRecordNumber.Text = nIndex.ToString();
                RefreshXYZ(section);
            }
        }

        private void btnStartCapture_Click(object sender, EventArgs e)
        {
            dataToSave = new List<section>(200);
            minRPM = Convert.ToInt32(txtMinRPM.Text);
            maxRPM = Convert.ToInt32(txtMaxRPM.Text);
            saveData = true;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstSectionSelector.Items.Count; i++)
            {
                lstSectionSelector.SetItemChecked(i, true);
            }
        }

        private void btnUnselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstSectionSelector.Items.Count; i++)
            {
                lstSectionSelector.SetItemChecked(i, false);
            }
        }

        private void btnUpdateAnalysisSection_Click(object sender, EventArgs e)
        {
            Analyze(sLastCSV);
        }

        private void btnExportWAV_Click(object sender, EventArgs e)
        {
            
            var dataY = new List<double>();
            foreach (int s in lstSectionSelector.CheckedIndices)
            {
                var sec = loadedSections[s];
                for (int i = 0; i < sec.records.Count; i++)
                {
                    dataY.Add(sec.records[i].y);
                }
            }
            
            String executablePath = Assembly.GetExecutingAssembly().Location;
            String wavFile = Path.Combine(Path.GetDirectoryName(executablePath), DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".wav");
            EquilibrageHelper.SaveWav(wavFile, dataY.ToArray(), Convert.ToInt32(txtSampleRate.Text));
        }

        private static List<PeakInfo> GetTopCommonPeaksWithAmplitude(
             int[][] samples,       // pics détectés par tour
             double[] data,         // tableau des amplitudes (360 * tourNumber)
             double tol = 10.0,
             int minSamples = 2,
             int topN = 5)
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
       

        private void btn200210_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstSectionSelector.Items.Count; i++)
            {
                SectionInfo item = lstSectionSelector.Items[i] as SectionInfo;
                if (item.rpm >= 200 && item.rpm <= 210)
                    lstSectionSelector.SetItemChecked(i, true);
            }
                    
        }

        private void btn210220_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstSectionSelector.Items.Count; i++)
            {
                SectionInfo item = lstSectionSelector.Items[i] as SectionInfo;
                if (item.rpm >= 210 && item.rpm <= 220)
                    lstSectionSelector.SetItemChecked(i, true);
            }
        }

        private void btn220230_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstSectionSelector.Items.Count; i++)
            {
                SectionInfo item = lstSectionSelector.Items[i] as SectionInfo;
                if (item.rpm >= 220 && item.rpm <= 230)
                    lstSectionSelector.SetItemChecked(i, true);
            }
        }

        private void btn230240_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstSectionSelector.Items.Count; i++)
            {
                SectionInfo item = lstSectionSelector.Items[i] as SectionInfo;
                if (item.rpm >= 230 && item.rpm <= 240)
                    lstSectionSelector.SetItemChecked(i, true);
            }
        }

        private void btn240250_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstSectionSelector.Items.Count; i++)
            {
                SectionInfo item = lstSectionSelector.Items[i] as SectionInfo;
                if (item.rpm >= 240 && item.rpm <= 250)
                    lstSectionSelector.SetItemChecked(i, true);
            }
        }

        private void formsPlotAnalysis_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender == null) return;

            FormsPlot plt = (sender as FormsPlot);
            var plottable = plt.Plot.GetPlottables();
            if (plottable.Length == 0) return;
            // determine point nearest the cursor
            (double mouseCoordX, double mouseCoordY) = plt.GetMouseCoordinates();
            String sData = String.Empty;
            foreach(var p in plottable)
            {
                if(p is ScottPlot.Plottable.ScatterPlot)  //for each scatterplot check if mouse is on a plot
                {
                    var sp = p as ScottPlot.Plottable.ScatterPlot;
                    if(sp.IsVisible)
                    {
                        (double pointX, double pointY, int pointIndex) = sp.GetPointNearestX(mouseCoordX);
                        //find Y at scatterplot hidden having name ANGLE + label
                        foreach (var p2 in plottable)
                        {
                            if (p2 is ScottPlot.Plottable.ScatterPlot)  //for each scatterplot check if mouse is on a plot
                            {
                                var sp2 = p2 as ScottPlot.Plottable.ScatterPlot;
                                if (sp2.Label == "ANGLE" + sp.Label)
                                {
                                    sData += $"{sp.Label} Freq: {pointX} Angle: {sp2.Ys[pointIndex]}\r\n";
                                }
                            }
                        }
                    }
                }
            }
            toolTip1.Show(sData, plt, e.Location, 3000);
        }


        private void btn250300_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstSectionSelector.Items.Count; i++)
            {
                SectionInfo item = lstSectionSelector.Items[i] as SectionInfo;
                if (item.rpm >= 250)
                    lstSectionSelector.SetItemChecked(i, true);
            }
        }
    
    }
    [Delimiter(",")]
    public class xyz
    {
        [Name("White")]
        public bool isWhite { get; set; }
        [Name("Timestamp")]
        public int ms { get; set; }
        [Name("X")]
        public double x { get; set; }
        [Name("Y")]
        public double y { get; set; }
        [Name("Z")]
        public double z { get; set; }
        [Name("GX")]
        public double gx { get; set; }
        [Name("GY")]
        public double gy { get; set; }
        [Name("GZ")]
        public double gz { get; set; }

    }
    public class SectionInfo
    {
        public double rpm;
        public double sampleRate;
        public int index;
        public override string ToString()
        {
            return "RPM: " + rpm.ToString() + " SampleRate: " + sampleRate.ToString();
        }
    }


    public class PeakInfo
    {
        public double Mean;
        public int Freq;
        public double SumAmplitude;
        public double AverageAmplitude => Freq == 0 ? 0 : SumAmplitude / Freq;
    }
    public class section
    {
        public int Size
        {
            get { return records.Count; }
        }
        public double SamplingRate
        {
            get
            {
                int startTS = records[0].ms;
                int endTS = records[records.Count-1].ms;
                if (endTS < startTS)
                    endTS += (ushort.MaxValue);
                startTS *= 64; //64 us
                endTS *= 64;
                double duration = (endTS - startTS)/1000.0; //ms
                return ((records.Count * 1000.0) / duration);
            }
        }
        public List<xyz> records = new List<xyz>();
    }
 

}
