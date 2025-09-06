using ScottPlot.WinForms;

namespace equilibreuse
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
            this.formsPlotX = new ScottPlot.WinForms.FormsPlot();
            this.formsPlotY = new ScottPlot.WinForms.FormsPlot();
            this.formsPlotZ = new ScottPlot.WinForms.FormsPlot();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.formsPlotAnalysis = new ScottPlot.WinForms.FormsPlot();
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
            this.formsPlotGlobal = new ScottPlot.WinForms.FormsPlot();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.lstPeakResultanteGyro = new System.Windows.Forms.ListBox();
            this.lstPeakGyroZ = new System.Windows.Forms.ListBox();
            this.lstPeakGyroY = new System.Windows.Forms.ListBox();
            this.lstPeakGyroX = new System.Windows.Forms.ListBox();
            this.formsPlotGyro = new ScottPlot.WinForms.FormsPlot();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.label43 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.formsPlotT1O = new ScottPlot.WinForms.FormsPlot();
            this.formsPlotT1X = new ScottPlot.WinForms.FormsPlot();
            this.label25 = new System.Windows.Forms.Label();
            this.formsPlotT1Y = new ScottPlot.WinForms.FormsPlot();
            this.formsPlotT1I = new ScottPlot.WinForms.FormsPlot();
            this.label18 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.tabPage15 = new System.Windows.Forms.TabPage();
            this.label46 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.btnClearAnalysisHistory = new System.Windows.Forms.Button();
            this.dataGridY = new System.Windows.Forms.DataGridView();
            this.dataGridX = new System.Windows.Forms.DataGridView();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.lstSelectToursAnalysis = new System.Windows.Forms.CheckedListBox();
            this.label31 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.cbxMagData = new System.Windows.Forms.ComboBox();
            this.cbxAngleData = new System.Windows.Forms.ComboBox();
            this.label51 = new System.Windows.Forms.Label();
            this.lblAngleYCorrect = new System.Windows.Forms.Label();
            this.lblAngleXCorrect = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.txtMagGrams1 = new System.Windows.Forms.TextBox();
            this.txtMagnitudeY = new System.Windows.Forms.TextBox();
            this.txtMagnitudeX = new System.Windows.Forms.TextBox();
            this.lblAngleYStat = new System.Windows.Forms.Label();
            this.lblAngleXStat = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.txtAngleYCalc = new System.Windows.Forms.TextBox();
            this.txtAngleXCalc = new System.Windows.Forms.TextBox();
            this.btnCalculateCorrection = new System.Windows.Forms.Button();
            this.txtCorrectYTemporal = new System.Windows.Forms.TextBox();
            this.txtCorrectXTemporal = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.btnFindAngles = new System.Windows.Forms.Button();
            this.chkScaleGyro = new System.Windows.Forms.CheckBox();
            this.txtCorrectAngleY = new System.Windows.Forms.TextBox();
            this.txtCorrectAngleX = new System.Windows.Forms.TextBox();
            this.chkUseYGyro = new System.Windows.Forms.CheckBox();
            this.chkUseXGyro = new System.Windows.Forms.CheckBox();
            this.btnSaveData = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label50 = new System.Windows.Forms.Label();
            this.txtXMagBalanced = new System.Windows.Forms.TextBox();
            this.label52 = new System.Windows.Forms.Label();
            this.txtXMagG1 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label47 = new System.Windows.Forms.Label();
            this.txtYMagBalanced = new System.Windows.Forms.TextBox();
            this.label48 = new System.Windows.Forms.Label();
            this.txtYMag1 = new System.Windows.Forms.TextBox();
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.chkOrderTracking = new System.Windows.Forms.CheckBox();
            this.txtFFTLimit = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkShowResultante = new System.Windows.Forms.CheckBox();
            this.chkRemoveDC = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cbxSensor = new System.Windows.Forms.ComboBox();
            this.cbxFFTSingle = new System.Windows.Forms.ComboBox();
            this.chkFFTSingle = new System.Windows.Forms.CheckBox();
            this.chkDb = new System.Windows.Forms.CheckBox();
            this.chkPassband = new System.Windows.Forms.CheckBox();
            this.txtGain = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtFilterOrder = new System.Windows.Forms.TextBox();
            this.label44 = new System.Windows.Forms.Label();
            this.lblStatX = new System.Windows.Forms.Label();
            this.lblStatY = new System.Windows.Forms.Label();
            this.cbxFilterTypes = new System.Windows.Forms.ComboBox();
            this.cbxSmoothing = new System.Windows.Forms.ComboBox();
            this.chkClockwise = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.tabPage15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridX)).BeginInit();
            this.tabPage7.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.formsPlotX.DisplayScale = 0F;
            this.formsPlotX.Location = new System.Drawing.Point(11, 35);
            this.formsPlotX.Name = "formsPlotX";
            this.formsPlotX.Size = new System.Drawing.Size(405, 361);
            this.formsPlotX.TabIndex = 15;
            this.formsPlotX.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formsPlotAnalysis_MouseMove);
            // 
            // formsPlotY
            // 
            this.formsPlotY.DisplayScale = 0F;
            this.formsPlotY.Location = new System.Drawing.Point(430, 35);
            this.formsPlotY.Name = "formsPlotY";
            this.formsPlotY.Size = new System.Drawing.Size(405, 361);
            this.formsPlotY.TabIndex = 16;
            this.formsPlotY.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formsPlotAnalysis_MouseMove);
            // 
            // formsPlotZ
            // 
            this.formsPlotZ.DisplayScale = 0F;
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
            this.formsPlotAnalysis.DisplayScale = 0F;
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
            this.chkFFT.Location = new System.Drawing.Point(790, 63);
            this.chkFFT.Name = "chkFFT";
            this.chkFFT.Size = new System.Drawing.Size(78, 17);
            this.chkFFT.TabIndex = 27;
            this.chkFFT.Text = "FFT Global";
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
            this.cbxFFT.Location = new System.Drawing.Point(870, 60);
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
            this.chkSum.Location = new System.Drawing.Point(744, 23);
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
            this.txtFilter.Location = new System.Drawing.Point(719, 1);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(25, 20);
            this.txtFilter.TabIndex = 41;
            this.txtFilter.Text = "10";
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
            this.tabControl1.Controls.Add(this.tabPage8);
            this.tabControl1.Controls.Add(this.tabPage15);
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
            this.formsPlotGlobal.DisplayScale = 0F;
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
            this.formsPlotGyro.DisplayScale = 0F;
            this.formsPlotGyro.Location = new System.Drawing.Point(6, 6);
            this.formsPlotGyro.Name = "formsPlotGyro";
            this.formsPlotGyro.Size = new System.Drawing.Size(1240, 392);
            this.formsPlotGyro.TabIndex = 52;
            this.formsPlotGyro.MouseMove += new System.Windows.Forms.MouseEventHandler(this.formsPlotAnalysis_MouseMove);
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.label43);
            this.tabPage8.Controls.Add(this.label24);
            this.tabPage8.Controls.Add(this.formsPlotT1O);
            this.tabPage8.Controls.Add(this.formsPlotT1X);
            this.tabPage8.Controls.Add(this.label25);
            this.tabPage8.Controls.Add(this.formsPlotT1Y);
            this.tabPage8.Controls.Add(this.formsPlotT1I);
            this.tabPage8.Controls.Add(this.label18);
            this.tabPage8.Controls.Add(this.label23);
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
            this.label43.Size = new System.Drawing.Size(468, 13);
            this.label43.TabIndex = 28;
            this.label43.Text = "BLUE: Turn by Turn, BLACK: Avg Turn by Turn, RED: Compiled, GREEN: Global, YELLOW" +
    ": Gyro";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(892, 248);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(113, 13);
            this.label24.TabIndex = 26;
            this.label24.Text = "Correction Outer angle";
            // 
            // formsPlotT1O
            // 
            this.formsPlotT1O.DisplayScale = 0F;
            this.formsPlotT1O.Location = new System.Drawing.Point(654, 264);
            this.formsPlotT1O.Name = "formsPlotT1O";
            this.formsPlotT1O.Size = new System.Drawing.Size(555, 227);
            this.formsPlotT1O.TabIndex = 25;
            // 
            // formsPlotT1X
            // 
            this.formsPlotT1X.DisplayScale = 0F;
            this.formsPlotT1X.Location = new System.Drawing.Point(24, 36);
            this.formsPlotT1X.Name = "formsPlotT1X";
            this.formsPlotT1X.Size = new System.Drawing.Size(555, 203);
            this.formsPlotT1X.TabIndex = 19;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(225, 248);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(111, 13);
            this.label25.TabIndex = 24;
            this.label25.Text = "Correction Inner angle";
            // 
            // formsPlotT1Y
            // 
            this.formsPlotT1Y.DisplayScale = 0F;
            this.formsPlotT1Y.Location = new System.Drawing.Point(654, 36);
            this.formsPlotT1Y.Name = "formsPlotT1Y";
            this.formsPlotT1Y.Size = new System.Drawing.Size(555, 203);
            this.formsPlotT1Y.TabIndex = 21;
            // 
            // formsPlotT1I
            // 
            this.formsPlotT1I.DisplayScale = 0F;
            this.formsPlotT1I.Location = new System.Drawing.Point(24, 264);
            this.formsPlotT1I.Name = "formsPlotT1I";
            this.formsPlotT1I.Size = new System.Drawing.Size(555, 227);
            this.formsPlotT1I.TabIndex = 23;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(163, 20);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(74, 13);
            this.label18.TabIndex = 20;
            this.label18.Text = "X Angle found";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(877, 20);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(74, 13);
            this.label23.TabIndex = 22;
            this.label23.Text = "Y Angle found";
            // 
            // tabPage15
            // 
            this.tabPage15.Controls.Add(this.label46);
            this.tabPage15.Controls.Add(this.label45);
            this.tabPage15.Controls.Add(this.btnClearAnalysisHistory);
            this.tabPage15.Controls.Add(this.dataGridY);
            this.tabPage15.Controls.Add(this.dataGridX);
            this.tabPage15.Location = new System.Drawing.Point(4, 22);
            this.tabPage15.Name = "tabPage15";
            this.tabPage15.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage15.Size = new System.Drawing.Size(1252, 523);
            this.tabPage15.TabIndex = 9;
            this.tabPage15.Text = "Analysis History";
            this.tabPage15.UseVisualStyleBackColor = true;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(555, 233);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(14, 13);
            this.label46.TabIndex = 4;
            this.label46.Text = "Y";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(557, 11);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(14, 13);
            this.label45.TabIndex = 3;
            this.label45.Text = "X";
            // 
            // btnClearAnalysisHistory
            // 
            this.btnClearAnalysisHistory.Location = new System.Drawing.Point(6, 7);
            this.btnClearAnalysisHistory.Name = "btnClearAnalysisHistory";
            this.btnClearAnalysisHistory.Size = new System.Drawing.Size(75, 23);
            this.btnClearAnalysisHistory.TabIndex = 2;
            this.btnClearAnalysisHistory.Text = "Clear";
            this.btnClearAnalysisHistory.UseVisualStyleBackColor = true;
            this.btnClearAnalysisHistory.Click += new System.EventHandler(this.btnClearAnalysisHistory_Click);
            // 
            // dataGridY
            // 
            this.dataGridY.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dataGridY.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridY.Location = new System.Drawing.Point(6, 255);
            this.dataGridY.Name = "dataGridY";
            this.dataGridY.Size = new System.Drawing.Size(1228, 272);
            this.dataGridY.TabIndex = 1;
            // 
            // dataGridX
            // 
            this.dataGridX.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dataGridX.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridX.Location = new System.Drawing.Point(6, 36);
            this.dataGridX.Name = "dataGridX";
            this.dataGridX.Size = new System.Drawing.Size(1228, 185);
            this.dataGridX.TabIndex = 0;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.lstSelectToursAnalysis);
            this.tabPage7.Controls.Add(this.label31);
            this.tabPage7.Controls.Add(this.label30);
            this.tabPage7.Controls.Add(this.cbxMagData);
            this.tabPage7.Controls.Add(this.cbxAngleData);
            this.tabPage7.Controls.Add(this.label51);
            this.tabPage7.Controls.Add(this.lblAngleYCorrect);
            this.tabPage7.Controls.Add(this.lblAngleXCorrect);
            this.tabPage7.Controls.Add(this.label29);
            this.tabPage7.Controls.Add(this.txtMagGrams1);
            this.tabPage7.Controls.Add(this.txtMagnitudeY);
            this.tabPage7.Controls.Add(this.txtMagnitudeX);
            this.tabPage7.Controls.Add(this.lblAngleYStat);
            this.tabPage7.Controls.Add(this.lblAngleXStat);
            this.tabPage7.Controls.Add(this.label28);
            this.tabPage7.Controls.Add(this.txtAngleYCalc);
            this.tabPage7.Controls.Add(this.txtAngleXCalc);
            this.tabPage7.Controls.Add(this.btnCalculateCorrection);
            this.tabPage7.Controls.Add(this.txtCorrectYTemporal);
            this.tabPage7.Controls.Add(this.txtCorrectXTemporal);
            this.tabPage7.Controls.Add(this.label27);
            this.tabPage7.Controls.Add(this.label26);
            this.tabPage7.Controls.Add(this.btnFindAngles);
            this.tabPage7.Controls.Add(this.chkScaleGyro);
            this.tabPage7.Controls.Add(this.txtCorrectAngleY);
            this.tabPage7.Controls.Add(this.txtCorrectAngleX);
            this.tabPage7.Controls.Add(this.chkUseYGyro);
            this.tabPage7.Controls.Add(this.chkUseXGyro);
            this.tabPage7.Controls.Add(this.btnSaveData);
            this.tabPage7.Controls.Add(this.groupBox2);
            this.tabPage7.Controls.Add(this.groupBox1);
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
            // lstSelectToursAnalysis
            // 
            this.lstSelectToursAnalysis.CheckOnClick = true;
            this.lstSelectToursAnalysis.FormattingEnabled = true;
            this.lstSelectToursAnalysis.Location = new System.Drawing.Point(1017, 8);
            this.lstSelectToursAnalysis.Name = "lstSelectToursAnalysis";
            this.lstSelectToursAnalysis.Size = new System.Drawing.Size(218, 109);
            this.lstSelectToursAnalysis.TabIndex = 104;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(967, 159);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(72, 13);
            this.label31.TabIndex = 103;
            this.label31.Text = "MAGNITUDE";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(968, 132);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(43, 13);
            this.label30.TabIndex = 102;
            this.label30.Text = "ANGLE";
            // 
            // cbxMagData
            // 
            this.cbxMagData.FormattingEnabled = true;
            this.cbxMagData.Items.AddRange(new object[] {
            "Global Magnitude",
            "Global Mag / nb of turn",
            "Global PSD",
            "Global PkPk Filter",
            "Global PkPk LP 5hz",
            "Compiled Mag",
            "Turn by Turn Mag"});
            this.cbxMagData.Location = new System.Drawing.Point(1045, 156);
            this.cbxMagData.Name = "cbxMagData";
            this.cbxMagData.Size = new System.Drawing.Size(121, 21);
            this.cbxMagData.TabIndex = 101;
            // 
            // cbxAngleData
            // 
            this.cbxAngleData.FormattingEnabled = true;
            this.cbxAngleData.Items.AddRange(new object[] {
            "Global FFT",
            "Global Temporal",
            "Compiled FFT",
            "Compiled Temporal",
            "Turn by Turn FFT",
            "Turn by Turn Temporal",
            "Global lock-in",
            "AVG FFT-Lockin"});
            this.cbxAngleData.Location = new System.Drawing.Point(1045, 129);
            this.cbxAngleData.Name = "cbxAngleData";
            this.cbxAngleData.Size = new System.Drawing.Size(121, 21);
            this.cbxAngleData.TabIndex = 100;
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(872, 339);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(13, 13);
            this.label51.TabIndex = 73;
            this.label51.Text = "g";
            // 
            // lblAngleYCorrect
            // 
            this.lblAngleYCorrect.AutoSize = true;
            this.lblAngleYCorrect.Location = new System.Drawing.Point(907, 227);
            this.lblAngleYCorrect.Name = "lblAngleYCorrect";
            this.lblAngleYCorrect.Size = new System.Drawing.Size(92, 13);
            this.lblAngleYCorrect.TabIndex = 99;
            this.lblAngleYCorrect.Text = "Angle to correct Y";
            // 
            // lblAngleXCorrect
            // 
            this.lblAngleXCorrect.AutoSize = true;
            this.lblAngleXCorrect.Location = new System.Drawing.Point(907, 185);
            this.lblAngleXCorrect.Name = "lblAngleXCorrect";
            this.lblAngleXCorrect.Size = new System.Drawing.Size(92, 13);
            this.lblAngleXCorrect.TabIndex = 98;
            this.lblAngleXCorrect.Text = "Angle to correct X";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(827, 160);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(72, 13);
            this.label29.TabIndex = 97;
            this.label29.Text = "MAGNITUDE";
            // 
            // txtMagGrams1
            // 
            this.txtMagGrams1.Location = new System.Drawing.Point(836, 336);
            this.txtMagGrams1.Name = "txtMagGrams1";
            this.txtMagGrams1.Size = new System.Drawing.Size(32, 20);
            this.txtMagGrams1.TabIndex = 72;
            // 
            // txtMagnitudeY
            // 
            this.txtMagnitudeY.Location = new System.Drawing.Point(836, 223);
            this.txtMagnitudeY.Name = "txtMagnitudeY";
            this.txtMagnitudeY.Size = new System.Drawing.Size(61, 20);
            this.txtMagnitudeY.TabIndex = 96;
            // 
            // txtMagnitudeX
            // 
            this.txtMagnitudeX.Location = new System.Drawing.Point(835, 185);
            this.txtMagnitudeX.Name = "txtMagnitudeX";
            this.txtMagnitudeX.Size = new System.Drawing.Size(61, 20);
            this.txtMagnitudeX.TabIndex = 95;
            // 
            // lblAngleYStat
            // 
            this.lblAngleYStat.AutoSize = true;
            this.lblAngleYStat.Location = new System.Drawing.Point(512, 227);
            this.lblAngleYStat.Name = "lblAngleYStat";
            this.lblAngleYStat.Size = new System.Drawing.Size(14, 13);
            this.lblAngleYStat.TabIndex = 94;
            this.lblAngleYStat.Text = "Y";
            // 
            // lblAngleXStat
            // 
            this.lblAngleXStat.AutoSize = true;
            this.lblAngleXStat.Location = new System.Drawing.Point(512, 185);
            this.lblAngleXStat.Name = "lblAngleXStat";
            this.lblAngleXStat.Size = new System.Drawing.Size(14, 13);
            this.lblAngleXStat.TabIndex = 93;
            this.lblAngleXStat.Text = "X";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(746, 160);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(43, 13);
            this.label28.TabIndex = 92;
            this.label28.Text = "ANGLE";
            // 
            // txtAngleYCalc
            // 
            this.txtAngleYCalc.Location = new System.Drawing.Point(735, 223);
            this.txtAngleYCalc.Name = "txtAngleYCalc";
            this.txtAngleYCalc.Size = new System.Drawing.Size(61, 20);
            this.txtAngleYCalc.TabIndex = 91;
            // 
            // txtAngleXCalc
            // 
            this.txtAngleXCalc.Location = new System.Drawing.Point(734, 185);
            this.txtAngleXCalc.Name = "txtAngleXCalc";
            this.txtAngleXCalc.Size = new System.Drawing.Size(61, 20);
            this.txtAngleXCalc.TabIndex = 90;
            // 
            // btnCalculateCorrection
            // 
            this.btnCalculateCorrection.Location = new System.Drawing.Point(682, 124);
            this.btnCalculateCorrection.Name = "btnCalculateCorrection";
            this.btnCalculateCorrection.Size = new System.Drawing.Size(165, 23);
            this.btnCalculateCorrection.TabIndex = 89;
            this.btnCalculateCorrection.Text = "Calculate correction";
            this.btnCalculateCorrection.UseVisualStyleBackColor = true;
            this.btnCalculateCorrection.Click += new System.EventHandler(this.btnCalculateCorrection_Click);
            // 
            // txtCorrectYTemporal
            // 
            this.txtCorrectYTemporal.Location = new System.Drawing.Point(1042, 465);
            this.txtCorrectYTemporal.Name = "txtCorrectYTemporal";
            this.txtCorrectYTemporal.Size = new System.Drawing.Size(100, 20);
            this.txtCorrectYTemporal.TabIndex = 88;
            this.txtCorrectYTemporal.Text = "0";
            // 
            // txtCorrectXTemporal
            // 
            this.txtCorrectXTemporal.Location = new System.Drawing.Point(1042, 439);
            this.txtCorrectXTemporal.Name = "txtCorrectXTemporal";
            this.txtCorrectXTemporal.Size = new System.Drawing.Size(100, 20);
            this.txtCorrectXTemporal.TabIndex = 87;
            this.txtCorrectXTemporal.Text = "0";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(935, 465);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(101, 13);
            this.label27.TabIndex = 86;
            this.label27.Text = "Temporal correction";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(935, 445);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(101, 13);
            this.label26.TabIndex = 85;
            this.label26.Text = "Temporal correction";
            // 
            // btnFindAngles
            // 
            this.btnFindAngles.Location = new System.Drawing.Point(951, 6);
            this.btnFindAngles.Name = "btnFindAngles";
            this.btnFindAngles.Size = new System.Drawing.Size(60, 111);
            this.btnFindAngles.TabIndex = 82;
            this.btnFindAngles.Text = "Complete Analysis";
            this.btnFindAngles.UseVisualStyleBackColor = true;
            this.btnFindAngles.Click += new System.EventHandler(this.btnExecuteAnalysis_Click);
            // 
            // chkScaleGyro
            // 
            this.chkScaleGyro.AutoSize = true;
            this.chkScaleGyro.Location = new System.Drawing.Point(546, 488);
            this.chkScaleGyro.Name = "chkScaleGyro";
            this.chkScaleGyro.Size = new System.Drawing.Size(102, 17);
            this.chkScaleGyro.TabIndex = 81;
            this.chkScaleGyro.Text = "Scale Gyro Mag";
            this.chkScaleGyro.UseVisualStyleBackColor = true;
            // 
            // txtCorrectAngleY
            // 
            this.txtCorrectAngleY.Location = new System.Drawing.Point(827, 462);
            this.txtCorrectAngleY.Name = "txtCorrectAngleY";
            this.txtCorrectAngleY.Size = new System.Drawing.Size(100, 20);
            this.txtCorrectAngleY.TabIndex = 80;
            this.txtCorrectAngleY.Text = "90";
            // 
            // txtCorrectAngleX
            // 
            this.txtCorrectAngleX.Location = new System.Drawing.Point(829, 439);
            this.txtCorrectAngleX.Name = "txtCorrectAngleX";
            this.txtCorrectAngleX.Size = new System.Drawing.Size(100, 20);
            this.txtCorrectAngleX.TabIndex = 79;
            this.txtCorrectAngleX.Text = "0";
            // 
            // chkUseYGyro
            // 
            this.chkUseYGyro.AutoSize = true;
            this.chkUseYGyro.Location = new System.Drawing.Point(546, 464);
            this.chkUseYGyro.Name = "chkUseYGyro";
            this.chkUseYGyro.Size = new System.Drawing.Size(278, 17);
            this.chkUseYGyro.TabIndex = 78;
            this.chkUseYGyro.Text = "Use Y gyro instead of accel X, correct angle FFT with";
            this.chkUseYGyro.UseVisualStyleBackColor = true;
            // 
            // chkUseXGyro
            // 
            this.chkUseXGyro.AutoSize = true;
            this.chkUseXGyro.Location = new System.Drawing.Point(546, 441);
            this.chkUseXGyro.Name = "chkUseXGyro";
            this.chkUseXGyro.Size = new System.Drawing.Size(278, 17);
            this.chkUseXGyro.TabIndex = 77;
            this.chkUseXGyro.Text = "Use X gyro instead of accel Y, correct angle FFT with";
            this.chkUseXGyro.UseVisualStyleBackColor = true;
            // 
            // btnSaveData
            // 
            this.btnSaveData.Location = new System.Drawing.Point(777, 412);
            this.btnSaveData.Name = "btnSaveData";
            this.btnSaveData.Size = new System.Drawing.Size(152, 23);
            this.btnSaveData.TabIndex = 76;
            this.btnSaveData.Text = "Save all parameters";
            this.btnSaveData.UseVisualStyleBackColor = true;
            this.btnSaveData.Click += new System.EventHandler(this.btnSaveData_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label50);
            this.groupBox2.Controls.Add(this.txtXMagBalanced);
            this.groupBox2.Controls.Add(this.label52);
            this.groupBox2.Controls.Add(this.txtXMagG1);
            this.groupBox2.Location = new System.Drawing.Point(542, 285);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(270, 116);
            this.groupBox2.TabIndex = 75;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "X calibration";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(36, 29);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(136, 13);
            this.label50.TabIndex = 68;
            this.label50.Text = "Global Mag Ratio balanced";
            // 
            // txtXMagBalanced
            // 
            this.txtXMagBalanced.Location = new System.Drawing.Point(179, 25);
            this.txtXMagBalanced.Name = "txtXMagBalanced";
            this.txtXMagBalanced.Size = new System.Drawing.Size(68, 20);
            this.txtXMagBalanced.TabIndex = 69;
            this.txtXMagBalanced.Text = "2.5";
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(36, 54);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(89, 13);
            this.label52.TabIndex = 70;
            this.label52.Text = "Global Mag Ratio";
            // 
            // txtXMagG1
            // 
            this.txtXMagG1.Location = new System.Drawing.Point(194, 51);
            this.txtXMagG1.Name = "txtXMagG1";
            this.txtXMagG1.Size = new System.Drawing.Size(50, 20);
            this.txtXMagG1.TabIndex = 71;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label47);
            this.groupBox1.Controls.Add(this.txtYMagBalanced);
            this.groupBox1.Controls.Add(this.label48);
            this.groupBox1.Controls.Add(this.txtYMag1);
            this.groupBox1.Location = new System.Drawing.Point(895, 285);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(297, 116);
            this.groupBox1.TabIndex = 74;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Y calibration";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(45, 29);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(136, 13);
            this.label47.TabIndex = 68;
            this.label47.Text = "Global Mag Ratio balanced";
            // 
            // txtYMagBalanced
            // 
            this.txtYMagBalanced.Location = new System.Drawing.Point(193, 25);
            this.txtYMagBalanced.Name = "txtYMagBalanced";
            this.txtYMagBalanced.Size = new System.Drawing.Size(68, 20);
            this.txtYMagBalanced.TabIndex = 69;
            this.txtYMagBalanced.Text = "2.5";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(45, 54);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(89, 13);
            this.label48.TabIndex = 70;
            this.label48.Text = "Global Mag Ratio";
            // 
            // txtYMag1
            // 
            this.txtYMag1.Location = new System.Drawing.Point(193, 51);
            this.txtYMag1.Name = "txtYMag1";
            this.txtYMag1.Size = new System.Drawing.Size(65, 20);
            this.txtYMag1.TabIndex = 71;
            // 
            // btnExportWAV
            // 
            this.btnExportWAV.Location = new System.Drawing.Point(514, 93);
            this.btnExportWAV.Name = "btnExportWAV";
            this.btnExportWAV.Size = new System.Drawing.Size(75, 23);
            this.btnExportWAV.TabIndex = 10;
            this.btnExportWAV.Text = "Export WAV";
            this.btnExportWAV.UseVisualStyleBackColor = true;
            this.btnExportWAV.Click += new System.EventHandler(this.btnExportWAV_Click);
            // 
            // btn240250
            // 
            this.btn240250.Location = new System.Drawing.Point(714, 37);
            this.btn240250.Name = "btn240250";
            this.btn240250.Size = new System.Drawing.Size(75, 23);
            this.btn240250.TabIndex = 9;
            this.btn240250.Text = "240-250";
            this.btn240250.UseVisualStyleBackColor = true;
            this.btn240250.Click += new System.EventHandler(this.btn240250_Click);
            // 
            // btn250300
            // 
            this.btn250300.Location = new System.Drawing.Point(806, 37);
            this.btn250300.Name = "btn250300";
            this.btn250300.Size = new System.Drawing.Size(75, 23);
            this.btn250300.TabIndex = 8;
            this.btn250300.Text = "250-300";
            this.btn250300.UseVisualStyleBackColor = true;
            this.btn250300.Click += new System.EventHandler(this.btn250300_Click);
            // 
            // btn230240
            // 
            this.btn230240.Location = new System.Drawing.Point(618, 37);
            this.btn230240.Name = "btn230240";
            this.btn230240.Size = new System.Drawing.Size(75, 23);
            this.btn230240.TabIndex = 7;
            this.btn230240.Text = "230-240";
            this.btn230240.UseVisualStyleBackColor = true;
            this.btn230240.Click += new System.EventHandler(this.btn230240_Click);
            // 
            // btn210220
            // 
            this.btn210220.Location = new System.Drawing.Point(714, 8);
            this.btn210220.Name = "btn210220";
            this.btn210220.Size = new System.Drawing.Size(75, 23);
            this.btn210220.TabIndex = 6;
            this.btn210220.Text = "210-220";
            this.btn210220.UseVisualStyleBackColor = true;
            this.btn210220.Click += new System.EventHandler(this.btn210220_Click);
            // 
            // btn220230
            // 
            this.btn220230.Location = new System.Drawing.Point(806, 8);
            this.btn220230.Name = "btn220230";
            this.btn220230.Size = new System.Drawing.Size(75, 23);
            this.btn220230.TabIndex = 5;
            this.btn220230.Text = "220-230";
            this.btn220230.UseVisualStyleBackColor = true;
            this.btn220230.Click += new System.EventHandler(this.btn220230_Click);
            // 
            // btn200210
            // 
            this.btn200210.Location = new System.Drawing.Point(618, 6);
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
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(15, 6);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1231, 501);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
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
            // txtFFTLimit
            // 
            this.txtFFTLimit.Location = new System.Drawing.Point(765, 60);
            this.txtFFTLimit.Name = "txtFFTLimit";
            this.txtFFTLimit.Size = new System.Drawing.Size(21, 20);
            this.txtFFTLimit.TabIndex = 49;
            this.txtFFTLimit.Text = "20";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(714, 64);
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
            this.chkRemoveDC.Checked = true;
            this.chkRemoveDC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemoveDC.Location = new System.Drawing.Point(788, 23);
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
            // cbxFFTSingle
            // 
            this.cbxFFTSingle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFFTSingle.FormattingEnabled = true;
            this.cbxFFTSingle.Items.AddRange(new object[] {
            "None",
            "Hamming",
            "HammingPeriodic",
            "Hann",
            "HannPeriodic",
            "BlackmanHarris",
            "BlackmanNuttal",
            "FlatTop"});
            this.cbxFFTSingle.Location = new System.Drawing.Point(869, 42);
            this.cbxFFTSingle.Name = "cbxFFTSingle";
            this.cbxFFTSingle.Size = new System.Drawing.Size(121, 21);
            this.cbxFFTSingle.TabIndex = 60;
            this.cbxFFTSingle.SelectedIndexChanged += new System.EventHandler(this.cbxFFT_SelectedIndexChanged);
            // 
            // chkFFTSingle
            // 
            this.chkFFTSingle.AutoSize = true;
            this.chkFFTSingle.Checked = true;
            this.chkFFTSingle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFFTSingle.Location = new System.Drawing.Point(789, 45);
            this.chkFFTSingle.Name = "chkFFTSingle";
            this.chkFFTSingle.Size = new System.Drawing.Size(77, 17);
            this.chkFFTSingle.TabIndex = 59;
            this.chkFFTSingle.Text = "FFT Single";
            this.chkFFTSingle.UseVisualStyleBackColor = true;
            this.chkFFTSingle.CheckedChanged += new System.EventHandler(this.chkFFT_CheckedChanged);
            // 
            // chkDb
            // 
            this.chkDb.AutoSize = true;
            this.chkDb.Location = new System.Drawing.Point(870, 22);
            this.chkDb.Name = "chkDb";
            this.chkDb.Size = new System.Drawing.Size(39, 17);
            this.chkDb.TabIndex = 62;
            this.chkDb.Text = "dB";
            this.chkDb.UseVisualStyleBackColor = true;
            this.chkDb.CheckedChanged += new System.EventHandler(this.chkFFT_CheckedChanged);
            // 
            // chkPassband
            // 
            this.chkPassband.AutoSize = true;
            this.chkPassband.Location = new System.Drawing.Point(748, 3);
            this.chkPassband.Name = "chkPassband";
            this.chkPassband.Size = new System.Drawing.Size(95, 17);
            this.chkPassband.TabIndex = 63;
            this.chkPassband.Text = "Passband filter";
            this.chkPassband.UseVisualStyleBackColor = true;
            this.chkPassband.CheckedChanged += new System.EventHandler(this.chkLowPassFilter_CheckedChanged);
            // 
            // txtGain
            // 
            this.txtGain.Location = new System.Drawing.Point(939, 20);
            this.txtGain.Name = "txtGain";
            this.txtGain.Size = new System.Drawing.Size(50, 20);
            this.txtGain.TabIndex = 64;
            this.txtGain.Text = "10000";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(911, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 65;
            this.label6.Text = "Gain";
            // 
            // txtFilterOrder
            // 
            this.txtFilterOrder.Location = new System.Drawing.Point(900, 0);
            this.txtFilterOrder.Name = "txtFilterOrder";
            this.txtFilterOrder.Size = new System.Drawing.Size(30, 20);
            this.txtFilterOrder.TabIndex = 66;
            this.txtFilterOrder.Text = "1";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(842, 4);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(56, 13);
            this.label44.TabIndex = 67;
            this.label44.Text = "Filter order";
            // 
            // lblStatX
            // 
            this.lblStatX.AutoSize = true;
            this.lblStatX.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblStatX.ForeColor = System.Drawing.Color.Red;
            this.lblStatX.Location = new System.Drawing.Point(1024, 7);
            this.lblStatX.Name = "lblStatX";
            this.lblStatX.Size = new System.Drawing.Size(0, 15);
            this.lblStatX.TabIndex = 68;
            // 
            // lblStatY
            // 
            this.lblStatY.AutoSize = true;
            this.lblStatY.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatY.ForeColor = System.Drawing.Color.Red;
            this.lblStatY.Location = new System.Drawing.Point(1148, 7);
            this.lblStatY.Name = "lblStatY";
            this.lblStatY.Size = new System.Drawing.Size(0, 15);
            this.lblStatY.TabIndex = 69;
            // 
            // cbxFilterTypes
            // 
            this.cbxFilterTypes.FormattingEnabled = true;
            this.cbxFilterTypes.Items.AddRange(new object[] {
            "None",
            "BiQuad BP",
            "BiQuad peaking",
            "Moving average",
            "Moving average recursive",
            "Savitzky-Golay",
            "RASTA",
            "Butterworth",
            "Chebyshev-I",
            "Chebyshev-II",
            "Bessel",
            "Custom FIR",
            "BiQuad LP"});
            this.cbxFilterTypes.Location = new System.Drawing.Point(939, 0);
            this.cbxFilterTypes.Margin = new System.Windows.Forms.Padding(2);
            this.cbxFilterTypes.Name = "cbxFilterTypes";
            this.cbxFilterTypes.Size = new System.Drawing.Size(144, 21);
            this.cbxFilterTypes.TabIndex = 70;
            this.cbxFilterTypes.Text = "None";
            // 
            // cbxSmoothing
            // 
            this.cbxSmoothing.FormattingEnabled = true;
            this.cbxSmoothing.Items.AddRange(new object[] {
            "None",
            "Moving average",
            "Moving average recursive",
            "Savitzky-Golay",
            "IQ"});
            this.cbxSmoothing.Location = new System.Drawing.Point(1088, 0);
            this.cbxSmoothing.Name = "cbxSmoothing";
            this.cbxSmoothing.Size = new System.Drawing.Size(121, 21);
            this.cbxSmoothing.TabIndex = 71;
            this.cbxSmoothing.Text = "None";
            // 
            // chkClockwise
            // 
            this.chkClockwise.AutoSize = true;
            this.chkClockwise.Location = new System.Drawing.Point(995, 23);
            this.chkClockwise.Name = "chkClockwise";
            this.chkClockwise.Size = new System.Drawing.Size(117, 17);
            this.chkClockwise.TabIndex = 72;
            this.chkClockwise.Text = "Clockwise Rotating";
            this.chkClockwise.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1287, 639);
            this.Controls.Add(this.chkClockwise);
            this.Controls.Add(this.cbxSmoothing);
            this.Controls.Add(this.cbxFilterTypes);
            this.Controls.Add(this.lblStatY);
            this.Controls.Add(this.lblStatX);
            this.Controls.Add(this.label44);
            this.Controls.Add(this.txtFilterOrder);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtGain);
            this.Controls.Add(this.chkPassband);
            this.Controls.Add(this.chkDb);
            this.Controls.Add(this.cbxFFTSingle);
            this.Controls.Add(this.chkFFTSingle);
            this.Controls.Add(this.cbxSensor);
            this.Controls.Add(this.chkRemoveDC);
            this.Controls.Add(this.chkShowResultante);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtFFTLimit);
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
            this.tabPage8.ResumeLayout(false);
            this.tabPage8.PerformLayout();
            this.tabPage15.ResumeLayout(false);
            this.tabPage15.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridX)).EndInit();
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private FormsPlot formsPlotX;
        private FormsPlot formsPlotY;
        private FormsPlot formsPlotZ;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
        private FormsPlot formsPlotAnalysis;
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
        private FormsPlot formsPlotGlobal;
        private System.Windows.Forms.ListBox lstPeakGlobalX;
        private System.Windows.Forms.ListBox lstPeakGlobalZ;
        private System.Windows.Forms.ListBox lstPeakGlobalY;
        private System.Windows.Forms.CheckBox chkOrderTracking;
        private System.Windows.Forms.TextBox txtFFTLimit;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage4;
        private FormsPlot formsPlotGyro;
        private System.Windows.Forms.CheckBox chkShowResultante;
        private System.Windows.Forms.ListBox lstPeakGyroZ;
        private System.Windows.Forms.ListBox lstPeakGyroY;
        private System.Windows.Forms.ListBox lstPeakGyroX;
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
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.Button btnUnselectAll;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnUpdateAnalysisSection;
        private System.Windows.Forms.CheckedListBox lstSectionSelector;
        private System.Windows.Forms.Button btn240250;
        private System.Windows.Forms.Button btn250300;
        private System.Windows.Forms.Button btn230240;
        private System.Windows.Forms.Button btn210220;
        private System.Windows.Forms.Button btn220230;
        private System.Windows.Forms.Button btn200210;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.Label label24;
        private FormsPlot formsPlotT1O;
        private System.Windows.Forms.Label label25;
        private FormsPlot formsPlotT1I;
        private System.Windows.Forms.Label label23;
        private FormsPlot formsPlotT1Y;
        private System.Windows.Forms.Label label18;
        private FormsPlot formsPlotT1X;
        private System.Windows.Forms.CheckBox chkRemoveDC;
        private System.Windows.Forms.Button btnExportWAV;
        private System.Windows.Forms.TabPage tabPage14;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox cbxSensor;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ComboBox cbxFFTSingle;
        private System.Windows.Forms.CheckBox chkFFTSingle;
        private System.Windows.Forms.CheckBox chkDb;
        private System.Windows.Forms.CheckBox chkPassband;
        private System.Windows.Forms.TextBox txtGain;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtFilterOrder;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.TabPage tabPage15;
        private System.Windows.Forms.DataGridView dataGridY;
        private System.Windows.Forms.DataGridView dataGridX;
        private System.Windows.Forms.Button btnClearAnalysisHistory;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.TextBox txtYMagBalanced;
        private System.Windows.Forms.TextBox txtYMag1;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.TextBox txtXMagBalanced;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.TextBox txtMagGrams1;
        private System.Windows.Forms.TextBox txtXMagG1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSaveData;
        private System.Windows.Forms.Label lblStatX;
        private System.Windows.Forms.Label lblStatY;
        private System.Windows.Forms.TextBox txtCorrectAngleY;
        private System.Windows.Forms.TextBox txtCorrectAngleX;
        private System.Windows.Forms.CheckBox chkUseYGyro;
        private System.Windows.Forms.CheckBox chkUseXGyro;
        private System.Windows.Forms.CheckBox chkScaleGyro;
        private System.Windows.Forms.Button btnFindAngles;
        private System.Windows.Forms.ComboBox cbxFilterTypes;
        private System.Windows.Forms.ComboBox cbxSmoothing;
        private System.Windows.Forms.TextBox txtCorrectYTemporal;
        private System.Windows.Forms.TextBox txtCorrectXTemporal;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Button btnCalculateCorrection;
        private System.Windows.Forms.TextBox txtAngleXCalc;
        private System.Windows.Forms.TextBox txtAngleYCalc;
        private System.Windows.Forms.Label lblAngleYStat;
        private System.Windows.Forms.Label lblAngleXStat;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox txtMagnitudeY;
        private System.Windows.Forms.TextBox txtMagnitudeX;
        private System.Windows.Forms.Label lblAngleYCorrect;
        private System.Windows.Forms.Label lblAngleXCorrect;
        private System.Windows.Forms.ComboBox cbxMagData;
        private System.Windows.Forms.ComboBox cbxAngleData;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.CheckedListBox lstSelectToursAnalysis;
        private System.Windows.Forms.CheckBox chkClockwise;
    }
}

