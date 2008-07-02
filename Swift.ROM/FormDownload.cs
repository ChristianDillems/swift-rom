using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Swift.ROM
{
    public partial class FormDownload : Form
    {
        public FormDownload(ToolStripLabel t)
        {
            InitializeComponent();

			this.tsl = t;
			tsl.Text = "";
        }

		public void download(string u)
		{ 
			//�ж��Ƿ�Ҫ���ص��ļ��Ѿ������������б�����
			foreach (ListViewItem lvi in this.listView.Items)
				if (lvi.Text == u)
					return;
			this.listView.Items.Add(u);
		}

		private ToolStripLabel tsl = null;

		private string url = "http://test.shanmin.com/";
		
        private ListViewItem nowLvi=null;

        private void buttonHide_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.listView.Items.Count==0 || this.webClient.IsBusy || this.nowLvi != null)
				return;

            this.nowLvi = this.listView.Items[0];
            
            try
            {
				tsl.Text = "Images Downloading ...";
                //this.webClient.DownloadFile(new Uri(url + lvi.Text), Application.StartupPath + "/" + lvi.Text);
				//if(this.nowLvi.Text.StartsWith("GBA"))
					this.webClient.DownloadFileAsync(new Uri(url + this.nowLvi.Text), Application.StartupPath + @"\" + this.nowLvi.Text);
				//else
				//	this.webClient.DownloadFileAsync(new Uri(url1 + this.nowLvi.Text), Application.StartupPath + "/" + this.nowLvi.Text);
            }
            catch
            {
                if (File.Exists(Application.StartupPath + @"\" + this.nowLvi.Text))
                {
                    try
                    {
                        File.Delete(Application.StartupPath + @"\" + this.nowLvi.Text);
                    }
                    catch { }
                }
            }

            //this.nowLvi.Remove();
        }

        private void FormDownload_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (File.Exists(Application.StartupPath + @"\" + this.nowLvi.Text))
                {
                    try
                    {
                        File.Delete(Application.StartupPath + @"\" + this.nowLvi.Text);
                    }
                    catch { }
                }
            }
			else//�ж����ص��ļ��ֽڳ����Ƿ�Ϊ0,Ϊ0�������ش������ɾ��
			{
				if(File.Open(Application.StartupPath + @"\" + this.nowLvi.Text,FileMode.Open).Length==0)
					File.Delete(Application.StartupPath + @"\" + this.nowLvi.Text);
			}

			tsl.Text = "";

            this.listView.Items.Remove(this.nowLvi);
            this.nowLvi = null;
        }
    }
}