namespace SLightParameterManager {
    partial class NumericIncrementRange {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.incrementUpDown = new System.Windows.Forms.NumericUpDown();
            this.fromUpDown = new System.Windows.Forms.NumericUpDown();
            this.toUpDown = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.incrementUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.incrementUpDown, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.fromUpDown, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.toUpDown, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(160, 78);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Increment by:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "From:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 26);
            this.label3.TabIndex = 2;
            this.label3.Text = "To:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // incrementUpDown
            // 
            this.incrementUpDown.AutoSize = true;
            this.incrementUpDown.Location = new System.Drawing.Point(80, 3);
            this.incrementUpDown.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.incrementUpDown.Name = "incrementUpDown";
            this.incrementUpDown.Size = new System.Drawing.Size(77, 20);
            this.incrementUpDown.TabIndex = 3;
            this.incrementUpDown.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // fromUpDown
            // 
            this.fromUpDown.AutoSize = true;
            this.fromUpDown.Location = new System.Drawing.Point(80, 29);
            this.fromUpDown.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.fromUpDown.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            this.fromUpDown.Name = "fromUpDown";
            this.fromUpDown.Size = new System.Drawing.Size(77, 20);
            this.fromUpDown.TabIndex = 4;
            this.fromUpDown.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // toUpDown
            // 
            this.toUpDown.AutoSize = true;
            this.toUpDown.Location = new System.Drawing.Point(80, 55);
            this.toUpDown.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.toUpDown.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            this.toUpDown.Name = "toUpDown";
            this.toUpDown.Size = new System.Drawing.Size(77, 20);
            this.toUpDown.TabIndex = 5;
            this.toUpDown.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // NumericIncrementRange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NumericIncrementRange";
            this.Size = new System.Drawing.Size(160, 78);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.incrementUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.NumericUpDown incrementUpDown;
        public System.Windows.Forms.NumericUpDown fromUpDown;
        public System.Windows.Forms.NumericUpDown toUpDown;
    }
}
