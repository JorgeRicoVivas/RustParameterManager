namespace SLightParameterManager
{
    partial class JsonForm
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
            this.jsonTable = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.jsonTable.AutoSize = true;
            this.jsonTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.jsonTable.ColumnCount = 2;
            this.jsonTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.jsonTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.jsonTable.Location = new System.Drawing.Point(12, 12);
            this.jsonTable.Name = "tableLayoutPanel1";
            this.jsonTable.RowCount = 1;
            this.jsonTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.jsonTable.Size = new System.Drawing.Size(0, 0);
            this.jsonTable.TabIndex = 0;
            // 
            // JsonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.jsonTable);
            this.Name = "JsonForm";
            this.Size = new System.Drawing.Size(15, 15);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.TableLayoutPanel jsonTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}