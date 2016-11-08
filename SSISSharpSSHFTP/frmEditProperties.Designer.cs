namespace SSISSFTPTask110
{
    partial class frmEditProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditProperties));
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkSleep = new System.Windows.Forms.CheckBox();
            this.chkOverwrite = new System.Windows.Forms.CheckBox();
            this.chkRecursive = new System.Windows.Forms.CheckBox();
            this.linkLabelCodeplex = new System.Windows.Forms.LinkLabel();
            this.groupBoxConnection = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbPort = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.optionEncryptionKey = new System.Windows.Forms.RadioButton();
            this.optionEncryptionPassword = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbPassword = new System.Windows.Forms.ComboBox();
            this.cmbUser = new System.Windows.Forms.ComboBox();
            this.cmbServer = new System.Windows.Forms.ComboBox();
            this.lbPassword = new System.Windows.Forms.Label();
            this.lbUser = new System.Windows.Forms.Label();
            this.lbServer = new System.Windows.Forms.Label();
            this.groupBoxEncryption = new System.Windows.Forms.GroupBox();
            this.cmbPassPhrase = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.optPublicKeyFileVariable = new System.Windows.Forms.RadioButton();
            this.optPublicKeyFileConnection = new System.Windows.Forms.RadioButton();
            this.cmbKeyFile = new System.Windows.Forms.ComboBox();
            this.btKeyFileExpression = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.groupbox = new System.Windows.Forms.GroupBox();
            this.lbDepth = new System.Windows.Forms.Label();
            this.txDepth = new System.Windows.Forms.NumericUpDown();
            this.chkDeleteFileOnTransferCompleted = new System.Windows.Forms.CheckBox();
            this.cmbRemote = new System.Windows.Forms.ComboBox();
            this.optFileVariable = new System.Windows.Forms.RadioButton();
            this.optFileConnection = new System.Windows.Forms.RadioButton();
            this.cmbLocal = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFilesList = new System.Windows.Forms.ComboBox();
            this.btDestinationFile = new System.Windows.Forms.Button();
            this.lbDestinationFile = new System.Windows.Forms.Label();
            this.btSourceFile = new System.Windows.Forms.Button();
            this.lbSourceFile = new System.Windows.Forms.Label();
            this.lbAction = new System.Windows.Forms.Label();
            this.cmbAction = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.domainUpDownIndex = new System.Windows.Forms.NumericUpDown();
            this.optFullPath = new System.Windows.Forms.RadioButton();
            this.optNameOnly = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbRecordset = new System.Windows.Forms.ComboBox();
            this.chkResultSetEnabled = new System.Windows.Forms.CheckBox();
            this.groupBoxConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
            this.groupBoxEncryption.SuspendLayout();
            this.groupbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txDepth)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.domainUpDownIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(756, 373);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(82, 26);
            this.btOK.TabIndex = 3;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(668, 373);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(82, 26);
            this.btCancel.TabIndex = 4;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // chkSleep
            // 
            this.chkSleep.AutoSize = true;
            this.chkSleep.Location = new System.Drawing.Point(92, 127);
            this.chkSleep.Name = "chkSleep";
            this.chkSleep.Size = new System.Drawing.Size(123, 17);
            this.chkSleep.TabIndex = 88;
            this.chkSleep.Text = "Sleep on disconnect";
            this.chkSleep.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.chkSleep, "To avoid overload connections. Give it the time to disconnect completely.");
            this.chkSleep.UseVisualStyleBackColor = true;
            this.chkSleep.Click += new System.EventHandler(this.chkSleep_Click);
            // 
            // chkOverwrite
            // 
            this.chkOverwrite.AutoSize = true;
            this.chkOverwrite.Location = new System.Drawing.Point(289, 80);
            this.chkOverwrite.Name = "chkOverwrite";
            this.chkOverwrite.Size = new System.Drawing.Size(112, 17);
            this.chkOverwrite.TabIndex = 74;
            this.chkOverwrite.Text = "Overwrite local file";
            this.toolTip1.SetToolTip(this.chkOverwrite, "Available only for Remote to Local file copy");
            this.chkOverwrite.UseVisualStyleBackColor = true;
            // 
            // chkRecursive
            // 
            this.chkRecursive.AutoSize = true;
            this.chkRecursive.Location = new System.Drawing.Point(92, 178);
            this.chkRecursive.Name = "chkRecursive";
            this.chkRecursive.Size = new System.Drawing.Size(116, 17);
            this.chkRecursive.TabIndex = 76;
            this.chkRecursive.Text = "Recursive file copy";
            this.toolTip1.SetToolTip(this.chkRecursive, "Explore subfolders");
            this.chkRecursive.UseVisualStyleBackColor = true;
            this.chkRecursive.Click += new System.EventHandler(this.chkRecursive_Click);
            // 
            // linkLabelCodeplex
            // 
            this.linkLabelCodeplex.AutoSize = true;
            this.linkLabelCodeplex.Location = new System.Drawing.Point(7, 380);
            this.linkLabelCodeplex.Name = "linkLabelCodeplex";
            this.linkLabelCodeplex.Size = new System.Drawing.Size(141, 13);
            this.linkLabelCodeplex.TabIndex = 10;
            this.linkLabelCodeplex.TabStop = true;
            this.linkLabelCodeplex.Text = "http://ssissftp.codeplex.com";
            this.linkLabelCodeplex.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCodeplex_LinkClicked);
            // 
            // groupBoxConnection
            // 
            this.groupBoxConnection.Controls.Add(this.label3);
            this.groupBoxConnection.Controls.Add(this.numericUpDown);
            this.groupBoxConnection.Controls.Add(this.label2);
            this.groupBoxConnection.Controls.Add(this.chkSleep);
            this.groupBoxConnection.Controls.Add(this.cmbPort);
            this.groupBoxConnection.Controls.Add(this.label7);
            this.groupBoxConnection.Controls.Add(this.optionEncryptionKey);
            this.groupBoxConnection.Controls.Add(this.optionEncryptionPassword);
            this.groupBoxConnection.Controls.Add(this.label5);
            this.groupBoxConnection.Controls.Add(this.cmbPassword);
            this.groupBoxConnection.Controls.Add(this.cmbUser);
            this.groupBoxConnection.Controls.Add(this.cmbServer);
            this.groupBoxConnection.Controls.Add(this.lbPassword);
            this.groupBoxConnection.Controls.Add(this.lbUser);
            this.groupBoxConnection.Controls.Add(this.lbServer);
            this.groupBoxConnection.Location = new System.Drawing.Point(10, 3);
            this.groupBoxConnection.Name = "groupBoxConnection";
            this.groupBoxConnection.Size = new System.Drawing.Size(411, 157);
            this.groupBoxConnection.TabIndex = 72;
            this.groupBoxConnection.TabStop = false;
            this.groupBoxConnection.Text = "Connection";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(339, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 91;
            this.label3.Text = "seconds";
            // 
            // numericUpDown
            // 
            this.numericUpDown.Enabled = false;
            this.numericUpDown.Location = new System.Drawing.Point(285, 125);
            this.numericUpDown.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown.Name = "numericUpDown";
            this.numericUpDown.Size = new System.Drawing.Size(48, 20);
            this.numericUpDown.TabIndex = 90;
            this.numericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(230, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 89;
            this.label2.Text = "Sleep for";
            // 
            // cmbPort
            // 
            this.cmbPort.FormattingEnabled = true;
            this.cmbPort.Location = new System.Drawing.Point(253, 44);
            this.cmbPort.Name = "cmbPort";
            this.cmbPort.Size = new System.Drawing.Size(148, 21);
            this.cmbPort.TabIndex = 87;
            this.cmbPort.Text = "22";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(218, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 86;
            this.label7.Text = "Port:";
            // 
            // optionEncryptionKey
            // 
            this.optionEncryptionKey.AutoSize = true;
            this.optionEncryptionKey.Location = new System.Drawing.Point(169, 46);
            this.optionEncryptionKey.Name = "optionEncryptionKey";
            this.optionEncryptionKey.Size = new System.Drawing.Size(43, 17);
            this.optionEncryptionKey.TabIndex = 85;
            this.optionEncryptionKey.Text = "Key";
            this.optionEncryptionKey.UseVisualStyleBackColor = true;
            this.optionEncryptionKey.Click += new System.EventHandler(this.optionEncryptionKey_Click);
            // 
            // optionEncryptionPassword
            // 
            this.optionEncryptionPassword.AutoSize = true;
            this.optionEncryptionPassword.Checked = true;
            this.optionEncryptionPassword.Location = new System.Drawing.Point(92, 46);
            this.optionEncryptionPassword.Name = "optionEncryptionPassword";
            this.optionEncryptionPassword.Size = new System.Drawing.Size(71, 17);
            this.optionEncryptionPassword.TabIndex = 84;
            this.optionEncryptionPassword.TabStop = true;
            this.optionEncryptionPassword.Text = "Password";
            this.optionEncryptionPassword.UseVisualStyleBackColor = true;
            this.optionEncryptionPassword.Click += new System.EventHandler(this.optionEncryptionPassword_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 83;
            this.label5.Text = "Encryption type:";
            // 
            // cmbPassword
            // 
            this.cmbPassword.FormattingEnabled = true;
            this.cmbPassword.Location = new System.Drawing.Point(92, 96);
            this.cmbPassword.Name = "cmbPassword";
            this.cmbPassword.Size = new System.Drawing.Size(309, 21);
            this.cmbPassword.TabIndex = 77;
            // 
            // cmbUser
            // 
            this.cmbUser.FormattingEnabled = true;
            this.cmbUser.Location = new System.Drawing.Point(92, 69);
            this.cmbUser.Name = "cmbUser";
            this.cmbUser.Size = new System.Drawing.Size(309, 21);
            this.cmbUser.TabIndex = 76;
            // 
            // cmbServer
            // 
            this.cmbServer.FormattingEnabled = true;
            this.cmbServer.Location = new System.Drawing.Point(92, 19);
            this.cmbServer.Name = "cmbServer";
            this.cmbServer.Size = new System.Drawing.Size(309, 21);
            this.cmbServer.TabIndex = 75;
            // 
            // lbPassword
            // 
            this.lbPassword.AutoSize = true;
            this.lbPassword.Location = new System.Drawing.Point(6, 100);
            this.lbPassword.Name = "lbPassword";
            this.lbPassword.Size = new System.Drawing.Size(56, 13);
            this.lbPassword.TabIndex = 74;
            this.lbPassword.Text = "Password:";
            // 
            // lbUser
            // 
            this.lbUser.AutoSize = true;
            this.lbUser.Location = new System.Drawing.Point(6, 74);
            this.lbUser.Name = "lbUser";
            this.lbUser.Size = new System.Drawing.Size(32, 13);
            this.lbUser.TabIndex = 73;
            this.lbUser.Text = "User:";
            // 
            // lbServer
            // 
            this.lbServer.AutoSize = true;
            this.lbServer.Location = new System.Drawing.Point(6, 22);
            this.lbServer.Name = "lbServer";
            this.lbServer.Size = new System.Drawing.Size(71, 13);
            this.lbServer.TabIndex = 72;
            this.lbServer.Text = "SFTP Server:";
            // 
            // groupBoxEncryption
            // 
            this.groupBoxEncryption.Controls.Add(this.cmbPassPhrase);
            this.groupBoxEncryption.Controls.Add(this.label6);
            this.groupBoxEncryption.Controls.Add(this.optPublicKeyFileVariable);
            this.groupBoxEncryption.Controls.Add(this.optPublicKeyFileConnection);
            this.groupBoxEncryption.Controls.Add(this.cmbKeyFile);
            this.groupBoxEncryption.Controls.Add(this.btKeyFileExpression);
            this.groupBoxEncryption.Controls.Add(this.label4);
            this.groupBoxEncryption.Location = new System.Drawing.Point(427, 3);
            this.groupBoxEncryption.Name = "groupBoxEncryption";
            this.groupBoxEncryption.Size = new System.Drawing.Size(411, 157);
            this.groupBoxEncryption.TabIndex = 73;
            this.groupBoxEncryption.TabStop = false;
            this.groupBoxEncryption.Text = "Encryption details";
            // 
            // cmbPassPhrase
            // 
            this.cmbPassPhrase.FormattingEnabled = true;
            this.cmbPassPhrase.Location = new System.Drawing.Point(92, 68);
            this.cmbPassPhrase.Name = "cmbPassPhrase";
            this.cmbPassPhrase.Size = new System.Drawing.Size(309, 21);
            this.cmbPassPhrase.TabIndex = 94;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 93;
            this.label6.Text = "Pass phrase:";
            // 
            // optPublicKeyFileVariable
            // 
            this.optPublicKeyFileVariable.AutoSize = true;
            this.optPublicKeyFileVariable.Location = new System.Drawing.Point(196, 46);
            this.optPublicKeyFileVariable.Name = "optPublicKeyFileVariable";
            this.optPublicKeyFileVariable.Size = new System.Drawing.Size(93, 17);
            this.optPublicKeyFileVariable.TabIndex = 92;
            this.optPublicKeyFileVariable.Text = "Variables / f(x)";
            this.optPublicKeyFileVariable.UseVisualStyleBackColor = true;
            this.optPublicKeyFileVariable.Click += new System.EventHandler(this.optPublicKeyFileVariable_Click);
            // 
            // optPublicKeyFileConnection
            // 
            this.optPublicKeyFileConnection.AutoSize = true;
            this.optPublicKeyFileConnection.Checked = true;
            this.optPublicKeyFileConnection.Location = new System.Drawing.Point(92, 46);
            this.optPublicKeyFileConnection.Name = "optPublicKeyFileConnection";
            this.optPublicKeyFileConnection.Size = new System.Drawing.Size(98, 17);
            this.optPublicKeyFileConnection.TabIndex = 91;
            this.optPublicKeyFileConnection.TabStop = true;
            this.optPublicKeyFileConnection.Text = "File Connection";
            this.optPublicKeyFileConnection.UseVisualStyleBackColor = true;
            this.optPublicKeyFileConnection.Click += new System.EventHandler(this.optPublicKeyFileConnection_Click);
            // 
            // cmbKeyFile
            // 
            this.cmbKeyFile.FormattingEnabled = true;
            this.cmbKeyFile.Location = new System.Drawing.Point(92, 19);
            this.cmbKeyFile.Name = "cmbKeyFile";
            this.cmbKeyFile.Size = new System.Drawing.Size(261, 21);
            this.cmbKeyFile.TabIndex = 90;
            // 
            // btKeyFileExpression
            // 
            this.btKeyFileExpression.Location = new System.Drawing.Point(359, 19);
            this.btKeyFileExpression.Name = "btKeyFileExpression";
            this.btKeyFileExpression.Size = new System.Drawing.Size(42, 21);
            this.btKeyFileExpression.TabIndex = 89;
            this.btKeyFileExpression.Text = "f(x)";
            this.btKeyFileExpression.UseVisualStyleBackColor = true;
            this.btKeyFileExpression.Click += new System.EventHandler(this.btKeyFileExpression_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 88;
            this.label4.Text = "Private key file:";
            // 
            // groupbox
            // 
            this.groupbox.Controls.Add(this.lbDepth);
            this.groupbox.Controls.Add(this.txDepth);
            this.groupbox.Controls.Add(this.chkRecursive);
            this.groupbox.Controls.Add(this.chkDeleteFileOnTransferCompleted);
            this.groupbox.Controls.Add(this.chkOverwrite);
            this.groupbox.Controls.Add(this.cmbRemote);
            this.groupbox.Controls.Add(this.optFileVariable);
            this.groupbox.Controls.Add(this.optFileConnection);
            this.groupbox.Controls.Add(this.cmbLocal);
            this.groupbox.Controls.Add(this.label1);
            this.groupbox.Controls.Add(this.cmbFilesList);
            this.groupbox.Controls.Add(this.btDestinationFile);
            this.groupbox.Controls.Add(this.lbDestinationFile);
            this.groupbox.Controls.Add(this.btSourceFile);
            this.groupbox.Controls.Add(this.lbSourceFile);
            this.groupbox.Controls.Add(this.lbAction);
            this.groupbox.Controls.Add(this.cmbAction);
            this.groupbox.Location = new System.Drawing.Point(10, 166);
            this.groupbox.Name = "groupbox";
            this.groupbox.Size = new System.Drawing.Size(411, 201);
            this.groupbox.TabIndex = 75;
            this.groupbox.TabStop = false;
            this.groupbox.Text = "Paths";
            // 
            // lbDepth
            // 
            this.lbDepth.AutoSize = true;
            this.lbDepth.Location = new System.Drawing.Point(230, 180);
            this.lbDepth.Name = "lbDepth";
            this.lbDepth.Size = new System.Drawing.Size(36, 13);
            this.lbDepth.TabIndex = 79;
            this.lbDepth.Text = "Depth";
            // 
            // txDepth
            // 
            this.txDepth.Location = new System.Drawing.Point(287, 176);
            this.txDepth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txDepth.Name = "txDepth";
            this.txDepth.Size = new System.Drawing.Size(46, 20);
            this.txDepth.TabIndex = 78;
            this.txDepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txDepth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkDeleteFileOnTransferCompleted
            // 
            this.chkDeleteFileOnTransferCompleted.AutoSize = true;
            this.chkDeleteFileOnTransferCompleted.Location = new System.Drawing.Point(92, 154);
            this.chkDeleteFileOnTransferCompleted.Name = "chkDeleteFileOnTransferCompleted";
            this.chkDeleteFileOnTransferCompleted.Size = new System.Drawing.Size(224, 17);
            this.chkDeleteFileOnTransferCompleted.TabIndex = 75;
            this.chkDeleteFileOnTransferCompleted.Text = "Delete File On Transfer Completed (Move)";
            this.chkDeleteFileOnTransferCompleted.UseVisualStyleBackColor = true;
            // 
            // cmbRemote
            // 
            this.cmbRemote.FormattingEnabled = true;
            this.cmbRemote.Location = new System.Drawing.Point(92, 99);
            this.cmbRemote.Name = "cmbRemote";
            this.cmbRemote.Size = new System.Drawing.Size(251, 21);
            this.cmbRemote.TabIndex = 73;
            // 
            // optFileVariable
            // 
            this.optFileVariable.AutoSize = true;
            this.optFileVariable.Location = new System.Drawing.Point(196, 79);
            this.optFileVariable.Name = "optFileVariable";
            this.optFileVariable.Size = new System.Drawing.Size(93, 17);
            this.optFileVariable.TabIndex = 72;
            this.optFileVariable.Text = "Variables / f(x)";
            this.optFileVariable.UseVisualStyleBackColor = true;
            this.optFileVariable.Click += new System.EventHandler(this.optFileVariable_Click);
            // 
            // optFileConnection
            // 
            this.optFileConnection.AutoSize = true;
            this.optFileConnection.Checked = true;
            this.optFileConnection.Location = new System.Drawing.Point(92, 79);
            this.optFileConnection.Name = "optFileConnection";
            this.optFileConnection.Size = new System.Drawing.Size(98, 17);
            this.optFileConnection.TabIndex = 71;
            this.optFileConnection.TabStop = true;
            this.optFileConnection.Text = "File Connection";
            this.optFileConnection.UseVisualStyleBackColor = true;
            this.optFileConnection.Click += new System.EventHandler(this.optFileConnection_Click);
            // 
            // cmbLocal
            // 
            this.cmbLocal.FormattingEnabled = true;
            this.cmbLocal.Location = new System.Drawing.Point(92, 52);
            this.cmbLocal.Name = "cmbLocal";
            this.cmbLocal.Size = new System.Drawing.Size(251, 21);
            this.cmbLocal.TabIndex = 70;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 130);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 69;
            this.label1.Text = "Returned files list ";
            this.label1.Visible = false;
            // 
            // cmbFilesList
            // 
            this.cmbFilesList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilesList.FormattingEnabled = true;
            this.cmbFilesList.Location = new System.Drawing.Point(92, 126);
            this.cmbFilesList.Name = "cmbFilesList";
            this.cmbFilesList.Size = new System.Drawing.Size(309, 21);
            this.cmbFilesList.TabIndex = 68;
            this.cmbFilesList.Visible = false;
            // 
            // btDestinationFile
            // 
            this.btDestinationFile.Location = new System.Drawing.Point(361, 99);
            this.btDestinationFile.Name = "btDestinationFile";
            this.btDestinationFile.Size = new System.Drawing.Size(40, 21);
            this.btDestinationFile.TabIndex = 67;
            this.btDestinationFile.Text = "f(x)";
            this.btDestinationFile.UseVisualStyleBackColor = true;
            this.btDestinationFile.Click += new System.EventHandler(this.btDestinationFile_Click);
            // 
            // lbDestinationFile
            // 
            this.lbDestinationFile.AutoSize = true;
            this.lbDestinationFile.Location = new System.Drawing.Point(2, 103);
            this.lbDestinationFile.Name = "lbDestinationFile";
            this.lbDestinationFile.Size = new System.Drawing.Size(68, 13);
            this.lbDestinationFile.TabIndex = 66;
            this.lbDestinationFile.Text = "Remote path";
            // 
            // btSourceFile
            // 
            this.btSourceFile.Location = new System.Drawing.Point(359, 51);
            this.btSourceFile.Name = "btSourceFile";
            this.btSourceFile.Size = new System.Drawing.Size(40, 21);
            this.btSourceFile.TabIndex = 65;
            this.btSourceFile.Text = "f(x)";
            this.btSourceFile.UseVisualStyleBackColor = true;
            this.btSourceFile.Click += new System.EventHandler(this.btSourceFile_Click);
            // 
            // lbSourceFile
            // 
            this.lbSourceFile.AutoSize = true;
            this.lbSourceFile.Location = new System.Drawing.Point(2, 51);
            this.lbSourceFile.Name = "lbSourceFile";
            this.lbSourceFile.Size = new System.Drawing.Size(58, 13);
            this.lbSourceFile.TabIndex = 64;
            this.lbSourceFile.Text = "Local Path";
            // 
            // lbAction
            // 
            this.lbAction.AutoSize = true;
            this.lbAction.Location = new System.Drawing.Point(2, 28);
            this.lbAction.Name = "lbAction";
            this.lbAction.Size = new System.Drawing.Size(37, 13);
            this.lbAction.TabIndex = 63;
            this.lbAction.Text = "Action";
            // 
            // cmbAction
            // 
            this.cmbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAction.FormattingEnabled = true;
            this.cmbAction.Location = new System.Drawing.Point(92, 25);
            this.cmbAction.Name = "cmbAction";
            this.cmbAction.Size = new System.Drawing.Size(307, 21);
            this.cmbAction.TabIndex = 62;
            this.cmbAction.SelectedIndexChanged += new System.EventHandler(this.cmbAction_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.domainUpDownIndex);
            this.groupBox1.Controls.Add(this.optFullPath);
            this.groupBox1.Controls.Add(this.optNameOnly);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cmbRecordset);
            this.groupBox1.Controls.Add(this.chkResultSetEnabled);
            this.groupBox1.Location = new System.Drawing.Point(427, 166);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(411, 201);
            this.groupBox1.TabIndex = 76;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Resultset";
            // 
            // domainUpDownIndex
            // 
            this.domainUpDownIndex.Enabled = false;
            this.domainUpDownIndex.Location = new System.Drawing.Point(169, 105);
            this.domainUpDownIndex.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.domainUpDownIndex.Name = "domainUpDownIndex";
            this.domainUpDownIndex.Size = new System.Drawing.Size(48, 20);
            this.domainUpDownIndex.TabIndex = 91;
            this.domainUpDownIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // optFullPath
            // 
            this.optFullPath.AutoSize = true;
            this.optFullPath.Location = new System.Drawing.Point(167, 157);
            this.optFullPath.Name = "optFullPath";
            this.optFullPath.Size = new System.Drawing.Size(84, 17);
            this.optFullPath.TabIndex = 8;
            this.optFullPath.TabStop = true;
            this.optFullPath.Text = "The full path";
            this.optFullPath.UseVisualStyleBackColor = true;
            // 
            // optNameOnly
            // 
            this.optNameOnly.AutoSize = true;
            this.optNameOnly.Location = new System.Drawing.Point(9, 157);
            this.optNameOnly.Name = "optNameOnly";
            this.optNameOnly.Size = new System.Drawing.Size(139, 17);
            this.optNameOnly.TabIndex = 7;
            this.optNameOnly.TabStop = true;
            this.optNameOnly.Text = "Only the name of the file";
            this.optNameOnly.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 141);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(145, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "The obtained value contains:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 107);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(157, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Index of the concerned column:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(159, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Choose the object type variable:";
            // 
            // cmbRecordset
            // 
            this.cmbRecordset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordset.FormattingEnabled = true;
            this.cmbRecordset.Location = new System.Drawing.Point(9, 71);
            this.cmbRecordset.Name = "cmbRecordset";
            this.cmbRecordset.Size = new System.Drawing.Size(392, 21);
            this.cmbRecordset.TabIndex = 1;
            // 
            // chkResultSetEnabled
            // 
            this.chkResultSetEnabled.AutoSize = true;
            this.chkResultSetEnabled.Location = new System.Drawing.Point(9, 28);
            this.chkResultSetEnabled.Name = "chkResultSetEnabled";
            this.chkResultSetEnabled.Size = new System.Drawing.Size(387, 17);
            this.chkResultSetEnabled.TabIndex = 0;
            this.chkResultSetEnabled.Text = "The source file(s) for GET or PUT or DEL operations is filled  via a Recordset";
            this.chkResultSetEnabled.UseVisualStyleBackColor = true;
            this.chkResultSetEnabled.Click += new System.EventHandler(this.chkResultSetEnabled_Click);
            // 
            // frmEditProperties
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(849, 408);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupbox);
            this.Controls.Add(this.groupBoxEncryption);
            this.Controls.Add(this.groupBoxConnection);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.linkLabelCodeplex);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEditProperties";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SFTP Task Properties";
            this.groupBoxConnection.ResumeLayout(false);
            this.groupBoxConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
            this.groupBoxEncryption.ResumeLayout(false);
            this.groupBoxEncryption.PerformLayout();
            this.groupbox.ResumeLayout(false);
            this.groupbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txDepth)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.domainUpDownIndex)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.LinkLabel linkLabelCodeplex;
        private System.Windows.Forms.GroupBox groupBoxConnection;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbPassword;
        private System.Windows.Forms.ComboBox cmbUser;
        private System.Windows.Forms.ComboBox cmbServer;
        private System.Windows.Forms.Label lbPassword;
        private System.Windows.Forms.Label lbUser;
        private System.Windows.Forms.Label lbServer;
        private System.Windows.Forms.RadioButton optionEncryptionKey;
        private System.Windows.Forms.RadioButton optionEncryptionPassword;
        private System.Windows.Forms.GroupBox groupBoxEncryption;
        private System.Windows.Forms.ComboBox cmbPassPhrase;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton optPublicKeyFileVariable;
        private System.Windows.Forms.RadioButton optPublicKeyFileConnection;
        private System.Windows.Forms.ComboBox cmbKeyFile;
        private System.Windows.Forms.Button btKeyFileExpression;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkSleep;
        private System.Windows.Forms.GroupBox groupbox;
        private System.Windows.Forms.CheckBox chkOverwrite;
        private System.Windows.Forms.ComboBox cmbRemote;
        private System.Windows.Forms.RadioButton optFileVariable;
        private System.Windows.Forms.RadioButton optFileConnection;
        private System.Windows.Forms.ComboBox cmbLocal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbFilesList;
        private System.Windows.Forms.Button btDestinationFile;
        private System.Windows.Forms.Label lbDestinationFile;
        private System.Windows.Forms.Button btSourceFile;
        private System.Windows.Forms.Label lbSourceFile;
        private System.Windows.Forms.Label lbAction;
        private System.Windows.Forms.ComboBox cmbAction;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkResultSetEnabled;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbRecordset;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton optFullPath;
        private System.Windows.Forms.RadioButton optNameOnly;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown domainUpDownIndex;
        private System.Windows.Forms.CheckBox chkDeleteFileOnTransferCompleted;
        private System.Windows.Forms.CheckBox chkRecursive;
        private System.Windows.Forms.Label lbDepth;
        private System.Windows.Forms.NumericUpDown txDepth;
    }
}