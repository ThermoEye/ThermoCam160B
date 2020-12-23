namespace ThermoCam160B
{
    partial class Main
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
            this.imageBox_ThermalView = new Emgu.CV.UI.ImageBox();
            this.button_Connect = new System.Windows.Forms.Button();
            this.label_MaxTemp = new System.Windows.Forms.Label();
            this.label_AvgTemp = new System.Windows.Forms.Label();
            this.label_MinTemp = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label_MainAppVersionName = new System.Windows.Forms.Label();
            this.label_BootloaderVersionName = new System.Windows.Forms.Label();
            this.label_SerialNumberName = new System.Windows.Forms.Label();
            this.label_SerialNumber = new System.Windows.Forms.Label();
            this.label_MainAppVersion = new System.Windows.Forms.Label();
            this.label_BootloaderVersion = new System.Windows.Forms.Label();
            this.button_GetCameraInfo = new System.Windows.Forms.Button();
            this.comboBox_camera_list = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl_Control = new System.Windows.Forms.TabControl();
            this.tabPage_Info = new System.Windows.Forms.TabPage();
            this.tabPage_Buzzer = new System.Windows.Forms.TabPage();
            this.button_buzzer_set = new System.Windows.Forms.Button();
            this.comboBox_buz_note = new System.Windows.Forms.ComboBox();
            this.numericUpDown_buz_duration = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_buz_octave = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage_Temperature = new System.Windows.Forms.TabPage();
            this.button_param_default = new System.Windows.Forms.Button();
            this.button_param_set = new System.Windows.Forms.Button();
            this.button_param_get = new System.Windows.Forms.Button();
            this.numericUpDown5 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox_status = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label_status_stablization = new System.Windows.Forms.Label();
            this.label_status_connection = new System.Windows.Forms.Label();
            this.comboBox_ColorMap = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_ThermalView)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabControl_Control.SuspendLayout();
            this.tabPage_Info.SuspendLayout();
            this.tabPage_Buzzer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_buz_duration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_buz_octave)).BeginInit();
            this.tabPage_Temperature.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            this.groupBox_status.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageBox_ThermalView
            // 
            this.imageBox_ThermalView.BackColor = System.Drawing.Color.Black;
            this.imageBox_ThermalView.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.imageBox_ThermalView.Location = new System.Drawing.Point(16, 16);
            this.imageBox_ThermalView.Name = "imageBox_ThermalView";
            this.imageBox_ThermalView.Size = new System.Drawing.Size(320, 240);
            this.imageBox_ThermalView.TabIndex = 2;
            this.imageBox_ThermalView.TabStop = false;
            // 
            // button_Connect
            // 
            this.button_Connect.Location = new System.Drawing.Point(212, 276);
            this.button_Connect.Name = "button_Connect";
            this.button_Connect.Size = new System.Drawing.Size(124, 25);
            this.button_Connect.TabIndex = 3;
            this.button_Connect.Text = "Connect";
            this.button_Connect.UseVisualStyleBackColor = true;
            this.button_Connect.Click += new System.EventHandler(this.button_Connect_Click);
            // 
            // label_MaxTemp
            // 
            this.label_MaxTemp.AutoSize = true;
            this.label_MaxTemp.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_MaxTemp.ForeColor = System.Drawing.Color.Crimson;
            this.label_MaxTemp.Location = new System.Drawing.Point(342, 16);
            this.label_MaxTemp.Name = "label_MaxTemp";
            this.label_MaxTemp.Size = new System.Drawing.Size(30, 15);
            this.label_MaxTemp.TabIndex = 4;
            this.label_MaxTemp.Text = "Max";
            // 
            // label_AvgTemp
            // 
            this.label_AvgTemp.AutoSize = true;
            this.label_AvgTemp.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_AvgTemp.ForeColor = System.Drawing.Color.LimeGreen;
            this.label_AvgTemp.Location = new System.Drawing.Point(342, 125);
            this.label_AvgTemp.Name = "label_AvgTemp";
            this.label_AvgTemp.Size = new System.Drawing.Size(27, 15);
            this.label_AvgTemp.TabIndex = 5;
            this.label_AvgTemp.Text = "Avg";
            // 
            // label_MinTemp
            // 
            this.label_MinTemp.AutoSize = true;
            this.label_MinTemp.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_MinTemp.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label_MinTemp.Location = new System.Drawing.Point(342, 241);
            this.label_MinTemp.Name = "label_MinTemp";
            this.label_MinTemp.Size = new System.Drawing.Size(28, 15);
            this.label_MinTemp.TabIndex = 6;
            this.label_MinTemp.Text = "Min";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.5F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.5F));
            this.tableLayoutPanel2.Controls.Add(this.label_MainAppVersionName, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label_BootloaderVersionName, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label_SerialNumberName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label_SerialNumber, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label_MainAppVersion, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label_BootloaderVersion, 1, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(38, 45);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(214, 64);
            this.tableLayoutPanel2.TabIndex = 8;
            // 
            // label_MainAppVersionName
            // 
            this.label_MainAppVersionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_MainAppVersionName.AutoSize = true;
            this.label_MainAppVersionName.Location = new System.Drawing.Point(11, 40);
            this.label_MainAppVersionName.Name = "label_MainAppVersionName";
            this.label_MainAppVersionName.Size = new System.Drawing.Size(66, 24);
            this.label_MainAppVersionName.TabIndex = 9;
            this.label_MainAppVersionName.Text = "Main App :";
            // 
            // label_BootloaderVersionName
            // 
            this.label_BootloaderVersionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_BootloaderVersionName.AutoSize = true;
            this.label_BootloaderVersionName.Location = new System.Drawing.Point(3, 20);
            this.label_BootloaderVersionName.Name = "label_BootloaderVersionName";
            this.label_BootloaderVersionName.Size = new System.Drawing.Size(74, 20);
            this.label_BootloaderVersionName.TabIndex = 8;
            this.label_BootloaderVersionName.Text = "Bootloader :";
            // 
            // label_SerialNumberName
            // 
            this.label_SerialNumberName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_SerialNumberName.AutoSize = true;
            this.label_SerialNumberName.Location = new System.Drawing.Point(23, 0);
            this.label_SerialNumberName.Name = "label_SerialNumberName";
            this.label_SerialNumberName.Size = new System.Drawing.Size(54, 20);
            this.label_SerialNumberName.TabIndex = 7;
            this.label_SerialNumberName.Text = "Serial # :";
            // 
            // label_SerialNumber
            // 
            this.label_SerialNumber.AutoSize = true;
            this.label_SerialNumber.Location = new System.Drawing.Point(83, 0);
            this.label_SerialNumber.Name = "label_SerialNumber";
            this.label_SerialNumber.Size = new System.Drawing.Size(0, 15);
            this.label_SerialNumber.TabIndex = 4;
            // 
            // label_MainAppVersion
            // 
            this.label_MainAppVersion.AutoSize = true;
            this.label_MainAppVersion.Location = new System.Drawing.Point(83, 40);
            this.label_MainAppVersion.Name = "label_MainAppVersion";
            this.label_MainAppVersion.Size = new System.Drawing.Size(0, 15);
            this.label_MainAppVersion.TabIndex = 6;
            // 
            // label_BootloaderVersion
            // 
            this.label_BootloaderVersion.AutoSize = true;
            this.label_BootloaderVersion.Location = new System.Drawing.Point(83, 20);
            this.label_BootloaderVersion.Name = "label_BootloaderVersion";
            this.label_BootloaderVersion.Size = new System.Drawing.Size(0, 15);
            this.label_BootloaderVersion.TabIndex = 5;
            // 
            // button_GetCameraInfo
            // 
            this.button_GetCameraInfo.Location = new System.Drawing.Point(284, 43);
            this.button_GetCameraInfo.Name = "button_GetCameraInfo";
            this.button_GetCameraInfo.Size = new System.Drawing.Size(72, 66);
            this.button_GetCameraInfo.TabIndex = 9;
            this.button_GetCameraInfo.Text = "Get";
            this.button_GetCameraInfo.UseVisualStyleBackColor = true;
            this.button_GetCameraInfo.Click += new System.EventHandler(this.button_GetCameraInfo_Click);
            // 
            // comboBox_camera_list
            // 
            this.comboBox_camera_list.DisplayMember = "Text";
            this.comboBox_camera_list.FormattingEnabled = true;
            this.comboBox_camera_list.Location = new System.Drawing.Point(73, 277);
            this.comboBox_camera_list.Name = "comboBox_camera_list";
            this.comboBox_camera_list.Size = new System.Drawing.Size(133, 23);
            this.comboBox_camera_list.TabIndex = 10;
            this.comboBox_camera_list.ValueMember = "Value";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 280);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Camera";
            // 
            // tabControl_Control
            // 
            this.tabControl_Control.Controls.Add(this.tabPage_Info);
            this.tabControl_Control.Controls.Add(this.tabPage_Buzzer);
            this.tabControl_Control.Controls.Add(this.tabPage_Temperature);
            this.tabControl_Control.Enabled = false;
            this.tabControl_Control.Location = new System.Drawing.Point(394, 7);
            this.tabControl_Control.Name = "tabControl_Control";
            this.tabControl_Control.SelectedIndex = 0;
            this.tabControl_Control.Size = new System.Drawing.Size(415, 422);
            this.tabControl_Control.TabIndex = 15;
            // 
            // tabPage_Info
            // 
            this.tabPage_Info.Controls.Add(this.button_GetCameraInfo);
            this.tabPage_Info.Controls.Add(this.tableLayoutPanel2);
            this.tabPage_Info.Controls.Add(this.label8);
            this.tabPage_Info.Controls.Add(this.comboBox_ColorMap);
            this.tabPage_Info.Location = new System.Drawing.Point(4, 24);
            this.tabPage_Info.Name = "tabPage_Info";
            this.tabPage_Info.Size = new System.Drawing.Size(407, 394);
            this.tabPage_Info.TabIndex = 2;
            this.tabPage_Info.Text = "Information";
            this.tabPage_Info.UseVisualStyleBackColor = true;
            // 
            // tabPage_Buzzer
            // 
            this.tabPage_Buzzer.Controls.Add(this.button_buzzer_set);
            this.tabPage_Buzzer.Controls.Add(this.comboBox_buz_note);
            this.tabPage_Buzzer.Controls.Add(this.numericUpDown_buz_duration);
            this.tabPage_Buzzer.Controls.Add(this.numericUpDown_buz_octave);
            this.tabPage_Buzzer.Controls.Add(this.label4);
            this.tabPage_Buzzer.Controls.Add(this.label3);
            this.tabPage_Buzzer.Controls.Add(this.label2);
            this.tabPage_Buzzer.Location = new System.Drawing.Point(4, 24);
            this.tabPage_Buzzer.Name = "tabPage_Buzzer";
            this.tabPage_Buzzer.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Buzzer.Size = new System.Drawing.Size(407, 394);
            this.tabPage_Buzzer.TabIndex = 0;
            this.tabPage_Buzzer.Text = "Buzzer";
            this.tabPage_Buzzer.UseVisualStyleBackColor = true;
            // 
            // button_buzzer_set
            // 
            this.button_buzzer_set.Location = new System.Drawing.Point(223, 36);
            this.button_buzzer_set.Name = "button_buzzer_set";
            this.button_buzzer_set.Size = new System.Drawing.Size(84, 94);
            this.button_buzzer_set.TabIndex = 3;
            this.button_buzzer_set.Text = "Set";
            this.button_buzzer_set.UseVisualStyleBackColor = true;
            this.button_buzzer_set.Click += new System.EventHandler(this.button_buzzer_set_Click);
            // 
            // comboBox_buz_note
            // 
            this.comboBox_buz_note.FormattingEnabled = true;
            this.comboBox_buz_note.Items.AddRange(new object[] {
            "C",
            "D",
            "E",
            "F",
            "G",
            "A",
            "B"});
            this.comboBox_buz_note.Location = new System.Drawing.Point(106, 73);
            this.comboBox_buz_note.Name = "comboBox_buz_note";
            this.comboBox_buz_note.Size = new System.Drawing.Size(63, 23);
            this.comboBox_buz_note.TabIndex = 2;
            this.comboBox_buz_note.Text = "A";
            // 
            // numericUpDown_buz_duration
            // 
            this.numericUpDown_buz_duration.Location = new System.Drawing.Point(106, 107);
            this.numericUpDown_buz_duration.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown_buz_duration.Name = "numericUpDown_buz_duration";
            this.numericUpDown_buz_duration.Size = new System.Drawing.Size(63, 23);
            this.numericUpDown_buz_duration.TabIndex = 1;
            this.numericUpDown_buz_duration.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // numericUpDown_buz_octave
            // 
            this.numericUpDown_buz_octave.Location = new System.Drawing.Point(106, 38);
            this.numericUpDown_buz_octave.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numericUpDown_buz_octave.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_buz_octave.Name = "numericUpDown_buz_octave";
            this.numericUpDown_buz_octave.Size = new System.Drawing.Size(63, 23);
            this.numericUpDown_buz_octave.TabIndex = 1;
            this.numericUpDown_buz_octave.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "Duration";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Note";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Octave";
            // 
            // tabPage_Temperature
            // 
            this.tabPage_Temperature.Controls.Add(this.button_param_default);
            this.tabPage_Temperature.Controls.Add(this.button_param_set);
            this.tabPage_Temperature.Controls.Add(this.button_param_get);
            this.tabPage_Temperature.Controls.Add(this.numericUpDown5);
            this.tabPage_Temperature.Controls.Add(this.numericUpDown4);
            this.tabPage_Temperature.Controls.Add(this.numericUpDown3);
            this.tabPage_Temperature.Controls.Add(this.label7);
            this.tabPage_Temperature.Controls.Add(this.label6);
            this.tabPage_Temperature.Controls.Add(this.label5);
            this.tabPage_Temperature.Location = new System.Drawing.Point(4, 24);
            this.tabPage_Temperature.Name = "tabPage_Temperature";
            this.tabPage_Temperature.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Temperature.Size = new System.Drawing.Size(407, 394);
            this.tabPage_Temperature.TabIndex = 1;
            this.tabPage_Temperature.Text = "Temperature";
            this.tabPage_Temperature.UseVisualStyleBackColor = true;
            // 
            // button_param_default
            // 
            this.button_param_default.Location = new System.Drawing.Point(233, 98);
            this.button_param_default.Name = "button_param_default";
            this.button_param_default.Size = new System.Drawing.Size(105, 23);
            this.button_param_default.TabIndex = 2;
            this.button_param_default.Text = "Default";
            this.button_param_default.UseVisualStyleBackColor = true;
            // 
            // button_param_set
            // 
            this.button_param_set.Location = new System.Drawing.Point(233, 68);
            this.button_param_set.Name = "button_param_set";
            this.button_param_set.Size = new System.Drawing.Size(105, 23);
            this.button_param_set.TabIndex = 2;
            this.button_param_set.Text = "Set";
            this.button_param_set.UseVisualStyleBackColor = true;
            // 
            // button_param_get
            // 
            this.button_param_get.Location = new System.Drawing.Point(233, 38);
            this.button_param_get.Name = "button_param_get";
            this.button_param_get.Size = new System.Drawing.Size(105, 23);
            this.button_param_get.TabIndex = 2;
            this.button_param_get.Text = "Get";
            this.button_param_get.UseVisualStyleBackColor = true;
            // 
            // numericUpDown5
            // 
            this.numericUpDown5.Location = new System.Drawing.Point(112, 98);
            this.numericUpDown5.Name = "numericUpDown5";
            this.numericUpDown5.Size = new System.Drawing.Size(90, 23);
            this.numericUpDown5.TabIndex = 1;
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.Location = new System.Drawing.Point(112, 69);
            this.numericUpDown4.Name = "numericUpDown4";
            this.numericUpDown4.Size = new System.Drawing.Size(90, 23);
            this.numericUpDown4.TabIndex = 1;
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Location = new System.Drawing.Point(112, 39);
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(90, 23);
            this.numericUpDown3.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(49, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "label5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(49, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "label5";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(49, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "label5";
            // 
            // groupBox_status
            // 
            this.groupBox_status.Controls.Add(this.label11);
            this.groupBox_status.Controls.Add(this.label10);
            this.groupBox_status.Controls.Add(this.label_status_stablization);
            this.groupBox_status.Controls.Add(this.label_status_connection);
            this.groupBox_status.Enabled = false;
            this.groupBox_status.Location = new System.Drawing.Point(21, 322);
            this.groupBox_status.Name = "groupBox_status";
            this.groupBox_status.Size = new System.Drawing.Size(315, 100);
            this.groupBox_status.TabIndex = 16;
            this.groupBox_status.TabStop = false;
            this.groupBox_status.Text = "Device Status";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(52, 58);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 15);
            this.label11.TabIndex = 1;
            this.label11.Text = "Sensor Stablized";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(52, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(104, 15);
            this.label10.TabIndex = 1;
            this.label10.Text = "Sensor Connected";
            // 
            // label_status_stablization
            // 
            this.label_status_stablization.AutoSize = true;
            this.label_status_stablization.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_status_stablization.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_status_stablization.ForeColor = System.Drawing.Color.Gray;
            this.label_status_stablization.Location = new System.Drawing.Point(19, 45);
            this.label_status_stablization.Name = "label_status_stablization";
            this.label_status_stablization.Size = new System.Drawing.Size(33, 36);
            this.label_status_stablization.TabIndex = 0;
            this.label_status_stablization.Text = "●";
            // 
            // label_status_connection
            // 
            this.label_status_connection.AutoSize = true;
            this.label_status_connection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_status_connection.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_status_connection.ForeColor = System.Drawing.Color.Gray;
            this.label_status_connection.Location = new System.Drawing.Point(19, 15);
            this.label_status_connection.Name = "label_status_connection";
            this.label_status_connection.Size = new System.Drawing.Size(33, 36);
            this.label_status_connection.TabIndex = 0;
            this.label_status_connection.Text = "●";
            // 
            // comboBox_ColorMap
            // 
            this.comboBox_ColorMap.DisplayMember = "Text";
            this.comboBox_ColorMap.FormattingEnabled = true;
            this.comboBox_ColorMap.Items.AddRange(new object[] {
            "None",
            "WhiteHot",
            "BlackHot",
            "ColdHot",
            "HotSpot",
            "ColdSpot",
            "Rainbow",
            "Ironbow",
            "Cool",
            "Hot",
            "Hue",
            "Jet"});
            this.comboBox_ColorMap.Location = new System.Drawing.Point(119, 148);
            this.comboBox_ColorMap.Name = "comboBox_ColorMap";
            this.comboBox_ColorMap.Size = new System.Drawing.Size(133, 23);
            this.comboBox_ColorMap.TabIndex = 10;
            this.comboBox_ColorMap.Text = "None";
            this.comboBox_ColorMap.ValueMember = "Value";
            this.comboBox_ColorMap.SelectedIndexChanged += new System.EventHandler(this.comboBox_ColorMap_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(43, 152);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 15);
            this.label8.TabIndex = 7;
            this.label8.Text = "Color Map :";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 441);
            this.Controls.Add(this.groupBox_status);
            this.Controls.Add(this.tabControl_Control);
            this.Controls.Add(this.comboBox_camera_list);
            this.Controls.Add(this.label_MinTemp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_MaxTemp);
            this.Controls.Add(this.label_AvgTemp);
            this.Controls.Add(this.button_Connect);
            this.Controls.Add(this.imageBox_ThermalView);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThermoCam160B C#";
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_ThermalView)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tabControl_Control.ResumeLayout(false);
            this.tabPage_Info.ResumeLayout(false);
            this.tabPage_Info.PerformLayout();
            this.tabPage_Buzzer.ResumeLayout(false);
            this.tabPage_Buzzer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_buz_duration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_buz_octave)).EndInit();
            this.tabPage_Temperature.ResumeLayout(false);
            this.tabPage_Temperature.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            this.groupBox_status.ResumeLayout(false);
            this.groupBox_status.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox imageBox_ThermalView;
        private System.Windows.Forms.Button button_Connect;
        private System.Windows.Forms.Label label_MaxTemp;
        private System.Windows.Forms.Label label_AvgTemp;
        private System.Windows.Forms.Label label_MinTemp;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label_MainAppVersionName;
        private System.Windows.Forms.Label label_BootloaderVersionName;
        private System.Windows.Forms.Label label_SerialNumberName;
        private System.Windows.Forms.Label label_SerialNumber;
        private System.Windows.Forms.Label label_MainAppVersion;
        private System.Windows.Forms.Label label_BootloaderVersion;
        private System.Windows.Forms.Button button_GetCameraInfo;
        private System.Windows.Forms.ComboBox comboBox_camera_list;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl_Control;
        private System.Windows.Forms.TabPage tabPage_Buzzer;
        private System.Windows.Forms.TabPage tabPage_Temperature;
        private System.Windows.Forms.Button button_buzzer_set;
        private System.Windows.Forms.ComboBox comboBox_buz_note;
        private System.Windows.Forms.NumericUpDown numericUpDown_buz_duration;
        private System.Windows.Forms.NumericUpDown numericUpDown_buz_octave;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_param_default;
        private System.Windows.Forms.Button button_param_set;
        private System.Windows.Forms.Button button_param_get;
        private System.Windows.Forms.NumericUpDown numericUpDown5;
        private System.Windows.Forms.NumericUpDown numericUpDown4;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage_Info;
        private System.Windows.Forms.GroupBox groupBox_status;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label_status_stablization;
        private System.Windows.Forms.Label label_status_connection;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBox_ColorMap;
    }
}

