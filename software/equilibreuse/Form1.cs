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
            cbxFilterTypes.SelectedItem = "Butterworth";
            cbxAngleData.SelectedItem = "Global FFT";
            

            cbxSensor.SelectedIndex = 0; //mpu per default
            Help.FillHelp(richTextBox1);
            btnClearAnalysisHistory_Click(null, EventArgs.Empty);

            txtMagGrams1.Text = Properties.Settings.Default.CalibGrams1.ToString();
          
            txtXMagBalanced.Text = Properties.Settings.Default.XMagBalanced.ToString();
            txtYMagBalanced.Text = Properties.Settings.Default.YMagBalance.ToString();
          
            txtXMagG1.Text = Properties.Settings.Default.XMag1.ToString();
            txtYMag1.Text = Properties.Settings.Default.YMag1.ToString();
            txtCorrectAngleX.Text = Properties.Settings.Default.XAngleCorrect.ToString();
            txtCorrectAngleY.Text = Properties.Settings.Default.YAngleCorrect.ToString();
            txtCorrectXTemporal.Text = Properties.Settings.Default.XAngleCorrectTemporal.ToString();
            txtCorrectYTemporal.Text = Properties.Settings.Default.YAngleCorrectTemporal.ToString();
            chkLowPassFilter.Checked = Properties.Settings.Default.LowPassChecked;
            chkPassband.Checked = Properties.Settings.Default.PassBandChecked;
            cbxFilterTypes.SelectedItem = Properties.Settings.Default.FilterSelect;
            txtFilterOrder.Text = Properties.Settings.Default.FilterOrder.ToString();
            cbxSmoothing.SelectedItem = Properties.Settings.Default.FilterSmooth;
            chkAbsolute.Checked = Properties.Settings.Default.AbsChecked;
            chkSum.Checked = Properties.Settings.Default.SumChecked;
            chkRemoveDC.Checked = Properties.Settings.Default.RemoveDCChecked;
            chkDb.Checked = Properties.Settings.Default.dbChecked;
            txtGain.Text = Properties.Settings.Default.Gain.ToString();
            chkOrderTracking.Checked = Properties.Settings.Default.OrderTrackingChecked;
            cbxFFTSingle.SelectedItem = Properties.Settings.Default.FFTSingle;
            cbxFFT.SelectedItem = Properties.Settings.Default.FFTGlobal;
            cbxAngleData.SelectedItem = Properties.Settings.Default.AngleData;
            
            chkClockwise.Checked = Properties.Settings.Default.ClockwiseRotating;
            txtXLockinBalanced.Text = Properties.Settings.Default.XLockinBalanced.ToString();
            txtYLockinBalanced.Text = Properties.Settings.Default.YLockinBalanced.ToString();
            txtXLockinG1.Text = Properties.Settings.Default.XLockinMag.ToString();
            txtYLockin1.Text = Properties.Settings.Default.YLockinMag.ToString();
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
                avg = new xyz();
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
                data.x = (double)((x) * 0.061 * 4 * G / 1000.0);
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
                //4096 for 8G
                //2048 for 16G
                data.x = (double)((x) * G / 4096.0);
                data.y = (double)((y) * G / 4096.0);
                data.z = (double)((z) * G / 4096.0);
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


            if (chkRemoveDC.Checked)
            {
                x = LowPassFilter.RemoveDCOffset(x);
                y = LowPassFilter.RemoveDCOffset(y);
                z = LowPassFilter.RemoveDCOffset(z);
                resultante = LowPassFilter.RemoveDCOffset(resultante);
            }

            //apply gain on signal
            var gain = Convert.ToDouble(txtGain.Text);
            x = x.Select(r => r * gain).ToArray();
            y = y.Select(r => r * gain).ToArray();
            z = z.Select(r => r * gain).ToArray();
            if (resultante == null || resultante.Length == 0)
                return;
            resultante = resultante.Select(r => r * gain).ToArray();
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
            return (x, y, z, resultante, angle, f_rot, rpm, sampleRate);
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

            return (analyzedX, analyzedY, analyzedZ, resultante, angles, f_rot, rpm, sampleRate);
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
                        angle[iCount] = i * angleIncrement;
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
                        angle[i + (tourNumber * alignedCount)] = i * angleIncrement;
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
            return (analyzedX, analyzedY, analyzedZ, resultante, whiteLine, angle, f_rot, rpm, sampleRate);
        }

        // analysis of all consecutives segments
        private (double[] x, double[] y, double[] z, double[] resultante, double[] whiteLine, double[] angle, double f_rot, double rpm, double sampleRate) GetGlobalTourGyroSignal()
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
                        angle[i + (tourNumber * alignedCount)] = i * angleIncrement;
                    iCount += alignedCount;

                    MathHelper.ResampleSectionGyroXYZ(se.records, alignedCount, 1.0 / sampleRate, analyzedX, analyzedY, analyzedZ, tourNumber * alignedCount);

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
            return (analyzedX, analyzedY, analyzedZ, resultante, whiteLine, angle, f_rot, rpm, sampleRate);
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
                DisplayData(analyzedX, angle, f_rot, rpm, sampleRate, lstPeakXCompiled, plotTemporal, plotFFT, "X", Colors.Blue);
            if (chkShowY.Checked)
                DisplayData(analyzedY, angle, f_rot, rpm, sampleRate, lstPeakYCompiled, plotTemporal, plotFFT, "Y", Colors.Red);
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

            var dataX = MathHelper.GetSegments(analyzedX, whiteLine);
            var dataY = MathHelper.GetSegments(analyzedY, whiteLine);
            var dataZ = MathHelper.GetSegments(analyzedY, whiteLine);
            int[][] peakX = new int[dataX.dataSegments.Count][];
            int[][] peakY = new int[dataX.dataSegments.Count][];
            int[][] peakZ = new int[dataX.dataSegments.Count][];
            for (int i = 0; i < dataX.dataSegments.Count; i++)
            {
                double angleIncrement = 360.0 / dataX.dataSegments.Count;
                //convert the return x axis (0 to number of sample for 1 turn) to 360°
                peakX[i] = MathHelper.GetPeakPerTurn(dataX.dataSegments[i].ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();
                peakY[i] = MathHelper.GetPeakPerTurn(dataY.dataSegments[i].ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();
                peakZ[i] = MathHelper.GetPeakPerTurn(dataZ.dataSegments[i].ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();
            }
           

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
        }

        private void RefreshGyro()
        {
            var data = GetGlobalTourGyroSignal();
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

            var dataX = MathHelper.GetSegments(analyzedX, whiteLine);
            var dataY = MathHelper.GetSegments(analyzedY, whiteLine);
            var dataZ = MathHelper.GetSegments(analyzedY, whiteLine);
            int[][] peakX = new int[dataX.dataSegments.Count][];
            int[][] peakY = new int[dataX.dataSegments.Count][];
            int[][] peakZ = new int[dataX.dataSegments.Count][];
            for (int i = 0; i < dataX.dataSegments.Count; i++)
            {
                double angleIncrement = 360.0 / dataX.dataSegments.Count;
                //convert the return x axis (0 to number of sample for 1 turn) to 360°
                peakX[i] = MathHelper.GetPeakPerTurn(dataX.dataSegments[i].ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();
                peakY[i] = MathHelper.GetPeakPerTurn(dataY.dataSegments[i].ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();
                peakZ[i] = MathHelper.GetPeakPerTurn(dataZ.dataSegments[i].ToArray()).Select(x => (int)(x * angleIncrement)).ToArray();
            }


            lstPeakGyroX.Items.Clear();
            lstPeakGyroY.Items.Clear();
            lstPeakGyroZ.Items.Clear();
            lstPeakResultanteGyro.Items.Clear();
            double fftLimit = Convert.ToDouble(txtFFTLimit.Text);

            formsPlotGyro.Reset();
            formsPlotGyro.Multiplot.Reset();
            var plotTemporal = formsPlotGyro.Plot;
            var plotFFT = formsPlotGyro.Multiplot.AddPlot();
            double max = 0.001;
            if (chkShowX.Checked)
            {
                DisplayData(analyzedX, angle, f_rot, rpm, sampleRate, lstPeakGyroX, plotTemporal, plotFFT, "X", Colors.Blue);
                max = Math.Max(max, analyzedX.Max());
            }
            if (chkShowY.Checked)
            {
                DisplayData(analyzedY, angle, f_rot, rpm, sampleRate, lstPeakGyroY, plotTemporal, plotFFT, "Y", Colors.Red);
                max = Math.Max(max, analyzedY.Max());
            }
            if (chkShowZ.Checked)
            {
                DisplayData(analyzedZ, angle, f_rot, rpm, sampleRate, lstPeakGyroZ, plotTemporal, plotFFT, "Z", Colors.Yellow);
                max = Math.Max(max, analyzedZ.Max());
            }
            if (chkShowResultante.Checked)
            {
                DisplayData(resultante, angle, f_rot, rpm, sampleRate, lstPeakResultanteGyro, plotTemporal, plotFFT, "Resultante", Colors.DeepPink);
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
                        {
                            if (s.records.Count >= 1)
                            {
                                //check if previous timestamp is good
                                double oldTS = s.records[s.records.Count - 1].ms;
                                double actualTS = r.ms;
                                if (actualTS < oldTS)
                                    actualTS += (ushort.MaxValue);
                                actualTS *= 64; //64 us
                                oldTS *= 64;
                                if (((actualTS - oldTS) / 1000.0) < 5) //if more than 5ms diff ->skip the record
                                    s.records.Add(r);
                                else
                                    Console.WriteLine("Skip record due to invalid timestamp");
                            }
                            else
                                s.records.Add(r);
                        }
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
            
            selectedSections.Clear();
        }

        private void DisplayData(double[] data, double[] angle, double f_rot, double rpm, double sampleRate, ListBox lstPeak, Plot pltTemporal, Plot pltFFT, string axis, ScottPlot.Color c)
        {
            lstPeak.Items.Clear();
            MathHelper.AnalyzeAxisTemporal(axis, data, angle, sampleRate, lstPeak, c, pltTemporal, f_rot);
            FFTData dataFFT = EquilibrageHelper.CalculateFFT(data, sampleRate, cbxFFTSingle, chkDb.Checked, rpm, f_rot, chkClockwise.Checked);
            double fftLimit = Convert.ToDouble(txtFFTLimit.Text);
            double[] temporal = Enumerable.Range(0, data.Length)
                                .Select(i => (double)i)
                                .ToArray();
            double maxOriginal = data.Max(s => Math.Abs(s));
            double maxInverse = dataFFT.SignalFFTInverse.Max(s => Math.Abs(s));

            if (maxInverse > 0)
            {
                double scaleFactor = maxOriginal / maxInverse;
                dataFFT.SignalFFTInverse = dataFFT.SignalFFTInverse
                    .Select(x => x * scaleFactor)
                    .ToArray();
            }

            var sp = pltTemporal.Add.Scatter(temporal, dataFFT.SignalFFTInverse, ScottPlot.Colors.Orange);
            sp.LegendText = "FFT Inverse";
            sp.MarkerShape = MarkerShape.None;
            MathHelper.AnalyzeAxis(axis, dataFFT, sampleRate, lstPeak, c, pltFFT, f_rot, fftLimit);
        }

        private void Analyze(string csvFile, bool bDisplayGraph = true)
        {
            if (String.IsNullOrEmpty(csvFile))
                return;
            if (lstSectionSelector.CheckedIndices.Count < 3)
                return;

            selectedSections.Clear();
            iTotalRecordsInCSV = 0;

            foreach (int s in lstSectionSelector.CheckedIndices)
            {
                selectedSections.Add(loadedSections[s]);
                iTotalRecordsInCSV += loadedSections[s].records.Count;
            }
            currentAnalysisX = new AnalysisData() { csvFile = csvFile + " " + selectedSections.Select(r => r.Rpm).Min().ToString("F0") + "-" + selectedSections.Select(r => r.Rpm).Max().ToString("F0") };
            currentAnalysisY = new AnalysisData() { csvFile = csvFile + " " + selectedSections.Select(r => r.Rpm).Min().ToString("F0") + "-" + selectedSections.Select(r => r.Rpm).Max().ToString("F0") };
            currentAnalysisX.numberOfTurn = selectedSections.Count;
            currentAnalysisY.numberOfTurn = selectedSections.Count;

            //            lblFFTAnalysis.Text = String.Empty;
            if (bDisplayGraph)
            {
                formsPlotT1X.Plot.Clear(); formsPlotT1Y.Plot.Clear(); formsPlotT1O.Plot.Clear(); formsPlotT1I.Plot.Clear();
            }


            if (selectedSections.Count > 0)
            {
                if (bDisplayGraph)
                {
                    RefreshAnalysisCompiled();
                    RefreshAnalysisGlobal();
                    RefreshGyro();
                    lblRecordNumber.Text = "0";
                    RefreshXYZ(selectedSections[0]);
                }
                ExecuteAnalysis();
            }

            dataGridX.Rows.Add(currentAnalysisX.toArray());
            dataGridY.Rows.Add(currentAnalysisY.toArray());

            if (bDisplayGraph)
            {
                formsPlotT1X.Plot.Axes.SetLimits(0, 360); formsPlotT1Y.Plot.Axes.SetLimits(0, 360); formsPlotT1O.Plot.Axes.SetLimits(0, 360); formsPlotT1I.Plot.Axes.SetLimits(0, 360);
                formsPlotT1X.Refresh(); formsPlotT1Y.Refresh(); formsPlotT1O.Refresh(); formsPlotT1I.Refresh();
            }
        
        }
        private void ExecuteAnalysis()
        {
            if (chkExcludeWrongTurn.Checked)
            {
                List<double> lstFFTX = new List<double>();
                List<double> lstFFTY = new List<double>();
                List<double> lstRMSX = new List<double>();
                List<double> lstRMSY = new List<double>();
                List<double> lstLockInX = new List<double>();
                List<double> lstLockInY = new List<double>();
                List<double> lstPkPkX = new List<double>();
                List<double> lstPkPkY = new List<double>();
                //we first find tours to exclude
                List<int> zScoreToExclude = new List<int>();
                foreach (section s in selectedSections)
                {
                    var dataSingle = GetSingleTourSignal(s);
                    var dataSingleX = GetPhaseMagnitude(dataSingle.x, dataSingle.angle, dataSingle.sampleRate, dataSingle.rpm, dataSingle.f_rot);
                    var dataSingleY = GetPhaseMagnitude(dataSingle.y, dataSingle.angle, dataSingle.sampleRate, dataSingle.rpm, dataSingle.f_rot);
                    //exclude out of scope tours
                    lstFFTX.Add(dataSingleX.fund.Magnitude);
                    lstFFTY.Add(dataSingleY.fund.Magnitude);
                    lstRMSX.Add(dataSingleX.rms);
                    lstLockInX.Add(dataSingleX.magLockin);
                    lstPkPkX.Add(dataSingleX.pkpkFilter);
                    lstRMSY.Add(dataSingleY.rms);
                    lstLockInY.Add(dataSingleY.magLockin);
                    lstPkPkY.Add(dataSingleY.pkpkFilter);
                }
                var resultsX = StabilityAnalyzer.AnalyzeStability(lstRMSX, lstLockInX, lstPkPkX, lstFFTX);

                foreach (var result in resultsX)
                {
                    Console.WriteLine($"--- for X {result.Variable} ---");
                    Console.WriteLine($"Best exclusion index: {result.BestExclusionIndex}");
                    Console.WriteLine($"Original range: {result.OriginalRange:F2}");
                    Console.WriteLine($"Range after exclusion: {result.RangeAfterExclusion:F2}");
                    Console.WriteLine($"Z-score outliers: {string.Join(", ", result.ZScoreOutliers)}");
                    zScoreToExclude.AddRange(result.ZScoreOutliers.ToArray());
                    Console.WriteLine();
                }
                var resultsY = StabilityAnalyzer.AnalyzeStability(lstRMSY, lstLockInY, lstPkPkY, lstFFTY);

                foreach (var result in resultsY)
                {
                    Console.WriteLine($"--- for Y {result.Variable} ---");
                    Console.WriteLine($"Best exclusion index: {result.BestExclusionIndex}");
                    Console.WriteLine($"Original range: {result.OriginalRange:F2}");
                    Console.WriteLine($"Range after exclusion: {result.RangeAfterExclusion:F2}");
                    Console.WriteLine($"Z-score outliers: {string.Join(", ", result.ZScoreOutliers)}");
                    zScoreToExclude.AddRange(result.ZScoreOutliers.ToArray());
                    Console.WriteLine();
                }

                //var allIndices = resultsX.Concat(resultsY).Select(r => r.BestExclusionIndex).ToList();
                var allIndices = zScoreToExclude;
                 var exclusionCounts = allIndices
                     .GroupBy(i => i)
                     .ToDictionary(g => g.Key, g => g.Count());

                 // Affiche les scores
                 Console.WriteLine("=== Tour exclusion scores ===");
                 foreach (var kvp in exclusionCounts.OrderByDescending(k => k.Value))
                 {
                     Console.WriteLine($"Tour #{kvp.Key} → Score: {kvp.Value}");
                 }

                 // Liste des tours à exclure (si score ≥ 2)
                 var toursToExclude = exclusionCounts
                     .Where(kvp => kvp.Value >= 2)
                     .Select(kvp => kvp.Key)
                     .ToList();

                 Console.WriteLine("\nTours à exclure : " + string.Join(", ", toursToExclude));
                 toursToExclude.Sort((a, c) => c.CompareTo(a)); // Tri décroissant
                 
                //we remove the ZScore only
             //  var toursToExclude = zScoreToExclude.Distinct().OrderByDescending(n => n).ToList();
                foreach (int index in toursToExclude)
                {
                    if (index >= 0 && index < selectedSections.Count)
                        selectedSections.RemoveAt(index);
                }

                currentAnalysisX.numberOfTurn = selectedSections.Count;
                currentAnalysisY.numberOfTurn = selectedSections.Count;
            }
            //get signal, apply low pass filter with 5hz to get temporal peaks, then apply selected user signal to determine phase and magnitude
            var dataCompiled = GetCompiledTourSignal();
            var dataCompiledX = GetPhaseMagnitude(dataCompiled.x, dataCompiled.angle, dataCompiled.sampleRate, dataCompiled.rpm, dataCompiled.f_rot);
            var dataCompiledY = GetPhaseMagnitude(dataCompiled.y, dataCompiled.angle, dataCompiled.sampleRate, dataCompiled.rpm, dataCompiled.f_rot);

            var dataGlobal = GetGlobalTourSignal();
            var dataGlobalX = GetPhaseMagnitude(dataGlobal.x, dataGlobal.angle, dataGlobal.sampleRate, dataGlobal.rpm, dataGlobal.f_rot);
            var dataGlobalY = GetPhaseMagnitude(dataGlobal.y, dataGlobal.angle, dataGlobal.sampleRate, dataGlobal.rpm, dataGlobal.f_rot);
            if(chkUseGyroXforY.Checked)
            {
                var dataGyro = GetGlobalTourGyroSignal();
                dataGlobalY = GetPhaseMagnitude(dataGyro.x, dataGyro.angle, dataGyro.sampleRate, dataGyro.rpm, dataGyro.f_rot);
            
            }

            currentAnalysisX.coAngleTemporal = dataCompiledX.angleTemporal;
            currentAnalysisX.coAngleFFT = dataCompiledX.fund.Angle;
            currentAnalysisX.coMagAvg = dataCompiledX.fund.Magnitude;
            currentAnalysisX.coPkPkTT = dataCompiledX.pkpkTT;
            currentAnalysisX.coPkPkFilter = dataCompiledX.pkpkFilter;
            currentAnalysisX.coRMS = dataCompiledX.rms;
            currentAnalysisX.coMagPhaseLockin = dataCompiledX.magLockin;
            currentAnalysisX.coGoertzelMag = dataCompiledX.goertzelMag;
            currentAnalysisX.coGoertzelPhase = dataCompiledX.goertzelAngle;
            currentAnalysisY.coAngleTemporal = dataCompiledY.angleTemporal;
            currentAnalysisY.coAngleFFT = dataCompiledY.fund.Angle;
            currentAnalysisY.coMagAvg = dataCompiledY.fund.Magnitude;
            currentAnalysisY.coPkPkTT = dataCompiledY.pkpkTT;
            currentAnalysisY.coPkPkFilter = dataCompiledY.pkpkFilter;
            currentAnalysisY.coRMS = dataCompiledY.rms;
            currentAnalysisY.coMagPhaseLockin = dataCompiledY.magLockin;
            currentAnalysisY.coGoertzelMag = dataCompiledY.goertzelMag;
            currentAnalysisY.coGoertzelPhase = dataCompiledY.goertzelAngle;

            currentAnalysisX.gAngleTemporal = dataGlobalX.angleTemporal;
            currentAnalysisX.gAngleFFT = dataGlobalX.fund.Angle;
            currentAnalysisX.gMagAvg = dataGlobalX.fund.Magnitude;
            currentAnalysisX.gPkPkTT = dataGlobalX.pkpkTT;
            currentAnalysisX.gPkPkFilter = dataGlobalX.pkpkFilter;
            currentAnalysisX.gRMS = dataGlobalX.rms;
            currentAnalysisX.gMagPhaseLockin = dataGlobalX.magLockin / currentAnalysisX.numberOfTurn;
            //psd = (magnitude * magnitude) / analyzedY.Length;
            currentAnalysisX.gMagPSD = (dataGlobalX.fund.Magnitude * dataGlobalX.fund.Magnitude) / dataGlobal.x.Length;
            currentAnalysisX.gMagRatio = dataGlobalX.fund.Magnitude / currentAnalysisX.numberOfTurn;
            currentAnalysisX.gAlternateFFT = dataGlobalX.angleLockin;
            currentAnalysisX.gGoertzelMag = dataGlobalX.goertzelMag / currentAnalysisX.numberOfTurn; ;
            currentAnalysisX.gGoertzelPhase = dataGlobalX.goertzelAngle;


            currentAnalysisY.gAngleTemporal = dataGlobalY.angleTemporal;
            currentAnalysisY.gAngleFFT = dataGlobalY.fund.Angle;
            currentAnalysisY.gMagAvg = dataGlobalY.fund.Magnitude;
            currentAnalysisY.gPkPkTT = dataGlobalY.pkpkTT;
            currentAnalysisY.gPkPkFilter = dataGlobalY.pkpkFilter;
            currentAnalysisY.gRMS = dataGlobalY.rms;
            currentAnalysisY.gMagPhaseLockin = dataGlobalY.magLockin / currentAnalysisY.numberOfTurn;
            //psd = (magnitude * magnitude) / analyzedY.Length;
            currentAnalysisY.gMagPSD = (dataGlobalY.fund.Magnitude * dataGlobalY.fund.Magnitude) / dataGlobal.y.Length;
            currentAnalysisY.gMagRatio = dataGlobalY.fund.Magnitude / currentAnalysisY.numberOfTurn;
            currentAnalysisY.gAlternateFFT = dataGlobalY.angleLockin;
            currentAnalysisY.gGoertzelMag = dataGlobalY.goertzelMag/ currentAnalysisY.numberOfTurn;
            currentAnalysisY.gGoertzelPhase = dataGlobalY.goertzelAngle;

            List<double> lstAngleFFTX = new List<double>();
            List<double> lstAngleTemporalX = new List<double>();
            List<double> lstAngleFFTY = new List<double>();
            List<double> lstAngleTemporalY = new List<double>();


            //we first find tours to exclude
            foreach (section s in selectedSections)
            {
                var dataSingle = GetSingleTourSignal(s);
                var dataSingleX = GetPhaseMagnitude(dataSingle.x, dataSingle.angle, dataSingle.sampleRate, dataSingle.rpm, dataSingle.f_rot);
                var dataSingleY = GetPhaseMagnitude(dataSingle.y, dataSingle.angle, dataSingle.sampleRate, dataSingle.rpm, dataSingle.f_rot);
             

                lstAngleFFTX.Add(dataSingleX.fund.Angle); lstAngleTemporalX.Add(dataSingleX.angleTemporal);
                lstAngleFFTY.Add(dataSingleY.fund.Angle); lstAngleTemporalY.Add(dataSingleY.angleTemporal);

                currentAnalysisX.ttMagAvg += dataSingleX.fund.Magnitude;
                currentAnalysisX.ttPkPkTT += dataSingleX.pkpkTT;
                currentAnalysisX.ttPkPkFilter += dataSingleX.pkpkFilter;
                currentAnalysisX.ttRMS += dataSingleX.rms;
                currentAnalysisX.ttMagPhaseLockin += dataSingleX.magLockin;
                currentAnalysisX.ttGoertzelMag += dataSingleX.goertzelMag;
                currentAnalysisY.ttMagAvg += dataSingleY.fund.Magnitude;
                currentAnalysisY.ttPkPkTT += dataSingleY.pkpkTT;
                currentAnalysisY.ttPkPkFilter += dataSingleY.pkpkFilter;
                currentAnalysisY.ttRMS += dataSingleY.rms;
                currentAnalysisY.ttMagPhaseLockin += dataSingleY.magLockin;
                currentAnalysisY.ttGoertzelMag += dataSingleY.goertzelMag;
            }
        
            currentAnalysisX.ttAngleFFT = MathHelper.CalculateMeanAngle(lstAngleFFTX.ToArray());
            currentAnalysisX.ttAngleTemporal = MathHelper.CalculateMeanAngle(lstAngleTemporalX.ToArray());
            currentAnalysisY.ttAngleFFT = MathHelper.CalculateMeanAngle(lstAngleFFTY.ToArray());
            currentAnalysisY.ttAngleTemporal = MathHelper.CalculateMeanAngle(lstAngleTemporalY.ToArray());

            currentAnalysisX.ttMagAvg /= currentAnalysisX.numberOfTurn;
            currentAnalysisX.ttPkPkTT /= currentAnalysisX.numberOfTurn;
            currentAnalysisX.ttPkPkFilter /= currentAnalysisX.numberOfTurn;
            currentAnalysisX.ttRMS /= currentAnalysisX.numberOfTurn;
            currentAnalysisX.ttMagPhaseLockin /= currentAnalysisX.numberOfTurn;
            currentAnalysisX.ttGoertzelMag /= currentAnalysisX.numberOfTurn;
            currentAnalysisY.ttMagAvg /= currentAnalysisY.numberOfTurn;
            currentAnalysisY.ttPkPkTT /= currentAnalysisY.numberOfTurn;
            currentAnalysisY.ttPkPkFilter /= currentAnalysisY.numberOfTurn;
            currentAnalysisY.ttRMS /= currentAnalysisY.numberOfTurn;
            currentAnalysisY.ttMagPhaseLockin /= currentAnalysisY.numberOfTurn;
            currentAnalysisY.ttGoertzelMag /= currentAnalysisY.numberOfTurn;

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
            currentAnalysisX.gAlternateFFT = (currentAnalysisX.gAlternateFFT + xCorrect) % 360;
            currentAnalysisX.gGoertzelPhase = (currentAnalysisX.gGoertzelPhase + xCorrect) % 360;
            currentAnalysisX.coGoertzelPhase = (currentAnalysisX.coGoertzelPhase + xCorrect) % 360;

            currentAnalysisY.ttAngleFFT = (currentAnalysisY.ttAngleFFT + yCorrect) % 360;
            currentAnalysisY.ttAngleTemporal = (currentAnalysisY.ttAngleTemporal + yCorrectTemp) % 360;
            currentAnalysisY.coAngleFFT = (currentAnalysisY.coAngleFFT + yCorrect) % 360;
            currentAnalysisY.coAngleTemporal = (currentAnalysisY.gAngleFFT + yCorrectTemp) % 360;
            currentAnalysisY.gAngleFFT = (currentAnalysisY.gAngleFFT + yCorrect) % 360;
            currentAnalysisY.gAngleTemporal = (currentAnalysisY.gAngleTemporal + yCorrectTemp) % 360;
            currentAnalysisY.gAlternateFFT = (currentAnalysisY.gAlternateFFT + yCorrect) % 360;
            currentAnalysisY.gGoertzelPhase = (currentAnalysisY.gGoertzelPhase + yCorrect) % 360;
            currentAnalysisY.coGoertzelPhase = (currentAnalysisY.coGoertzelPhase + yCorrect) % 360;

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
            
            var dynamicGlobal = EquilibrageHelper.EstimateDynamicImbalanceCorrection(currentAnalysisX.gAngleFFT, currentAnalysisX.gMagRatio, currentAnalysisY.gAngleFFT, currentAnalysisY.gMagRatio);
            var dynamicCompiled = EquilibrageHelper.EstimateDynamicImbalanceCorrection(currentAnalysisX.coAngleFFT, currentAnalysisX.coMagAvg, currentAnalysisY.coAngleFFT, currentAnalysisY.coMagAvg);
            var dynamicTT = EquilibrageHelper.EstimateDynamicImbalanceCorrection(currentAnalysisX.ttAngleFFT, currentAnalysisX.ttMagAvg, currentAnalysisY.ttAngleFFT, currentAnalysisY.ttMagAvg);
            currentAnalysisX.gCorrection = dynamicGlobal.AngleOuterDeg;
            currentAnalysisX.coCorrection = dynamicCompiled.AngleOuterDeg;
            currentAnalysisX.ttCorrection = dynamicTT.AngleOuterDeg;
            currentAnalysisY.gCorrection = dynamicGlobal.AngleInnerDeg;
            currentAnalysisY.coCorrection = dynamicCompiled.AngleInnerDeg;
            currentAnalysisY.ttCorrection = dynamicTT.AngleInnerDeg;
            formsPlotT1I.Plot.Add.VerticalLine(dynamicGlobal.AngleInnerDeg, color: Colors.Green, width: 3);
            formsPlotT1O.Plot.Add.VerticalLine(dynamicGlobal.AngleOuterDeg, color: Colors.Green, width: 3);
            formsPlotT1I.Plot.Add.VerticalLine(dynamicCompiled.AngleInnerDeg, color: Colors.Red, width: 3);
            formsPlotT1O.Plot.Add.VerticalLine(dynamicCompiled.AngleOuterDeg, color: Colors.Red, width: 3);
            formsPlotT1I.Plot.Add.VerticalLine(dynamicTT.AngleInnerDeg, color: Colors.Black, width: 3);
            formsPlotT1O.Plot.Add.VerticalLine(dynamicTT.AngleOuterDeg, color: Colors.Black, width: 3);

        }

        private (double angleTemporal, double goertzelAngle, double goertzelMag, double angleLockin, double magLockin, double pkpkTT, double pkpkFilter, double rms, Fundamentale fund) GetPhaseMagnitude(double[] data, double[] angle, double sampleRate, double rpm, double f_rot)
        {
            double angleTemporal = 0;
            var l = new LowPassFilter(5, sampleRate);
            var dataFiltered = LowPassFilter.ApplyZeroPhase(data, l);
            if (chkRemoveDC.Checked)
                dataFiltered = LowPassFilter.RemoveDCOffset(dataFiltered);
            //apply gain on signal
            var gain = Convert.ToDouble(txtGain.Text);
            dataFiltered = dataFiltered.Select(r => r * gain).ToArray();
            var peaks = MathHelper.GetPeakPerTurn(dataFiltered);
            List<double> lstAnglePeaks = new List<double>();
            foreach (var p in peaks)
                lstAnglePeaks.Add(angle[p]);

            angleTemporal = MathHelper.CalculateMeanAngle(lstAnglePeaks.ToArray());

            //fake data
            double[] y = new double[1];
            double[] z = new double[1];
            double[] resultante = new double[1];
            ApplyFilters(sampleRate, f_rot, ref data, ref y, ref z, ref resultante);
            var res = EquilibrageHelper.ComputeLockInPhase(data, f_rot, sampleRate,true,true);
            var go = EquilibrageHelper.GoertzelEstimator(data, f_rot, sampleRate);
            FFTData dataFFT = EquilibrageHelper.CalculateFFT(data, sampleRate, cbxFFTSingle, chkDb.Checked, rpm, f_rot, chkClockwise.Checked);
            var fund = EquilibrageHelper.GetFundamentalPhase(dataFFT.Frequence, dataFFT.Magnitude, dataFFT.AngleDeg, f_rot);
            return (angleTemporal,go.Phase, go.Magnitude, res.phase, res.amp, dataFiltered.Max() - dataFiltered.Min(), data.Max() - data.Min(), Statistics.RootMeanSquare(data), fund);
        }

        private void btnExecuteAnalysis_Click(object sender, EventArgs e)
        {
            List<double> angleX = new List<double>();
            List<double> angleY = new List<double>();
            List<double> magnitudeX = new List<double>();
            List<double> magnitudeY = new List<double>();
            List<double> lockinX = new List<double>();
            List<double> lockinY = new List<double>();
            lstAngleXAnalysis.Clear();
            lstAngleYAnalysis.Clear();
            btnUnselectAll_Click(null, EventArgs.Empty);
            formsPlotT1X.Plot.Clear(); formsPlotT1Y.Plot.Clear(); formsPlotT1O.Plot.Clear(); formsPlotT1I.Plot.Clear();
            for (int i = 0; i < lstSelectToursAnalysis.CheckedItems.Count;i++)
            {
                var sSelected = lstSelectToursAnalysis.CheckedItems[i];
                btnUnselectAll_Click(null, EventArgs.Empty);
                
                if (sSelected.ToString().StartsWith("200-210"))
                {
                    btn200210_Click(null, EventArgs.Empty);
                }
                else if (sSelected.ToString().StartsWith("210-220"))
                {
                    btn210220_Click(null, EventArgs.Empty);
                }
                else if (sSelected.ToString().StartsWith("220-230"))
                {
                    btn220230_Click(null, EventArgs.Empty);
                }
                else if (sSelected.ToString().StartsWith("230-240"))
                {
                    btn230240_Click(null, EventArgs.Empty);
                }
                else if (sSelected.ToString().StartsWith("240-250"))
                {
                    btn240250_Click(null, EventArgs.Empty);
                }
                
                currentAnalysisX = currentAnalysisY = null;
                Analyze(sLastCSV, false);
                if (currentAnalysisX == null)
                    continue;
                var data = GetDataForCalculation();
                angleX.Add(data.angleX);
                magnitudeX.Add(data.magX);
                angleY.Add(data.angleY);
                magnitudeY.Add(data.magY);
                lockinX.Add(data.lockinX);
                lockinY.Add(data.lockinY);
                for (int j = 0; j < lstSelectToursAnalysis.Items.Count;j++)
                {
                    if(lstSelectToursAnalysis.Items[j].ToString() == sSelected.ToString())
                    {
                        lstSelectToursAnalysis.Items[j] = sSelected.ToString().Substring(0,7) + $" AngX {data.angleX.ToString("F0")} M {data.magX.ToString("F1")} L {data.lockinX.ToString("F1")} AngY {data.angleY.ToString("F0")} M {data.magY.ToString("F1")} L {data.lockinY.ToString("F1")}";
                    }
                }
            }
            if (angleX.Count > 0)
            {
                var statAngleX = MathHelper.ComputeStatistics(angleX);
                var statAngleY = MathHelper.ComputeStatistics(angleY);
                var statMagX = MathHelper.ComputeStatistics(magnitudeX);
                var statMagY = MathHelper.ComputeStatistics(magnitudeY);
                var statLockinX = MathHelper.ComputeStatistics(lockinX);
                var statLockinY = MathHelper.ComputeStatistics(lockinY);
                lstStats.Items.Clear();
                lstStats.Items.Add("AngleX " + statAngleX.ToString());
                lstStats.Items.Add("AngleY " + statAngleY.ToString());
                lstStats.Items.Add("MagX " + statMagX.ToString());
                lstStats.Items.Add("MagY " + statMagY.ToString());
                lstStats.Items.Add("LockinX " + statLockinX.ToString());
                lstStats.Items.Add("LockinY " + statLockinY.ToString());
                lblAngleXStat.Text = $"X - Angle:{angleX.Min().ToString("F0")}-{angleX.Max().ToString("F0")}";
                lblAngleYStat.Text = $"Y - Angle:{angleY.Min().ToString("F0")}-{angleY.Max().ToString("F0")}";
                txtAngleXCalc.Text = MathHelper.CalculateMeanAngle(angleX.ToArray()).ToString("F0");
                txtAngleYCalc.Text = MathHelper.CalculateMeanAngle(angleY.ToArray()).ToString("F0");
                txtMagnitudeX.Text = magnitudeX.Average().ToString("F2");
                txtMagnitudeY.Text = magnitudeY.Average().ToString("F2");
                txtLockinX.Text = lockinX.Average().ToString("F2");
                txtLockinY.Text = lockinY.Average().ToString("F2");

                btnCalculateCorrection_Click(null, EventArgs.Empty);
                
            }
            formsPlotT1X.Plot.Axes.SetLimits(0, 360); formsPlotT1Y.Plot.Axes.SetLimits(0, 360); formsPlotT1O.Plot.Axes.SetLimits(0, 360); formsPlotT1I.Plot.Axes.SetLimits(0, 360);
            formsPlotT1X.Refresh(); formsPlotT1Y.Refresh(); formsPlotT1O.Refresh(); formsPlotT1I.Refresh();

        }

        private void btnCalculateCorrection_Click(object sender, EventArgs e)
        {
            var magX = Convert.ToDouble(txtMagnitudeX.Text);
            var magY = Convert.ToDouble(txtMagnitudeY.Text);
            
            var dynamicGlobal = EquilibrageHelper.EstimateDynamicImbalanceCorrection(Convert.ToDouble(txtAngleXCalc.Text), Convert.ToDouble(txtMagnitudeX.Text), Convert.ToDouble(txtAngleYCalc.Text), Convert.ToDouble(txtMagnitudeY.Text));
            lblAngleXCorrect.Text = "Mag:   Outer Angle: " + dynamicGlobal.AngleOuterDeg.ToString("F0");
            lblAngleYCorrect.Text = "Mag:   Inner Angle: " + dynamicGlobal.AngleInnerDeg.ToString("F0");
            try
            {
                double mass1 = Convert.ToDouble(txtMagGrams1.Text);   
                double magX1 = Convert.ToDouble(txtXMagG1.Text);
                double magY1 = Convert.ToDouble(txtYMag1.Text);
                
                double magXBalanced = Convert.ToDouble(txtXMagBalanced.Text);
                double magYBalanced = Convert.ToDouble(txtYMagBalanced.Text);

                double actualMagX = Convert.ToDouble(txtMagnitudeX.Text);
                double angleX = Convert.ToDouble(txtAngleXCalc.Text);
                double actualMagY = Convert.ToDouble(txtMagnitudeY.Text);
                double angleY = Convert.ToDouble(txtAngleYCalc.Text);

                var kx1 = EquilibrageHelper.CalculateGrowthConstant(magXBalanced, magX1, mass1);
                var ky1 = EquilibrageHelper.CalculateGrowthConstant(magYBalanced, magY1, mass1);
                var massX = EquilibrageHelper.EstimateCorrectiveMass(magX, magXBalanced, kx1);
                var massY = EquilibrageHelper.EstimateCorrectiveMass(magY, magYBalanced, ky1);

                var res = EquilibrageHelper.EstimateDynamicBalancing(actualMagX, angleX, actualMagY, angleY, kx1, ky1);
                //   var kx = EquilibrageHelper.CalculateAttenuationConstantFrom3Points(Convert.ToDouble(txtXMagBalanced.Text), Convert.ToDouble(txtXMagG1.Text), Convert.ToDouble(txtMagGrams1.Text), Convert.ToDouble(txtXMagG2.Text), Convert.ToDouble(txtMagGrams2.Text));
                //   var ky = EquilibrageHelper.CalculateAttenuationConstantFrom3Points(Convert.ToDouble(txtYMagBalanced.Text), Convert.ToDouble(txtYMag1.Text), Convert.ToDouble(txtMagGrams1.Text), Convert.ToDouble(txtYMag2.Text), Convert.ToDouble(txtMagGrams2.Text));
                //   var res = EquilibrageHelper.EstimateDynamicBalancing(Convert.ToDouble(txtMagnitudeX.Text), Convert.ToDouble(txtAngleXCalc.Text), Convert.ToDouble(txtMagnitudeY.Text), Convert.ToDouble(txtAngleYCalc.Text), kx1, ky);
                

                var kx2 = EquilibrageHelper.CalculateAttenuationConstant(magXBalanced, magX1, mass1);
                var ky2 = EquilibrageHelper.CalculateAttenuationConstant(magYBalanced, magY1, mass1);
                var res2 = EquilibrageHelper.EstimateDynamicBalancing2(magX, angleX, magY, angleY, kx2, ky2);
                var kx3 = EquilibrageHelper.CalculateGrowthConstantLinear(magXBalanced, magX1, mass1);
                var ky3 = EquilibrageHelper.CalculateGrowthConstantLinear(magYBalanced, magY1, mass1);
                var massLX = EquilibrageHelper.EstimateCorrectiveMassLinear(magX, magXBalanced, kx3);
                var massLY = EquilibrageHelper.EstimateCorrectiveMassLinear(magY, magYBalanced, ky3);
                lblAngleXCorrect.Text += " Mass:" + res2.MassInner.ToString("F0") + " MassLog " + massX.ToString("F0") + " MassLinear " + massLX.ToString("F0");
                lblAngleYCorrect.Text += " Mass:" + res2.MassOuter.ToString("F0") + " MassLog " + massY.ToString("F0") + " MassLinear " + massLY.ToString("F0");
            }
            catch
            { }
            var lockinX = Convert.ToDouble(txtLockinX.Text);
            var lockinY = Convert.ToDouble(txtLockinY.Text);
            dynamicGlobal = EquilibrageHelper.EstimateDynamicImbalanceCorrection(Convert.ToDouble(txtAngleXCalc.Text), lockinX, Convert.ToDouble(txtAngleYCalc.Text), lockinY);
            lblAngleXCorrect.Text += "\r\nLockin:Outer Angle: " + dynamicGlobal.AngleOuterDeg.ToString("F0");
            lblAngleYCorrect.Text += "\r\nLockin:Inner Angle: " + dynamicGlobal.AngleInnerDeg.ToString("F0");
            try
            {
                double mass1 = Convert.ToDouble(txtMagGrams1.Text);
                double magX1 = Convert.ToDouble(txtXLockinG1.Text);
                double magY1 = Convert.ToDouble(txtYLockin1.Text);

                double magXBalanced = Convert.ToDouble(txtXLockinBalanced.Text);
                double magYBalanced = Convert.ToDouble(txtYLockinBalanced.Text);

                double actualMagX = lockinX;
                double angleX = Convert.ToDouble(txtAngleXCalc.Text);
                double actualMagY = lockinY;
                double angleY = Convert.ToDouble(txtAngleYCalc.Text);

                var kx1 = EquilibrageHelper.CalculateGrowthConstant(magXBalanced, magX1, mass1);
                var ky1 = EquilibrageHelper.CalculateGrowthConstant(magYBalanced, magY1, mass1);
                var massX = EquilibrageHelper.EstimateCorrectiveMass(actualMagX, magXBalanced, kx1);
                var massY = EquilibrageHelper.EstimateCorrectiveMass(actualMagY, magYBalanced, ky1);

                var res = EquilibrageHelper.EstimateDynamicBalancing(actualMagX, angleX, actualMagY, angleY, kx1, ky1);
                //   var kx = EquilibrageHelper.CalculateAttenuationConstantFrom3Points(Convert.ToDouble(txtXMagBalanced.Text), Convert.ToDouble(txtXMagG1.Text), Convert.ToDouble(txtMagGrams1.Text), Convert.ToDouble(txtXMagG2.Text), Convert.ToDouble(txtMagGrams2.Text));
                //   var ky = EquilibrageHelper.CalculateAttenuationConstantFrom3Points(Convert.ToDouble(txtYMagBalanced.Text), Convert.ToDouble(txtYMag1.Text), Convert.ToDouble(txtMagGrams1.Text), Convert.ToDouble(txtYMag2.Text), Convert.ToDouble(txtMagGrams2.Text));
                //   var res = EquilibrageHelper.EstimateDynamicBalancing(Convert.ToDouble(txtMagnitudeX.Text), Convert.ToDouble(txtAngleXCalc.Text), Convert.ToDouble(txtMagnitudeY.Text), Convert.ToDouble(txtAngleYCalc.Text), kx1, ky);


                var kx2 = EquilibrageHelper.CalculateAttenuationConstant(magXBalanced, magX1, mass1);
                var ky2 = EquilibrageHelper.CalculateAttenuationConstant(magYBalanced, magY1, mass1);
                var res2 = EquilibrageHelper.EstimateDynamicBalancing2(actualMagX, angleX, actualMagY, angleY, kx2, ky2);
                var kx3 = EquilibrageHelper.CalculateGrowthConstantLinear(magXBalanced, magX1, mass1);
                var ky3 = EquilibrageHelper.CalculateGrowthConstantLinear(magYBalanced, magY1, mass1);
                var massLX = EquilibrageHelper.EstimateCorrectiveMassLinear(actualMagX, magXBalanced, kx3);
                var massLY = EquilibrageHelper.EstimateCorrectiveMassLinear(actualMagY, magYBalanced, ky3);
                lblAngleXCorrect.Text += " Mass:" + res2.MassInner.ToString("F0") + " MassLog " + massX.ToString("F0") + " MassLinear " + massLX.ToString("F0");
                lblAngleYCorrect.Text += " Mass:" + res2.MassOuter.ToString("F0") + " MassLog " + massY.ToString("F0") + " MassLinear " + massLY.ToString("F0");
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

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            try
            {
                lstSelectToursAnalysis.Items.Clear();
                lstSelectToursAnalysis.Items.Add("200-210", true);
                lstSelectToursAnalysis.Items.Add("210-220", true);
                lstSelectToursAnalysis.Items.Add("220-230", true);
                lstSelectToursAnalysis.Items.Add("230-240", true);
                lstSelectToursAnalysis.Items.Add("240-250", true);
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
                if (plottable.Count() == 0)
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
                                if (sp2.LegendText == "ANGLE" + sp.LegendText)
                                {
                                    sData += $"{sp.LegendText} Freq: {nearest.X} Angle: {sp2.GetIDataSource().GetY(nearest.Index)}\r\n";
                                }
                            }
                        }
                    }
                }
            }
            toolTip1.Show(sData, plt, e.Location, 3000);
        }

       

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CalibGrams1 = Convert.ToDouble(txtMagGrams1.Text);
            Properties.Settings.Default.XMagBalanced = Convert.ToDouble(txtXMagBalanced.Text);
            Properties.Settings.Default.YMagBalance = Convert.ToDouble(txtYMagBalanced.Text);
            Properties.Settings.Default.XMag1 = Convert.ToDouble(txtXMagG1.Text);
            Properties.Settings.Default.YMag1 = Convert.ToDouble(txtYMag1.Text);
            Properties.Settings.Default.XAngleCorrect = Convert.ToDouble(txtCorrectAngleX.Text);
            Properties.Settings.Default.YAngleCorrect = Convert.ToDouble(txtCorrectAngleY.Text);
            Properties.Settings.Default.XAngleCorrectTemporal = Convert.ToDouble(txtCorrectXTemporal.Text);
            Properties.Settings.Default.YAngleCorrectTemporal = Convert.ToDouble(txtCorrectYTemporal.Text);
            Properties.Settings.Default.LowPassChecked = chkLowPassFilter.Checked;
            Properties.Settings.Default.PassBandChecked = chkPassband.Checked;
            Properties.Settings.Default.FilterSelect = cbxFilterTypes.SelectedItem.ToString();
            Properties.Settings.Default.FilterOrder = Convert.ToInt32(txtFilterOrder.Text);
            Properties.Settings.Default.FilterSmooth = cbxSmoothing.SelectedItem.ToString();
            Properties.Settings.Default.AbsChecked = chkAbsolute.Checked;
            Properties.Settings.Default.SumChecked = chkSum.Checked;
            Properties.Settings.Default.RemoveDCChecked = chkRemoveDC.Checked;
            Properties.Settings.Default.dbChecked = chkDb.Checked;
            Properties.Settings.Default.Gain = Convert.ToInt32(txtGain.Text);
            Properties.Settings.Default.OrderTrackingChecked = chkOrderTracking.Checked;
            Properties.Settings.Default.FFTSingle = cbxFFTSingle.SelectedItem.ToString();
            Properties.Settings.Default.FFTGlobal = cbxFFT.SelectedItem.ToString();
            Properties.Settings.Default.AngleData = cbxAngleData.SelectedItem.ToString();
            
            Properties.Settings.Default.ClockwiseRotating = chkClockwise.Checked;
            Properties.Settings.Default.XLockinBalanced = Convert.ToDouble(txtXLockinBalanced.Text);
            Properties.Settings.Default.YLockinBalanced = Convert.ToDouble(txtYLockinBalanced.Text);
            Properties.Settings.Default.XLockinMag = Convert.ToDouble(txtXLockinG1.Text);
            Properties.Settings.Default.YLockinMag = Convert.ToDouble(txtYLockin1.Text);
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
                new DataGridViewColumn(){ HeaderText = "Global Correction", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Correction", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Correction", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag PSD", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag Ratio" , CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Goertzel Mag", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Amplitude Lockin", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Mag AVG", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Mag AVG", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},

                new DataGridViewColumn(){ HeaderText = "Global Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Angle Lock-in", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Angle Temporal", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Angle Temporal", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Angle Temporal", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Pk-Pk Filter", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Pk-Pk Filter", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Pk-Pk Filter", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Pk-Pk LowPass5hz", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Pk-PkLowPass5hz", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Pk-PkLowPass5hz", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                
                new DataGridViewColumn(){ HeaderText = "Compiled Amplitude Lockin", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Amplitude Lockin", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                
                new DataGridViewColumn(){ HeaderText = "Compiled Goertzel Mag", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Goertzel Mag", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Goertzel Phase", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Goertzel Phase", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Goertzel Phase", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global RMS", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled RMS", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn RMS", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
            };
            List<DataGridViewColumn> dgvcY = new List<DataGridViewColumn>()
            {
                        new DataGridViewColumn(){ HeaderText = "CSV File", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells} ,
                new DataGridViewColumn(){ HeaderText = "Selected Nb of turn", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Correction", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Correction", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Correction", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag PSD", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Mag Ratio" , CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Goertzel Mag", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Amplitude Lockin", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Mag AVG", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Mag AVG", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},

                new DataGridViewColumn(){ HeaderText = "Global Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Angle Lock-in", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Angle FFT", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Angle Temporal", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Angle Temporal", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Angle Temporal", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Pk-Pk Filter", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Pk-Pk Filter", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Pk-Pk Filter", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Pk-Pk LowPass5hz", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Pk-PkLowPass5hz", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Pk-PkLowPass5hz", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},

                new DataGridViewColumn(){ HeaderText = "Compiled Amplitude Lockin", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Amplitude Lockin", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                
                new DataGridViewColumn(){ HeaderText = "Compiled Goertzel Mag", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Goertzel Mag", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Global Goertzel Phase", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Compiled Goertzel Phase", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
                new DataGridViewColumn(){ HeaderText = "Turn-Turn Goertzel Phase", CellTemplate = new DataGridViewTextBoxCell(), AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells},
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


        private (double angleX, double magX, double angleY, double magY, double lockinX, double lockinY) GetDataForCalculation()
        {
            double angleX, angleY, magX, magY, lockinX, lockinY;
            switch (cbxAngleData.SelectedItem.ToString())
            {
                case "Global FFT":
                    angleX = currentAnalysisX.gAngleFFT;
                    angleY = currentAnalysisY.gAngleFFT;
                    break;
                case "Global Temporal":
                    angleX = currentAnalysisX.gAngleTemporal;
                    angleY = currentAnalysisY.gAngleTemporal;
                    break;
                case "Compiled FFT":
                    angleX = currentAnalysisX.coAngleFFT;
                    angleY = currentAnalysisY.coAngleFFT;
                    break;
                case "Compiled Temporal":
                    angleX = currentAnalysisX.coAngleTemporal;
                    angleY = currentAnalysisY.coAngleTemporal;
                    break;
                case "Turn by Turn FFT":
                    angleX = currentAnalysisX.ttAngleFFT;
                    angleY = currentAnalysisY.ttAngleFFT;
                    break;
                case "Turn by Turn Temporal":
                    angleX = currentAnalysisX.ttAngleTemporal;
                    angleY = currentAnalysisY.ttAngleTemporal;
                    break;
                case "Global lock-in":
                    angleX = currentAnalysisX.gAlternateFFT;
                    angleY = currentAnalysisY.gAlternateFFT;
                    break;
                case "AVG FFT-Lockin":
                    angleX = MathHelper.CalculateMeanAngle(new double[] { currentAnalysisX.gAlternateFFT, currentAnalysisX.gAngleFFT });
                    angleY = MathHelper.CalculateMeanAngle(new double[] { currentAnalysisY.gAlternateFFT, currentAnalysisY.gAngleFFT });
                    break;
                case "Global Goertzel":
                    angleX = currentAnalysisX.gGoertzelPhase;
                    angleY = currentAnalysisY.gGoertzelPhase;
                    break;
                default:
                    angleX = angleY = 0;
                    break;
            }
            lockinX = currentAnalysisX.gMagPhaseLockin;
            lockinY = currentAnalysisY.gMagPhaseLockin;
            magX = currentAnalysisX.gMagRatio;
            magY = currentAnalysisY.gMagRatio;
            
            return (angleX, magX, angleY, magY, lockinX, lockinY);
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
        public double gCorrection;
        public double coCorrection;
        public double ttCorrection;
        public double gPkPkTT;
        public double gPkPkFilter;
        public double gMagAvg;
        public double gMagPhaseLockin;
        public double coMagPhaseLockin;
        public double ttMagPhaseLockin;
        public double gMagPSD;
        public double gMagRatio;
        public double gAngleFFT;
        public double gAngleTemporal;
        public double gAlternateFFT;
        public double ttPkPkTT;
        public double ttPkPkFilter;
        public double ttMagAvg;
        public double ttAngleFFT;
        public double ttAngleTemporal;

        public double coPkPkTT;
        public double coPkPkFilter;
        public double coMagAvg;
        public double coAngleFFT;
        public double coAngleTemporal;

        public double gRMS;
        public double ttRMS;
        public double coRMS;

        public double gGoertzelPhase;
        public double coGoertzelPhase;
        public double ttGoertzelPhase;
        public double gGoertzelMag;
        public double coGoertzelMag;
        public double ttGoertzelMag;
        public String[] toArray()
        {
            List<String> s = new List<string>()
            {
                Path.GetFileName(csvFile),
                numberOfTurn.ToString(),
                gCorrection.ToString("F4"),
                coCorrection.ToString("F4"),
                ttCorrection.ToString("F4"),
                gMagAvg.ToString("F4"),
                gMagPSD.ToString("F4"),
                gMagRatio.ToString("F4"),
                gGoertzelMag.ToString("F4"),
                gMagPhaseLockin.ToString("F4"),
                coMagAvg.ToString("F4"),
                ttMagAvg.ToString("F4"),
                gAngleFFT.ToString("F4"),
                gAlternateFFT.ToString("F4"),
                coAngleFFT.ToString("F4"),
                ttAngleFFT.ToString("F4"),
                gAngleTemporal.ToString("F4"),
                coAngleTemporal.ToString("F4"),
                ttAngleTemporal.ToString("F4"),
                gPkPkFilter.ToString("F4"),
                coPkPkFilter.ToString("F4"),
                ttPkPkFilter.ToString("F4"),
                gPkPkTT.ToString("F4"),
                coPkPkTT.ToString("F4"),
                ttPkPkTT.ToString("F4"),
                
                coMagPhaseLockin.ToString("F4"),
                ttMagPhaseLockin.ToString("F4"),
                
                coGoertzelMag.ToString("F4"),
                ttGoertzelMag.ToString("F4"),
                gGoertzelPhase.ToString("F4"),
                coGoertzelPhase.ToString("F4"),
                ttGoertzelPhase.ToString("F4"),
                gRMS.ToString("F4"),
                coRMS.ToString("F4"),
                ttRMS.ToString("F4")
            };
            return s.ToArray();
        }
    }
}
