namespace JumpToTop
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnDoubleLineSearch = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.btnSingleLineSearch = new System.Windows.Forms.Button();
            this.btnThreeLineSearch = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(369, 622);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 668);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(396, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // btnDoubleLineSearch
            // 
            this.btnDoubleLineSearch.Location = new System.Drawing.Point(147, 641);
            this.btnDoubleLineSearch.Name = "btnDoubleLineSearch";
            this.btnDoubleLineSearch.Size = new System.Drawing.Size(94, 23);
            this.btnDoubleLineSearch.TabIndex = 2;
            this.btnDoubleLineSearch.Text = "搜索（双行）";
            this.btnDoubleLineSearch.UseVisualStyleBackColor = true;
            this.btnDoubleLineSearch.Click += new System.EventHandler(this.btnDoubleLineSearch_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // btnSingleLineSearch
            // 
            this.btnSingleLineSearch.Location = new System.Drawing.Point(50, 641);
            this.btnSingleLineSearch.Name = "btnSingleLineSearch";
            this.btnSingleLineSearch.Size = new System.Drawing.Size(91, 23);
            this.btnSingleLineSearch.TabIndex = 3;
            this.btnSingleLineSearch.Text = "搜索（单行）";
            this.btnSingleLineSearch.UseVisualStyleBackColor = true;
            this.btnSingleLineSearch.Click += new System.EventHandler(this.btnSingleLineSearch_Click);
            // 
            // btnThreeLineSearch
            // 
            this.btnThreeLineSearch.Location = new System.Drawing.Point(247, 641);
            this.btnThreeLineSearch.Name = "btnThreeLineSearch";
            this.btnThreeLineSearch.Size = new System.Drawing.Size(99, 23);
            this.btnThreeLineSearch.TabIndex = 4;
            this.btnThreeLineSearch.Text = "搜索（三行）";
            this.btnThreeLineSearch.UseVisualStyleBackColor = true;
            this.btnThreeLineSearch.Click += new System.EventHandler(this.btnThreeLineSearch_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 690);
            this.Controls.Add(this.btnThreeLineSearch);
            this.Controls.Add(this.btnSingleLineSearch);
            this.Controls.Add(this.btnDoubleLineSearch);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "冲顶搜索";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.Button btnDoubleLineSearch;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btnSingleLineSearch;
        private System.Windows.Forms.Button btnThreeLineSearch;

    }
}

