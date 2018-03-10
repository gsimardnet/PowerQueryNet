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
            this.dgvResult = new System.Windows.Forms.DataGridView();
            this.BtnList2 = new System.Windows.Forms.Button();
            this.BtnList1 = new System.Windows.Forms.Button();
            this.BtnList1and2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvResult
            // 
            this.dgvResult.AllowUserToAddRows = false;
            this.dgvResult.AllowUserToDeleteRows = false;
            this.dgvResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResult.Location = new System.Drawing.Point(12, 41);
            this.dgvResult.Name = "dgvResult";
            this.dgvResult.ReadOnly = true;
            this.dgvResult.Size = new System.Drawing.Size(493, 445);
            this.dgvResult.TabIndex = 100;
            // 
            // BtnList2
            // 
            this.BtnList2.Location = new System.Drawing.Point(112, 12);
            this.BtnList2.Name = "BtnList2";
            this.BtnList2.Size = new System.Drawing.Size(94, 23);
            this.BtnList2.TabIndex = 102;
            this.BtnList2.Text = "List2.pq";
            this.BtnList2.UseVisualStyleBackColor = true;
            this.BtnList2.Click += new System.EventHandler(this.BtnList2_Click);
            // 
            // BtnList1
            // 
            this.BtnList1.Location = new System.Drawing.Point(12, 12);
            this.BtnList1.Name = "BtnList1";
            this.BtnList1.Size = new System.Drawing.Size(94, 23);
            this.BtnList1.TabIndex = 101;
            this.BtnList1.Text = "List1.pq";
            this.BtnList1.UseVisualStyleBackColor = true;
            this.BtnList1.Click += new System.EventHandler(this.BtnList1_Click);
            // 
            // BtnList1and2
            // 
            this.BtnList1and2.Location = new System.Drawing.Point(212, 12);
            this.BtnList1and2.Name = "BtnList1and2";
            this.BtnList1and2.Size = new System.Drawing.Size(94, 23);
            this.BtnList1and2.TabIndex = 103;
            this.BtnList1and2.Text = "List1and2.pq";
            this.BtnList1and2.UseVisualStyleBackColor = true;
            this.BtnList1and2.Click += new System.EventHandler(this.BtnList1and2_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 498);
            this.Controls.Add(this.BtnList2);
            this.Controls.Add(this.BtnList1);
            this.Controls.Add(this.BtnList1and2);
            this.Controls.Add(this.dgvResult);
            this.MinimumSize = new System.Drawing.Size(533, 533);
            this.Name = "FormMain";
            this.Text = "PowerQuery.Samples";
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvResult;
        private System.Windows.Forms.Button BtnList2;
        private System.Windows.Forms.Button BtnList1;
        private System.Windows.Forms.Button BtnList1and2;
    }
}

