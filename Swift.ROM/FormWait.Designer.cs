namespace Swift.ROM
{
    partial class FormWait
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWait));
            this.labelMessage = new System.Windows.Forms.Label();
            this.pictureBoxInfo = new System.Windows.Forms.PictureBox();
            this.labelInfo = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelMessage, "labelMessage");
            this.labelMessage.Name = "labelMessage";
            // 
            // pictureBoxInfo
            // 
            this.pictureBoxInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.pictureBoxInfo, "pictureBoxInfo");
            this.pictureBoxInfo.Name = "pictureBoxInfo";
            this.pictureBoxInfo.TabStop = false;
            this.pictureBoxInfo.Click += new System.EventHandler(this.pictureBoxInfo_Click);
            // 
            // labelInfo
            // 
            this.labelInfo.BackColor = System.Drawing.Color.Transparent;
            this.labelInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.labelInfo, "labelInfo");
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Click += new System.EventHandler(this.pictureBoxInfo_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // FormWait
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::Swift.ROM.Properties.Resources.nds;
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.pictureBoxInfo);
            this.Controls.Add(this.labelMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormWait";
            this.ShowInTaskbar = false;
            this.TransparencyKey = System.Drawing.Color.Blue;
            this.Load += new System.EventHandler(this.FormWait_Load);
            this.Click += new System.EventHandler(this.FormWait_Click);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWait_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.PictureBox pictureBoxInfo;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
    }
}