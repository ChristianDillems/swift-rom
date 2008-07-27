using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Swift.ROM
{
    public partial class FormEdit : Form
    {
        public FormEdit(string t, string a,string f,string size)
        {
            InitializeComponent();

            this.textBoxType.Text = t;

            if (File.Exists(Application.StartupPath + "/Update-" + t + ".db"))
            {
                File.Copy(Application.StartupPath + "/Update-" + t + ".db", Application.StartupPath + "/Update/" + t + DateTime.Now.ToString("yyyyMMddHHmmss") + ".db");
                this.dataSet1.ReadXml(Application.StartupPath + "/Update-" + t + ".db");
            }

            this.textBoxF.Text = f;
            this.textBoxA.Text = a;

            DataRow[] rows = this.dataTableR.Select("A='" + this.textBoxA.Text + "'");
			if (rows.Length > 0)
			{
				this.textBoxC.Text = rows[0]["C"].ToString();
				this.textBoxE.Text = rows[0]["E"].ToString();
				this.textBoxI.Text = rows[0]["I"].ToString();
				this.textBoxL.Text = rows[0]["L"].ToString();
				this.textBoxN.Text = rows[0]["N"].ToString();
				this.textBoxS.Text = rows[0]["S"].ToString();
				this.textBoxX.Text = rows[0]["X"].ToString();
				this.textBoxY.Text = rows[0]["Y"].ToString();
				this.textBoxT.Text = rows[0]["T"].ToString();
				this.checkBoxH.Checked = !rows[0].IsNull("H");
				this.checkBoxHH.Checked = rows[0]["H"].ToString() == "2";
			}
			else
            {
                this.textBoxS.Text = size;

                try
                {
					Convert.ToInt32(Path.GetFileName(f).Substring(0, 4));
					this.textBoxX.Text = Path.GetFileName(f).Substring(0, 4);
					this.textBoxE.Text = Path.GetFileName(f).Substring(5);
                }
                catch
				{
					this.textBoxE.Text = Path.GetFileName(f);
				}

            }
			
			//读出版本号
			if (this.dataTableV.Rows.Count == 0)
				this.textBoxVersion.Text = "0";
			else
				this.textBoxVersion.Text = this.dataTableV.Rows[0][0].ToString();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
			this.textBoxN.Text = this.textBoxN.Text.Replace("（", " (").Replace("）", ")").Replace("  ", " ").Replace("：",":");

			int m = this.listViewImage.Items.Count;

			DataRow[] rows = this.dataTableR.Select("A='" + this.textBoxA.Text + "'");
            DataRow row;
            if (rows.Length > 0)
                row = rows[0];
            else
            {
                row = this.dataTableR.NewRow();
                this.dataTableR.Rows.Add(row);
				if (this.textBoxImg1.Text != "") m++;
				if (this.textBoxImg2.Text != "") m++;
            }

            //看有没有图标,如果有则置为负数
           // if (this.pictureBoxIcon.Image != null)
             //   m = -m;

            row["A"] = this.textBoxA.Text.Trim();//SHA1
			row["C"] = this.textBoxC.Text.Trim() == "" ? null : this.textBoxC.Text.Trim();//出品公司
			row["E"] = this.textBoxE.Text.Trim() == "" ? null : this.textBoxE.Text.Trim();//英文名
			row["I"] = this.textBoxI.Text.Trim() == "" ? null : this.textBoxI.Text.Trim();//信息
			row["L"] = this.textBoxL.Text.Trim() == "" ? null : this.textBoxL.Text.Trim();//语言
			row["N"] = this.textBoxN.Text.Trim() == "" ? null : this.textBoxN.Text.Trim();//中文名
			row["S"] = this.textBoxS.Text.Trim() == "" ? null : this.textBoxS.Text.Trim();//容量
			row["X"] = this.textBoxX.Text.Trim() == "" ? null : this.textBoxX.Text.Trim();//编号
			row["Y"] = this.textBoxY.Text.Trim() == "" ? null : this.textBoxY.Text.Trim();//年份
			row["T"] = this.textBoxT.Text.Trim() == "" ? null : this.textBoxT.Text.Trim();
            if (this.pictureBoxIcon.Image == null)
                row["i"] = DBNull.Value;
            else
            {
                MemoryStream s = new MemoryStream();
                this.pictureBoxIcon.Image.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                byte[] b = s.GetBuffer();
                row["i"] = Convert.ToBase64String(b);
            }
            row["u"] = (int)(DateTime.Now-(new DateTime(2008,1,1))).TotalSeconds;
            if (m == 2)
                row["m"] = DBNull.Value;
            else
			    row["m"] = m;

			if (this.checkBoxHH.Checked)
				row["H"] = "2";
			else
			{
				if (this.checkBoxH.Checked)
					row["H"] = "1";
				else
					row["H"] = DBNull.Value;
			}

            row.AcceptChanges();

			//写入新的版本号
			if (this.dataTableV.Rows.Count == 0)
			{
				DataRow r = this.dataTableV.NewRow();
				r[0] = this.textBoxVersion.Text;
				this.dataTableV.Rows.Add(r);
			}
			else
				this.dataTableV.Rows[0][0] = this.textBoxVersion.Text;

            try
            {
                if (this.textBoxImg1.Text != "")
                {
                    File.Copy(this.textBoxImg1.Text, Application.StartupPath + "/" + this.textBoxType.Text + "/" + this.textBoxA.Text.Substring(0,2)+"/" + this.textBoxA.Text + "_01.png");
                    File.Copy(this.textBoxImg2.Text, Application.StartupPath + "/" + this.textBoxType.Text + "/" + this.textBoxA.Text.Substring(0, 2) + "/" + this.textBoxA.Text + "_02.png");
                }
            }
            catch
            {
                MessageBox.Show("图片拷贝错误");
            }

            this.Close();
        }

        private void FormEdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.dataSet1.WriteXml(Application.StartupPath + "/Update-" + this.textBoxType.Text + ".db");
            this.dataSet1.WriteXml(Application.StartupPath + "/Update-" + this.textBoxType.Text + ".xml");
            this.dataSet1.WriteXml(Application.StartupPath + "/../../Update-" + this.textBoxType.Text + ".xml");
            this.dataSet1.WriteXml(Application.StartupPath + "/../Release/Update-" + this.textBoxType.Text + ".xml");
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxInfo_TextChanged(object sender, EventArgs e)
        {
             string[] ss = this.textBoxInfo.Text.Split('\n');

            foreach (string s in ss)
            {
					 if (s.StartsWith("N=")) this.textBoxN.Text = s.Substring(2).Replace("\r","");
                else if (s.StartsWith("C=")) this.textBoxC.Text = s.Substring(2).Replace("\r", "");
                else if (s.StartsWith("L=")) this.textBoxL.Text = s.Substring(2).Replace("\r", "");
                else if (s.StartsWith("I=")) this.textBoxI.Text = s.Substring(2).Replace("\r", "");
                else if (s.StartsWith("Y"))  this.textBoxY.Text = s.Substring(2).Replace("\r", "");
                else if (s.StartsWith("X=")) this.textBoxX.Text = s.Substring(2).Replace("\r", "");
                else if (s.StartsWith("E=")) this.textBoxE.Text = s.Substring(2).Replace("\r", "");
                else if (s.StartsWith("T=")) this.textBoxT.Text = s.Substring(2).Replace("\r", "");
            }

            this.textBoxInfo.Text = null;
          
        }

        private void FormEdit_Load(object sender, EventArgs e)
        {
			int m=2;

			DataRow[] rows = this.dataTableR.Select("A='" + this.textBoxA.Text + "'");
			if (rows.Length > 0)
			{
				try
				{
					m = (int)rows[0]["m"];
				}
				catch { }

                if (!rows[0].IsNull("i"))
                {
                    byte[] b = Convert.FromBase64String(rows[0]["i"].ToString());
                    MemoryStream stream = new MemoryStream(b);
                    this.pictureBoxIcon.Image = new Bitmap(stream);
                }
            }

            m = Math.Abs(m);

			for (int i = 1; i <= m; i++)
			{
				try
				{
					this.imageList1.Images.Add("k" + i, Tools.GetImage(Application.StartupPath + "/" + this.textBoxType.Text + "/"+this.textBoxA.Text.Substring(0,2)+"/" + this.textBoxA.Text + "_" + i.ToString("00") + ".png"));
					this.listViewImage.Items.Add(i.ToString("00"), "k" + i);
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}

			this.numericUpDown1.Value = this.listViewImage.Items.Count + 1;
        }

        private void _Validated(object sender, EventArgs e)
        {
            this.textBoxUnInfo.Text =
                "Y=" + this.textBoxY.Text + "\r\n" +
                "L=" + this.textBoxL.Text + "\r\n" +
                "C=" + this.textBoxC.Text + "\r\n" +
                "X=" + this.textBoxX.Text + "\r\n" +
                "N=" + this.textBoxN.Text + "\r\n" +
                "E=" + this.textBoxE.Text + "\r\n" +
                "T=" + this.textBoxT.Text + "\r\n" +
                "I=" + this.textBoxI.Text;
                
        }

        private void textBoxOffline_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(this.textBoxOffline.Text);

                int picn = int.Parse(xd.SelectSingleNode("/game/imageNumber").InnerText);
               	if (picn > 2500 && this.textBoxType.Text=="GBA")
				{
					this.textBoxImg1.Text = @"D:\OfflineList 0.7.2\imgs\GBA - Official OfflineList\2501-3000/" + xd.SelectSingleNode("/game/imageNumber").InnerText + "a.png";
					this.textBoxImg2.Text = @"D:\OfflineList 0.7.2\imgs\GBA - Official OfflineList\2501-3000/" + xd.SelectSingleNode("/game/imageNumber").InnerText + "b.png";
				}
				else if(picn<=2500)
				{
					this.textBoxImg1.Text = @"D:\OfflineList 0.7.2\imgs\ADVANsCEne Nintendo DS Collection\2001-2500/" + xd.SelectSingleNode("/game/imageNumber").InnerText + "a.png";
					this.textBoxImg2.Text = @"D:\OfflineList 0.7.2\imgs\ADVANsCEne Nintendo DS Collection\2001-2500/" + xd.SelectSingleNode("/game/imageNumber").InnerText + "b.png";
				}
				else
				{
					this.textBoxImg1.Text = @"D:\OfflineList 0.7.2\imgs\ADVANsCEne Nintendo DS Collection\2501-3000/" + xd.SelectSingleNode("/game/imageNumber").InnerText + "a.png";
					this.textBoxImg2.Text = @"D:\OfflineList 0.7.2\imgs\ADVANsCEne Nintendo DS Collection\2501-3000/" + xd.SelectSingleNode("/game/imageNumber").InnerText + "b.png";
				}

                this.textBoxE.Text = xd.SelectSingleNode("/game/title").InnerText;
                this.textBoxI.Text = "Source:" + xd.SelectSingleNode("/game/sourceRom").InnerText + "|";
                this.textBoxI.Text += xd.SelectSingleNode("/game/saveType").InnerText;
                this.textBoxC.Text = xd.SelectSingleNode("/game/publisher").InnerText;
				if(picn>2500 && this.textBoxType.Text=="GBA")
					this.textBoxX.Text = xd.SelectSingleNode("/game/releaseNumber").InnerText;
				else
					this.textBoxX.Text = xd.SelectSingleNode("/game/comment").InnerText;

                int y = int.Parse(xd.SelectSingleNode("/game/language").InnerText);

				string yy = "";
				if ((y & 0x00000001) > 0) yy += ".FR";//发文
				if ((y & 0x00000002) > 0) yy += ".EN";//英文
				if ((y & 0x00000004) > 0) MessageBox.Show("no language");
				if ((y & 0x00000008) > 0) yy += ".DA";//丹麦语
				if ((y & 0x00000010) > 0) yy += ".DU";//荷兰语
				if ((y & 0x00000020) > 0) MessageBox.Show("no language");
				if ((y & 0x00000040) > 0) yy += ".DE";//德文
				if ((y & 0x00000080) > 0) yy += ".IT";//意大利
				if ((y & 0x00000100) > 0) yy += ".JP";//日文
				if ((y & 0x00000200) > 0) yy += ".NO";//挪威文
				if ((y & 0x00000400) > 0) MessageBox.Show("no language");
				if ((y & 0x00000800) > 0) MessageBox.Show("no language");
				if ((y & 0x00001000) > 0) yy += ".ES";//西班牙
				if ((y & 0x00002000) > 0) yy += ".SW";//瑞典
				if ((y & 0x00004000) > 0) MessageBox.Show("no language");
				if ((y & 0x00008000) > 0) MessageBox.Show("no language");
				if ((y & 0x00010000) > 0) yy += ".KR";//韩国

				this.textBoxL.Text = yy.Substring(1);

                this.textBoxOffline.Text = null;
				if(this.numericUpDown1.Value==1) this.numericUpDown1.Value = 3;
            }
            catch { }
        }

		private void buttonBrowseImage_Click(object sender, EventArgs e)
		{
			if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
				this.textBoxImageFileName.Text = this.openFileDialog1.FileName;
		}

		private void buttonAddImage_Click(object sender, EventArgs e)
		{
			try
			{
                File.Copy(this.textBoxImageFileName.Text, Application.StartupPath + "/" + this.textBoxType.Text + "/" + this.textBoxA.Text.Substring(0,2)+"/"+ this.textBoxA.Text + "_" + this.numericUpDown1.Value.ToString("00") + ".png");
				this.imageList1.Images.Add("k" + this.numericUpDown1.Value,Tools.GetImage(Application.StartupPath + "/" + this.textBoxType.Text + "/"+this.textBoxA.Text.Substring(0,2)+"/" + this.textBoxA.Text + "_" + this.numericUpDown1.Value.ToString("00") + ".png"));
				this.listViewImage.Items.Add(this.numericUpDown1.Value.ToString("00"), "k" + this.numericUpDown1.Value);
				this.numericUpDown1.Value++;

				File.Delete(this.textBoxImageFileName.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonDeleteImage_Click(object sender, EventArgs e)
		{
			if (this.listViewImage.SelectedItems.Count == 0)
				return;

			//删除文件
			File.Delete(Application.StartupPath+"/"+this.textBoxType.Text+"/"+this.textBoxA.Text.Substring(0,2)+"/"+this.textBoxA.Text+"_"+this.listViewImage.SelectedItems[0].Text+".png");

			//删除列表中的图标
			this.listViewImage.SelectedItems[0].Remove();
		}

        private void pictureBoxIcon_DoubleClick(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.pictureBoxIcon.Image = Tools.GetImage(this.openFileDialog1.FileName);
            }
        }

        private void buttonDeleteIcon_Click(object sender, EventArgs e)
        {
            this.pictureBoxIcon.Image = null;
         //   File.Delete(Application.StartupPath + "/" + this.textBoxType.Text + "/" + this.textBoxA.Text.Substring(0, 2) + "/" + this.textBoxA.Text + "_00.png");
        }

        private void buttonGetIcon_Click(object sender, EventArgs e)
        {
            Bitmap bmp = Tools.GetIcon(this.textBoxF.Text);
            this.pictureBoxIcon.Image = bmp;
          //  bmp.Save(Application.StartupPath + "/" + this.textBoxType.Text + "/" + this.textBoxA.Text.Substring(0, 2) + "/" + this.textBoxA.Text + "_00.png");
        }


    }
}