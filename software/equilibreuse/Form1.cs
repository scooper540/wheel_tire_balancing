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
using MathNet.Numerics.Statistics;
using ScottPlot.WinForms;

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
        private AnalysisData currentAnalysisX, currentAnalysisY;
        private List<double> lstAngleXAnalysis = new List<double>();
        private List<double> lstAngleYAnalysis = new List<double>();
        public Form1()
        {
            InitializeComponent();
            t1.Tick += T1_Tick;
            t1.Interval = 1000;
            cbxFFT.SelectedItem = "BlackmanNuttal";
            cbxFFTSingle.SelectedItem = "BlackmanNuttal";
            cbxFilterTypes.SelectedItem = "Chebyshev-II";
            cbxSensor.SelectedIndex = 0; //mpu per default
            Help.FillHelp(richTextBox1);
            btnClearAnalysisHistory_Click(null, EventArgs.Empty);

            txtXMagGrams.Text = Properties.Settings.Default.XGrams.ToString();
            txtYMagGrams.Text = Properties.Settings.Default.YGrams.ToString();
            txtXMagBalanced.Text = Properties.Settings.Default.XMagTarget.ToString();
            txtYMagBalanced.Text = Properties.Settings.Default.YMagTarget.ToString();
            txtXMagExt.Text = Properties.Settings.Default.XMagInitial.ToString();
            txtYMagExt.Text = Properties.Settings.Default.YMagInitial.ToString();
            txtXMagInt.Text = Properties.Settings.Default.XMagFinal.ToString();
            txtYMagInt.Text = Properties.Settings.Default.YMagFinal.ToString();
            txtCorrectAngleX.Text = Properties.Settings.Default.XAngleCorrect.ToString();
            txtCorrectAngleY.Text = Properties.Settings.Default.YAngleCorrect.ToString();
            txtCorrectXTemporal.Text = Properties.Settings.Default.XAngleCorrectTemporal.ToString();
            txtCorrectYTemporal.Text = Properties.Settings.Default.YAngleCorrectTemporal.ToString();
            chkUseXGyro.Checked = Properties.Settings.Default.UseXGyro;
            chkUseYGyro.Checked = Properties.Settings.Default.UseYGyro;
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
                                        //new section
                                        if (s != null)
                                        {
                                            rpm = (float)s.Rpm;
                                        }
                                        s = new section();
                                        s.records = new List<xyz>(360);
                                        bNewSection = true;
                                        count = 0;
                                    }
                                    isWhite = true;
                                }
                                else
                                    isWhite = false;
                                avgTemp = calcXYZ(ref bufDataRemaining, pos);
                                pos += 15;
                                if (!bNewSection && (avgTemp.x == lastX && avgTemp.y == lastY && avgTemp.z == lastZ)) //duplicate, skip only if not white line
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
                                    if (s != null)
                                        s.records.Add(avgTemp);
                                    if (saveData && (rpm > minRPM && rpm < maxRPM))
                                    {
                                        if (bNewSection)
                                        {
                                            dataToSave.Add(s);
                                        }
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
            UInt16 timestamp = (UInt16)((buff[pos + 1] << 8) | (buff[pos]));
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
                data.x = (double)((x) * 0.061*4 * G / 1000.0);
                data.y = (double)((y) * 0.061 * 4 * G / 1000.0);
                data.z = (double)((z) * 0.061 * 4 * G / 1000.0);

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

        private void ApplyFilters(double sampleRate, double f_rot, ref double[] x, ref double[] y, ref double[] z, ref double[] resultante)
        {
            int filterOrder = Convert.ToInt32(txtFilterOrder.Text);
            if (chkLowPassFilter.Checked || chkPassband.Checked)
            {
                if (chkLowPassFilter.Checked)
                {
                    var l = new LowPassFilter(Convert.ToDouble(txtFilter.Text), sampleRate);
                    x = LowPassFilter.ApplyZeroPhase(x, l);
                    l = new LowPassFilter(Convert.ToDouble(txtFilter.Text), sampleRate);
                    y = LowPassFilter.ApplyZeroPhase(y, l);
                    l = new LowPassFilter(Convert.ToDouble(txtFilter.Text), sampleRate);
                    z = LowPassFilter.ApplyZeroPhase(z, l);
                    l = new LowPassFilter(Convert.ToDouble(txtFilter.Text), sampleRate);
                    resultante = LowPassFilter.ApplyZeroPhase(resultante, l);

                    /*x = LowPassFilter.ApplyLowPassFilterZeroPhase(x, Convert.ToDouble(txtFilter.Text), sampleRate, filterOrder);
                    y = LowPassFilter.ApplyLowPassFilterZeroPhase(y, Convert.ToDouble(txtFilter.Text), sampleRate, filterOrder);
                    z = LowPassFilter.ApplyLowPassFilterZeroPhase(z, Convert.ToDouble(txtFilter.Text), sampleRate, filterOrder);
                    resultante = LowPassFilter.ApplyLowPassFilterZeroPhase(resultante, Convert.ToDouble(txtFilter.Text), sampleRate, filterOrder);
                    */
                }

                if (chkPassband.Checked)
                {
                    x = LowPassFilter.ApplyNarrowBandPassFilter(x, f_rot, sampleRate, filterOrder);
                    y = LowPassFilter.ApplyNarrowBandPassFilter(y, f_rot, sampleRate, filterOrder);
                    z = LowPassFilter.ApplyNarrowBandPassFilter(z, f_rot, sampleRate, filterOrder);
                    resultante = LowPassFilter.ApplyNarrowBandPassFilter(resultante, f_rot, sampleRate, filterOrder);
                }
            }
            else
            {
                if (cbxFilterTypes.SelectedItem.ToString() != "None")
                {
                    var xFilter = LowPassFilter.ApplyFilter(x, sampleRate, f_rot, filterOrder, cbxFilterTypes.SelectedItem.ToString(), Convert.ToDouble(txtFilter.Text));
                    x = xFilter.Samples.Select(d => (double)d).ToArray();
                    var yFilter = LowPassFilter.ApplyFilter(y, sampleRate, f_rot, filterOrder, cbxFilterTypes.SelectedItem.ToString(), Convert.ToDouble(txtFilter.Text));
                    y = yFilter.Samples.Select(d => (double)d).ToArray();
                    var zFilter = LowPassFilter.ApplyFilter(z, sampleRate, f_rot, filterOrder, cbxFilterTypes.SelectedItem.ToString(), Convert.ToDouble(txtFilter.Text));
                    z = zFilter.Samples.Select(d => (double)d).ToArray();
                    var resultanteFilter = LowPassFilter.ApplyFilter(resultante, sampleRate, f_rot, filterOrder, cbxFilterTypes.SelectedItem.ToString(), Convert.ToDouble(txtFilter.Text));
                    resultante = resultanteFilter.Samples.Select(d => (double)d).ToArray();
                }
                if (cbxSmoothing.SelectedItem.ToString() != "None")
                {
                    if (cbxSmoothing.SelectedItem.ToString() == "IQ")
                    {
                        x = LowPassFilter.ComputePhaseIQ(x, sampleRate, f_rot);
                        y = LowPassFilter.ComputePhaseIQ(y, sampleRate, f_rot);
                        z = LowPassFilter.ComputePhaseIQ(z, sampleRate, f_rot);
                        resultante = LowPassFilter.ComputePhaseIQ(resultante, sampleRate, f_rot);
                    }
                    else
                    {
                        var xFilter = LowPassFilter.ApplyFilter(x, sampleRate, f_rot, filterOrder, cbxFilterTypes.SelectedItem.ToString(), Convert.ToDouble(txtFilter.Text));
                        x = xFilter.Samples.Select(d => (double)d).ToArray();
                        var yFilter = LowPassFilter.ApplyFilter(y, sampleRate, f_rot, filterOrder, cbxFilterTypes.SelectedItem.ToString(), Convert.ToDouble(txtFilter.Text));
                        y = yFilter.Samples.Select(d => (double)d).ToArray();
                        var zFilter = LowPassFilter.ApplyFilter(z, sampleRate, f_rot, filterOrder, cbxFilterTypes.SelectedItem.ToString(), Convert.ToDouble(txtFilter.Text));
                        z = zFilter.Samples.Select(d => (double)d).ToArray();
                        var resultanteFilter = LowPassFilter.ApplyFilter(resultante, sampleRate, f_rot, filterOrder, cbxFilterTypes.SelectedItem.ToString(), Convert.ToDouble(txtFilter.Text));
                        resultante = resultanteFilter.Samples.Select(d => (double)d).ToArray();
                    }
                }
            }

            
              if(chkRemoveDC.Checked)
              {
                  x = LowPassFilter.RemoveDCOffset(x);
                  y = LowPassFilter.RemoveDCOffset(y);
                  z = LowPassFilter.RemoveDCOffset(z);
                  resultante = LowPassFilter.RemoveDCOffset(resultante);
              }

            //apply gain on signal
            var gain = Convert.ToDouble(txtGain.Text);
            x = x.Select(r => Math.Sign(r) * Math.Pow(Math.Abs(r), gain)).ToArray();
            y = y.Select(r => Math.Sign(r) * Math.Pow(Math.Abs(r), gain)).ToArray();
            z = z.Select(r => Math.Sign(r) * Math.Pow(Math.Abs(r), gain)).ToArray();
            if (resultante == null || resultante.Length == 0)
                return;
            resultante = resultante.Select(r => Math.Sign(r) * Math.Pow(Math.Abs(r), gain)).ToArray();
        }

        private (double[] x, double[] y, double[] z, double[] resultante, double[] angle, double f_rot, double rpm, double sampleRate) GetSingleTourSignal(section s)
        {
            double sampleRate = s.SamplingRate;
            double rpm = s.Rpm;
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
            for (int i = 0; i < count; i++)
            {
                resultante[i] = Math.Sqrt(Math.Pow(x[i], 2)
                                            + Math.Pow(y[i], 2)
                                            );
            }
            double avgTourTime = count / sampleRate;
            double f_rot = 1.0 / avgTourTime;
            double[] angle = new double[count];
            double anglePerCount = 360.0 / count;
            for (int i = 0; i < count; i++)
                angle[i] = i * anglePerCount;
            return (x, y, z, resultante, angle,f_rot, rpm, sampleRate);
        }
        private (double[] x, double[] y, double[] z, double[] resultante, double[] angle, double f_rot, double rpm, double sampleRate) GetCompiledTourSignal()
        {
            // analysis of all segments in one graph aligned on the max size of segment
            int alignedCount = selectedSections.Select(x => x.records.Count).Max();
            //get avg samplerate
            double rpm = selectedSections.Select(x => x.Rpm).Min();
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

            int countX = analyzedX.Count();
            for (int i = 0; i < countX; i++)
            {
                resultante[i] = Math.Sqrt(Math.Pow(analyzedX[i], 2)
                                            + Math.Pow(analyzedY[i], 2)
                                            );
            }
            double avgTourTime = alignedCount / sampleRate;
            double f_rot = 1.0 / avgTourTime;
            //display the graph on 360° basis
            double[] angles = new double[alignedCount];
            double anglePerCount = 360.0 / alignedCount;
            for (int i = 0; i < alignedCount; i++)
                angles[i] = i * anglePerCount;

            return (analyzedX, analyzedY, analyzedZ, resultante, angles,f_rot, rpm, sampleRate);
        }
        // analysis of all consecutives segments
        private (double[] x, double[] y, double[] z, double[] resultante, double[] whiteLine, double[] angle, double f_rot, double rpm, double sampleRate) GetGlobalTourSignal()
        {
            int countTotal = iTotalRecordsInCSV; //number of total records for selected sections in the dataset
            int alignedCount = selectedSections.Select(x => x.records.Count).Max();
            double rpm = selectedSections.Select(x => x.Rpm).Min();
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
            double[] angle = new double[countTotal];


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
                        angle[iCount] = i*angleIncrement;
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
                    for (int i = 0; i < alignedCount; i++)
                        angle[i+ (tourNumber* alignedCount)] = i * angleIncrement;
                    iCount += alignedCount;

                    MathHelper.ResampleSectionAngularXYZ(se.records, alignedCount, 1.0 / sampleRate, analyzedX, analyzedY, analyzedZ, tourNumber * alignedCount);

                }


                tourNumber++;

            }
            int countX = analyzedX.Count();
            for (int i = 0; i < countX; i++)
            {
                resultante[i] = Math.Sqrt(Math.Pow(analyzedX[i], 2)
                                            + Math.Pow(analyzedY[i], 2)
                                            );
            }

            double avgTourTime = countTotal / sampleRate / selectedSections.Count;  // en secondes  
            double f_rot = 1.0 / avgTourTime;
            return (analyzedX, analyzedY, analyzedZ, resultante, whiteLine, angle,f_rot, rpm, sampleRate);
        }

        private void RefreshAnalysisCompiled()
        {
            var data = GetCompiledTourSignal();
            double f_rot = data.f_rot;
            double[] analyzedX = data.x;
            double[] analyzedY = data.y;
            double[] analyzedZ = data.z;
            double[] resultante = data.resultante;
            double[] angle = data.angle;
            double sampleRate = data.sampleRate;
            double rpm = data.rpm;
            ApplyFilters(sampleRate, f_rot, ref analyzedX, ref analyzedY, ref analyzedZ, ref resultante);

            lstPeakXCompiled.Items.Clear();
            lstPeakYCompiled.Items.Clear();
            lstPeakZCompiled.Items.Clear();
            lstPeakResultanteCompiled.Items.Clear();
            double fftLimit = Convert.ToDouble(txtFFTLimit.Text);

            formsPlotAnalysis.Reset();
            formsPlotAnalysis.Multiplot.Reset();
            var plotTemporal = formsPlotAnalysis.Plot;
            var plotFFT = formsPlotAnalysis.Multiplot.AddPlot();
            if (chkShowX.Checked)
                DisplayData(analyzedX, angle, f_rot, rpm, sampleRate, lstPeakXCompiled,plotTemporal, plotFFT, "X", Colors.Blue);
            if (chkShowY.Checked)
                DisplayData(analyzedY, angle, f_rot, rpm, sampleRate, lstPeakYCompiled,plotTemporal, plotFFT, "Y", Colors.Red);
            if (chkShowZ.Checked)
                DisplayData(analyzedZ, angle, f_rot, rpm, sampleRate, lstPeakZCompiled, plotTemporal, plotFFT, "Z", Colors.Yellow);
            if (chkShowResultante.Checked)
                DisplayData(resultante, angle, f_rot, rpm, sampleRate, lstPeakResultanteCompiled, plotTemporal, plotFFT, "Resultante", Colors.DeepPink);
            
            
            plotTemporal.Axes.AutoScale();
            plotTemporal.ShowLegend();

            plotFFT.Axes.AutoScale();
            plotFFT.ShowLegend();
            String sText = $"Fundamental: {f_rot}Hz\r\n1er order: {f_rot * 2}\r\n2eme order: {f_rot * 3}\r\n3eme order: {f_rot * 4}\r\n4er order: {f_rot * 5}\r\n5er order: {f_rot * 6}\r\n";
            plotFFT.Add.Annotation(sText);
            formsPlotAnalysis.Refresh();


        }
