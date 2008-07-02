namespace Swift.ROM
{
    partial class FormFind
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFind));
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxKeyword = new System.Windows.Forms.TextBox();
			this.buttonFind = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AccessibleDescription = null;
			this.label1.AccessibleName = null;
			resources.ApplyResources(this.label1, "label1");
			this.label1.Font = null;
			this.label1.Name = "label1";
			// 
			// textBoxKeyword
			// 
			this.textBoxKeyword.AccessibleDescription = null;
			this.textBoxKeyword.AccessibleName = null;
			resources.ApplyResources(this.textBoxKeyword, "textBoxKeyword");
			this.textBoxKeyword.BackgroundImage = null;
			this.textBoxKeyword.Font = null;
			this.textBoxKeyword.Name = "textBoxKeyword";
			// 
			// buttonFind
			// 
			this.buttonFind.AccessibleDescription = null;
			this.buttonFind.AccessibleName = null;
			resources.ApplyResources(this.buttonFind, "buttonFind");
			this.buttonFind.BackgroundImage = null;
			this.buttonFind.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonFind.Font = null;
			this.buttonFind.Name = "buttonFind";
			this.buttonFind.UseVisualStyleBackColor = true;
			this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
			// 
			// FormFind
			// 
			this.AcceptButton = this.buttonFind;
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = null;
			this.Controls.Add(this.buttonFind);
			this.Controls.Add(this.textBoxKeyword);
			this.Controls.Add(this.label1);
			this.Font = null;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = null;
			this.Name = "FormFind";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox textBoxKeyword;
        private System.Windows.Forms.Button buttonFind;
    }
}