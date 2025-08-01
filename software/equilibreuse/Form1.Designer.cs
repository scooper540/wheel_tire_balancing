﻿namespace equilibreuse
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.txtCom = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRPM = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnStartCapture = new System.Windows.Forms.Button();
            this.btnEndCapture = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMinRPM = new System.Windows.Forms.TextBox();
            this.txtMaxRPM = new System.Windows.Forms.TextBox();
            this.lblStats = new System.Windows.Forms.Label();
            this.formsPlotX = new ScottPlot.FormsPlot();
            this.formsPlotY = new ScottPlot.FormsPlot();
            this.formsPlotZ = new ScottPlot.FormsPlot();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.formsPlotAnalysis = new ScottPlot.FormsPlot();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.lblRecordNumber = new System.Windows.Forms.Label();
            this.chkShowX = new System.Windows.Forms.CheckBox();
            this.chkShowY = new System.Windows.Forms.CheckBox();
            this.chkShowZ = new System.Windows.Forms.CheckBox();
            this.chkFFT = new System.Windows.Forms.CheckBox();
            this.cbxFFT = new System.Windows.Forms.ComboBox();
            this.chkAbsolute = new System.Windows.Forms.CheckBox();
            this.lstPeakX = new System.Windows.Forms.ListBox();
            this.lstPeakY = new System.Windows.Forms.ListBox();
            this.lstPeakZ = new System.Windows.Forms.ListBox();
            this.lstPeakZCompiled = new System.Windows.Forms.ListBox();
            this.lstPeakYCompiled = new System.Windows.Forms.ListBox();
            this.lstPeakXCompiled = new System.Windows.Forms.ListBox();
            this.chkSum = new System.Windows.Forms.CheckBox();
            this.chkLowPassFilter = new System.Windows.Forms.CheckBox();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.txtSampleRate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lstPeakResultanteCompiled = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.lstPeakResultanteGlobal = new System.Windows.Forms.ListBox();
            this.lstPeakGlobalZ = new System.Windows.Forms.ListBox();
            this.lstPeakGlobalY = new System.Windows.Forms.ListBox();
            this.lstPeakGlobalX = new System.Windows.Forms.ListBox();
            this.formsPlotGlobal = new ScottPlot.FormsPlot();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.lstPeakResultanteGyro = new System.Windows.Forms.ListBox();
            this.lstPeakGyroZ = new System.Windows.Forms.ListBox();
            this.lstPeakGyroY = new System.Windows.Forms.ListBox();
            this.lstPeakGyroX = new System.Windows.Forms.ListBox();
            this.formsPlotGyro = new ScottPlot.FormsPlot();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.label42 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.lstSimulationTurnByTurn = new System.Windows.Forms.ListBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.lstSimulationGyro = new System.Windows.Forms.ListBox();
            this.lstSimulationGlobal = new System.Windows.Forms.ListBox();
            this.lstSimulationCompiled = new System.Windows.Forms.ListBox();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.label43 = new System.Windows.Forms.Label();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.lblTotalSelected = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.formsPlotT1O = new ScottPlot.FormsPlot();
            this.label25 = new System.Windows.Forms.Label();
            this.formsPlotT1I = new ScottPlot.FormsPlot();
            this.label23 = new System.Windows.Forms.Label();
            this.formsPlotT1Y = new ScottPlot.FormsPlot();
            this.label18 = new System.Windows.Forms.Label();
            this.formsPlotT1X = new ScottPlot.FormsPlot();
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.label26 = new System.Windows.Forms.Label();
            this.formsPlotT2O = new ScottPlot.FormsPlot();
            this.label27 = new System.Windows.Forms.Label();
            this.formsPlotT2I = new ScottPlot.FormsPlot();
            this.label28 = new System.Windows.Forms.Label();
            this.formsPlotT2Y = new ScottPlot.FormsPlot();
            this.label29 = new System.Windows.Forms.Label();
            this.formsPlotT2X = new ScottPlot.FormsPlot();
            this.tabPage11 = new System.Windows.Forms.TabPage();
            this.label30 = new System.Windows.Forms.Label();
            this.formsPlotT3O = new ScottPlot.FormsPlot();
            this.label31 = new System.Windows.Forms.Label();
            this.formsPlotT3I = new ScottPlot.FormsPlot();
            this.label32 = new System.Windows.Forms.Label();
            this.formsPlotT3Y = new ScottPlot.FormsPlot();
            this.label33 = new System.Windows.Forms.Label();
            this.formsPlotT3X = new ScottPlot.FormsPlot();
            this.tabPage12 = new System.Windows.Forms.TabPage();
            this.label34 = new System.Windows.Forms.Label();
            this.formsPlotT4O = new ScottPlot.FormsPlot();
            this.label35 = new System.Windows.Forms.Label();
            this.formsPlotT4I = new ScottPlot.FormsPlot();
            this.label36 = new System.Windows.Forms.Label();
            this.formsPlotT4Y = new ScottPlot.FormsPlot();
            this.label37 = new System.Windows.Forms.Label();
            this.formsPlotT4X = new ScottPlot.FormsPlot();
            this.tabPage13 = new System.Windows.Forms.TabPage();
            this.label38 = new System.Windows.Forms.Label();
            this.formsPlotT5O = new ScottPlot.FormsPlot();
            this.label39 = new System.Windows.Forms.Label();
            this.formsPlotT5I = new ScottPlot.FormsPlot();
            this.label40 = new System.Windows.Forms.Label();
            this.formsPlotT5Y = new ScottPlot.FormsPlot();
            this.label41 = new System.Windows.Forms.Label();
            this.formsPlotT5X = new ScottPlot.FormsPlot();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.formsPlotAnalysisTemporalX = new ScottPlot.FormsPlot();
            this.formsPlotAnalysisTemporalY = new ScottPlot.FormsPlot();
            this.formsPlotAnalysisTemporalZ = new ScottPlot.FormsPlot();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.btnExportWAV = new System.Windows.Forms.Button();
            this.btn240250 = new System.Windows.Forms.Button();
            this.btn250300 = new System.Windows.Forms.Button();
            this.btn230240 = new System.Windows.Forms.Button();
            this.btn210220 = new System.Windows.Forms.Button();
            this.btn220230 = new System.Windows.Forms.Button();
            this.btn200210 = new System.Windows.Forms.Button();
            this.btnUnselectAll = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnUpdateAnalysisSection = new System.Windows.Forms.Button();
            this.lstSectionSelector = new System.Windows.Forms.CheckedListBox();
            this.tabPage14 = new System.Windows.Forms.TabPage();
            this.chkOrderTracking = new System.Windows.Forms.CheckBox();
            this.chkZeroPhase = new System.Windows.Forms.CheckBox();
            this.lblPeak = new System.Windows.Forms.Label();
            this.txtFFTLimit = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkShowResultante = new System.Windows.Forms.CheckBox();
            this.chkRemoveDC = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cbxSensor = new System.Windows.Forms.ComboBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage9.SuspendLayout();
            this.tabPage10.SuspendLayout();
            this.tabPage11.SuspendLayout();
            this.tabPage12.SuspendLayout();
            this.tabPage13.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.tabPage14.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "COM";
            // 
            // txtCom
            // 
            this.txtCom.Location = new System.Drawing.Point(61, 10);
            this.txtCom.Name = "txtCom";
            this.txtCom.Size = new System.Drawing.Size(59, 20);
            this.txtCom.TabIndex = 1;
            this.txtCom.Text = "COM8";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(221, 7);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "RPM";
            // 
            // txtRPM
            // 
            this.txtRPM.AutoSize = true;
            this.txtRPM.Location = new System.Drawing.Point(58, 35);
            this.txtRPM.Name = "txtRPM";
            this.txtRPM.Size = new System.Drawing.Size(0, 13);
            this.txtRPM.TabIndex = 4;
            // 
            // txtStatus
            // 
            this.txtStatus.AutoSize = true;
            this.txtStatus.Location = new System.Drawing.Point(13, 55);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(40, 13);
            this.txtStatus.TabIndex = 5;
            this.txtStatus.Text = "Status:";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(302, 7);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(82, 23);
            this.btnDisconnect.TabIndex = 6;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnStartCapture
            // 
            this.btnStartCapture.Location = new System.Drawing.Point(397, 8);
            this.btnStartCapture.Name = "btnStartCapture";
            this.btnStartCapture.Size = new System.Drawing.Size(112, 23);
            this.btnStartCapture.TabIndex = 7;
            this.btnStartCapture.Text = "Start capture";
            this.btnStartCapture.UseVisualStyleBackColor = true;
            this.btnStartCapture.Click += new System.EventHandler(this.btnStartCapture_Click);
            // 
            // btnEndCapture
            // 
            this.btnEndCapture.Location = new System.Drawing.Point(397, 37);
            this.btnEndCapture.Name = "btnEndCapture";
            this.btnEndCapture.Size = new System.Drawing.Size(112, 23);
            this.btnEndCapture.TabIndex = 8;
            this.btnEndCapture.Text = "End Capture";
            this.btnEndCapture.UseVisualStyleBackColor = true;
            this.btnEndCapture.Click += new System.EventHandler(this.btnEndCapture_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(105, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Save data between RPM";
            // 
            // txtMinRPM
            // 
            this.txtMinRPM.Location = new System.Drawing.Point(247, 38);
            this.txtMinRPM.Name = "txtMinRPM";
            this.txtMinRPM.Size = new System.Drawing.Size(63, 20);
            this.txtMinRPM.TabIndex = 10;
            this.txtMinRPM.Text = "200";
            // 
            // txtMaxRPM
            // 
            this.txtMaxRPM.Location = new System.Drawing.Point(316, 38);
            this.txtMaxRPM.Name = "txtMaxRPM";
            this.txtMaxRPM.Size = new System.Drawing.Size(60, 20);
            this.txtMaxRPM.TabIndex = 11;
            this.txtMaxRPM.Text = "250";
            // 
            // lblStats
            // 
            this.lblStats.AutoSize = true;
            this.lblStats.Location = new System.Drawing.Point(379, 13);
            this.lblStats.Name = "lblStats";
            this.lblStats.Size = new System.Drawing.Size(0, 13);
            this.lblStats.TabIndex = 14;
            // 
            // formsPlotX
            // 
            this.formsPlotX.Location = new System.Drawing.Point(11, 35);
            this.formsPlotX.Name = "formsPlotX";
            this.formsPlotX.Size = new System.Drawing.Size(405, 361);
            this.formsPlotX.TabIndex = 15;
            this.formsPlotX.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formsPlotAnalysis_MouseMove);
            // 
            // formsPlotY
            // 
            this.formsPlotY.Location = new System.Drawing.Point(430, 35);
            this.formsPlotY.Name = "formsPlotY";
            this.formsPlotY.Size = new System.Drawing.Size(405, 361);
            this.formsPlotY.TabIndex = 16;
            this.formsPlotY.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formsPlotAnalysis_MouseMove);
            // 
            // formsPlotZ
            // 
            this.formsPlotZ.Location = new System.Drawing.Point(841, 35);
            this.formsPlotZ.Name = "formsPlotZ";
            this.formsPlotZ.Size = new System.Drawing.Size(405, 361);
            this.formsPlotZ.TabIndex = 17;
            this.formsPlotZ.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formsPlotAnalysis_MouseMove);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(11, 6);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(56, 23);
            this.btnPrev.TabIndex = 18;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(97, 6);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(56, 23);
            this.btnNext.TabIndex = 19;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // formsPlotAnalysis
            // 
            this.formsPlotAnalysis.Location = new System.Drawing.Point(6, 6);
            this.formsPlotAnalysis.Name = "formsPlotAnalysis";
            this.formsPlotAnalysis.Size = new System.Drawing.Size(1240, 392);
            this.formsPlotAnalysis.TabIndex = 20;
            this.formsPlotAnalysis.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formsPlotAnalysis_MouseMove);
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Location = new System.Drawing.Point(526, 7);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(92, 23);
            this.btnAnalyze.TabIndex = 21;
            this.btnAnalyze.Text = "Analyze CSV";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // lblRecordNumber
            // 
            this.lblRecordNumber.AutoSize = true;
            this.lblRecordNumber.Location = new System.Drawing.Point(79, 11);
            this.lblRecordNumber.Name = "lblRecordNumber";
            this.lblRecordNumber.Size = new System.Drawing.Size(0, 13);
            this.lblRecordNumber.TabIndex = 22;
            // 
            // chkShowX
            // 
            this.chkShowX.AutoSize = true;
            this.chkShowX.Checked = true;
            this.chkShowX.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowX.Location = new System.Drawing.Point(511, 64);
            this.chkShowX.Name = "chkShowX";
            this.chkShowX.Size = new System.Drawing.Size(63, 17);
            this.chkShowX.TabIndex = 23;
            this.chkShowX.Text = "Show X";
            this.chkShowX.UseVisualStyleBackColor = true;
            this.chkShowX.CheckedChanged += new System.EventHandler(this.chkShowX_CheckedChanged);
            // 
            // chkShowY
            // 
            this.chkShowY.AutoSize = true;
            this.chkShowY.Checked = true;
            this.chkShowY.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowY.Location = new System.Drawing.Point(582, 65);
            this.chkShowY.Name = "chkShowY";
            this.chkShowY.Size = new System.Drawing.Size(63, 17);
            this.chkShowY.TabIndex = 24;
            this.chkShowY.Text = "Show Y";
            this.chkShowY.UseVisualStyleBackColor = true;
            this.chkShowY.CheckedChanged += new System.EventHandler(this.chkShowY_CheckedChanged);
            // 
            // chkShowZ
            // 
            this.chkShowZ.AutoSize = true;
            this.chkShowZ.Location = new System.Drawing.Point(651, 65);
            this.chkShowZ.Name = "chkShowZ";
            this.chkShowZ.Size = new System.Drawing.Size(63, 17);
            this.chkShowZ.TabIndex = 25;
            this.chkShowZ.Text = "Show Z";
            this.chkShowZ.UseVisualStyleBackColor = true;
            this.chkShowZ.CheckedChanged += new System.EventHandler(this.chkShowZ_CheckedChanged);
            // 
            // chkFFT
            // 
            this.chkFFT.AutoSize = true;
            this.chkFFT.Checked = true;
            this.chkFFT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFFT.Location = new System.Drawing.Point(762, 64);
            this.chkFFT.Name = "chkFFT";
            this.chkFFT.Size = new System.Drawing.Size(45, 17);
            this.chkFFT.TabIndex = 27;
            this.chkFFT.Text = "FFT";
            this.chkFFT.UseVisualStyleBackColor = true;
            this.chkFFT.CheckedChanged += new System.EventHandler(this.chkFFT_CheckedChanged);
            // 
            // cbxFFT
            // 
            this.cbxFFT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFFT.FormattingEnabled = true;
            this.cbxFFT.Items.AddRange(new object[] {
            "None",
            "Hamming",
            "HammingPeriodic",
            "Hann",
            "HannPeriodic",
            "BlackmanHarris",
            "BlackmanNuttal",
            "FlatTop"});
            this.cbxFFT.Location = new System.Drawing.Point(813, 61);
            this.cbxFFT.Name = "cbxFFT";
            this.cbxFFT.Size = new System.Drawing.Size(121, 21);
            this.cbxFFT.TabIndex = 28;
            this.cbxFFT.SelectedIndexChanged += new System.EventHandler(this.cbxFFT_SelectedIndexChanged);
            // 
            // chkAbsolute
            // 
            this.chkAbsolute.AutoSize = true;
            this.chkAbsolute.Location = new System.Drawing.Point(637, 21);
            this.chkAbsolute.Name = "chkAbsolute";
            this.chkAbsolute.Size = new System.Drawing.Size(101, 17);
            this.chkAbsolute.TabIndex = 31;
            this.chkAbsolute.Text = "Absolute values";
            this.chkAbsolute.UseVisualStyleBackColor = true;
            this.chkAbsolute.CheckedChanged += new System.EventHandler(this.chkAbsolute_CheckedChanged);
            // 
            // lstPeakX
            // 
            this.lstPeakX.FormattingEnabled = true;
            this.lstPeakX.HorizontalScrollbar = true;
            this.lstPeakX.Location = new System.Drawing.Point(11, 402);
            this.lstPeakX.Name = "lstPeakX";
            this.lstPeakX.ScrollAlwaysVisible = true;
            this.lstPeakX.Size = new System.Drawing.Size(405, 108);
            this.lstPeakX.TabIndex = 33;
            // 
            // lstPeakY
            // 
            this.lstPeakY.FormattingEnabled = true;
            this.lstPeakY.HorizontalScrollbar = true;
            this.lstPeakY.Location = new System.Drawing.Point(430, 402);
            this.lstPeakY.Name = "lstPeakY";
            this.lstPeakY.ScrollAlwaysVisible = true;
            this.lstPeakY.Size = new System.Drawing.Size(406, 108);
            this.lstPeakY.TabIndex = 34;
            // 
            // lstPeakZ
            // 
            this.lstPeakZ.FormattingEnabled = true;
            this.lstPeakZ.HorizontalScrollbar = true;
            this.lstPeakZ.Location = new System.Drawing.Point(841, 402);
            this.lstPeakZ.Name = "lstPeakZ";
            this.lstPeakZ.ScrollAlwaysVisible = true;
            this.lstPeakZ.Size = new System.Drawing.Size(405, 108);
            this.lstPeakZ.TabIndex = 35;
            // 
            // lstPeakZCompiled
            // 
            this.lstPeakZCompiled.FormattingEnabled = true;
            this.lstPeakZCompiled.HorizontalScrollbar = true;
            this.lstPeakZCompiled.Location = new System.Drawing.Point(667, 415);
            this.lstPeakZCompiled.Name = "lstPeakZCompiled";
            this.lstPeakZCompiled.ScrollAlwaysVisible = true;
            this.lstPeakZCompiled.Size = new System.Drawing.Size(294, 95);
            this.lstPeakZCompiled.TabIndex = 38;
            // 
            // lstPeakYCompiled
            // 
            this.lstPeakYCompiled.FormattingEnabled = true;
            this.lstPeakYCompiled.HorizontalScrollbar = true;
            this.lstPeakYCompiled.Location = new System.Drawing.Point(327, 415);
            this.lstPeakYCompiled.Name = "lstPeakYCompiled";
            this.lstPeakYCompiled.ScrollAlwaysVisible = true;
            this.lstPeakYCompiled.Size = new System.Drawing.Size(334, 95);
            this.lstPeakYCompiled.TabIndex = 37;
            // 
            // lstPeakXCompiled
            // 
            this.lstPeakXCompiled.FormattingEnabled = true;
            this.lstPeakXCompiled.HorizontalScrollbar = true;
            this.lstPeakXCompiled.Location = new System.Drawing.Point(8, 415);
            this.lstPeakXCompiled.Name = "lstPeakXCompiled";
            this.lstPeakXCompiled.ScrollAlwaysVisible = true;
            this.lstPeakXCompiled.Size = new System.Drawing.Size(313, 95);
            this.lstPeakXCompiled.TabIndex = 36;
            // 
            // chkSum
            // 
            this.chkSum.AutoSize = true;
            this.chkSum.Location = new System.Drawing.Point(744, 21);
            this.chkSum.Name = "chkSum";
            this.chkSum.Size = new System.Drawing.Size(47, 17);
            this.chkSum.TabIndex = 39;
            this.chkSum.Text = "Sum";
            this.chkSum.UseVisualStyleBackColor = true;
            this.chkSum.CheckedChanged += new System.EventHandler(this.chkSum_CheckedChanged);
            // 
            // chkLowPassFilter
            // 
            this.chkLowPassFilter.AutoSize = true;
            this.chkLowPassFilter.Location = new System.Drawing.Point(637, 3);
            this.chkLowPassFilter.Name = "chkLowPassFilter";
            this.chkLowPassFilter.Size = new System.Drawing.Size(86, 17);
            this.chkLowPassFilter.TabIndex = 40;
            this.chkLowPassFilter.Text = "lowpass filter";
            this.chkLowPassFilter.UseVisualStyleBackColor = true;
            this.chkLowPassFilter.CheckedChanged += new System.EventHandler(this.chkLowPassFilter_CheckedChanged);
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(813, 0);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(41, 20);
            this.txtFilter.TabIndex = 41;
            this.txtFilter.Text = "100";
            // 
            // txtSampleRate
            // 
            this.txtSampleRate.Location = new System.Drawing.Point(594, 38);
            this.txtSampleRate.Name = "txtSampleRate";
            this.txtSampleRate.Size = new System.Drawing.Size(40, 20);
            this.txtSampleRate.TabIndex = 43;
            this.txtSampleRate.Text = "1000";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(523, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 44;
            this.label4.Text = "SampleRate";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage8);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Controls.Add(this.tabPage14);
            this.tabControl1.Location = new System.Drawing.Point(15, 82);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1260, 549);
            this.tabControl1.TabIndex = 45;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.lstPeakResultanteCompiled);
            this.tabPage1.Controls.Add(this.formsPlotAnalysis);
            this.tabPage1.Controls.Add(this.lstPeakXCompiled);
            this.tabPage1.Controls.Add(this.lstPeakYCompiled);
            this.tabPage1.Controls.Add(this.lstPeakZCompiled);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1252, 523);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Compiled data (all in turn)";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(967, 401);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 13);
            this.label12.TabIndex = 43;
            this.label12.Text = "Resultant X + Y";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(664, 401);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(14, 13);
            this.label11.TabIndex = 42;
            this.label11.Text = "Z";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(324, 399);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 13);
            this.label10.TabIndex = 41;
            this.label10.Text = "Y";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 399);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 13);
            this.label7.TabIndex = 40;
            this.label7.Text = "X";
            // 
            // lstPeakResultanteCompiled
            // 
            this.lstPeakResultanteCompiled.FormattingEnabled = true;
            this.lstPeakResultanteCompiled.HorizontalScrollbar = true;
            this.lstPeakResultanteCompiled.Location = new System.Drawing.Point(967, 415);
            this.lstPeakResultanteCompiled.Name = "lstPeakResultanteCompiled";
            this.lstPeakResultanteCompiled.ScrollAlwaysVisible = true;
            this.lstPeakResultanteCompiled.Size = new System.Drawing.Size(266, 95);
            this.lstPeakResultanteCompiled.TabIndex = 39;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnNext);
            this.tabPage2.Controls.Add(this.btnPrev);
            this.tabPage2.Controls.Add(this.lblRecordNumber);
            this.tabPage2.Controls.Add(this.formsPlotX);
            this.tabPage2.Controls.Add(this.formsPlotY);
            this.tabPage2.Controls.Add(this.formsPlotZ);
            this.tabPage2.Controls.Add(this.lstPeakZ);
            this.tabPage2.Controls.Add(this.lstPeakX);
            this.tabPage2.Controls.Add(this.lstPeakY);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1252, 523);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Single";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.label13);
            this.tabPage3.Controls.Add(this.label14);
            this.tabPage3.Controls.Add(this.label15);
            this.tabPage3.Controls.Add(this.lstPeakResultanteGlobal);
            this.tabPage3.Controls.Add(this.lstPeakGlobalZ);
            this.tabPage3.Controls.Add(this.lstPeakGlobalY);
            this.tabPage3.Controls.Add(this.lstPeakGlobalX);
            this.tabPage3.Controls.Add(this.formsPlotGlobal);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1252, 523);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Global";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(967, 403);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 13);
            this.label9.TabIndex = 54;
            this.label9.Text = "Resultant X + Y";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(664, 403);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(14, 13);
            this.label13.TabIndex = 53;
            this.label13.Text = "Z";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(324, 401);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(14, 13);
            this.label14.TabIndex = 52;
            this.label14.Text = "Y";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 401);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(14, 13);
            this.label15.TabIndex = 51;
            this.label15.Text = "X";
            // 
            // lstPeakResultanteGlobal
            // 
            this.lstPeakResultanteGlobal.FormattingEnabled = true;
            this.lstPeakResultanteGlobal.HorizontalScrollbar = true;
            this.lstPeakResultanteGlobal.Location = new System.Drawing.Point(921, 417);
            this.lstPeakResultanteGlobal.Name = "lstPeakResultanteGlobal";
            this.lstPeakResultanteGlobal.ScrollAlwaysVisible = true;
            this.lstPeakResultanteGlobal.Size = new System.Drawing.Size(325, 95);
            this.lstPeakResultanteGlobal.TabIndex = 50;
            // 
            // lstPeakGlobalZ
            // 
            this.lstPeakGlobalZ.FormattingEnabled = true;
            this.lstPeakGlobalZ.HorizontalScrollbar = true;
            this.lstPeakGlobalZ.Location = new System.Drawing.Point(644, 417);
            this.lstPeakGlobalZ.Name = "lstPeakGlobalZ";
            this.lstPeakGlobalZ.ScrollAlwaysVisible = true;
            this.lstPeakGlobalZ.Size = new System.Drawing.Size(271, 95);
            this.lstPeakGlobalZ.TabIndex = 49;
            // 
            // lstPeakGlobalY
            // 
            this.lstPeakGlobalY.FormattingEnabled = true;
            this.lstPeakGlobalY.HorizontalScrollbar = true;
            this.lstPeakGlobalY.Location = new System.Drawing.Point(320, 417);
            this.lstPeakGlobalY.Name = "lstPeakGlobalY";
            this.lstPeakGlobalY.ScrollAlwaysVisible = true;
            this.lstPeakGlobalY.Size = new System.Drawing.Size(318, 95);
            this.lstPeakGlobalY.TabIndex = 48;
            // 
            // lstPeakGlobalX
            // 
            this.lstPeakGlobalX.FormattingEnabled = true;
            this.lstPeakGlobalX.HorizontalScrollbar = true;
            this.lstPeakGlobalX.Location = new System.Drawing.Point(6, 417);
            this.lstPeakGlobalX.Name = "lstPeakGlobalX";
            this.lstPeakGlobalX.ScrollAlwaysVisible = true;
            this.lstPeakGlobalX.Size = new System.Drawing.Size(315, 95);
            this.lstPeakGlobalX.TabIndex = 47;
            // 
            // formsPlotGlobal
            // 
            this.formsPlotGlobal.Location = new System.Drawing.Point(6, 6);
            this.formsPlotGlobal.Name = "formsPlotGlobal";
            this.formsPlotGlobal.Size = new System.Drawing.Size(1240, 392);
            this.formsPlotGlobal.TabIndex = 46;
            this.formsPlotGlobal.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formsPlotAnalysis_MouseMove);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.lstPeakResultanteGyro);
            this.tabPage4.Controls.Add(this.lstPeakGyroZ);
            this.tabPage4.Controls.Add(this.lstPeakGyroY);
            this.tabPage4.Controls.Add(this.lstPeakGyroX);
            this.tabPage4.Controls.Add(this.formsPlotGyro);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1252, 523);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Gyro";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // lstPeakResultanteGyro
            // 
            this.lstPeakResultanteGyro.FormattingEnabled = true;
            this.lstPeakResultanteGyro.HorizontalScrollbar = true;
            this.lstPeakResultanteGyro.Location = new System.Drawing.Point(956, 404);
            this.lstPeakResultanteGyro.Name = "lstPeakResultanteGyro";
            this.lstPeakResultanteGyro.ScrollAlwaysVisible = true;
            this.lstPeakResultanteGyro.Size = new System.Drawing.Size(290, 108);
            this.lstPeakResultanteGyro.TabIndex = 56;
            // 
            // lstPeakGyroZ
            // 
            this.lstPeakGyroZ.FormattingEnabled = true;
            this.lstPeakGyroZ.HorizontalScrollbar = true;
            this.lstPeakGyroZ.Location = new System.Drawing.Point(638, 404);
            this.lstPeakGyroZ.Name = "lstPeakGyroZ";
            this.lstPeakGyroZ.ScrollAlwaysVisible = true;
            this.lstPeakGyroZ.Size = new System.Drawing.Size(312, 108);
            this.lstPeakGyroZ.TabIndex = 55;
            // 
            // lstPeakGyroY
            // 
            this.lstPeakGyroY.FormattingEnabled = true;
            this.lstPeakGyroY.HorizontalScrollbar = true;
            this.lstPeakGyroY.Location = new System.Drawing.Point(327, 404);
            this.lstPeakGyroY.Name = "lstPeakGyroY";
            this.lstPeakGyroY.ScrollAlwaysVisible = true;
            this.lstPeakGyroY.Size = new System.Drawing.Size(305, 108);
            this.lstPeakGyroY.TabIndex = 54;
            // 
            // lstPeakGyroX
            // 
            this.lstPeakGyroX.FormattingEnabled = true;
            this.lstPeakGyroX.HorizontalScrollbar = true;
            this.lstPeakGyroX.Location = new System.Drawing.Point(6, 404);
            this.lstPeakGyroX.Name = "lstPeakGyroX";
            this.lstPeakGyroX.ScrollAlwaysVisible = true;
            this.lstPeakGyroX.Size = new System.Drawing.Size(315, 108);
            this.lstPeakGyroX.TabIndex = 53;
            // 
            // formsPlotGyro
            // 
            this.formsPlotGyro.Location = new System.Drawing.Point(6, 6);
            this.formsPlotGyro.Name = "formsPlotGyro";
            this.formsPlotGyro.Size = new System.Drawing.Size(1240, 392);
            this.formsPlotGyro.TabIndex = 52;
            this.formsPlotGyro.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formsPlotAnalysis_MouseMove);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.label42);
            this.tabPage5.Controls.Add(this.label22);
            this.tabPage5.Controls.Add(this.lstSimulationTurnByTurn);
            this.tabPage5.Controls.Add(this.label21);
            this.tabPage5.Controls.Add(this.label20);
            this.tabPage5.Controls.Add(this.label19);
            this.tabPage5.Controls.Add(this.lstSimulationGyro);
            this.tabPage5.Controls.Add(this.lstSimulationGlobal);
            this.tabPage5.Controls.Add(this.lstSimulationCompiled);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1252, 523);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Analysis";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(577, 250);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(252, 52);
            this.label42.TabIndex = 71;
            this.label42.Text = "Coeff variation\r\nCV < 0.1 → données très proches (faible dispersion)\r\n0.1 ≤ CV < " +
    "0.3 → données modérément dispersées\r\nCV ≥ 0.3 → données très dispersées";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(50, 257);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(185, 13);
            this.label22.TabIndex = 70;
            this.label22.Text = "Static balancing with turn by turn data";
            // 
            // lstSimulationTurnByTurn
            // 
            this.lstSimulationTurnByTurn.FormattingEnabled = true;
            this.lstSimulationTurnByTurn.HorizontalScrollbar = true;
            this.lstSimulationTurnByTurn.Location = new System.Drawing.Point(53, 273);
            this.lstSimulationTurnByTurn.Name = "lstSimulationTurnByTurn";
            this.lstSimulationTurnByTurn.ScrollAlwaysVisible = true;
            this.lstSimulationTurnByTurn.Size = new System.Drawing.Size(516, 238);
            this.lstSimulationTurnByTurn.TabIndex = 69;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(904, 289);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(91, 13);
            this.label21.TabIndex = 68;
            this.label21.Text = "gyro data analysis";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(572, 19);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(99, 13);
            this.label20.TabIndex = 67;
            this.label20.Text = "global data analysis";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(50, 19);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(114, 13);
            this.label19.TabIndex = 66;
            this.label19.Text = "Compiled data analysis";
            // 
            // lstSimulationGyro
            // 
            this.lstSimulationGyro.FormattingEnabled = true;
            this.lstSimulationGyro.HorizontalScrollbar = true;
            this.lstSimulationGyro.Location = new System.Drawing.Point(580, 312);
            this.lstSimulationGyro.Name = "lstSimulationGyro";
            this.lstSimulationGyro.ScrollAlwaysVisible = true;
            this.lstSimulationGyro.Size = new System.Drawing.Size(648, 199);
            this.lstSimulationGyro.TabIndex = 63;
            // 
            // lstSimulationGlobal
            // 
            this.lstSimulationGlobal.FormattingEnabled = true;
            this.lstSimulationGlobal.HorizontalScrollbar = true;
            this.lstSimulationGlobal.Location = new System.Drawing.Point(575, 35);
            this.lstSimulationGlobal.Name = "lstSimulationGlobal";
            this.lstSimulationGlobal.ScrollAlwaysVisible = true;
            this.lstSimulationGlobal.Size = new System.Drawing.Size(653, 212);
            this.lstSimulationGlobal.TabIndex = 61;
            // 
            // lstSimulationCompiled
            // 
            this.lstSimulationCompiled.FormattingEnabled = true;
            this.lstSimulationCompiled.HorizontalScrollbar = true;
            this.lstSimulationCompiled.Location = new System.Drawing.Point(53, 35);
            this.lstSimulationCompiled.Name = "lstSimulationCompiled";
            this.lstSimulationCompiled.ScrollAlwaysVisible = true;
            this.lstSimulationCompiled.Size = new System.Drawing.Size(516, 212);
            this.lstSimulationCompiled.TabIndex = 55;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.label43);
            this.tabPage8.Controls.Add(this.tabControl2);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(1252, 523);
            this.tabPage8.TabIndex = 7;
            this.tabPage8.Text = "Analysis Graphical";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(400, 3);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(379, 13);
            this.label43.TabIndex = 28;
            this.label43.Text = "BLUE: Turn by Turn, RED: Compiled (360ms), GREEN: Global, YELLOW: Gyro";
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage9);
            this.tabControl2.Controls.Add(this.tabPage10);
            this.tabControl2.Controls.Add(this.tabPage11);
            this.tabControl2.Controls.Add(this.tabPage12);
            this.tabControl2.Controls.Add(this.tabPage13);
            this.tabControl2.Location = new System.Drawing.Point(13, 26);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1233, 491);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.lblTotalSelected);
            this.tabPage9.Controls.Add(this.label24);
            this.tabPage9.Controls.Add(this.formsPlotT1O);
            this.tabPage9.Controls.Add(this.label25);
            this.tabPage9.Controls.Add(this.formsPlotT1I);
            this.tabPage9.Controls.Add(this.label23);
            this.tabPage9.Controls.Add(this.formsPlotT1Y);
            this.tabPage9.Controls.Add(this.label18);
            this.tabPage9.Controls.Add(this.formsPlotT1X);
            this.tabPage9.Location = new System.Drawing.Point(4, 22);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage9.Size = new System.Drawing.Size(1225, 465);
            this.tabPage9.TabIndex = 0;
            this.tabPage9.Text = "Fundamental";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // lblTotalSelected
            // 
            this.lblTotalSelected.AutoSize = true;
            this.lblTotalSelected.Location = new System.Drawing.Point(383, 224);
            this.lblTotalSelected.Name = "lblTotalSelected";
            this.lblTotalSelected.Size = new System.Drawing.Size(0, 13);
            this.lblTotalSelected.TabIndex = 27;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(705, 224);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(104, 13);
            this.label24.TabIndex = 26;
            this.label24.Text = "Dynamic angle outer";
            // 
            // formsPlotT1O
            // 
            this.formsPlotT1O.Location = new System.Drawing.Point(581, 243);
            this.formsPlotT1O.Name = "formsPlotT1O";
            this.formsPlotT1O.Size = new System.Drawing.Size(555, 227);
            this.formsPlotT1O.TabIndex = 25;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(118, 224);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(103, 13);
            this.label25.TabIndex = 24;
            this.label25.Text = "Dynamic angle inner";
            // 
            // formsPlotT1I
            // 
            this.formsPlotT1I.Location = new System.Drawing.Point(6, 243);
            this.formsPlotT1I.Name = "formsPlotT1I";
            this.formsPlotT1I.Size = new System.Drawing.Size(555, 227);
            this.formsPlotT1I.TabIndex = 23;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(693, 2);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(128, 13);
            this.label23.TabIndex = 22;
            this.label23.Text = "Static angle outer (Y only)";
            // 
            // formsPlotT1Y
            // 
            this.formsPlotT1Y.Location = new System.Drawing.Point(581, 18);
            this.formsPlotT1Y.Name = "formsPlotT1Y";
            this.formsPlotT1Y.Size = new System.Drawing.Size(555, 203);
            this.formsPlotT1Y.TabIndex = 21;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(118, 2);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(128, 13);
            this.label18.TabIndex = 20;
            this.label18.Text = "Static angle Inner (X only)";
            // 
            // formsPlotT1X
            // 
            this.formsPlotT1X.Location = new System.Drawing.Point(6, 18);
            this.formsPlotT1X.Name = "formsPlotT1X";
            this.formsPlotT1X.Size = new System.Drawing.Size(555, 203);
            this.formsPlotT1X.TabIndex = 19;
            // 
            // tabPage10
            // 
            this.tabPage10.Controls.Add(this.label26);
            this.tabPage10.Controls.Add(this.formsPlotT2O);
            this.tabPage10.Controls.Add(this.label27);
            this.tabPage10.Controls.Add(this.formsPlotT2I);
            this.tabPage10.Controls.Add(this.label28);
            this.tabPage10.Controls.Add(this.formsPlotT2Y);
            this.tabPage10.Controls.Add(this.label29);
            this.tabPage10.Controls.Add(this.formsPlotT2X);
            this.tabPage10.Location = new System.Drawing.Point(4, 22);
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage10.Size = new System.Drawing.Size(1225, 465);
            this.tabPage10.TabIndex = 1;
            this.tabPage10.Text = "Order 2";
            this.tabPage10.UseVisualStyleBackColor = true;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(705, 224);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(104, 13);
            this.label26.TabIndex = 34;
            this.label26.Text = "Dynamic angle outer";
            // 
            // formsPlotT2O
            // 
            this.formsPlotT2O.Location = new System.Drawing.Point(581, 243);
            this.formsPlotT2O.Name = "formsPlotT2O";
            this.formsPlotT2O.Size = new System.Drawing.Size(555, 227);
            this.formsPlotT2O.TabIndex = 33;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(118, 224);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(103, 13);
            this.label27.TabIndex = 32;
            this.label27.Text = "Dynamic angle inner";
            // 
            // formsPlotT2I
            // 
            this.formsPlotT2I.Location = new System.Drawing.Point(6, 243);
            this.formsPlotT2I.Name = "formsPlotT2I";
            this.formsPlotT2I.Size = new System.Drawing.Size(555, 227);
            this.formsPlotT2I.TabIndex = 31;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(693, 2);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(14, 13);
            this.label28.TabIndex = 30;
            this.label28.Text = "Y";
            // 
            // formsPlotT2Y
            // 
            this.formsPlotT2Y.Location = new System.Drawing.Point(581, 18);
            this.formsPlotT2Y.Name = "formsPlotT2Y";
            this.formsPlotT2Y.Size = new System.Drawing.Size(555, 203);
            this.formsPlotT2Y.TabIndex = 29;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(118, 2);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(14, 13);
            this.label29.TabIndex = 28;
            this.label29.Text = "X";
            // 
            // formsPlotT2X
            // 
            this.formsPlotT2X.Location = new System.Drawing.Point(6, 18);
            this.formsPlotT2X.Name = "formsPlotT2X";
            this.formsPlotT2X.Size = new System.Drawing.Size(555, 203);
            this.formsPlotT2X.TabIndex = 27;
            // 
            // tabPage11
            // 
            this.tabPage11.Controls.Add(this.label30);
            this.tabPage11.Controls.Add(this.formsPlotT3O);
            this.tabPage11.Controls.Add(this.label31);
            this.tabPage11.Controls.Add(this.formsPlotT3I);
            this.tabPage11.Controls.Add(this.label32);
            this.tabPage11.Controls.Add(this.formsPlotT3Y);
            this.tabPage11.Controls.Add(this.label33);
            this.tabPage11.Controls.Add(this.formsPlotT3X);
            this.tabPage11.Location = new System.Drawing.Point(4, 22);
            this.tabPage11.Name = "tabPage11";
            this.tabPage11.Size = new System.Drawing.Size(1225, 465);
            this.tabPage11.TabIndex = 2;
            this.tabPage11.Text = "Order 3";
            this.tabPage11.UseVisualStyleBackColor = true;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(717, 228);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(104, 13);
            this.label30.TabIndex = 34;
            this.label30.Text = "Dynamic angle outer";
            // 
            // formsPlotT3O
            // 
            this.formsPlotT3O.Location = new System.Drawing.Point(593, 247);
            this.formsPlotT3O.Name = "formsPlotT3O";
            this.formsPlotT3O.Size = new System.Drawing.Size(555, 227);
            this.formsPlotT3O.TabIndex = 33;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(130, 228);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(103, 13);
            this.label31.TabIndex = 32;
            this.label31.Text = "Dynamic angle inner";
            // 
            // formsPlotT3I
            // 
            this.formsPlotT3I.Location = new System.Drawing.Point(18, 247);
            this.formsPlotT3I.Name = "formsPlotT3I";
            this.formsPlotT3I.Size = new System.Drawing.Size(555, 227);
            this.formsPlotT3I.TabIndex = 31;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(705, 6);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(14, 13);
            this.label32.TabIndex = 30;
            this.label32.Text = "Y";
            // 
            // formsPlotT3Y
            // 
            this.formsPlotT3Y.Location = new System.Drawing.Point(593, 22);
            this.formsPlotT3Y.Name = "formsPlotT3Y";
            this.formsPlotT3Y.Size = new System.Drawing.Size(555, 203);
            this.formsPlotT3Y.TabIndex = 29;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(130, 6);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(14, 13);
            this.label33.TabIndex = 28;
            this.label33.Text = "X";
            // 
            // formsPlotT3X
            // 
            this.formsPlotT3X.Location = new System.Drawing.Point(18, 22);
            this.formsPlotT3X.Name = "formsPlotT3X";
            this.formsPlotT3X.Size = new System.Drawing.Size(555, 203);
            this.formsPlotT3X.TabIndex = 27;
            // 
            // tabPage12
            // 
            this.tabPage12.Controls.Add(this.label34);
            this.tabPage12.Controls.Add(this.formsPlotT4O);
            this.tabPage12.Controls.Add(this.label35);
            this.tabPage12.Controls.Add(this.formsPlotT4I);
            this.tabPage12.Controls.Add(this.label36);
            this.tabPage12.Controls.Add(this.formsPlotT4Y);
            this.tabPage12.Controls.Add(this.label37);
            this.tabPage12.Controls.Add(this.formsPlotT4X);
            this.tabPage12.Location = new System.Drawing.Point(4, 22);
            this.tabPage12.Name = "tabPage12";
            this.tabPage12.Size = new System.Drawing.Size(1225, 465);
            this.tabPage12.TabIndex = 3;
            this.tabPage12.Text = "Order 4";
            this.tabPage12.UseVisualStyleBackColor = true;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(702, 227);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(104, 13);
            this.label34.TabIndex = 34;
            this.label34.Text = "Dynamic angle outer";
            // 
            // formsPlotT4O
            // 
            this.formsPlotT4O.Location = new System.Drawing.Point(578, 246);
            this.formsPlotT4O.Name = "formsPlotT4O";
            this.formsPlotT4O.Size = new System.Drawing.Size(555, 227);
            this.formsPlotT4O.TabIndex = 33;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(115, 227);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(103, 13);
            this.label35.TabIndex = 32;
            this.label35.Text = "Dynamic angle inner";
            // 
            // formsPlotT4I
            // 
            this.formsPlotT4I.Location = new System.Drawing.Point(3, 246);
            this.formsPlotT4I.Name = "formsPlotT4I";
            this.formsPlotT4I.Size = new System.Drawing.Size(555, 227);
            this.formsPlotT4I.TabIndex = 31;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(690, 5);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(14, 13);
            this.label36.TabIndex = 30;
            this.label36.Text = "Y";
            // 
            // formsPlotT4Y
            // 
            this.formsPlotT4Y.Location = new System.Drawing.Point(578, 21);
            this.formsPlotT4Y.Name = "formsPlotT4Y";
            this.formsPlotT4Y.Size = new System.Drawing.Size(555, 203);
            this.formsPlotT4Y.TabIndex = 29;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(115, 5);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(14, 13);
            this.label37.TabIndex = 28;
            this.label37.Text = "X";
            // 
            // formsPlotT4X
            // 
            this.formsPlotT4X.Location = new System.Drawing.Point(3, 21);
            this.formsPlotT4X.Name = "formsPlotT4X";
            this.formsPlotT4X.Size = new System.Drawing.Size(555, 203);
            this.formsPlotT4X.TabIndex = 27;
            // 
            // tabPage13
            // 
            this.tabPage13.Controls.Add(this.label38);
            this.tabPage13.Controls.Add(this.formsPlotT5O);
            this.tabPage13.Controls.Add(this.label39);
            this.tabPage13.Controls.Add(this.formsPlotT5I);
            this.tabPage13.Controls.Add(this.label40);
            this.tabPage13.Controls.Add(this.formsPlotT5Y);
            this.tabPage13.Controls.Add(this.label41);
            this.tabPage13.Controls.Add(this.formsPlotT5X);
            this.tabPage13.Location = new System.Drawing.Point(4, 22);
            this.tabPage13.Name = "tabPage13";
            this.tabPage13.Size = new System.Drawing.Size(1225, 465);
            this.tabPage13.TabIndex = 4;
            this.tabPage13.Text = "Order 5";
            this.tabPage13.UseVisualStyleBackColor = true;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(702, 226);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(104, 13);
            this.label38.TabIndex = 34;
            this.label38.Text = "Dynamic angle outer";
            // 
            // formsPlotT5O
            // 
            this.formsPlotT5O.Location = new System.Drawing.Point(578, 245);
            this.formsPlotT5O.Name = "formsPlotT5O";
            this.formsPlotT5O.Size = new System.Drawing.Size(555, 227);
            this.formsPlotT5O.TabIndex = 33;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(115, 226);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(103, 13);
            this.label39.TabIndex = 32;
            this.label39.Text = "Dynamic angle inner";
            // 
            // formsPlotT5I
            // 
            this.formsPlotT5I.Location = new System.Drawing.Point(3, 245);
            this.formsPlotT5I.Name = "formsPlotT5I";
            this.formsPlotT5I.Size = new System.Drawing.Size(555, 227);
            this.formsPlotT5I.TabIndex = 31;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(690, 4);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(14, 13);
            this.label40.TabIndex = 30;
            this.label40.Text = "Y";
            // 
            // formsPlotT5Y
            // 
            this.formsPlotT5Y.Location = new System.Drawing.Point(578, 20);
            this.formsPlotT5Y.Name = "formsPlotT5Y";
            this.formsPlotT5Y.Size = new System.Drawing.Size(555, 203);
            this.formsPlotT5Y.TabIndex = 29;
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(115, 4);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(14, 13);
            this.label41.TabIndex = 28;
            this.label41.Text = "X";
            // 
            // formsPlotT5X
            // 
            this.formsPlotT5X.Location = new System.Drawing.Point(3, 20);
            this.formsPlotT5X.Name = "formsPlotT5X";
            this.formsPlotT5X.Size = new System.Drawing.Size(555, 203);
            this.formsPlotT5X.TabIndex = 27;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.label17);
            this.tabPage6.Controls.Add(this.label16);
            this.tabPage6.Controls.Add(this.label8);
            this.tabPage6.Controls.Add(this.formsPlotAnalysisTemporalX);
            this.tabPage6.Controls.Add(this.formsPlotAnalysisTemporalY);
            this.tabPage6.Controls.Add(this.formsPlotAnalysisTemporalZ);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(1252, 523);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Analysis Temporal";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(595, 14);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(14, 13);
            this.label17.TabIndex = 23;
            this.label17.Text = "Y";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(965, 14);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(58, 13);
            this.label16.TabIndex = 22;
            this.label16.Text = "Resultante";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(178, 14);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "X";
            // 
            // formsPlotAnalysisTemporalX
            // 
            this.formsPlotAnalysisTemporalX.Location = new System.Drawing.Point(6, 30);
            this.formsPlotAnalysisTemporalX.Name = "formsPlotAnalysisTemporalX";
            this.formsPlotAnalysisTemporalX.Size = new System.Drawing.Size(405, 361);
            this.formsPlotAnalysisTemporalX.TabIndex = 18;
            // 
            // formsPlotAnalysisTemporalY
            // 
            this.formsPlotAnalysisTemporalY.Location = new System.Drawing.Point(425, 30);
            this.formsPlotAnalysisTemporalY.Name = "formsPlotAnalysisTemporalY";
            this.formsPlotAnalysisTemporalY.Size = new System.Drawing.Size(405, 361);
            this.formsPlotAnalysisTemporalY.TabIndex = 19;
            // 
            // formsPlotAnalysisTemporalZ
            // 
            this.formsPlotAnalysisTemporalZ.Location = new System.Drawing.Point(836, 30);
            this.formsPlotAnalysisTemporalZ.Name = "formsPlotAnalysisTemporalZ";
            this.formsPlotAnalysisTemporalZ.Size = new System.Drawing.Size(405, 361);
            this.formsPlotAnalysisTemporalZ.TabIndex = 20;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.btnExportWAV);
            this.tabPage7.Controls.Add(this.btn240250);
            this.tabPage7.Controls.Add(this.btn250300);
            this.tabPage7.Controls.Add(this.btn230240);
            this.tabPage7.Controls.Add(this.btn210220);
            this.tabPage7.Controls.Add(this.btn220230);
            this.tabPage7.Controls.Add(this.btn200210);
            this.tabPage7.Controls.Add(this.btnUnselectAll);
            this.tabPage7.Controls.Add(this.btnSelectAll);
            this.tabPage7.Controls.Add(this.btnUpdateAnalysisSection);
            this.tabPage7.Controls.Add(this.lstSectionSelector);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(1252, 523);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "Section selector";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // btnExportWAV
            // 
            this.btnExportWAV.Location = new System.Drawing.Point(629, 7);
            this.btnExportWAV.Name = "btnExportWAV";
            this.btnExportWAV.Size = new System.Drawing.Size(75, 23);
            this.btnExportWAV.TabIndex = 10;
            this.btnExportWAV.Text = "Export WAV";
            this.btnExportWAV.UseVisualStyleBackColor = true;
            this.btnExportWAV.Click += new System.EventHandler(this.btnExportWAV_Click);
            // 
            // btn240250
            // 
            this.btn240250.Location = new System.Drawing.Point(725, 66);
            this.btn240250.Name = "btn240250";
            this.btn240250.Size = new System.Drawing.Size(75, 23);
            this.btn240250.TabIndex = 9;
            this.btn240250.Text = "240-250";
            this.btn240250.UseVisualStyleBackColor = true;
            this.btn240250.Click += new System.EventHandler(this.btn240250_Click);
            // 
            // btn250300
            // 
            this.btn250300.Location = new System.Drawing.Point(817, 66);
            this.btn250300.Name = "btn250300";
            this.btn250300.Size = new System.Drawing.Size(75, 23);
            this.btn250300.TabIndex = 8;
            this.btn250300.Text = "250-300";
            this.btn250300.UseVisualStyleBackColor = true;
            this.btn250300.Click += new System.EventHandler(this.btn250300_Click);
            // 
            // btn230240
            // 
            this.btn230240.Location = new System.Drawing.Point(629, 66);
            this.btn230240.Name = "btn230240";
            this.btn230240.Size = new System.Drawing.Size(75, 23);
            this.btn230240.TabIndex = 7;
            this.btn230240.Text = "230-240";
            this.btn230240.UseVisualStyleBackColor = true;
            this.btn230240.Click += new System.EventHandler(this.btn230240_Click);
            // 
            // btn210220
            // 
            this.btn210220.Location = new System.Drawing.Point(725, 37);
            this.btn210220.Name = "btn210220";
            this.btn210220.Size = new System.Drawing.Size(75, 23);
            this.btn210220.TabIndex = 6;
            this.btn210220.Text = "210-220";
            this.btn210220.UseVisualStyleBackColor = true;
            this.btn210220.Click += new System.EventHandler(this.btn210220_Click);
            // 
            // btn220230
            // 
            this.btn220230.Location = new System.Drawing.Point(817, 37);
            this.btn220230.Name = "btn220230";
            this.btn220230.Size = new System.Drawing.Size(75, 23);
            this.btn220230.TabIndex = 5;
            this.btn220230.Text = "220-230";
            this.btn220230.UseVisualStyleBackColor = true;
            this.btn220230.Click += new System.EventHandler(this.btn220230_Click);
            // 
            // btn200210
            // 
            this.btn200210.Location = new System.Drawing.Point(629, 37);
            this.btn200210.Name = "btn200210";
            this.btn200210.Size = new System.Drawing.Size(75, 23);
            this.btn200210.TabIndex = 4;
            this.btn200210.Text = "200-210";
            this.btn200210.UseVisualStyleBackColor = true;
            this.btn200210.Click += new System.EventHandler(this.btn200210_Click);
            // 
            // btnUnselectAll
            // 
            this.btnUnselectAll.Location = new System.Drawing.Point(514, 66);
            this.btnUnselectAll.Name = "btnUnselectAll";
            this.btnUnselectAll.Size = new System.Drawing.Size(75, 23);
            this.btnUnselectAll.TabIndex = 3;
            this.btnUnselectAll.Text = "unselect all";
            this.btnUnselectAll.UseVisualStyleBackColor = true;
            this.btnUnselectAll.Click += new System.EventHandler(this.btnUnselectAll_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(514, 37);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 2;
            this.btnSelectAll.Text = "Select Alll";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnUpdateAnalysisSection
            // 
            this.btnUpdateAnalysisSection.Location = new System.Drawing.Point(514, 7);
            this.btnUpdateAnalysisSection.Name = "btnUpdateAnalysisSection";
            this.btnUpdateAnalysisSection.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateAnalysisSection.TabIndex = 1;
            this.btnUpdateAnalysisSection.Text = "Analyze";
            this.btnUpdateAnalysisSection.UseVisualStyleBackColor = true;
            this.btnUpdateAnalysisSection.Click += new System.EventHandler(this.btnUpdateAnalysisSection_Click);
            // 
            // lstSectionSelector
            // 
            this.lstSectionSelector.CheckOnClick = true;
            this.lstSectionSelector.FormattingEnabled = true;
            this.lstSectionSelector.Location = new System.Drawing.Point(22, 6);
            this.lstSectionSelector.Name = "lstSectionSelector";
            this.lstSectionSelector.Size = new System.Drawing.Size(468, 499);
            this.lstSectionSelector.TabIndex = 0;
            // 
            // tabPage14
            // 
            this.tabPage14.Controls.Add(this.richTextBox1);
            this.tabPage14.Location = new System.Drawing.Point(4, 22);
            this.tabPage14.Name = "tabPage14";
            this.tabPage14.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage14.Size = new System.Drawing.Size(1252, 523);
            this.tabPage14.TabIndex = 8;
            this.tabPage14.Text = "Help";
            this.tabPage14.UseVisualStyleBackColor = true;
            // 
            // chkOrderTracking
            // 
            this.chkOrderTracking.AutoSize = true;
            this.chkOrderTracking.Checked = true;
            this.chkOrderTracking.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOrderTracking.Location = new System.Drawing.Point(638, 39);
            this.chkOrderTracking.Name = "chkOrderTracking";
            this.chkOrderTracking.Size = new System.Drawing.Size(149, 17);
            this.chkOrderTracking.TabIndex = 46;
            this.chkOrderTracking.Text = "Order Tracking interpolate";
            this.chkOrderTracking.UseVisualStyleBackColor = true;
            this.chkOrderTracking.CheckedChanged += new System.EventHandler(this.chkOrderTracking_CheckedChanged);
            // 
            // chkZeroPhase
            // 
            this.chkZeroPhase.AutoSize = true;
            this.chkZeroPhase.Location = new System.Drawing.Point(729, 1);
            this.chkZeroPhase.Name = "chkZeroPhase";
            this.chkZeroPhase.Size = new System.Drawing.Size(78, 17);
            this.chkZeroPhase.TabIndex = 47;
            this.chkZeroPhase.Text = "ZeroPhase";
            this.chkZeroPhase.UseVisualStyleBackColor = true;
            this.chkZeroPhase.CheckedChanged += new System.EventHandler(this.chkZeroPhase_CheckedChanged);
            // 
            // lblPeak
            // 
            this.lblPeak.AutoSize = true;
            this.lblPeak.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPeak.Location = new System.Drawing.Point(1130, 8);
            this.lblPeak.Name = "lblPeak";
            this.lblPeak.Size = new System.Drawing.Size(0, 9);
            this.lblPeak.TabIndex = 48;
            // 
            // txtFFTLimit
            // 
            this.txtFFTLimit.Location = new System.Drawing.Point(916, 0);
            this.txtFFTLimit.Name = "txtFFTLimit";
            this.txtFFTLimit.Size = new System.Drawing.Size(40, 20);
            this.txtFFTLimit.TabIndex = 49;
            this.txtFFTLimit.Text = "100";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(861, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 50;
            this.label5.Text = "Limit FFT";
            // 
            // chkShowResultante
            // 
            this.chkShowResultante.AutoSize = true;
            this.chkShowResultante.Location = new System.Drawing.Point(399, 64);
            this.chkShowResultante.Name = "chkShowResultante";
            this.chkShowResultante.Size = new System.Drawing.Size(107, 17);
            this.chkShowResultante.TabIndex = 52;
            this.chkShowResultante.Text = "Show Resultante";
            this.chkShowResultante.UseVisualStyleBackColor = true;
            this.chkShowResultante.CheckedChanged += new System.EventHandler(this.chkShowResultante_CheckedChanged);
            // 
            // chkRemoveDC
            // 
            this.chkRemoveDC.AutoSize = true;
            this.chkRemoveDC.Location = new System.Drawing.Point(788, 20);
            this.chkRemoveDC.Name = "chkRemoveDC";
            this.chkRemoveDC.Size = new System.Drawing.Size(84, 17);
            this.chkRemoveDC.TabIndex = 57;
            this.chkRemoveDC.Text = "Remove DC";
            this.chkRemoveDC.UseVisualStyleBackColor = true;
            this.chkRemoveDC.CheckedChanged += new System.EventHandler(this.chkRemoveDC_CheckedChanged);
            // 
            // cbxSensor
            // 
            this.cbxSensor.FormattingEnabled = true;
            this.cbxSensor.Items.AddRange(new object[] {
            "MPU6500/9250",
            "LSM6DS3"});
            this.cbxSensor.Location = new System.Drawing.Point(126, 9);
            this.cbxSensor.Name = "cbxSensor";
            this.cbxSensor.Size = new System.Drawing.Size(89, 21);
            this.cbxSensor.TabIndex = 58;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(15, 6);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1231, 501);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1287, 639);
            this.Controls.Add(this.cbxSensor);
            this.Controls.Add(this.chkRemoveDC);
            this.Controls.Add(this.chkShowResultante);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtFFTLimit);
            this.Controls.Add(this.lblPeak);
            this.Controls.Add(this.chkZeroPhase);
            this.Controls.Add(this.chkOrderTracking);
            this.Controls.Add(this.chkShowZ);
            this.Controls.Add(this.chkShowX);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.chkShowY);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSampleRate);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.chkLowPassFilter);
            this.Controls.Add(this.chkSum);
            this.Controls.Add(this.chkAbsolute);
            this.Controls.Add(this.cbxFFT);
            this.Controls.Add(this.chkFFT);
            this.Controls.Add(this.btnAnalyze);
            this.Controls.Add(this.lblStats);
            this.Controls.Add(this.txtMaxRPM);
            this.Controls.Add(this.txtMinRPM);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnEndCapture);
            this.Controls.Add(this.btnStartCapture);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.txtRPM);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtCom);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Equilibreuse";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage8.ResumeLayout(false);
            this.tabPage8.PerformLayout();
            this.tabControl2.ResumeLayout(false);
            this.tabPage9.ResumeLayout(false);
            this.tabPage9.PerformLayout();
            this.tabPage10.ResumeLayout(false);
            this.tabPage10.PerformLayout();
            this.tabPage11.ResumeLayout(false);
            this.tabPage11.PerformLayout();
            this.tabPage12.ResumeLayout(false);
            this.tabPage12.PerformLayout();
            this.tabPage13.ResumeLayout(false);
            this.tabPage13.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.tabPage7.ResumeLayout(false);
            this.tabPage14.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCom;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label txtRPM;
        private System.Windows.Forms.Label txtStatus;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnStartCapture;
        private System.Windows.Forms.Button btnEndCapture;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMinRPM;
        private System.Windows.Forms.TextBox txtMaxRPM;
        private System.Windows.Forms.Label lblStats;
        private ScottPlot.FormsPlot formsPlotX;
        private ScottPlot.FormsPlot formsPlotY;
        private ScottPlot.FormsPlot formsPlotZ;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
        private ScottPlot.FormsPlot formsPlotAnalysis;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.Label lblRecordNumber;
        private System.Windows.Forms.CheckBox chkShowX;
        private System.Windows.Forms.CheckBox chkShowY;
        private System.Windows.Forms.CheckBox chkShowZ;
        private System.Windows.Forms.CheckBox chkFFT;
        private System.Windows.Forms.ComboBox cbxFFT;
        private System.Windows.Forms.CheckBox chkAbsolute;
        private System.Windows.Forms.ListBox lstPeakX;
        private System.Windows.Forms.ListBox lstPeakY;
        private System.Windows.Forms.ListBox lstPeakZ;
        private System.Windows.Forms.ListBox lstPeakZCompiled;
        private System.Windows.Forms.ListBox lstPeakYCompiled;
        private System.Windows.Forms.ListBox lstPeakXCompiled;
        private System.Windows.Forms.CheckBox chkSum;
        private System.Windows.Forms.CheckBox chkLowPassFilter;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.TextBox txtSampleRate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private ScottPlot.FormsPlot formsPlotGlobal;
        private System.Windows.Forms.ListBox lstPeakGlobalX;
        private System.Windows.Forms.ListBox lstPeakGlobalZ;
        private System.Windows.Forms.ListBox lstPeakGlobalY;
        private System.Windows.Forms.CheckBox chkOrderTracking;
        private System.Windows.Forms.CheckBox chkZeroPhase;
        private System.Windows.Forms.Label lblPeak;
        private System.Windows.Forms.TextBox txtFFTLimit;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage4;
        private ScottPlot.FormsPlot formsPlotGyro;
        private System.Windows.Forms.CheckBox chkShowResultante;
        private System.Windows.Forms.ListBox lstPeakGyroZ;
        private System.Windows.Forms.ListBox lstPeakGyroY;
        private System.Windows.Forms.ListBox lstPeakGyroX;
        private System.Windows.Forms.TabPage tabPage6;
        private ScottPlot.FormsPlot formsPlotAnalysisTemporalX;
        private ScottPlot.FormsPlot formsPlotAnalysisTemporalY;
        private ScottPlot.FormsPlot formsPlotAnalysisTemporalZ;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListBox lstPeakResultanteCompiled;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ListBox lstPeakResultanteGlobal;
        private System.Windows.Forms.ListBox lstPeakResultanteGyro;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.Button btnUnselectAll;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnUpdateAnalysisSection;
        private System.Windows.Forms.CheckedListBox lstSectionSelector;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.ListBox lstSimulationGyro;
        private System.Windows.Forms.ListBox lstSimulationGlobal;
        private System.Windows.Forms.ListBox lstSimulationCompiled;
        private System.Windows.Forms.Button btn240250;
        private System.Windows.Forms.Button btn250300;
        private System.Windows.Forms.Button btn230240;
        private System.Windows.Forms.Button btn210220;
        private System.Windows.Forms.Button btn220230;
        private System.Windows.Forms.Button btn200210;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ListBox lstSimulationTurnByTurn;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage9;
        private System.Windows.Forms.Label label24;
        private ScottPlot.FormsPlot formsPlotT1O;
        private System.Windows.Forms.Label label25;
        private ScottPlot.FormsPlot formsPlotT1I;
        private System.Windows.Forms.Label label23;
        private ScottPlot.FormsPlot formsPlotT1Y;
        private System.Windows.Forms.Label label18;
        private ScottPlot.FormsPlot formsPlotT1X;
        private System.Windows.Forms.TabPage tabPage10;
        private System.Windows.Forms.Label label26;
        private ScottPlot.FormsPlot formsPlotT2O;
        private System.Windows.Forms.Label label27;
        private ScottPlot.FormsPlot formsPlotT2I;
        private System.Windows.Forms.Label label28;
        private ScottPlot.FormsPlot formsPlotT2Y;
        private System.Windows.Forms.Label label29;
        private ScottPlot.FormsPlot formsPlotT2X;
        private System.Windows.Forms.TabPage tabPage11;
        private System.Windows.Forms.Label label30;
        private ScottPlot.FormsPlot formsPlotT3O;
        private System.Windows.Forms.Label label31;
        private ScottPlot.FormsPlot formsPlotT3I;
        private System.Windows.Forms.Label label32;
        private ScottPlot.FormsPlot formsPlotT3Y;
        private System.Windows.Forms.Label label33;
        private ScottPlot.FormsPlot formsPlotT3X;
        private System.Windows.Forms.TabPage tabPage12;
        private System.Windows.Forms.Label label34;
        private ScottPlot.FormsPlot formsPlotT4O;
        private System.Windows.Forms.Label label35;
        private ScottPlot.FormsPlot formsPlotT4I;
        private System.Windows.Forms.Label label36;
        private ScottPlot.FormsPlot formsPlotT4Y;
        private System.Windows.Forms.Label label37;
        private ScottPlot.FormsPlot formsPlotT4X;
        private System.Windows.Forms.TabPage tabPage13;
        private System.Windows.Forms.Label label38;
        private ScottPlot.FormsPlot formsPlotT5O;
        private System.Windows.Forms.Label label39;
        private ScottPlot.FormsPlot formsPlotT5I;
        private System.Windows.Forms.Label label40;
        private ScottPlot.FormsPlot formsPlotT5Y;
        private System.Windows.Forms.Label label41;
        private ScottPlot.FormsPlot formsPlotT5X;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label lblTotalSelected;
        private System.Windows.Forms.CheckBox chkRemoveDC;
        private System.Windows.Forms.Button btnExportWAV;
        private System.Windows.Forms.TabPage tabPage14;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox cbxSensor;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

