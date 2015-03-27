namespace ADBConsole
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.consoleBox = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TagNameBox = new System.Windows.Forms.TextBox();
            this.TagEnable = new System.Windows.Forms.CheckBox();
            this.fileLocationMsg = new System.Windows.Forms.Label();
            this.ExitBtn = new System.Windows.Forms.Button();
            this.ADBStopBtn = new System.Windows.Forms.Button();
            this.ADBStartBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.consoleBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.fileLocationMsg);
            this.splitContainer1.Panel2.Controls.Add(this.ExitBtn);
            this.splitContainer1.Panel2.Controls.Add(this.ADBStopBtn);
            this.splitContainer1.Panel2.Controls.Add(this.ADBStartBtn);
            this.splitContainer1.Size = new System.Drawing.Size(1041, 506);
            this.splitContainer1.SplitterDistance = 440;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            // 
            // consoleBox
            // 
            this.consoleBox.BackColor = System.Drawing.Color.Black;
            this.consoleBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.consoleBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.consoleBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consoleBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.consoleBox.Location = new System.Drawing.Point(0, 0);
            this.consoleBox.MultiSelect = false;
            this.consoleBox.Name = "consoleBox";
            this.consoleBox.Size = new System.Drawing.Size(1041, 440);
            this.consoleBox.TabIndex = 0;
            this.consoleBox.UseCompatibleStateImageBehavior = false;
            this.consoleBox.View = System.Windows.Forms.View.Details;
            this.consoleBox.VirtualMode = true;
            this.consoleBox.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.consoleBox_RetrieveVirtualItem);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 200;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.TagNameBox);
            this.groupBox1.Controls.Add(this.TagEnable);
            this.groupBox1.Location = new System.Drawing.Point(127, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(398, 59);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Tag Name :";
            // 
            // TagNameBox
            // 
            this.TagNameBox.Location = new System.Drawing.Point(98, 33);
            this.TagNameBox.MaxLength = 40;
            this.TagNameBox.Name = "TagNameBox";
            this.TagNameBox.Size = new System.Drawing.Size(284, 20);
            this.TagNameBox.TabIndex = 5;
            // 
            // TagEnable
            // 
            this.TagEnable.AutoSize = true;
            this.TagEnable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.TagEnable.Location = new System.Drawing.Point(4, 12);
            this.TagEnable.Margin = new System.Windows.Forms.Padding(1);
            this.TagEnable.Name = "TagEnable";
            this.TagEnable.Size = new System.Drawing.Size(109, 17);
            this.TagEnable.TabIndex = 4;
            this.TagEnable.Text = "Enable Tag filter :";
            this.TagEnable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.TagEnable.UseVisualStyleBackColor = true;
            this.TagEnable.CheckedChanged += new System.EventHandler(this.TagEnable_CheckedChanged);
            // 
            // fileLocationMsg
            // 
            this.fileLocationMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileLocationMsg.Location = new System.Drawing.Point(531, 22);
            this.fileLocationMsg.Name = "fileLocationMsg";
            this.fileLocationMsg.Size = new System.Drawing.Size(296, 17);
            this.fileLocationMsg.TabIndex = 3;
            this.fileLocationMsg.Text = "The log file is saved at  :  .\\AdbMessage.log";
            // 
            // ExitBtn
            // 
            this.ExitBtn.Location = new System.Drawing.Point(12, 7);
            this.ExitBtn.Name = "ExitBtn";
            this.ExitBtn.Size = new System.Drawing.Size(84, 44);
            this.ExitBtn.TabIndex = 2;
            this.ExitBtn.Text = "Exit";
            this.ExitBtn.UseVisualStyleBackColor = true;
            this.ExitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            // 
            // ADBStopBtn
            // 
            this.ADBStopBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ADBStopBtn.Location = new System.Drawing.Point(945, 7);
            this.ADBStopBtn.Name = "ADBStopBtn";
            this.ADBStopBtn.Size = new System.Drawing.Size(84, 44);
            this.ADBStopBtn.TabIndex = 1;
            this.ADBStopBtn.Text = "Pause";
            this.ADBStopBtn.UseVisualStyleBackColor = true;
            this.ADBStopBtn.Click += new System.EventHandler(this.ADBStopBtn_Click);
            // 
            // ADBStartBtn
            // 
            this.ADBStartBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ADBStartBtn.Location = new System.Drawing.Point(845, 9);
            this.ADBStartBtn.Name = "ADBStartBtn";
            this.ADBStartBtn.Size = new System.Drawing.Size(84, 44);
            this.ADBStartBtn.TabIndex = 0;
            this.ADBStartBtn.Text = "Start/Resume";
            this.ADBStartBtn.UseVisualStyleBackColor = true;
            this.ADBStartBtn.Click += new System.EventHandler(this.ADBStartBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1041, 506);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "adb console";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button ADBStopBtn;
        private System.Windows.Forms.Button ADBStartBtn;
        private System.Windows.Forms.Button ExitBtn;
        private System.Windows.Forms.Label fileLocationMsg;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TagNameBox;
        private System.Windows.Forms.CheckBox TagEnable;
        private System.Windows.Forms.ListView consoleBox;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}

