namespace Swift.ROM
{
    partial class FormDownload
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDownload));
			this.listView = new System.Windows.Forms.ListView();
			this.buttonHide = new System.Windows.Forms.Button();
			this.webClient = new System.Net.WebClient();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// listView
			// 
			resources.ApplyResources(this.listView, "listView");
			this.listView.Name = "listView";
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.List;
			// 
			// buttonHide
			// 
			resources.ApplyResources(this.buttonHide, "buttonHide");
			this.buttonHide.Name = "buttonHide";
			this.buttonHide.UseVisualStyleBackColor = true;
			this.buttonHide.Click += new System.EventHandler(this.buttonHide_Click);
			// 
			// webClient
			// 
			this.webClient.BaseAddress = "";
			this.webClient.CachePolicy = null;
			this.webClient.Credentials = null;
			this.webClient.Encoding = ((System.Text.Encoding)(resources.GetObject("webClient.Encoding")));
			this.webClient.Headers = ((System.Net.WebHeaderCollection)(resources.GetObject("webClient.Headers")));
			this.webClient.QueryString = ((System.Collections.Specialized.NameValueCollection)(resources.GetObject("webClient.QueryString")));
			this.webClient.UseDefaultCredentials = false;
			this.webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.webClient_DownloadFileCompleted);
			// 
			// progressBar1
			// 
			resources.ApplyResources(this.progressBar1, "progressBar1");
			this.progressBar1.Name = "progressBar1";
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Interval = 1000;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// FormDownload
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.buttonHide);
			this.Controls.Add(this.listView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "FormDownload";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDownload_FormClosing);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonHide;
        private System.Net.WebClient webClient;
        private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.ListView listView;
    }
}