/*
        private void CalculateXYZ()
        {
            List<CalculationResult> lstCR = new List<CalculationResult>();
            List<double> pkpkX = new List<double>();
            List<double> pkpkY = new List<double>();
            List<double> pkpkZ = new List<double>();
            List<double> rmsX = new List<double>();
            List<double> rmsY = new List<double>();
            List<double> pkpkXInv = new List<double>();
            List<double> pkpkYInv = new List<double>();
            List<double> pkXTiming = new List<double>();
            List<double> pkYTiming = new List<double>();
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
                for (int i = 0; i < count; i++)
                {
                    resultante[i] = Math.Sqrt(Math.Pow(x[i], 2)
                                                + Math.Pow(y[i], 2)
                                                );
                }
                double avgTourTime = count / sampleRate;
                double f_rot = 1.0 / avgTourTime;
                ApplyFilters(sampleRate, f_rot, ref x, ref y, ref z, ref resultante);
                count = x.Length;
                pkpkX.Add(x.Max() - x.Min());
                pkpkY.Add(y.Max() - y.Min());
                pkpkZ.Add(z.Max() - z.Min());
                rmsX.Add(Statistics.RootMeanSquare(x));
                rmsY.Add(Statistics.RootMeanSquare(y));

                var maxValue = x.Max();
                int maxIndex = x.ToList().IndexOf(maxValue);
                //add timing of the max value
                pkXTiming.Add(maxIndex * 360.0 / count);
                maxValue = y.Max();
                maxIndex = y.ToList().IndexOf(maxValue);
                pkYTiming.Add(maxIndex * 360.0 / count);
                FFTData cmpX = EquilibrageHelper.CalculateFFT(x, sampleRate, cbxFFTSingle, chkDb.Checked, s.Rpm, f_rot);
                FFTData cmpY = EquilibrageHelper.CalculateFFT(y, sampleRate, cbxFFTSingle, chkDb.Checked, s.Rpm, f_rot);
                FFTData cmpZ = EquilibrageHelper.CalculateFFT(z, sampleRate, cbxFFTSingle, chkDb.Checked, s.Rpm, f_rot);
                FFTData cmpResultante = EquilibrageHelper.CalculateFFT(resultante, sampleRate, cbxFFTSingle, chkDb.Checked, s.Rpm, f_rot);

                pkpkXInv.Add(cmpX.SignalFFTInverse.Max() - cmpX.SignalFFTInverse.Min());
                pkpkYInv.Add(cmpY.SignalFFTInverse.Max() - cmpY.SignalFFTInverse.Min());


                var cs = EquilibrageHelper.CompleteSimulation(null, "turn by turn", cmpX, cmpY, cmpZ, cmpResultante, sampleRate, Convert.ToDouble(txtCorrectAngleX.Text), Convert.ToDouble(txtCorrectAngleY.Text), f_rot);
                lstCR.Add(cs);
            }
            //for the 5 orders, calculate the average and most possible value =

            //            lblPeak.Text = $"X Pk-Pk (turn by turn) : {pkpkX.Average()}\r\nY Pk-Pk (turn by turn) : {pkpkY.Average()}\r\nZ Pk-Pk (turn by turn) : {pkpkZ.Average()}\r\n";
            currentAnalysisX.ttPkPk = pkpkX.Average();
            currentAnalysisY.ttPkPk = pkpkY.Average();
            currentAnalysisX.ttPkPkInverse = pkpkXInv.Average();
            currentAnalysisY.ttPkPkInverse = pkpkYInv.Average();
            currentAnalysisX.ttRMS = rmsX.Average();
            currentAnalysisY.ttRMS = rmsY.Average();
            currentAnalysisX.ttPkTiming = pkXTiming.Average();
            currentAnalysisY.ttPkTiming = pkYTiming.Average();

            for (int i = 0; i < 5; i++)
            {
                Dictionary<int, int> lstBestAngleX = new Dictionary<int, int>(), lstBestAngleY = new Dictionary<int, int>(), lstBestAngleZ = new Dictionary<int, int>(), lstBestAngleRes = new Dictionary<int, int>(), lstBestAngleInner = new Dictionary<int, int>(), lstBestAngleOuter = new Dictionary<int, int>();
                List<Tuple<double, double>> lstMagnitudeAngles = new List<Tuple<double, double>>();
                List<Tuple<double, double>> lstMagnitudeX = new List<Tuple<double, double>>();
                List<Tuple<double, double>> lstMagnitudeY = new List<Tuple<double, double>>();
                List<Tuple<double, double>> lstMagnitudeZ = new List<Tuple<double, double>>();
                List<Tuple<double, double>> lstMagnitudeRes = new List<Tuple<double, double>>();
                foreach (var cr in lstCR)
                {
                    int key = (int)cr.px[i].UnbalanceAngleDeg;
                    if (lstBestAngleX.ContainsKey(key))
                        lstBestAngleX[key]++;
                    else
                        lstBestAngleX.Add((int)cr.px[i].UnbalanceAngleDeg, 1);
                    key = (int)cr.py[i].UnbalanceAngleDeg;
                    if (lstBestAngleY.ContainsKey(key))
                        lstBestAngleY[key]++;
                    else
                        lstBestAngleY.Add((int)cr.py[i].UnbalanceAngleDeg, 1);
                    key = (int)cr.pz[i].UnbalanceAngleDeg;
                    if (lstBestAngleZ.ContainsKey(key))
                        lstBestAngleZ[key]++;
                    else
                        lstBestAngleZ.Add((int)cr.pz[i].UnbalanceAngleDeg, 1);
                    key = (int)cr.pResultante[i].UnbalanceAngleDeg;
                    if (lstBestAngleRes.ContainsKey(key))
                        lstBestAngleRes[key]++;
                    else
                        lstBestAngleRes.Add((int)cr.pResultante[i].UnbalanceAngleDeg, 1);

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
                double mean = 0, coeffVariation = 0, variance = 0, standardDeviation = 0;
                MathHelper.CalculateStatistics("Angle Inner", i, lstBestAngleInner, ref mean, ref coeffVariation, ref variance, ref standardDeviation, lstSimulationTurnByTurn);
                if (i == 0 && mean != double.NaN)
                {
                    formsPlotT1I.Plot.Add.VerticalLine(mean, color: Colors.Black, width: 3);
                    currentAnalysisX.ttAngleDynamicSimple = mean;

                }
                MathHelper.CalculateStatistics("Angle Outer", i, lstBestAngleOuter, ref mean, ref coeffVariation, ref variance, ref standardDeviation, lstSimulationTurnByTurn);
                if (i == 0 && mean != double.NaN)
                {
                    formsPlotT1O.Plot.Add.VerticalLine(mean, color: Colors.Black, width: 3);
                    currentAnalysisY.ttAngleDynamicSimple = mean;
                }
                MathHelper.CalculateStatistics("X", i, lstBestAngleX, ref mean, ref coeffVariation, ref variance, ref standardDeviation, lstSimulationTurnByTurn);
                if (i == 0 && mean != double.NaN)
                {
                    formsPlotT1X.Plot.Add.VerticalLine(mean, color: Colors.Black, width: 3);
                    currentAnalysisX.ttAngle = mean;
                }
                MathHelper.CalculateStatistics("Y", i, lstBestAngleY, ref mean, ref coeffVariation, ref variance, ref standardDeviation, lstSimulationTurnByTurn);
                if (i == 0 && mean != double.NaN)
                {
                    formsPlotT1Y.Plot.Add.VerticalLine(mean, color: Colors.Black, width: 3);
                    currentAnalysisY.ttAngle = mean;
                }
                MathHelper.CalculateStatistics("Z", i, lstBestAngleZ, ref mean, ref coeffVariation, ref variance, ref standardDeviation, lstSimulationTurnByTurn);
                MathHelper.CalculateStatistics("Resultante", i, lstBestAngleRes, ref mean, ref coeffVariation, ref variance, ref standardDeviation, lstSimulationTurnByTurn);



                if (i == 0)
                {
                    MathHelper.DisplayTurnByTurnGraph(lstBestAngleInner, lstBestAngleOuter, lstBestAngleX, lstBestAngleY, formsPlotT1I, formsPlotT1O, formsPlotT1X, formsPlotT1Y);

                    //    lblFFTAnalysis.Text += "Turn by turn X AVG Mag: " + lstMagnitudeX.Average(t => t.Item2).ToString("F4") + "\r\n";
                    //    lblFFTAnalysis.Text += "Turn by turn Y AVG Mag: " + lstMagnitudeY.Average(t => t.Item2).ToString("F4") + "\r\n";
                    currentAnalysisX.ttMagAvg = lstMagnitudeX.Average(t => t.Item2);
                    currentAnalysisY.ttMagAvg = lstMagnitudeY.Average(t => t.Item2);

                    /* var k = EquilibrageHelper.CalculateAttenuationConstant(Convert.ToDouble(txtXMagExt.Text), Convert.ToDouble(txtXMagXInt.Text), Convert.ToDouble(txtXMagGrams.Text));
                     currentAnalysisX.ttWeight = EquilibrageHelper.CalculateRequiredMass(k, currentAnalysisX.ttMagAvg, Convert.ToDouble(txtXMagBalanced.Text));

                     k = EquilibrageHelper.CalculateAttenuationConstant(Convert.ToDouble(txtYMagExt.Text), Convert.ToDouble(txtYMagInt.Text), Convert.ToDouble(txtYMagGrams.Text));
                     currentAnalysisY.ttWeight = EquilibrageHelper.CalculateRequiredMass(k, currentAnalysisY.ttMagAvg, Convert.ToDouble(txtYMagBalanced.Text));
                     
                    var result = EquilibrageHelper.CalculateAttenuationConstantsXY(Convert.ToDouble(txtXMagGrams.Text) / 1000.0,
                                            Convert.ToDouble(txtXMagBalanced.Text),
                                            Convert.ToDouble(txtYMagBalanced.Text),
                                            Convert.ToDouble(txtXMagExt.Text),
                                            Convert.ToDouble(txtYMagExt.Text),
                                            Convert.ToDouble(txtYMagGrams.Text) / 1000.0,
                                            Convert.ToDouble(txtXMagInt.Text),
                                            Convert.ToDouble(txtYMagInt.Text));
                    var res = EquilibrageHelper.EstimateMassCorrection(currentAnalysisX.ttMagAvg, (currentAnalysisX.ttAngle + 180) % 360, currentAnalysisY.ttMagAvg, (currentAnalysisY.ttAngle + 180) % 360, result.KextX, result.KextY, result.KintX, result.KintY);
                    currentAnalysisX.ttAngleDynamicComplex = res.AngleIntDeg;
                    currentAnalysisY.ttAngleDynamicComplex = res.AngleExtDeg;
                    currentAnalysisX.ttWeight = res.MassInt;
                    currentAnalysisY.ttWeight = res.MassExt;
                }

            }

        }
*/
        private void RefreshXYZ(section s)
        {
            var data = GetSingleTourSignal(s);
            double f_rot = data.f_rot;
            double[] analyzedX = data.x;
            double[] analyzedY = data.y;
            double[] analyzedZ = data.z;
            double[] resultante = data.resultante;
            double[] angle = data.angle;
            double sampleRate = data.sampleRate;
            double rpm = data.rpm;
            ApplyFilters(sampleRate, f_rot, ref analyzedX, ref analyzedY, ref analyzedZ, ref resultante);

            formsPlotX.Reset();
            formsPlotX.Multiplot.Reset();
            var plotTemporalX = formsPlotX.Plot;
            var plotFFTX = formsPlotX.Multiplot.AddPlot();

            formsPlotY.Reset();
            formsPlotY.Multiplot.Reset();
            var plotTemporalY = formsPlotY.Plot;
            var plotFFTY = formsPlotY.Multiplot.AddPlot();

            formsPlotZ.Reset();
            formsPlotZ.Multiplot.Reset();
            var plotTemporalZ = formsPlotZ.Plot;
            var plotFFTZ = formsPlotZ.Multiplot.AddPlot();

            lstPeakX.Items.Clear();
            lstPeakY.Items.Clear();
            lstPeakZ.Items.Clear();
            double fftLimit = Convert.ToDouble(txtFFTLimit.Text);

            if (chkShowX.Checked)
                DisplayData(analyzedX, angle, f_rot, rpm, sampleRate, lstPeakX, plotTemporalX, plotFFTX, "X", Colors.Blue);
            if (chkShowY.Checked)
                DisplayData(analyzedY, angle, f_rot, rpm, sampleRate, lstPeakY, plotTemporalY, plotFFTY, "Y", Colors.Red);
            if (chkShowZ.Checked)
                DisplayData(analyzedZ, angle, f_rot, rpm, sampleRate, lstPeakZ, plotTemporalZ, plotFFTZ, "Z", Colors.Yellow);

            plotTemporalX.Axes.AutoScale();
            plotTemporalX.ShowLegend();
            plotFFTX.Axes.AutoScale();
            plotFFTX.ShowLegend();
            String sText = $"Fundamental: {f_rot}Hz";
            plotFFTX.Add.Annotation(sText);

            
            plotTemporalY.Axes.AutoScale();
            plotTemporalY.ShowLegend();
            plotFFTY.Axes.AutoScale();
            plotFFTY.ShowLegend();
            plotFFTY.Add.Annotation(sText);

            
            plotTemporalZ.Axes.AutoScale();
            plotTemporalZ.ShowLegend();
            plotFFTZ.Axes.AutoScale();
            plotFFTZ.ShowLegend();
            plotFFTZ.Add.Annotation(sText);


            formsPlotX.Refresh();
            formsPlotY.Refresh();
            formsPlotZ.Refresh();
        }
        
        // analysis of all consecutives segments
        private void RefreshAnalysisGlobal()
        {
            var data = GetGlobalTourSignal();
            double f_rot = data.f_rot;
            double[] resultante = data.resultante;
            double rpm = data.rpm;
            double sampleRate = data.sampleRate;
            double[] whiteLine = data.whiteLine;
            double[] analyzedX = data.x;
            double[] analyzedY = data.y;
            double[] analyzedZ = data.z;
            double[] angle = data.angle;
            ApplyFilters(sampleRate, f_rot, ref analyzedX, ref analyzedY, ref analyzedZ, ref resultante);


            lstPeakGlobalX.Items.Clear();
            lstPeakGlobalY.Items.Clear();
            lstPeakGlobalZ.Items.Clear();
            lstPeakResultanteGlobal.Items.Clear();
            double fftLimit = Convert.ToDouble(txtFFTLimit.Text);

            formsPlotGlobal.Reset();
            formsPlotGlobal.Multiplot.Reset();
            var plotTemporal = formsPlotGlobal.Plot;
            var plotFFT = formsPlotGlobal.Multiplot.AddPlot();
            double max = 0.001;
            if (chkShowX.Checked)
            {
                DisplayData(analyzedX, angle, f_rot, rpm, sampleRate, lstPeakGlobalX, plotTemporal, plotFFT, "X", Colors.Blue);
                max = Math.Max(max, analyzedX.Max());
            }
            if (chkShowY.Checked)
            {
                DisplayData(analyzedY, angle, f_rot, rpm, sampleRate, lstPeakGlobalY, plotTemporal, plotFFT, "Y", Colors.Red);
                max = Math.Max(max, analyzedY.Max());
            }
            if (chkShowZ.Checked)
            {
                DisplayData(analyzedZ, angle, f_rot, rpm, sampleRate, lstPeakGlobalZ, plotTemporal, plotFFT, "Z", Colors.Yellow);
                max = Math.Max(max, analyzedZ.Max());
            }
            if (chkShowResultante.Checked)
            {
                DisplayData(resultante, angle, f_rot, rpm, sampleRate, lstPeakResultanteCompiled, plotTemporal, plotFFT, "Resultante", Colors.DeepPink);
                max = Math.Max(max, resultante.Max());
            }
            max = max * 2;
            for (int i = 0; i < whiteLine.Length; i++)
            {
                if (whiteLine[i] == 10)
                    whiteLine[i] = max;
            }
            double[] temporal = Enumerable.Range(0, whiteLine.Length)
                                 .Select(i => (double)i)
                                 .ToArray();

            plotTemporal.Add.Scatter(temporal, whiteLine, Colors.Black).LegendText = "WhiteLine";
            //formsPlotAnalysis.Plot.Axis(0, 360, -1, 1);
            plotTemporal.Axes.SetLimitsX(0, angle.Length);
            plotTemporal.Axes.AutoScaleY();
            plotTemporal.ShowLegend();

            plotFFT.Axes.AutoScale();
            plotFFT.ShowLegend();
            String sText = $"Fundamental: {f_rot}Hz\r\n1er order: {f_rot * 2}\r\n2eme order: {f_rot * 3}\r\n3eme order: {f_rot * 4}\r\n4er order: {f_rot * 5}\r\n5er order: {f_rot * 6}\r\n";
            plotFFT.Add.Annotation(sText);


            formsPlotGlobal.Refresh();



            /*

            //split filtered signal by removing first and last section and analyze one by one
            var dataX = MathHelper.GetSegments(analyzedX, whiteLine);
            var dataY = MathHelper.GetSegments(analyzedY, whiteLine);
            List<PhaseAnalysis> lstPhaseX = new List<PhaseAnalysis>();
            List<PhaseAnalysis> lstPhaseY = new List<PhaseAnalysis>();
            //analysis only the mid segments
            int skip = selectedSections.Count / 4;         // sauter les 25% premiers et derniers tours
            
            int use = selectedSections.Count/2;// selectedSections.Count - 2 * skip;
            int end = use + skip;   // analyser les 50% centrau
            analyzedX = dataX.dataSegments.Skip(skip).Take(use).SelectMany(arr => arr).ToArray();
            analyzedY = dataY.dataSegments.Skip(skip).Take(use).SelectMany(arr => arr).ToArray();
            for (int i = skip; i < end; i++) //skip 2 firsts segments
            {
                var signalX = dataX.dataSegments[i];
                var signalY = dataY.dataSegments[i];
                lstPhaseX.Add(EquilibrageHelper.AnalyzeSignal(signalX, sampleRate, f_rot, cbxFFT, chkDb, rpm));
                lstPhaseY.Add(EquilibrageHelper.AnalyzeSignal(signalY, sampleRate, f_rot, cbxFFT, chkDb, rpm));
            }
            
            var globalX = EquilibrageHelper.AnalyzeSignal(analyzedX, sampleRate, f_rot, cbxFFT, chkDb, rpm);
            var globalY = EquilibrageHelper.AnalyzeSignal(analyzedY, sampleRate, f_rot, cbxFFT, chkDb, rpm);
          
            //reconstruct whiteLine in case of modification in the filter
            whiteLine = dataX.dataSegments.SelectMany(signal =>
            {
                if (signal.Length == 0)
                    return new double[0];

                double[] modified = new double[signal.Length];
                modified[0] = 10;
                // les autres sont déjà à 0 par défaut
                return modified;
            }).ToArray();

            //find temporal peaks in globalX and Y
            int[] maxAmpIdxX = MathHelper.FindPeaks(globalX, 7, globalX.Average() * 0.02);

            int[] maxAmpIdxY = MathHelper.FindPeaks(globalY, 7, globalY.Average() * 0.02);

            Console.WriteLine($"Moyenne rMaxTemporal X G + TT: {globalX.rMaxTemporal} {ttX.rMaxTemporal}");
            Console.WriteLine($"Moyenne rPhaseLockIn X G + TT: {globalX.rPhaseLockIn} {ttX.rPhaseLockIn}");
            Console.WriteLine($"Moyenne rFitSinusoid X G + TT: {globalX.rFitSinusoid} {ttX.rFitSinusoid}");
            Console.WriteLine($"Moyenne rDetectPhase X G + TT: {globalX.rDetectPhase} {ttX.rDetectPhase}");
            Console.WriteLine($"Moyenne rFFT X G + TT: {globalX.rFFT} {ttX.rFFT}");

            Console.WriteLine($"Moyenne rMaxTemporal Y G + TT: {globalY.rMaxTemporal} {ttY.rMaxTemporal}");
            Console.WriteLine($"Moyenne rPhaseLockIn Y G + TT: {globalY.rPhaseLockIn} {ttY.rPhaseLockIn}");
            Console.WriteLine($"Moyenne rFitSinusoid Y G + TT: {globalY.rFitSinusoid} {ttY.rFitSinusoid}");
            Console.WriteLine($"Moyenne rDetectPhase Y G + TT: {globalY.rDetectPhase} {ttY.rDetectPhase}");
            Console.WriteLine($"Moyenne rFFT Y G + TT: {globalY.rFFT} {ttY.rFFT}");
            //            lblPeak.Text += $"\r\nX Pk-Pk (global) : {analyzedX.Max() - analyzedX.Min()}\r\nY Pk-Pk (global) : {analyzedY.Max() - analyzedY.Min()}\r\nZ Pk-Pk (global) : {analyzedZ.Max() - analyzedZ.Min()}";
            currentAnalysisX.gPkPk = analyzedX.Max() - analyzedX.Min();
            currentAnalysisY.gPkPk = analyzedY.Max() - analyzedY.Min();
            currentAnalysisX.gRMS = Statistics.RootMeanSquare(analyzedX);
            currentAnalysisY.gRMS = Statistics.RootMeanSquare(analyzedY);

            var tpx = MathHelper.GetTopCommonPeaksWithAmplitude(peakX, analyzedX, 10, 2, peakX.Count());
            MathHelper.ShowPeakHistogram(tpx, formsPlotAnalysisTemporalX);
            var tpy = MathHelper.GetTopCommonPeaksWithAmplitude(peakY, analyzedY, 10, 2, peakY.Count());
            MathHelper.ShowPeakHistogram(tpy, formsPlotAnalysisTemporalY);
            var tpz = MathHelper.GetTopCommonPeaksWithAmplitude(peakZ, analyzedZ, 10, 2, peakZ.Count());
            MathHelper.ShowPeakHistogram(tpz, formsPlotAnalysisTemporalZ);


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
                    formsPlotGlobal.Plot.Add.Scatter(temporal, analyzedX, Colors.Blue).LegendText = "X";
                    MathHelper.DisplayPeaksTemporal(analyzedX, temporal, "Top Peak X", formsPlotGlobal.Plot, lstPeakGlobalX);
                    max = analyzedX.Max();
                }
                if (chkShowY.Checked)
                {
                    formsPlotGlobal.Plot.Add.Scatter(temporal, analyzedY, Colors.Red).LegendText = "Y";
                    MathHelper.DisplayPeaksTemporal(analyzedY, temporal, "Top Peak Y", formsPlotGlobal.Plot, lstPeakGlobalY);
                    max = Math.Max(max, analyzedY.Max());
                }
                if (chkShowZ.Checked)
                {
                    formsPlotGlobal.Plot.Add.Scatter(temporal, analyzedZ, Colors.Yellow).LegendText = "Z";
                    MathHelper.DisplayPeaksTemporal(analyzedZ, temporal, "Top Peak Z", formsPlotGlobal.Plot, lstPeakGlobalZ);
                    max = Math.Max(max, analyzedZ.Max());
                }
                if (chkShowResultante.Checked)
                {
                    formsPlotGlobal.Plot.Add.Scatter(temporal, resultante, Colors.DeepPink).LegendText= "Resultante";
                    MathHelper.DisplayPeaksTemporal(resultante, temporal, "Resultante", formsPlotGlobal.Plot, lstPeakResultanteGlobal);
                    max = Math.Max(max, resultante.Max());
                }
                max = max * 2;
                for (int i = 0; i < whiteLine.Length; i++)
                {
                    if (whiteLine[i] == 10)
                        whiteLine[i] = max;
                }
                formsPlotGlobal.Plot.Add.Scatter(temporal, whiteLine, Colors.Black).LegendText ="WhiteLine";
                //formsPlotAnalysis.Plot.Axis(0, 360, -1, 1);
                formsPlotGlobal.Plot.Axes.SetLimitsX(0, countTotal);
                formsPlotGlobal.Plot.Axes.AutoScaleY();
                formsPlotGlobal.Plot.ShowLegend();
            }
            else
            {

                FFTData cmpX = EquilibrageHelper.CalculateFFT(analyzedX, sampleRate, cbxFFT, chkDb.Checked, rpm, f_rot);
                FFTData cmpY = EquilibrageHelper.CalculateFFT(analyzedY, sampleRate, cbxFFT, chkDb.Checked, rpm, f_rot);
                FFTData cmpZ = EquilibrageHelper.CalculateFFT(analyzedZ, sampleRate, cbxFFT, chkDb.Checked, rpm, f_rot);
                FFTData cmpResultante = EquilibrageHelper.CalculateFFT(resultante, sampleRate, cbxFFT, chkDb.Checked, rpm, f_rot);
                var fundX = EquilibrageHelper.GetFundamentalPhase(cmpX.Frequence, cmpX.Magnitude, cmpX.AngleDeg, f_rot);
                var fundY = EquilibrageHelper.GetFundamentalPhase(cmpY.Frequence, cmpY.Magnitude, cmpY.AngleDeg, f_rot);

                lstAnglesX.Items.Add("Angle FFT Global limited: " + (globalX.rFFT + Convert.ToDouble(txtCorrectAngleX.Text)) % 360);
                lstAnglesX.Items.Add("Angle FFT TurnTurn limited: " + (ttX.rFFT + Convert.ToDouble(txtCorrectAngleX.Text)) % 360);
                lstAnglesX.Items.Add("Angle FFT Global complete: " + (fundX.Angle + Convert.ToDouble(txtCorrectAngleX.Text)) % 360);
                lstAnglesY.Items.Add("Angle FFT Global limited: " + (globalY.rFFT + Convert.ToDouble(txtCorrectAngleY.Text)) % 360);
                lstAnglesY.Items.Add("Angle FFT TurnTurn limited: " + (ttY.rFFT + Convert.ToDouble(txtCorrectAngleY.Text)) % 360);
                lstAnglesY.Items.Add("Angle FFT Global complete: " + (fundY.Angle + Convert.ToDouble(txtCorrectAngleY.Text)) % 360);

                //check if values are in the same range
                var a1 = MathHelper.CalcAngleDistance(globalX.rFFT, ttX.rFFT);
                var a2 = MathHelper.CalcAngleDistance(globalX.rFFT, fundX.Angle);
                var a3 = MathHelper.CalcAngleDistance(ttX.rFFT, fundX.Angle);
                var t = new List<double>() { a1, a2, a3 };
                if (t.Max() < 45) // consider
                {
                    lstAngleXAnalysis.Add((globalX.rFFT + Convert.ToDouble(txtCorrectAngleX.Text)) % 360);
                    lstAngleXAnalysis.Add((ttX.rFFT + Convert.ToDouble(txtCorrectAngleX.Text)) % 360);
                    lstAngleXAnalysis.Add((fundX.Angle + Convert.ToDouble(txtCorrectAngleX.Text)) % 360);
                }

                a1 = MathHelper.CalcAngleDistance(globalY.rFFT, ttY.rFFT);
                a2 = MathHelper.CalcAngleDistance(globalY.rFFT, fundY.Angle);
                a3 = MathHelper.CalcAngleDistance(ttY.rFFT, fundY.Angle);
                t = new List<double>() { a1, a2, a3 };
                if (t.Max() - t.Min() > 45) // do not consider
                {
                    lstAngleYAnalysis.Add((globalY.rFFT + Convert.ToDouble(txtCorrectAngleY.Text)) % 360);
                    lstAngleYAnalysis.Add((ttY.rFFT + Convert.ToDouble(txtCorrectAngleY.Text)) % 360);
                    lstAngleYAnalysis.Add((fundY.Angle + Convert.ToDouble(txtCorrectAngleY.Text)) % 360);
                }
                formsPlotGlobal.Plot.Clear();
                lstPeakGlobalX.Items.Clear();
                lstPeakGlobalY.Items.Clear();
                lstPeakGlobalZ.Items.Clear();
                lstPeakResultanteGlobal.Items.Clear();
                double fftLimit = Convert.ToDouble(txtFFTLimit.Text);
                if (chkShowX.Checked)
                {
                    MathHelper.AnalyzeAxis("X", cmpX, sampleRate, lstPeakGlobalX, Colors.Blue, formsPlotGlobal.Plot, f_rot, fftLimit);
                }
                if (chkShowY.Checked)
                {
                    MathHelper.AnalyzeAxis("Y", cmpY, sampleRate, lstPeakGlobalY, Colors.Red, formsPlotGlobal.Plot, f_rot, fftLimit);
                }
                if (chkShowZ.Checked)
                {
                    MathHelper.AnalyzeAxis("Z", cmpZ, sampleRate, lstPeakGlobalZ, Colors.Yellow, formsPlotGlobal.Plot, f_rot, fftLimit);
                }
                if (chkShowResultante.Checked)
                {
                    MathHelper.AnalyzeAxis("Resultante", cmpResultante, sampleRate, lstPeakResultanteGlobal, Colors.DeepPink, formsPlotGlobal.Plot, f_rot, fftLimit);
                }
                String sText = $"Fundamental: {f_rot}Hz\r\n1er order: {f_rot * 2}\r\n2eme order: {f_rot * 3}\r\n3eme order: {f_rot * 4}\r\n4er order: {f_rot * 5}\r\n5er order: {f_rot * 6}\r\n";
                formsPlotGlobal.Plot.Add.Annotation(sText);
                lstSimulationGlobal.Items.Clear();
                
                var calcResult = EquilibrageHelper.CompleteSimulation(lstSimulationGlobal, "Global", cmpX, cmpY, cmpZ, cmpResultante, sampleRate, f_rot,Convert.ToDouble(txtCorrectAngleX.Text), Convert.ToDouble(txtCorrectAngleY.Text), 1);
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT1I.Plot.Add.VerticalLine(calcResult.dir[i].correction.AngleInnerDeg, color:Colors.Green, width: 3);
                            formsPlotT1O.Plot.Add.VerticalLine(calcResult.dir[i].correction.AngleOuterDeg, color:Colors.Green, width: 3);
                        }
                        formsPlotT1X.Plot.Add.VerticalLine(calcResult.px[i].UnbalanceAngleDeg, color:Colors.Green, width: 3);
                        formsPlotT1Y.Plot.Add.VerticalLine(calcResult.py[i].UnbalanceAngleDeg, color:Colors.Green, width: 3);
                    }
                   
                }


                // 5. PSD (densité spectrale de puissance)
                // PSD = |X(f)|^2 / (fs * N) ou / (fs * durée)
                // return $"{sTitle}: {magnitudePerSecond:F4}\r\n{magnitudePerRevolution:F4} {psd:F4}";
                double magnitude = calcResult.px[0].ActualAmplitude;
                double magPerTurn = magnitude / selectedSections.Count;
                double psd = (magnitude * magnitude) / analyzedX.Length;

                //lblFFTAnalysis.Text += "Global X Mag: " + magnitude.ToString("F4") + "\r\nBy turn count: " + magPerTurn.ToString("F4") + "\r\nPSD: " + psd.ToString("F4") + "\r\n";
                currentAnalysisX.gMagAvg = magnitude;
                currentAnalysisX.gMagRatio = magPerTurn;
                currentAnalysisX.gMagPSD = psd;
                currentAnalysisX.gPkPkInverse = (cmpX.SignalFFTInverse.Max() - cmpX.SignalFFTInverse.Min());
                currentAnalysisX.gPkPkInverseRatio = (cmpX.SignalFFTInverse.Max() - cmpX.SignalFFTInverse.Min()) / selectedSections.Count;
                currentAnalysisX.gAngle = calcResult.px[0].UnbalanceAngleDeg;

                magnitude = calcResult.py[0].ActualAmplitude;
                magPerTurn = magnitude / selectedSections.Count;
                psd = (magnitude * magnitude) / analyzedY.Length;

                //lblFFTAnalysis.Text += "Global Y Mag: " + magnitude.ToString("F4") + "\r\nBy turn count: " + magPerTurn.ToString("F4") + "\r\nPSD: " + psd.ToString("F4") + "\r\n";

                currentAnalysisY.gMagAvg = magnitude;
                currentAnalysisY.gMagRatio = magPerTurn;
                currentAnalysisY.gMagPSD = psd;
                currentAnalysisY.gPkPkInverse = (cmpY.SignalFFTInverse.Max() - cmpY.SignalFFTInverse.Min());
                currentAnalysisY.gPkPkInverseRatio = (cmpY.SignalFFTInverse.Max() - cmpY.SignalFFTInverse.Min()) / selectedSections.Count;
                currentAnalysisY.gAngle = calcResult.py[0].UnbalanceAngleDeg;

                var dx2 = EquilibrageHelper.ComputeLockInPhase(analyzedX, f_rot, sampleRate);
                var dy2 = EquilibrageHelper.ComputeLockInPhase(analyzedY, f_rot, sampleRate);
                var dx3 = EquilibrageHelper.ComputePhaseHilbert(analyzedX, sampleRate);
                var dy3 = EquilibrageHelper.ComputePhaseHilbert(analyzedY, sampleRate);
                var dx4 = EquilibrageHelper.FitSinusoidPhase(analyzedX, f_rot, sampleRate);
                var dy4 = EquilibrageHelper.FitSinusoidPhase(analyzedY, f_rot, sampleRate);
                var dx = EquilibrageHelper.DetectPhaseWithSweep(analyzedX, sampleRate, f_rot);
                var dy = EquilibrageHelper.DetectPhaseWithSweep(analyzedY, sampleRate, f_rot);
                currentAnalysisX.gAngleAlternatif = dx.phaseDegrees;
                currentAnalysisX.gMagRatio = dx.amplitude;
                currentAnalysisY.gAngleAlternatif = dy.phaseDegrees;
                currentAnalysisY.gMagRatio = dy.amplitude;
                if (calcResult.dir[0].IsDynamic)
                {
                    currentAnalysisX.gAngleDynamicSimple = calcResult.dir[0].correction.AngleInnerDeg;
                    currentAnalysisY.gAngleDynamicSimple = calcResult.dir[0].correction.AngleOuterDeg;
                }
                /*
                                var k = EquilibrageHelper.CalculateAttenuationConstant(Convert.ToDouble(txtXMagExt.Text), Convert.ToDouble(txtXMagInt.Text), Convert.ToDouble(txtXMagGrams.Text));
                                currentAnalysisX.gWeight = EquilibrageHelper.CalculateRequiredMass(k, currentAnalysisX.gMagRatio, Convert.ToDouble(txtXMagBalanced.Text));

                                k = EquilibrageHelper.CalculateAttenuationConstant(Convert.ToDouble(txtYMagExt.Text), Convert.ToDouble(txtYMagInt.Text), Convert.ToDouble(txtYMagGrams.Text));
                                currentAnalysisY.gWeight = EquilibrageHelper.CalculateRequiredMass(k, currentAnalysisY.gMagRatio, Convert.ToDouble(txtYMagBalanced.Text));
                  */

        }

        private void RefreshGyro()
        {
            if (String.IsNullOrEmpty(sLastCSV)) return;

            int countTotal = iTotalRecordsInCSV; //number of total records for selected sections in the dataset
            int alignedCount = selectedSections.Select(x => x.records.Count).Max();
            double rpm = selectedSections.Select(x => x.Rpm).Min();
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
                    //  for (int i = 0; i < alignedCount; i++)
                    //      angle[i+ (tourNumber* alignedCount)] = i * angleIncrement;
                    iCount += alignedCount;

                    MathHelper.ResampleSectionGyroXYZ(se.records, alignedCount, 1.0 / sampleRate, analyzedX, analyzedY, analyzedZ, tourNumber * alignedCount);

                }

                //convert the return x axis (0 to number of sample for 1 turn) to 360°
                peakX[tourNumber] = MathHelper.GetPeakPerTurn(analyzedX.Skip(startCount).Take(iCount - startCount).ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();
                peakY[tourNumber] = MathHelper.GetPeakPerTurn(analyzedY.Skip(startCount).Take(iCount - startCount).ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();
                peakZ[tourNumber] = MathHelper.GetPeakPerTurn(analyzedZ.Skip(startCount).Take(iCount - startCount).ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();


                tourNumber++;

            }
            int countX = analyzedX.Count();
            for (int i = 0; i < countX; i++)
            {
                resultante[i] = Math.Sqrt(Math.Pow(analyzedX[i], 2)
                                            + Math.Pow(analyzedY[i], 2)
                                            );
            }

            double avgTourTime = countTotal / sampleRate / selectedSections.Count;  // en secondes  
            double f_rot = 1.0 / avgTourTime;

            ApplyFilters(sampleRate, f_rot, ref analyzedX, ref analyzedY, ref analyzedZ, ref resultante);
            countTotal = analyzedX.Length;
            //CalculateGyroAngles(ref analyzedX, ref analyzedY, ref analyzedZ, ref resultante, ref angleX, ref angleY, ref angleZ, ref pitch, ref roll);

            //    lblPeak.Text += $"\r\nGyro X Pk-Pk (global) : {analyzedX.Max() - analyzedX.Min()}\r\nGyro Y Pk-Pk (global) : {analyzedY.Max() - analyzedY.Min()}\r\nGyro Z Pk-Pk (global) : {analyzedZ.Max() - analyzedZ.Min()}";

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

                var top5 = MathHelper.GetTopCommonPeaksWithAmplitude(peakX, analyzedX, tol: 10, minSamples: 2, topN: 5);
                foreach (var item in top5)
                    lstPeakGyroX.Items.Add($"Average PeakX: Angle {item.Mean} – Fréquence {item.Freq} - Force {item.AverageAmplitude}");
                top5 = MathHelper.GetTopCommonPeaksWithAmplitude(peakY, analyzedY, tol: 10, minSamples: 2, topN: 5);
                foreach (var item in top5)
                    lstPeakGyroY.Items.Add($"Average PeakY: Angle {item.Mean} – Fréquence {item.Freq} - Force {item.AverageAmplitude}");
                top5 = MathHelper.GetTopCommonPeaksWithAmplitude(peakZ, analyzedZ, tol: 10, minSamples: 2, topN: 5);
                foreach (var item in top5)
                    lstPeakGyroZ.Items.Add($"Average PeakZ: Angle {item.Mean} – Fréquence {item.Freq} - Force {item.AverageAmplitude}");

                double max = 0;
                if (chkShowX.Checked)
                {
                    formsPlotGyro.Plot.Add.Scatter(temporal, analyzedX, Colors.Blue).LegendText = "X";
                    MathHelper.DisplayPeaksTemporal(analyzedX, temporal, "Top Peak X", formsPlotGyro.Plot, lstPeakGyroX);
                    max = analyzedX.Max();
                }
                if (chkShowY.Checked)
                {
                    formsPlotGyro.Plot.Add.Scatter(temporal, analyzedY, Colors.Red).LegendText = "Y";
                    MathHelper.DisplayPeaksTemporal(analyzedY, temporal, "Top Peak Y", formsPlotGyro.Plot, lstPeakGyroY);
                    max = Math.Max(max, analyzedY.Max());
                }
                if (chkShowZ.Checked)
                {
                    formsPlotGyro.Plot.Add.Scatter(temporal, analyzedZ, Colors.Yellow).LegendText = "Z";
                    MathHelper.DisplayPeaksTemporal(analyzedZ, temporal, "Top Peak Z", formsPlotGyro.Plot, lstPeakGyroZ);
                    max = Math.Max(max, analyzedZ.Max());
                }
                if (chkShowResultante.Checked)
                {
                    formsPlotGyro.Plot.Add.Scatter(temporal, resultante, Colors.DeepPink).LegendText = "Resultante"; ;
                    MathHelper.DisplayPeaksTemporal(resultante, temporal, "Top Peak Resultante", formsPlotGyro.Plot, lstPeakResultanteGyro);
                    max = Math.Max(max, resultante.Max());
                }
                max += 5;
                for (int i = 0; i < iCount; i++)
                {
                    if (whiteLine[i] == 10)
                        whiteLine[i] = max;
                }
                var s = formsPlotGyro.Plot.Add.Scatter(temporal, whiteLine, Colors.Black);
                s.LineWidth = 1;
                s.LegendText = "WhiteLine";
                //formsPlotAnalysis.Plot.Axis(0, 360, -1, 1);
                formsPlotGyro.Plot.Axes.SetLimitsX(0, countTotal);
                formsPlotGyro.Plot.Axes.AutoScaleY();
                formsPlotGyro.Plot.ShowLegend();
            }
            else
            {
                FFTData cmpX = EquilibrageHelper.CalculateFFT(analyzedX, sampleRate, cbxFFT, chkDb.Checked, rpm, f_rot);
                FFTData cmpY = EquilibrageHelper.CalculateFFT(analyzedY, sampleRate, cbxFFT, chkDb.Checked, rpm, f_rot);
                FFTData cmpZ = EquilibrageHelper.CalculateFFT(analyzedZ, sampleRate, cbxFFT, chkDb.Checked, rpm, f_rot);
                FFTData cmpResultante = EquilibrageHelper.CalculateFFT(resultante, sampleRate, cbxFFT, chkDb.Checked, rpm, f_rot);

                formsPlotGyro.Plot.Clear();
                lstPeakGyroX.Items.Clear();
                lstPeakGyroY.Items.Clear();
                lstPeakGyroZ.Items.Clear();
                lstPeakResultanteGyro.Items.Clear();
                double fftLimit = Convert.ToDouble(txtFFTLimit.Text);
                if (chkShowX.Checked)
                {
                    MathHelper.AnalyzeAxis("X", cmpX, sampleRate, lstPeakGyroX, Colors.Blue, formsPlotGyro.Plot, f_rot, fftLimit);
                }
                if (chkShowY.Checked)
                {
                    MathHelper.AnalyzeAxis("Y", cmpY, sampleRate, lstPeakGyroY, Colors.Red, formsPlotGyro.Plot, f_rot, fftLimit);
                }
                if (chkShowZ.Checked)
                {
                    MathHelper.AnalyzeAxis("Z", cmpZ, sampleRate, lstPeakGyroZ, Colors.Yellow, formsPlotGyro.Plot, f_rot, fftLimit);
                }
                if (chkShowResultante.Checked)
                {
                    MathHelper.AnalyzeAxis("Resultante", cmpResultante, sampleRate, lstPeakResultanteGyro, Colors.DeepPink, formsPlotGyro.Plot, f_rot, fftLimit);
                }
                String sText = $"Fundamental: {f_rot}Hz\r\n1er order: {f_rot * 2}\r\n2eme order: {f_rot * 3}\r\n3eme order: {f_rot * 4}\r\n4er order: {f_rot * 5}\r\n5er order: {f_rot * 6}\r\n";
                formsPlotGyro.Plot.Add.Annotation(sText);

                lstSimulationGyro.Items.Clear();

                //X and Y are inversed on GYRO
                var calcResult = EquilibrageHelper.CompleteSimulation(lstSimulationGyro, "Gyroscope", cmpY, cmpX, cmpZ, cmpResultante, sampleRate, f_rot, Convert.ToDouble(txtCorrectAngleX.Text), Convert.ToDouble(txtCorrectAngleY.Text), 1);
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        if (calcResult.dir[i].IsDynamic)
                        {
                            formsPlotT1I.Plot.Add.VerticalLine(calcResult.dir[i].correction.AngleInnerDeg, color:Colors.Yellow, width: 3);
                            formsPlotT1O.Plot.Add.VerticalLine(calcResult.dir[i].correction.AngleOuterDeg, color: Colors.Yellow, width: 3);
                        }
                        formsPlotT1X.Plot.Add.VerticalLine(calcResult.px[i].UnbalanceAngleDeg, color: Colors.Yellow, width: 3);
                        formsPlotT1Y.Plot.Add.VerticalLine(calcResult.py[i].UnbalanceAngleDeg, color: Colors.Yellow, width: 3);
                    }
                  
                }
                //check if we use gyro X for accel Y
                if (chkUseXGyro.Checked)
                {
                    // 5. PSD (densité spectrale de puissance)
                    // PSD = |X(f)|^2 / (fs * N) ou / (fs * durée)
                    // return $"{sTitle}: {magnitudePerSecond:F4}\r\n{magnitudePerRevolution:F4} {psd:F4}";
                    double magnitude = calcResult.px[0].ActualAmplitude;
                    double magPerTurn = magnitude / selectedSections.Count;
                    double psd = (magnitude * magnitude) / analyzedX.Length;

                    //lblFFTAnalysis.Text += "Global X Mag: " + magnitude.ToString("F4") + "\r\nBy turn count: " + magPerTurn.ToString("F4") + "\r\nPSD: " + psd.ToString("F4") + "\r\n";
                    currentAnalysisY.gMagAvg = magnitude;
                    currentAnalysisY.gMagRatio = magPerTurn;
                    currentAnalysisY.gMagPSD = psd;
                    

                }
                
                if (chkUseYGyro.Checked)
                {
                    var magnitude = calcResult.py[0].ActualAmplitude;
                    var magPerTurn = magnitude / selectedSections.Count;
                    var psd = (magnitude * magnitude) / analyzedY.Length;

                    //lblFFTAnalysis.Text += "Global Y Mag: " + magnitude.ToString("F4") + "\r\nBy turn count: " + magPerTurn.ToString("F4") + "\r\nPSD: " + psd.ToString("F4") + "\r\n";

                    currentAnalysisX.gMagAvg = magnitude;
                    currentAnalysisX.gMagRatio = magPerTurn;
                    currentAnalysisX.gMagPSD = psd;
                   

                }
                
            }
            formsPlotGyro.Refresh();
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
                        //if (Math.Abs(r.x) > 15 || Math.Abs(r.y) > 15 || Math.Abs(r.z) > 15 || Math.Abs(r.gx) > 15 || Math.Abs(r.gy) > 15 || Math.Abs(r.gz) > 15)
                        //    continue;
                        if (r.isWhite && !bPreviousWhite)
                        {
                            if (s != null)
                            {
                                //normalize the section
                                //   double squarerpm = s.Rpm/1000.0;
                                //   s.records = s.records.Select(xyz => new xyz() { x = xyz.x / squarerpm, y = xyz.y / squarerpm, z = xyz.z / squarerpm, gx = xyz.gx / squarerpm, gy = xyz.gy / squarerpm, gz = xyz.gz / squarerpm, isWhite = xyz.isWhite, ms = xyz.ms }).ToList();
                                if (s.SamplingRate == -1)
                                    s.SamplingRate = Convert.ToDouble(txtSampleRate.Text);
                            }
                            s = new section();
                            s.records = new List<xyz>(360);
                            loadedSections.Add(s);
                        }
                        if (s != null)
                            s.records.Add(r);
                        bPreviousWhite = r.isWhite;
                        iTotalRecordsInCSV++;
                    }
                    if (s != null)
                    {
                        //normalize the section
                        //double squarerpm = s.Rpm / 1000.0;
                        //s.records = s.records.Select(xyz => new xyz() { x = xyz.x / squarerpm, y = xyz.y / squarerpm, z = xyz.z / squarerpm, gx = xyz.gx / squarerpm, gy = xyz.gy / squarerpm, gz = xyz.gz / squarerpm, isWhite = xyz.isWhite, ms = xyz.ms }).ToList();
                        if (s.SamplingRate == -1)
                            s.SamplingRate = Convert.ToDouble(txtSampleRate.Text);
                    }
                }
            }
            int i = 0;
            lstSectionSelector.Items.Clear();
            foreach (section se in loadedSections)
            {
                lstSectionSelector.Items.Add(new SectionInfo() { sampleRate = se.SamplingRate, rpm = se.Rpm, index = i, count = se.Size });
                i++;
            }
            lstSimulationCompiled.Items.Clear();
            selectedSections.Clear();
        }

        private void DisplayData(double[] data, double[] angle, double f_rot, double rpm, double sampleRate, ListBox lstPeak, Plot pltTemporal, Plot pltFFT, string axis, ScottPlot.Color c)
        {
            lstPeak.Items.Clear();
            MathHelper.AnalyzeAxisTemporal(axis, data, angle, sampleRate, lstPeak, c, pltTemporal, f_rot);
            FFTData dataFFT = EquilibrageHelper.CalculateFFT(data, sampleRate, cbxFFTSingle, chkDb.Checked, rpm, f_rot);
            double fftLimit = Convert.ToDouble(txtFFTLimit.Text);
            MathHelper.AnalyzeAxis(axis, dataFFT, sampleRate, lstPeak, c, pltFFT, f_rot, fftLimit);
        }

        private void Analyze(string csvFile)
        {
            if (String.IsNullOrEmpty(csvFile))
                return;
            if (lstSectionSelector.CheckedIndices.Count < 3)
                return;


            lstSimulationTurnByTurn.Items.Clear();
            selectedSections.Clear();
            iTotalRecordsInCSV = 0;
           
            foreach (int s in lstSectionSelector.CheckedIndices)
            {
                selectedSections.Add(loadedSections[s]);
                iTotalRecordsInCSV += loadedSections[s].records.Count;
            }
            currentAnalysisX = new AnalysisData() { csvFile = csvFile };
            currentAnalysisY = new AnalysisData() { csvFile = csvFile };
            currentAnalysisX.numberOfTurn = selectedSections.Count;
            currentAnalysisY.numberOfTurn = selectedSections.Count;

            //            lblFFTAnalysis.Text = String.Empty;
            formsPlotT1X.Plot.Clear(); formsPlotT1Y.Plot.Clear(); formsPlotT1O.Plot.Clear(); formsPlotT1I.Plot.Clear();
          

            if (selectedSections.Count > 0)
            {
                RefreshAnalysisCompiled();
                RefreshAnalysisGlobal();
                RefreshGyro();
                lblRecordNumber.Text = "0";
                RefreshXYZ(selectedSections[0]);
                ExecuteAnalysis();
            }
            formsPlotT1X.Plot.Axes.SetLimits(0, 360); formsPlotT1Y.Plot.Axes.SetLimits(0, 360); formsPlotT1O.Plot.Axes.SetLimits(0, 360); formsPlotT1I.Plot.Axes.SetLimits(0, 360);
          
            formsPlotT1X.Refresh(); formsPlotT1Y.Refresh(); formsPlotT1O.Refresh(); formsPlotT1I.Refresh();


            /*   var xCorrect = Convert.ToDouble(txtCorrectAngleX.Text);
               currentAnalysisX.gAngle = (currentAnalysisX.gAngle + xCorrect) % 360;
               currentAnalysisX.gAngleAlternatif = (currentAnalysisX.gAngleAlternatif+ xCorrect) % 360;
               currentAnalysisX.ttPkTiming = (currentAnalysisX.ttPkTiming + xCorrect) % 360;
               currentAnalysisX.ttAngle = (currentAnalysisX.ttAngle + xCorrect) % 360;
               var yCorrect = Convert.ToDouble(txtCorrectAngleY.Text);
               currentAnalysisY.gAngle = (currentAnalysisY.gAngle + yCorrect) % 360;
               currentAnalysisY.gAngleAlternatif = (currentAnalysisY.gAngleAlternatif + yCorrect) % 360;
               currentAnalysisY.ttPkTiming = (currentAnalysisY.ttPkTiming + yCorrect) % 360;
               currentAnalysisY.ttAngle = (currentAnalysisY.ttAngle + yCorrect) % 360;


               currentAnalysisY.gAngleGyro = (currentAnalysisY.gAngleGyro + 180) % 360;
               currentAnalysisX.gAngleGyro = (currentAnalysisX.gAngleGyro + 90) % 360;

               */
            /*  if (chkUseXGyro.Checked && chkScaleGyro.Checked) //scale X Gyro
              {
                  currentAnalysisY.gMagRatio *= 0.1;
              }
              if (chkUseYGyro.Checked && chkScaleGyro.Checked) //scale X Gyro
              {
                  currentAnalysisX.gMagRatio *= 0.1;
              }*/
            /*var result = EquilibrageHelper.CalculateAttenuationConstantsXY(Convert.ToDouble(txtXMagGrams.Text) / 1000.0,
                                      Convert.ToDouble(txtXMagBalanced.Text),
                                      Convert.ToDouble(txtYMagBalanced.Text),
                                      Convert.ToDouble(txtXMagExt.Text),
                                      Convert.ToDouble(txtYMagExt.Text),
                                      Convert.ToDouble(txtYMagGrams.Text) / 1000.0,
                                      Convert.ToDouble(txtXMagInt.Text),
                                      Convert.ToDouble(txtYMagInt.Text));
            var res = EquilibrageHelper.EstimateDynamicBalancing(currentAnalysisX.gMagRatio, currentAnalysisX.gAngle, currentAnalysisY.gMagRatio, currentAnalysisY.gAngle, result.KextX, result.KextY, result.KintX, result.KintY);
            currentAnalysisX.gAngleDynamicComplex = res.AngleIntDeg;
            currentAnalysisY.gAngleDynamicComplex = res.AngleExtDeg;
            currentAnalysisX.gWeight = res.MassInt;
            currentAnalysisY.gWeight = res.MassExt;
            */
            var dynamic = EquilibrageHelper.EstimateDynamicImbalanceCorrection(currentAnalysisX.gAngleFFT, currentAnalysisX.gMagRatio, currentAnalysisY.gAngleFFT, currentAnalysisY.gMagRatio);
            
            dataGridX.Rows.Add(currentAnalysisX.toArray());
            dataGridY.Rows.Add(currentAnalysisY.toArray());
/*
            lblStatX.Text = $"X\r\nGlobal {currentAnalysisX.gWeight.ToString("F0")}g @ {currentAnalysisX.gAngleDynamicComplex.ToString("F0")}°\r\nTurn-turn {currentAnalysisX.ttWeight.ToString("F0")}g @ {currentAnalysisX.ttAngle.ToString("F0")}°\r\nCompiled {currentAnalysisX.coWeight.ToString("F0")}g @ {currentAnalysisX.coAngle.ToString("F0")}°";
            lblStatY.Text = $"Y\r\nGlobal {currentAnalysisY.gWeight.ToString("F0")}g @ {currentAnalysisY.gAngleDynamicComplex.ToString("F0")}°\r\nTurn-turn {currentAnalysisY.ttWeight.ToString("F0")}g @ {currentAnalysisY.ttAngle.ToString("F0")}°\r\nCompiled {currentAnalysisY.coWeight.ToString("F0")}g @ {currentAnalysisY.coAngle.ToString("F0")}°";
            lblStatX.Refresh();
            lblStatY.Refresh();

            //verify if there is not big gap between angles
            //if not enough selected data, display warning
            if (Math.Abs(currentAnalysisX.gAngle - currentAnalysisX.gAngleGyro) > 45 || Math.Abs(currentAnalysisY.gAngle - currentAnalysisY.gAngleGyro) > 45)
            {
                //     MessageBox.Show("Be carefull, X or Y angles have more than 45° between Global and Gyro ! results may not be good");*
            }*/
        }
        private void ExecuteAnalysis()
        {
            //get signal, apply low pass filter with 5hz to get temporal peaks, then apply selected user signal to determine phase and magnitude
            var dataCompiled = GetCompiledTourSignal();
            var dataCompiledX = GetPhaseMagnitude(dataCompiled.x, dataCompiled.angle, dataCompiled.sampleRate, dataCompiled.rpm, dataCompiled.f_rot);
            var dataCompiledY = GetPhaseMagnitude(dataCompiled.y, dataCompiled.angle, dataCompiled.sampleRate, dataCompiled.rpm, dataCompiled.f_rot);

            var dataGlobal = GetGlobalTourSignal();
            var dataGlobalX = GetPhaseMagnitude(dataGlobal.x, dataGlobal.angle, dataGlobal.sampleRate, dataGlobal.rpm, dataGlobal.f_rot);
            var dataGlobalY = GetPhaseMagnitude(dataGlobal.y, dataGlobal.angle, dataGlobal.sampleRate, dataGlobal.rpm, dataGlobal.f_rot);

            currentAnalysisX.coAngleTemporal = dataCompiledX.angleTemporal;
            currentAnalysisX.coAngleFFT = dataCompiledX.fund.Angle;
            currentAnalysisX.coMagAvg = dataCompiledX.fund.Magnitude;
            currentAnalysisX.coPkPk = dataCompiledX.pkpk;
            currentAnalysisX.coRMS = dataCompiledX.rms;

            currentAnalysisY.coAngleTemporal = dataCompiledY.angleTemporal;
            currentAnalysisY.coAngleFFT = dataCompiledY.fund.Angle;
            currentAnalysisY.coMagAvg = dataCompiledY.fund.Magnitude;
            currentAnalysisY.coPkPk = dataCompiledY.pkpk;
            currentAnalysisY.coRMS = dataCompiledY.rms;

            currentAnalysisX.gAngleTemporal = dataGlobalX.angleTemporal;
            currentAnalysisX.gAngleFFT = dataGlobalX.fund.Angle;
            currentAnalysisX.gMagAvg = dataGlobalX.fund.Magnitude;
            currentAnalysisX.gPkPk = dataGlobalX.pkpk;
            currentAnalysisX.gRMS = dataGlobalX.rms;
            //psd = (magnitude * magnitude) / analyzedY.Length;
            currentAnalysisX.gMagPSD = (dataGlobalX.fund.Magnitude * dataGlobalX.fund.Magnitude) / dataGlobal.x.Length;
            currentAnalysisX.gMagRatio = dataGlobalX.fund.Magnitude / currentAnalysisX.numberOfTurn;

            currentAnalysisY.gAngleTemporal = dataGlobalY.angleTemporal;
            currentAnalysisY.gAngleFFT = dataGlobalY.fund.Angle;
            currentAnalysisY.gMagAvg = dataGlobalY.fund.Magnitude;
            currentAnalysisY.gPkPk = dataGlobalY.pkpk;
            currentAnalysisY.gRMS = dataGlobalY.rms;
            //psd = (magnitude * magnitude) / analyzedY.Length;
            currentAnalysisY.gMagPSD = (dataGlobalY.fund.Magnitude * dataGlobalY.fund.Magnitude) / dataGlobal.y.Length;
            currentAnalysisY.gMagRatio = dataGlobalY.fund.Magnitude / currentAnalysisY.numberOfTurn;

            List<double> lstAngleFFTX = new List<double>();
            List<double> lstAngleTemporalX = new List<double>();
            List<double> lstAngleFFTY = new List<double>();
            List<double> lstAngleTemporalY = new List<double>();
            foreach (section s in selectedSections)
            {
                var dataSingle = GetSingleTourSignal(s);
                var dataSingleX = GetPhaseMagnitude(dataSingle.x, dataSingle.angle, dataSingle.sampleRate, dataSingle.rpm, dataSingle.f_rot);
                var dataSingleY = GetPhaseMagnitude(dataSingle.y, dataSingle.angle, dataSingle.sampleRate, dataSingle.rpm, dataSingle.f_rot);
                lstAngleFFTX.Add(dataSingleX.fund.Angle); lstAngleTemporalX.Add(dataSingleX.angleTemporal);
                lstAngleFFTY.Add(dataSingleY.fund.Angle); lstAngleTemporalY.Add(dataSingleY.angleTemporal);

                currentAnalysisX.ttMagAvg += dataSingleX.fund.Magnitude;
                currentAnalysisX.ttPkPk += dataSingleX.pkpk;
                currentAnalysisX.ttRMS += dataSingleX.rms;

                currentAnalysisY.ttMagAvg += dataSingleY.fund.Magnitude;
                currentAnalysisY.ttPkPk += dataSingleY.pkpk;
                currentAnalysisY.ttRMS += dataSingleY.rms;
            }
            currentAnalysisX.ttAngleFFT = MathHelper.CalculateMeanAngle(lstAngleFFTX.ToArray());
            currentAnalysisX.ttAngleTemporal = MathHelper.CalculateMeanAngle(lstAngleTemporalX.ToArray());
            currentAnalysisY.ttAngleFFT = MathHelper.CalculateMeanAngle(lstAngleFFTY.ToArray());
            currentAnalysisY.ttAngleTemporal = MathHelper.CalculateMeanAngle(lstAngleTemporalY.ToArray());

            currentAnalysisX.ttMagAvg /= currentAnalysisX.numberOfTurn;
            currentAnalysisX.ttPkPk /= currentAnalysisX.numberOfTurn;
            currentAnalysisX.ttRMS /= currentAnalysisX.numberOfTurn;
            currentAnalysisY.ttMagAvg /= currentAnalysisY.numberOfTurn;
            currentAnalysisY.ttPkPk /= currentAnalysisY.numberOfTurn;
            currentAnalysisY.ttRMS /= currentAnalysisY.numberOfTurn;

            //Adjust the angles and draw the data to the graphs
            var xCorrect = Convert.ToDouble(txtCorrectAngleX.Text);
            var yCorrect = Convert.ToDouble(txtCorrectAngleY.Text);
            var xCorrectTemp = Convert.ToDouble(txtCorrectXTemporal.Text);
            var yCorrectTemp = Convert.ToDouble(txtCorrectYTemporal.Text);
            currentAnalysisX.ttAngleFFT = (currentAnalysisX.ttAngleFFT + xCorrect) % 360;
            currentAnalysisX.ttAngleTemporal = (currentAnalysisX.ttAngleTemporal + xCorrectTemp) % 360;
            currentAnalysisX.coAngleFFT = (currentAnalysisX.coAngleFFT + xCorrect) % 360;
            currentAnalysisX.coAngleTemporal = (currentAnalysisX.coAngleTemporal + xCorrectTemp) % 360;
            currentAnalysisX.gAngleFFT = (currentAnalysisX.gAngleFFT + xCorrect) % 360;
            currentAnalysisX.gAngleTemporal = (currentAnalysisX.gAngleTemporal + xCorrectTemp) % 360;

            currentAnalysisY.ttAngleFFT = (currentAnalysisY.ttAngleFFT + yCorrect) % 360;
            currentAnalysisY.ttAngleTemporal = (currentAnalysisY.ttAngleTemporal + yCorrectTemp) % 360;
            currentAnalysisY.coAngleFFT = (currentAnalysisY.coAngleFFT + yCorrect) % 360;
            currentAnalysisY.coAngleTemporal = (currentAnalysisY.coAngleTemporal + yCorrectTemp) % 360;
            currentAnalysisY.gAngleFFT = (currentAnalysisY.gAngleFFT + yCorrect) % 360;
            currentAnalysisY.gAngleTemporal = (currentAnalysisY.gAngleTemporal + yCorrectTemp) % 360;

            formsPlotT1X.Plot.Add.VerticalLine(currentAnalysisX.gAngleFFT, color: Colors.Green, width: 3);
            formsPlotT1Y.Plot.Add.VerticalLine(currentAnalysisY.gAngleFFT, color: Colors.Green, width: 3);
            formsPlotT1X.Plot.Add.VerticalLine(currentAnalysisX.coAngleFFT, color: Colors.Red, width: 3);
            formsPlotT1Y.Plot.Add.VerticalLine(currentAnalysisY.coAngleFFT, color: Colors.Red, width: 3);
            formsPlotT1X.Plot.Add.VerticalLine(currentAnalysisX.ttAngleFFT, color: Colors.Black, width: 3);
            formsPlotT1Y.Plot.Add.VerticalLine(currentAnalysisY.ttAngleFFT, color: Colors.Black, width: 3);

            // convert turn by turn to int and sort to key, count data for displaying blue bars
            var ttFFTX = lstAngleFFTX.Select(v => (int)(v + xCorrect) % 360).GroupBy(v => v).Select(g => new { Value = g.Key, Occurrence = g.Count() }).ToList();
            var ttFFTY = lstAngleFFTY.Select(v => (int)(v + yCorrect) % 360).GroupBy(v => v).Select(g => new { Value = g.Key, Occurrence = g.Count() }).ToList();
            var b = formsPlotT1X.Plot.Add.Bars(ttFFTX.Select(v => (double)v.Value).ToArray(), ttFFTX.Select(v => v.Occurrence).ToArray());
            b.Bars.ForEach(ba => { ba.LineWidth = 0.5f; ba.FillColor = ba.LineColor = Colors.Blue; });
            b = formsPlotT1Y.Plot.Add.Bars(ttFFTY.Select(v => (double)v.Value).ToArray(), ttFFTY.Select(v => v.Occurrence).ToArray());
            b.Bars.ForEach(ba => { ba.LineWidth = 0.5f; ba.FillColor = ba.LineColor = Colors.Blue; });

        }

        private (double angleTemporal, double pkpk,double rms, Fundamentale fund) GetPhaseMagnitude(double[] data, double[] angle, double sampleRate, double rpm,double f_rot)
        {
            double angleTemporal = 0;
            var l = new LowPassFilter(5, sampleRate);
            var dataFiltered = LowPassFilter.ApplyZeroPhase(data, l);
            if (chkRemoveDC.Checked)
                dataFiltered = LowPassFilter.RemoveDCOffset(dataFiltered);
            //apply gain on signal
            var gain = Convert.ToDouble(txtGain.Text);
            dataFiltered  = dataFiltered.Select(r => Math.Sign(r) * Math.Pow(Math.Abs(r), gain)).ToArray();
            var peaks = MathHelper.GetPeakPerTurn(dataFiltered);
            List<double> lstAnglePeaks = new List<double>();
            foreach(var p in peaks)
                lstAnglePeaks.Add(angle[p]);

            angleTemporal = MathHelper.CalculateMeanAngle(lstAnglePeaks.ToArray());
            
            //fake data
            double[] y = new double[0];
            double[] z = new double[0];
            double[] resultante = new double[0];
            ApplyFilters(sampleRate, f_rot, ref data, ref y, ref z, ref resultante);
            FFTData dataFFT = EquilibrageHelper.CalculateFFT(data, sampleRate, cbxFFTSingle, chkDb.Checked, rpm, f_rot);
            var fund = EquilibrageHelper.GetFundamentalPhase(dataFFT.Frequence, dataFFT.Magnitude, dataFFT.AngleDeg, f_rot);
            return (angleTemporal, dataFiltered.Max()- dataFiltered.Min(), Statistics.RootMeanSquare(data),fund);
        }

        private void btnFindAngles_Click(object sender, EventArgs e)
        {
            lstAngleXAnalysis.Clear();
            lstAngleYAnalysis.Clear();
            lstAnglesX.Items.Clear();
            lstAnglesY.Items.Clear();
            //get each turn and perform analysis
            for (int i = 0; i < selectedSections.Count; i++)
            {
                var data = GetSingleTourSignal(selectedSections[i]);
            }
            //get compiled and perform analysis
            var compiled = GetCompiledTourSignal();
            //get global and perform analysis
            var global = GetGlobalTourSignal();
            //get gyro and perform analysis

            lstAnglesX.Items.Add("200-210");
            lstAnglesY.Items.Add("200-210");
            btnUnselectAll_Click(null, EventArgs.Empty);
            btn200210_Click(null, EventArgs.Empty);

            Analyze(sLastCSV);
            lstAnglesX.Items.Add("210-220");
            lstAnglesY.Items.Add("210-220");
            btnUnselectAll_Click(null, EventArgs.Empty);
            btn210220_Click(null, EventArgs.Empty);
            Analyze(sLastCSV);

            lstAnglesX.Items.Add("220-230");
            lstAnglesY.Items.Add("220-230");
            btnUnselectAll_Click(null, EventArgs.Empty);
            btn220230_Click(null, EventArgs.Empty);
            Analyze(sLastCSV);

            lstAnglesX.Items.Add("230-240");
            lstAnglesY.Items.Add("230-240");
            btnUnselectAll_Click(null, EventArgs.Empty);
            btn230240_Click(null, EventArgs.Empty);
            Analyze(sLastCSV);

            lstAnglesX.Items.Add("240-250");
            lstAnglesY.Items.Add("240-250");
            btnUnselectAll_Click(null, EventArgs.Empty);
            btn240250_Click(null, EventArgs.Empty);
            Analyze(sLastCSV);

            lstAnglesX.Items.Add("230-250");
            lstAnglesY.Items.Add("230-250");
            btn230240_Click(null, EventArgs.Empty);
            Analyze(sLastCSV);

            lstAnglesX.Items.Add("200-230");
            lstAnglesY.Items.Add("200-230");
            btnUnselectAll_Click(null, EventArgs.Empty);
            btn200210_Click(null, EventArgs.Empty);
            btn210220_Click(null, EventArgs.Empty);
            btn220230_Click(null, EventArgs.Empty);
            Analyze(sLastCSV);
            

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
            // Dans MouseMove ou timer
            Pixel mousePixel = new Pixel(e.Location.X, e.Location.Y);
            // Trouver le subplot sous la souris
            var subplot = plt.Multiplot.GetPlotAtPixel(mousePixel);

            if (subplot is null)
                return;
            
            var plottable = subplot.GetPlottables();
            if (plottable.Count() == 0)
            {
                plottable = plt.Plot.GetPlottables();
                if(plottable.Count() == 0)
                    return;
            }
            // determine point nearest the cursor
            
            String sData = String.Empty;
            Coordinates mouseLocation = subplot.GetCoordinates(mousePixel);

            foreach (var p in plottable)
            {
                if (p is ScottPlot.Plottables.Scatter)  //for each scatterplot check if mouse is on a plot
                {
                    var sp = p as ScottPlot.Plottables.Scatter;
                    if (sp.IsVisible)
                    {
                        var nearest = sp.Data.GetNearestX(mouseLocation, subplot.LastRender);
                        //find Y at scatterplot hidden having name ANGLE + label
                        if (nearest.Index == -1 || nearest.X == double.NaN)
                            continue;
                        foreach (var p2 in plottable)
                        {
                            if (p2 is ScottPlot.Plottables.Scatter)  //for each scatterplot check if mouse is on a plot
                            {
                                var sp2 = p2 as ScottPlot.Plottables.Scatter;
                                if (sp2.Label == "ANGLE" + sp.Label)
                                {
                                    sData += $"{sp.Label} Freq: {nearest.X} Angle: {sp2.GetIDataSource().GetY(nearest.Index)}\r\n";
                                }
                            }
                        }
                    }
                }
            }
            toolTip1.Show(sData, plt, e.Location, 3000);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txtFFTLimit_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.XGrams = Convert.ToDouble(txtXMagGrams.Text);
            Properties.Settings.Default.YGrams = Convert.ToDouble(txtYMagGrams.Text);
            Properties.Settings.Default.XMagTarget = Convert.ToDouble(txtXMagBalanced.Text);
            Properties.Settings.Default.YMagTarget = Convert.ToDouble(txtYMagBalanced.Text);
            Properties.Settings.Default.XMagInitial = Convert.ToDouble(txtXMagExt.Text);
            Properties.Settings.Default.YMagInitial = Convert.ToDouble(txtYMagExt.Text);
            Properties.Settings.Default.XMagFinal = Convert.ToDouble(txtXMagInt.Text);
            Properties.Settings.Default.YMagFinal = Convert.ToDouble(txtYMagInt.Text);
            Properties.Settings.Default.XAngleCorrect = Convert.ToDouble(txtCorrectAngleX.Text);
            Properties.Settings.Default.YAngleCorrect = Convert.ToDouble(txtCorrectAngleY.Text);
            Properties.Settings.Default.XAngleCorrectTemporal = Convert.ToDouble(txtCorrectXTemporal.Text);
            Properties.Settings.Default.YAngleCorrectTemporal = Convert.ToDouble(txtCorrectYTemporal.Text);
            Properties.Settings.Default.UseXGyro = chkUseXGyro.Checked;
            Properties.Settings.Default.UseYGyro = chkUseYGyro.Checked;
            Properties.Settings.Default.Save();
        }

        private void btnClearAnalysisHistory_Click(object sender, EventArgs e)
        {
            dataGridX.Rows.Clear();
            dataGridY.Rows.Clear();
            dataGridX.Columns.Clear();
            dataGridY.Columns.Clear();
            
            List<DataGridViewColumn> dgvcX = new List<DataGridViewColumn>()
            {
                new DataGridViewColumn(){ HeaderText = "CSV File", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells} ,
                new DataGridViewColumn(){ HeaderText = "Selected Nb of turn", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag PSD", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag Ratio" , CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Mag AVG", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Mag AVG", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                
                new DataGridViewColumn(){ HeaderText = "Global Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Angle Temporal", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Angle Temporal", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Angle Temporal", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Pk-Pk", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Pk-Pk", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Pk-Pk", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global RMS", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled RMS", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn RMS", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
            };
            List<DataGridViewColumn> dgvcY = new List<DataGridViewColumn>()
            {
                new DataGridViewColumn(){ HeaderText = "CSV File", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells} ,
                new DataGridViewColumn(){ HeaderText = "Selected Nb of turn", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag PSD", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag Ratio" , CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Mag AVG", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Mag AVG", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},

                new DataGridViewColumn(){ HeaderText = "Global Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Angle Temporal", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Angle Temporal", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Angle Temporal", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Pk-Pk", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Pk-Pk", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Pk-Pk", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global RMS", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled RMS", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn RMS", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
            };

            foreach (var c in dgvcX)
                dataGridX.Columns.Add(c);

            foreach (var c in dgvcY)
                dataGridY.Columns.Add(c);


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
        public int count;
        public override string ToString()
        {
            return "RPM: " + rpm.ToString() + " SampleRate: " + sampleRate.ToString() + " Count: " + count;
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
        public section()
        {
            _sampleRate = -1;
        }
        public int Size
        {
            get { return records.Count; }
        }
        private double _sampleRate;
        public double SamplingRate
        {
            get
            {
                int startTS = records[0].ms;
                int endTS = records[records.Count-1].ms;
                if (startTS == endTS)
                {
                    return _sampleRate;
                }
                else
                {
                    if (endTS < startTS)
                        endTS += (ushort.MaxValue);
                    startTS *= 64; //64 us
                    endTS *= 64;
                    double duration = (endTS - startTS) / 1000.0; //ms
                    return ((records.Count * 1000.0) / duration);
                }
                
            }
            set
            {
                _sampleRate = value;
            }
        }
        public double Rpm
        {
            get
            {
                return ((float)(60.0 * SamplingRate / Size));
            }
        }
        public List<xyz> records = new List<xyz>();
    }
    public class AnalysisData
    {
        public string csvFile;
        public int numberOfTurn;
        public double gPkPk;
        public double gMagAvg;
        public double gMagPSD;
        public double gMagRatio;
        public double gAngleFFT;
        public double gAngleTemporal;
        
        public double ttPkPk;
        public double ttMagAvg;
        public double ttAngleFFT;
        public double ttAngleTemporal;

        public double coPkPk;
        public double coMagAvg;
        public double coAngleFFT;
        public double coAngleTemporal;

        public double gRMS;
        public double ttRMS;
        public double coRMS;

        public String[] toArray()
        {
            /*
                       new DataGridViewColumn(){ HeaderText = "CSV File", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells} ,
                new DataGridViewColumn(){ HeaderText = "Selected Nb of turn", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag PSD", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag Ratio" , CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},

                new DataGridViewColumn(){ HeaderText = "Global Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Angle Alternatif FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Angle Gyro FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},

                new DataGridViewColumn(){ HeaderText = "Turn-Turn Mag AVG", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Mag AVG", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Angle correction Simple", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Angle correction Simple", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Angle correction Simple", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "AVG Angle correction Simple", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},

                new DataGridViewColumn(){ HeaderText = "Global Angle correction Complex", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Angle correction Complex", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Angle correction Complex", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "AVG Angle correction Complex", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},

                new DataGridViewColumn(){ HeaderText = "Global Weight Estimate", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Weight Estimate", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Weight Estimate", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "AVG Weight Estimate", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                
                
                new DataGridViewColumn(){ HeaderText = "Compiled Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "AVG Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Pk-Pk", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Pk-Pk", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Pk-Pk", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global FFT inverse Pk-Pk", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global FFT inverse Ratio Pk-Pk", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn FFT inverse Pk-Pk", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled FFT inverse Pk-Pk", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global RMS", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn RMS", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled RMS", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag Avg", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},*/
            List<String> s = new List<string>()
            {
                Path.GetFileName(csvFile),
                numberOfTurn.ToString(),
                gMagAvg.ToString("F4"),
                gMagPSD.ToString("F4"),
                gMagRatio.ToString("F4"),
                coMagAvg.ToString("F4"),
                ttMagAvg.ToString("F4"),
                gAngleFFT.ToString("F4"),
                coAngleFFT.ToString("F4"),
                ttAngleFFT.ToString("F4"),
                gAngleTemporal.ToString("F4"),
                coAngleTemporal.ToString("F4"),
                ttAngleTemporal.ToString("F4"),
                gPkPk.ToString("F4"),
                coPkPk.ToString("F4"),
                ttPkPk.ToString("F4"),
                gRMS.ToString("F4"),
                coRMS.ToString("F4"),
                ttRMS.ToString("F4")
            };
            return s.ToArray();
        }
    }
 

}
