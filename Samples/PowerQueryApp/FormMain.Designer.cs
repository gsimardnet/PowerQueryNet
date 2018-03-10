namespace PowerQuery.Samples
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.SplitContainerV = new System.Windows.Forms.SplitContainer();
            this.ListBoxPQ = new System.Windows.Forms.ListBox();
            this.SplitContainerH = new System.Windows.Forms.SplitContainer();
            this.TextBoxPQ = new System.Windows.Forms.TextBox();
            this.GridResult = new System.Windows.Forms.DataGridView();
            this.ToolStripTop = new System.Windows.Forms.ToolStrip();
            this.ButtonOpenFolder = new System.Windows.Forms.ToolStripButton();
            this.ButtonExecute = new System.Windows.Forms.ToolStripButton();
            this.StatusStripBottom = new System.Windows.Forms.StatusStrip();
            this.LabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.LabelSeparator = new System.Windows.Forms.ToolStripStatusLabel();
            this.LabelCredential = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerV)).BeginInit();
            this.SplitContainerV.Panel1.SuspendLayout();
            this.SplitContainerV.Panel2.SuspendLayout();
            this.SplitContainerV.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerH)).BeginInit();
            this.SplitContainerH.Panel1.SuspendLayout();
            this.SplitContainerH.Panel2.SuspendLayout();
            this.SplitContainerH.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridResult)).BeginInit();
            this.ToolStripTop.SuspendLayout();
            this.StatusStripBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // SplitContainerV
            // 
            this.SplitContainerV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitContainerV.Location = new System.Drawing.Point(12, 28);
            this.SplitContainerV.Name = "SplitContainerV";
            // 
            // SplitContainerV.Panel1
            // 
            this.SplitContainerV.Panel1.Controls.Add(this.ListBoxPQ);
            this.SplitContainerV.Panel1MinSize = 150;
            // 
            // SplitContainerV.Panel2
            // 
            this.SplitContainerV.Panel2.Controls.Add(this.SplitContainerH);
            this.SplitContainerV.Size = new System.Drawing.Size(1421, 704);
            this.SplitContainerV.SplitterDistance = 225;
            this.SplitContainerV.TabIndex = 102;
            // 
            // ListBoxPQ
            // 
            this.ListBoxPQ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListBoxPQ.FormattingEnabled = true;
            this.ListBoxPQ.Location = new System.Drawing.Point(0, 0);
            this.ListBoxPQ.Name = "ListBoxPQ";
            this.ListBoxPQ.Size = new System.Drawing.Size(225, 704);
            this.ListBoxPQ.TabIndex = 102;
            this.ListBoxPQ.SelectedIndexChanged += new System.EventHandler(this.ListBoxPQ_SelectedIndexChanged);
            // 
            // SplitContainerH
            // 
            this.SplitContainerH.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainerH.Location = new System.Drawing.Point(0, 0);
            this.SplitContainerH.Name = "SplitContainerH";
            this.SplitContainerH.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainerH.Panel1
            // 
            this.SplitContainerH.Panel1.Controls.Add(this.TextBoxPQ);
            // 
            // SplitContainerH.Panel2
            // 
            this.SplitContainerH.Panel2.Controls.Add(this.GridResult);
            this.SplitContainerH.Size = new System.Drawing.Size(1192, 704);
            this.SplitContainerH.SplitterDistance = 268;
            this.SplitContainerH.TabIndex = 103;
            // 
            // TextBoxPQ
            // 
            this.TextBoxPQ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBoxPQ.Location = new System.Drawing.Point(0, 0);
            this.TextBoxPQ.Multiline = true;
            this.TextBoxPQ.Name = "TextBoxPQ";
            this.TextBoxPQ.ReadOnly = true;
            this.TextBoxPQ.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBoxPQ.Size = new System.Drawing.Size(1192, 268);
            this.TextBoxPQ.TabIndex = 103;
            // 
            // GridResult
            // 
            this.GridResult.AllowUserToAddRows = false;
            this.GridResult.AllowUserToDeleteRows = false;
            this.GridResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridResult.Location = new System.Drawing.Point(0, 0);
            this.GridResult.Name = "GridResult";
            this.GridResult.ReadOnly = true;
            this.GridResult.Size = new System.Drawing.Size(1192, 432);
            this.GridResult.TabIndex = 102;
            // 
            // ToolStripTop
            // 
            this.ToolStripTop.BackColor = System.Drawing.SystemColors.Control;
            this.ToolStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ButtonOpenFolder,
            this.ButtonExecute});
            this.ToolStripTop.Location = new System.Drawing.Point(0, 0);
            this.ToolStripTop.Name = "ToolStripTop";
            this.ToolStripTop.Size = new System.Drawing.Size(1445, 25);
            this.ToolStripTop.TabIndex = 103;
            this.ToolStripTop.Text = "toolStrip1";
            // 
            // ButtonOpenFolder
            // 
            this.ButtonOpenFolder.Image = ((System.Drawing.Image)(resources.GetObject("ButtonOpenFolder.Image")));
            this.ButtonOpenFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ButtonOpenFolder.Name = "ButtonOpenFolder";
            this.ButtonOpenFolder.Size = new System.Drawing.Size(92, 22);
            this.ButtonOpenFolder.Text = "Open Folder";
            this.ButtonOpenFolder.Click += new System.EventHandler(this.ButtonOpenFolder_Click);
            // 
            // ButtonExecute
            // 
            this.ButtonExecute.Image = ((System.Drawing.Image)(resources.GetObject("ButtonExecute.Image")));
            this.ButtonExecute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ButtonExecute.Name = "ButtonExecute";
            this.ButtonExecute.Size = new System.Drawing.Size(67, 22);
            this.ButtonExecute.Text = "Execute";
            this.ButtonExecute.Click += new System.EventHandler(this.ButtonExecute_Click);
            // 
            // StatusStripBottom
            // 
            this.StatusStripBottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LabelStatus,
            this.LabelSeparator,
            this.LabelCredential});
            this.StatusStripBottom.Location = new System.Drawing.Point(0, 722);
            this.StatusStripBottom.Name = "StatusStripBottom";
            this.StatusStripBottom.Size = new System.Drawing.Size(1445, 22);
            this.StatusStripBottom.TabIndex = 104;
            this.StatusStripBottom.Text = "statusStrip1";
            // 
            // LabelStatus
            // 
            this.LabelStatus.BackColor = System.Drawing.SystemColors.Control;
            this.LabelStatus.Name = "LabelStatus";
            this.LabelStatus.Size = new System.Drawing.Size(118, 17);
            this.LabelStatus.Text = "toolStripStatusLabel1";
            // 
            // LabelSeparator
            // 
            this.LabelSeparator.BackColor = System.Drawing.SystemColors.Control;
            this.LabelSeparator.Name = "LabelSeparator";
            this.LabelSeparator.Size = new System.Drawing.Size(10, 17);
            this.LabelSeparator.Text = "|";
            // 
            // LabelCredential
            // 
            this.LabelCredential.BackColor = System.Drawing.SystemColors.Control;
            this.LabelCredential.Name = "LabelCredential";
            this.LabelCredential.Size = new System.Drawing.Size(118, 17);
            this.LabelCredential.Text = "toolStripStatusLabel2";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1445, 744);
            this.Controls.Add(this.StatusStripBottom);
            this.Controls.Add(this.ToolStripTop);
            this.Controls.Add(this.SplitContainerV);
            this.MinimumSize = new System.Drawing.Size(533, 533);
            this.Name = "FormMain";
            this.Text = "Power Query App";
            this.Activated += new System.EventHandler(this.FormMain_Activated);
            this.SplitContainerV.Panel1.ResumeLayout(false);
            this.SplitContainerV.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerV)).EndInit();
            this.SplitContainerV.ResumeLayout(false);
            this.SplitContainerH.Panel1.ResumeLayout(false);
            this.SplitContainerH.Panel1.PerformLayout();
            this.SplitContainerH.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerH)).EndInit();
            this.SplitContainerH.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GridResult)).EndInit();
            this.ToolStripTop.ResumeLayout(false);
            this.ToolStripTop.PerformLayout();
            this.StatusStripBottom.ResumeLayout(false);
            this.StatusStripBottom.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.SplitContainer SplitContainerV;
        private System.Windows.Forms.ListBox ListBoxPQ;
        private System.Windows.Forms.ToolStrip ToolStripTop;
        private System.Windows.Forms.ToolStripButton ButtonExecute;
        private System.Windows.Forms.SplitContainer SplitContainerH;
        private System.Windows.Forms.TextBox TextBoxPQ;
        private System.Windows.Forms.DataGridView GridResult;
        private System.Windows.Forms.StatusStrip StatusStripBottom;
        private System.Windows.Forms.ToolStripStatusLabel LabelStatus;
        private System.Windows.Forms.ToolStripStatusLabel LabelSeparator;
        private System.Windows.Forms.ToolStripStatusLabel LabelCredential;
        private System.Windows.Forms.ToolStripButton ButtonOpenFolder;
    }
}

