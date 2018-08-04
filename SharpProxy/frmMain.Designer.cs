namespace SharpProxy
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtExternalPort = new System.Windows.Forms.TextBox();
            this.txtInternalPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbIPAddress = new System.Windows.Forms.ComboBox();
            this.chkRewriteHostHeaders = new System.Windows.Forms.CheckBox();
            this.lnkLblCopy = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(15, 190);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 28);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "&Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(167, 190);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 28);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "S&top";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 60);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "External Port";
            // 
            // txtExternalPort
            // 
            this.txtExternalPort.Location = new System.Drawing.Point(15, 80);
            this.txtExternalPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtExternalPort.MaxLength = 7;
            this.txtExternalPort.Name = "txtExternalPort";
            this.txtExternalPort.Size = new System.Drawing.Size(251, 22);
            this.txtExternalPort.TabIndex = 3;
            this.txtExternalPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPorts_KeyPress);
            // 
            // txtInternalPort
            // 
            this.txtInternalPort.Location = new System.Drawing.Point(15, 128);
            this.txtInternalPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtInternalPort.MaxLength = 7;
            this.txtInternalPort.Name = "txtInternalPort";
            this.txtInternalPort.Size = new System.Drawing.Size(251, 22);
            this.txtInternalPort.TabIndex = 5;
            this.txtInternalPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPorts_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 108);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Internal Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 11);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Your IP Address";
            // 
            // cmbIPAddress
            // 
            this.cmbIPAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIPAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbIPAddress.FormattingEnabled = true;
            this.cmbIPAddress.Location = new System.Drawing.Point(15, 31);
            this.cmbIPAddress.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbIPAddress.Name = "cmbIPAddress";
            this.cmbIPAddress.Size = new System.Drawing.Size(251, 25);
            this.cmbIPAddress.TabIndex = 8;
            // 
            // chkRewriteHostHeaders
            // 
            this.chkRewriteHostHeaders.AutoSize = true;
            this.chkRewriteHostHeaders.Checked = true;
            this.chkRewriteHostHeaders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRewriteHostHeaders.Location = new System.Drawing.Point(17, 161);
            this.chkRewriteHostHeaders.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkRewriteHostHeaders.Name = "chkRewriteHostHeaders";
            this.chkRewriteHostHeaders.Size = new System.Drawing.Size(247, 21);
            this.chkRewriteHostHeaders.TabIndex = 9;
            this.chkRewriteHostHeaders.Text = "&Rewrite host headers (IIS Express)";
            this.chkRewriteHostHeaders.UseVisualStyleBackColor = true;
            // 
            // lnkLblCopy
            // 
            this.lnkLblCopy.AutoSize = true;
            this.lnkLblCopy.Location = new System.Drawing.Point(227, 11);
            this.lnkLblCopy.Name = "lnkLblCopy";
            this.lnkLblCopy.Size = new System.Drawing.Size(40, 17);
            this.lnkLblCopy.TabIndex = 10;
            this.lnkLblCopy.TabStop = true;
            this.lnkLblCopy.Text = "Copy";
            this.lnkLblCopy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLblCopy_LinkClicked);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 228);
            this.Controls.Add(this.lnkLblCopy);
            this.Controls.Add(this.chkRewriteHostHeaders);
            this.Controls.Add(this.cmbIPAddress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtInternalPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtExternalPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "SharpProxy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtExternalPort;
        private System.Windows.Forms.TextBox txtInternalPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbIPAddress;
        private System.Windows.Forms.CheckBox chkRewriteHostHeaders;
        private System.Windows.Forms.LinkLabel lnkLblCopy;
    }
}

