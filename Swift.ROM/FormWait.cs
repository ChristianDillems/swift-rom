using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Swift.ROM
{
    public partial class FormWait : Form
    {
        public FormWait()
        {
            InitializeComponent();
        }

        private void pictureBoxInfo_Click(object sender, EventArgs e)
        {
            Tools.Start(this.pictureBoxInfo.Tag.ToString());
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();
            this.timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("Opacity="+this.Opacity);
            this.Opacity-=0.1;
            Application.DoEvents();
            if (this.Opacity == 0)
            {
                this.timer2.Stop();
                this.Close();
                Debug.WriteLine("透明=0,关闭窗口");
            }
        }

        private void FormWait_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Opacity > 0)
            {
                this.timer1.Start();
                e.Cancel = true;
                Debug.WriteLine("取消关闭窗口");
            }
        }

        private void FormWait_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void FormWait_Load(object sender, EventArgs e)
        {
            if(File.Exists(Application.StartupPath+"/ad.png"))
                this.pictureBoxInfo.Image = Tools.GetImage(Application.StartupPath + "/ad.png");

            if(!File.Exists(Application.StartupPath+"/ad.xml"))
                return;

            XmlDocument xd = new XmlDocument();
            xd.Load(Application.StartupPath+"/ad.xml");
            XmlElement xe = (XmlElement)xd.FirstChild;

            if (Application.CurrentCulture.Name.StartsWith("zh"))    //中文
            {
                this.labelInfo.Text = xe.GetAttribute("chText");
                this.pictureBoxInfo.Tag = xe.GetAttribute("chLink");
            }
            else //非中文
            {
                this.labelInfo.Text = xe.GetAttribute("enText");
                this.pictureBoxInfo.Tag = xe.GetAttribute("enLink");
            }
        }

    }
}