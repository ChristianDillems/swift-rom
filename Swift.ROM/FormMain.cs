using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace Swift.ROM
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            //��ʼ���ȴ�����
         //   this.fw = new FormWait();
        }

        /// <summary>
        /// �ȴ�����
        /// </summary>
      //  private FormWait fw;

        private String sortColumn;

		/// <summary>
		/// ��ǰ�汾�ţ������������ݰ汾��
		/// </summary>
		private string nowVersion;

        public String nowType=null;

        /// <summary>
        /// �Ƿ��Զ�����ͼƬ�ı��
        /// </summary>
        private Boolean autoDownload;
        /// <summary>
        /// �Զ�����ͼƬʱ���Ƿ�����������չͼƬ
        /// </summary>
        private Boolean autoDownloadOther;
		private Boolean autoVerify;
		private Boolean UseLevel;

        private FormDownload fd;

		private Thread verifyThread;

        #region �������ļ�������
        private void update(string type)
        {
            //�ж������ļ��Ƿ���ڣ������������ֱ���˳�
            if (!File.Exists(Application.StartupPath + "/Update-" + type + ".xml"))
                return;

			this.waitThreadStop();

            XmlDocument xd = new XmlDocument();
            XmlElement xe;
            DataRow row;

            //��¼�������µĸ���ʱ��
            DateTime ndt=new DateTime(2000,1,1);

            //��ȡ���µĸ�������
            DateTime udt = new DateTime(2000, 1, 1);
            if (File.Exists(Application.StartupPath + "/Swift.ROM.xml"))
            {
                xd.Load(Application.StartupPath + "/Swift.ROM.xml");
                xe = (XmlElement)xd.SelectSingleNode("/ROM/"+type);
				if (xe.GetAttribute("u") != "")
					ndt = udt = DateTime.Parse(xe.GetAttribute("u"));
            }

            this.dataTableR.Rows.Clear();

            if (File.Exists(Application.StartupPath + "/" + type + "/ROM.xml"))
                this.dataSet.ReadXml(Application.StartupPath + "/" + type + "/ROM.xml");
            xd.Load(Application.StartupPath + "/Update-" + type + ".xml");

            this.labelMessage.Text = "����" + type + "�����ļ�����������......";
			this.toolStripProgressBar.Visible = true;
            this.toolStripProgressBar.Maximum = xd.SelectSingleNode("/dataSet").ChildNodes.Count;
            this.toolStripProgressBar.Value = 0;
            Application.DoEvents();

            foreach (XmlNode xn in xd.SelectSingleNode("/dataSet").ChildNodes)
            {
				if(this.toolStripProgressBar.Value<this.toolStripProgressBar.Maximum)
					this.toolStripProgressBar.Value++;

                xe = (XmlElement)xn;

				if (xe.Name == "Version")
				{
					Debug.WriteLine("�����İ汾��:" + xe.GetAttribute("Version"));

					string[] tv = xe.GetAttribute("Version").Split('.');
					string[] vv=Application.ProductVersion.Split('.');
					if (tv.Length >= 4)
					{ 
						try
						{
							int t1=int.Parse(tv[0]);
							int t2=int.Parse(tv[1]);
							int t3=int.Parse(tv[2]);
							int t4=int.Parse(tv[3]);

							int v1=int.Parse(vv[0]);
							int v2=int.Parse(vv[1]);
							int v3=int.Parse(vv[2]);
							int v4=int.Parse(vv[3]);

							if(t1>=v1 && t2>=v2 && t3>=v3 && t4>=v4)
								this.nowVersion = xe.GetAttribute("Version");
						}
							catch{}
					}
					continue;
				}
               
                //�жϸ��������Ƿ����µģ���������µ���ֱ����������
                if (xe.GetAttribute("u") == "")
                    continue;
                else
                    if (DateTime.Parse(xe.GetAttribute("u")) <= udt)
                        continue;

                if (DateTime.Parse(xe.GetAttribute("u")) > ndt)
                    ndt = DateTime.Parse(xe.GetAttribute("u"));

				DataRow[] rows = this.dataTableR.Select("A='" + xe.GetAttribute("A") + "'");
                if (rows.Length == 0)
                {
					rows = this.dataTableR.Select("I='" + xe.GetAttribute("A") + "'");
					if (rows.Length == 0)
					{
						row = this.dataTableR.NewRow();
						this.dataTableR.Rows.Add(row);
					}
					else
					{
						row = rows[0];
					}
                }
                else
                {
					row = rows[0];
                }

				row["A"] = xe.GetAttribute("A") == "" ? null : xe.GetAttribute("A"); ;
				row["X"] = xe.GetAttribute("X") == "" ? null : xe.GetAttribute("X"); ;
				row["N"] = xe.GetAttribute("N") == "" ? null : xe.GetAttribute("N"); ;
				row["E"] = xe.GetAttribute("E") == "" ? null : xe.GetAttribute("E"); ;
				row["T"] = xe.GetAttribute("T") == "" ? null : xe.GetAttribute("T"); ;
				row["L"] = xe.GetAttribute("L") == "" ? null : xe.GetAttribute("L"); ;
				row["Y"] = xe.GetAttribute("Y") == "" ? null : xe.GetAttribute("Y"); ;
				row["S"] = xe.GetAttribute("S") == "" ? null : xe.GetAttribute("S"); ;
				row["C"] = xe.GetAttribute("C") == "" ? null : xe.GetAttribute("C"); ;
				row["I"] = xe.GetAttribute("I") == "" ? null : xe.GetAttribute("I"); ;
				row["H"] = xe.GetAttribute("H") == "" ? null : xe.GetAttribute("H");
				if (xe.GetAttribute("m") == "" || xe.GetAttribute("m") == "2")
					row["m"] = DBNull.Value;
				else
					row["m"] = xe.GetAttribute("m");
				row.AcceptChanges();

                Application.DoEvents();
            }

            //���¼���δʶ���ROM
			//this.labelMessage.Text = "����ʶ����ǰδʶ���ROM";
			//this.toolStripProgressBar.Maximum = this.dataTableR.Select("A is null").Length;
			//this.toolStripProgressBar.Value = 0;
			//Application.DoEvents();
			//foreach (DataRow r in this.dataTableR.Select("A is null"))
			//{
			//    string sha1;
			//    if (r["I"].ToString() == "")
			//    {
			//        sha1 = Tools.GetFileSHA1(r["f"].ToString());
			//        r["I"] = sha1;
			//        r.AcceptChanges();
			//    }
			//    else
			//        sha1 = r["I"].ToString();

			//    DataRow[] rows = this.dataTableR.Select("A='" + sha1 + "'");
			//    if (rows.Length > 0)
			//    {
			//        rows[0]["f"] = r["f"];
			//        r.Delete();
			//    }

			//    this.toolStripProgressBar.Value++;
			//}
            this.dataTableR.AcceptChanges();
            this.dataSet.WriteXml(Application.StartupPath + "/" + type + "/ROM.xml");

            //ɾ�������ļ�
            File.Delete(Application.StartupPath + "/Update-" + type + ".xml");

            //�洢����ʱ��
            xd.Load(Application.StartupPath + "/Swift.ROM.xml");
            xe = (XmlElement)xd.SelectSingleNode("/ROM/" + type);
            xe.SetAttribute("u", ndt.ToString());
			//xe.SetAttribute("version", this.nowVersion);
            xd.Save(Application.StartupPath + "/Swift.ROM.xml");

			this.toolStripProgressBar.Visible = false;
            this.labelMessage.Text = "Ready.";
        }
        #endregion

		private void FormMain_Load(object sender, EventArgs e)
        {
			//ɾ����ʱ�ļ�
			try
			{
				if (Directory.Exists(Application.StartupPath + @"\TEMP"))
					Directory.Delete(Application.StartupPath + @"\TEMP", true);
			}
			catch 
			{
				Debug.WriteLine("ɾ��/tempĿ¼ʱ����!");
			}

			//�����ش���
            fd = new FormDownload(this.toolStripStatusLabelDownload);
#if !DEBUG
            this.miRom.Visible = false;
			Application.DoEvents();
#endif
            FormWait fw = new FormWait();
            fw.Show();
            this.labelMessage.Text = "���ڽ��г�ʼ������......";
            this.Show();
            Application.DoEvents();

            //�ж�����Ŀ¼�Ƿ���ڣ��������������
			if (!Directory.Exists(Application.StartupPath + "/NES")) Directory.CreateDirectory(Application.StartupPath + "/NES");
            if (!Directory.Exists(Application.StartupPath + "/GBA")) Directory.CreateDirectory(Application.StartupPath + "/GBA");
            if (!Directory.Exists(Application.StartupPath + "/NDS")) Directory.CreateDirectory(Application.StartupPath + "/NDS");

            //��ȡ�����ļ�
            XmlDocument xd = new XmlDocument();
            XmlElement xe;

			//��������ļ������ڣ������´���һ��
			if (!File.Exists(Application.StartupPath + "/Swift.ROM.xml"))
			{
				xd = new XmlDocument();
				xd.AppendChild((XmlNode)xd.CreateElement("ROM"));
				xe = (XmlElement)xd.SelectSingleNode("/ROM");
				xe.SetAttribute("sort", "X");
				xe.SetAttribute("nowType", "NDS");
				xd.Save(Application.StartupPath + "/Swift.ROM.xml");
			}

			this.nowVersion = Application.ProductVersion;

			//��ȡ���Ի���Ϣ�ļ�
			string nType = null;
			xd.Load("Swift.ROM.xml");
			if (xd.SelectSingleNode("/ROM") != null)
			{
				xe = (XmlElement)xd.SelectSingleNode("/ROM");

				this.autoDownload = (xe.GetAttribute("AutoDownload") == "0") ? false : true;
                this.autoDownloadOther = (xe.GetAttribute("AutoDownloadOther") == "0") ? false : true;
				this.autoVerify = (xe.GetAttribute("AutoVerify") == "0") ? false : true;
				this.UseLevel = (xe.GetAttribute("UseLevel") == "0") ? false : true;
				this.miShowT0.Checked = (xe.GetAttribute("miShowT0") == "1") ? true : false;
				this.miShowT1.Checked = (xe.GetAttribute("miShowT1") == "1") ? true : false;
				this.miShowT2.Checked = (xe.GetAttribute("miShowT2") == "1") ? true : false;
				this.nowVersion = (xe.GetAttribute("dataVersion") == null) ? Application.ProductVersion : xe.GetAttribute("dataVersion");

				//�жϰ汾
				Debug.WriteLine("�����İ汾��:" + this.nowVersion);
				string[] tv =this.nowVersion.Split('.');
				string[] vv = Application.ProductVersion.Split('.');
				if (tv.Length >= 4)
				{
					try
					{
						int t1 = int.Parse(tv[0]);
						int t2 = int.Parse(tv[1]);
						int t3 = int.Parse(tv[2]);
						int t4 = int.Parse(tv[3]);

						int v1 = int.Parse(vv[0]);
						int v2 = int.Parse(vv[1]);
						int v3 = int.Parse(vv[2]);
						int v4 = int.Parse(vv[3]);

						if (!(t1 >= v1 && t2 >= v2 && t3 >= v3 && t4 >= v4))
							this.nowVersion = Application.ProductVersion;
					}
					catch { }
				}
				else
					this.nowVersion = Application.ProductVersion;
				

				//��ȡ������
				this.sortColumn = xe.GetAttribute("sort");
				if (this.sortColumn == null) this.sortColumn = "X";

				nType = xe.GetAttribute("nowType");

				//������ͼ���п�
				foreach (ColumnHeader ch in this.listView.Columns)
				{
					try
					{
						ch.Width = int.Parse(xe.GetAttribute("lvcw" + ch.Index));
					}
					catch { }
				}

				// TODO ��δ���2009��1��ɾ��
				//�鿴��ǰ�Ƿ�ʹ�õľɰ������ļ�����������������
				if (xd.SelectSingleNode("/ROM/NES") != null)
				{
					if (((XmlElement)xd.SelectSingleNode("/ROM/NES")).GetAttribute("Emu") != "")
					{
						string emu = ((XmlElement)xd.SelectSingleNode("/ROM/NES")).GetAttribute("Emu").Replace("<swift.rom>", Application.StartupPath);
						xd.SelectSingleNode("/ROM/NES").AppendChild((XmlNode)xd.CreateElement("EMU"));
						((XmlElement)xd.SelectSingleNode("/ROM/NES/EMU")).SetAttribute("Name",System.IO.Path.GetFileName(emu).Split('.')[0]);
						((XmlElement)xd.SelectSingleNode("/ROM/NES/EMU")).SetAttribute("File", emu.Replace(Application.StartupPath,"<swift.rom>"));
						((XmlElement)xd.SelectSingleNode("/ROM/NES/EMU")).SetAttribute("Default", "Default");
						((XmlElement)xd.SelectSingleNode("/ROM/NES")).RemoveAttribute("Emu");
					}
				}
				if (xd.SelectSingleNode("/ROM/GBA") != null)
				{
					if (((XmlElement)xd.SelectSingleNode("/ROM/GBA")).GetAttribute("Emu") != "")
					{
						string emu = ((XmlElement)xd.SelectSingleNode("/ROM/GBA")).GetAttribute("Emu").Replace("<swift.rom>", Application.StartupPath);
						xd.SelectSingleNode("/ROM/GBA").AppendChild((XmlNode)xd.CreateElement("EMU"));
						((XmlElement)xd.SelectSingleNode("/ROM/GBA/EMU")).SetAttribute("Name",System.IO.Path.GetFileName(emu).Split('.')[0]);
						((XmlElement)xd.SelectSingleNode("/ROM/GBA/EMU")).SetAttribute("File", emu.Replace(Application.StartupPath,"<swift.rom>"));
						((XmlElement)xd.SelectSingleNode("/ROM/GBA/EMU")).SetAttribute("Default", "Default");
						((XmlElement)xd.SelectSingleNode("/ROM/GBA")).RemoveAttribute("Emu");
					}
				}
				if (xd.SelectSingleNode("/ROM/NDS") != null)
				{
					if (((XmlElement)xd.SelectSingleNode("/ROM/NDS")).GetAttribute("Emu") != "")
					{
						string emu = ((XmlElement)xd.SelectSingleNode("/ROM/NDS")).GetAttribute("Emu").Replace("<swift.rom>", Application.StartupPath);
						xd.SelectSingleNode("/ROM/NDS").AppendChild((XmlNode)xd.CreateElement("EMU"));
						((XmlElement)xd.SelectSingleNode("/ROM/NDS/EMU")).SetAttribute("Name", System.IO.Path.GetFileName(emu).Split('.')[0]);
						((XmlElement)xd.SelectSingleNode("/ROM/NDS/EMU")).SetAttribute("File", emu.Replace(Application.StartupPath, "<swift.rom>"));
						((XmlElement)xd.SelectSingleNode("/ROM/NDS/EMU")).SetAttribute("Default", "Default");
						((XmlElement)xd.SelectSingleNode("/ROM/NDS")).RemoveAttribute("Emu");
					}
				}
				//2009.1ɾ������

				//�ж������ļ��Ƿ��������������Ļ���������
				//nes
				if (xd.SelectSingleNode("/ROM/NES") == null)
					xd.SelectSingleNode("/ROM").AppendChild((XmlNode)xd.CreateElement("NES"));
				if (xd.SelectSingleNode("/ROM/NES/EMU") == null)
				{
					xd.SelectSingleNode("/ROM/NES").AppendChild((XmlNode)xd.CreateElement("EMU"));
					((XmlElement)xd.SelectSingleNode("/ROM/NES/EMU")).SetAttribute("Name", "Nestopia");
					((XmlElement)xd.SelectSingleNode("/ROM/NES/EMU")).SetAttribute("File", "<swift.rom>/Nestopia/nestopia.exe");
					((XmlElement)xd.SelectSingleNode("/ROM/NES/EMU")).SetAttribute("Default", "Default");
				}
				//gba
				if (xd.SelectSingleNode("/ROM/GBA") == null)
					xd.SelectSingleNode("/ROM").AppendChild((XmlNode)xd.CreateElement("GBA"));
				if (xd.SelectSingleNode("/ROM/GBA/EMU") == null)
				{
					xd.SelectSingleNode("/ROM/GBA").AppendChild((XmlNode)xd.CreateElement("EMU"));
					((XmlElement)xd.SelectSingleNode("/ROM/GBA/EMU")).SetAttribute("Name", "VisualBoyAdvance");
					((XmlElement)xd.SelectSingleNode("/ROM/GBA/EMU")).SetAttribute("File", "<swift.rom>/VisualBoyAdvance/VisualBoyAdvance.exe");
					((XmlElement)xd.SelectSingleNode("/ROM/GBA/EMU")).SetAttribute("Default", "Default");
				}
				//nds
				if (xd.SelectSingleNode("/ROM/NDS") == null)
					xd.SelectSingleNode("/ROM").AppendChild((XmlNode)xd.CreateElement("NDS"));
				if (xd.SelectSingleNode("/ROM/NDS/EMU") == null)
				{
					xd.SelectSingleNode("/ROM/NDS").AppendChild((XmlNode)xd.CreateElement("EMU"));
					((XmlElement)xd.SelectSingleNode("/ROM/NDS/EMU")).SetAttribute("Name", "desmume");
					((XmlElement)xd.SelectSingleNode("/ROM/NDS/EMU")).SetAttribute("File", "<swift.rom>/desmume/desmume.exe");
					((XmlElement)xd.SelectSingleNode("/ROM/NDS/EMU")).SetAttribute("Default", "Default");
				}

				xd.Save(Application.StartupPath + "/Swift.ROM.xml");
			}

            //����ROM��Ϣ
			this.update("NES"); Application.DoEvents();
			this.update("GBA"); Application.DoEvents();
			this.update("NDS"); Application.DoEvents();

			//��ʾ����汾
			this.Text += this.nowVersion;

            //��ʾROM����
			//NES
			this.dataTableR.Rows.Clear();
			if (File.Exists(Application.StartupPath + "/NES/ROM.xml"))
			{
				this.dataSet.ReadXml(Application.StartupPath + "/NES/ROM.xml");
				this.labelNES.Text = this.dataTableR.Select("A is not null and f is not null").Length.ToString() + "/" + this.dataTableR.Select("A is not null").Length.ToString();
			}
            //GBA
            this.dataTableR.Rows.Clear();
            if (File.Exists(Application.StartupPath + "/GBA/ROM.xml"))
            {
                this.dataSet.ReadXml(Application.StartupPath + "/GBA/ROM.xml");
                this.labelGBA.Text = this.dataTableR.Select("A is not null and f is not null").Length.ToString() + "/" + this.dataTableR.Select("A is not null").Length.ToString();
            }
            //NDS
            this.dataTableR.Rows.Clear();
            if (File.Exists(Application.StartupPath + "/NDS/ROM.xml"))
            {
                this.dataSet.ReadXml(Application.StartupPath + "/NDS/ROM.xml");
                this.labelNDS.Text = this.dataTableR.Select("A is not null and f is not null").Length.ToString() + "/" + this.dataTableR.Select("A is not null").Length.ToString();
            }

			//�ж���������������Ǽ������� zh_CN �����(������������)������ʾ������
			if (!Application.CurrentCulture.Name.StartsWith("zh"))
				this.listView.Columns[1].Width = 0;

            this.labelMessage.Text = "Ready.";

			//��ȡĬ����ʾ��ROM����
			switch (nType)
			{
				case "NES": this.miShowNES.PerformClick(); break;
				case "GBA": this.miShowGBA.PerformClick(); break;
				case "NDS": this.miShowNDS.PerformClick(); break;
				default: this.miShowNDS.PerformClick(); break;
			}

			this.timerState.Start();

			//����Ƿ����µİ汾������н�����ʾ
            if (((XmlElement)xd.SelectSingleNode("/ROM")).GetAttribute("AutoUpdate") != "0")
                this.miCheckUpdate.PerformClick();
            else
            {
                this.toolStripStatusLabelUpdate.Text = "Disabled upgrade.";
                this.toolStripStatusLabelUpdate.ForeColor = Color.Gray;
            }

            fw.Close();
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

		#region ���ڴ���
        private void miAbout_Click(object sender, EventArgs e)
        {
            FormAbout f = new FormAbout();
            f.ShowDialog();
		}
		#endregion

		private void miRefresh_Click(object sender, EventArgs e)
        {
            //ͬ���˵��͹�����
            this.tsbShowVerifyRom.Checked = this.miShowVerifyROM.Checked;
            this.tsbShowUnseenROM.Checked = this.miShowUnseenROM.Checked;
            this.tsbShowUnknowRom.Checked = this.miShowUnknowROM.Checked;
            this.tsbShowFavorite.Checked = this.miShowFavorite.Checked;
			this.tsbShowT0.Checked = this.miShowT0.Checked;
			this.tsbShowT1.Checked = this.miShowT1.Checked;
			this.tsbShowT2.Checked = this.miShowT2.Checked;

            //��ʾ�ȴ�����
            FormWait fw = new FormWait();
            fw.Show();
            this.labelMessage.Text = "����ˢ���б�...";
            Application.DoEvents();

			//�ȴ�ROM��֤�߳�ֹͣ����Ϊ��ֹͣ�Ļ����ܻ�������ݳ�ͻ
			this.waitThreadStop();

            //��ȡ��ǰѡ����,����ˢ�������ʾ���λ��
			string nowItem = (this.listView.SelectedItems.Count > 0) ? this.listView.SelectedItems[0].Tag.ToString() : null;

            this.listView.BeginUpdate();

            //���������Ϣ
            this.listView.Items.Clear();
            this.listView.Groups.Clear();

            //��������
            this.listView.ShowGroups = this.miViewGroups.Checked;
            if (this.miViewGroups.Checked)
            {
                if (this.miViewGroupC.Checked)  //��Ʒ��˾
                {
                    this.dataView.RowFilter = null;
                    this.dataView.Sort = "C asc";
                    string ty = null;
                    foreach (DataRowView row in this.dataView)
                    {
                        if (row["C"].ToString() == ty)
                            continue;
                        this.listView.Groups.Add(row["C"].ToString(),row["C"].ToString());
                    }
                }
                else if (this.miViewGroupYear.Checked)  //�������
                {
                    this.dataView.RowFilter = null;
                    this.dataView.Sort = "Y asc";
                    string ty = null;
                    foreach (DataRowView row in this.dataView)
                    {
                        string t;
                        if (row["Y"].ToString().Length >= 4)
                            t = row["Y"].ToString().Substring(0, 4);
                        else
                            t = row["Y"].ToString();

						if (t == ty) continue;
							
						this.listView.Groups.Add(t, t);
                    }
                }
            }

            //���������������
            string where = "";

            if (this.miShowVerifyROM.Checked) where += " or A is not null and f is not null";
            if (this.miShowUnseenROM.Checked) where += " or A is not null and f is null";
            if (this.miShowUnknowROM.Checked) where += " or A is null and f is not null";

			where = (where == "") ? "" : where.Substring(3);

			//�������Ҫ��������ROM����ʾ
			string tw = "";
			if (this.miShowT0.Checked) tw += " or H is null";// or f is not null";
			if (this.miShowT1.Checked) tw += " or H=1";
			if (this.miShowT2.Checked) tw += " or H=2";
			where = (tw == "") ? where : (where=="")?tw.Substring(3):"(" + where + ") and (" + tw.Substring(3) + ")";

			//�����ղؼ���ʾ
            if (this.miShowFavorite.Checked)
            {
                where += "(" + where + ") and F=1";
                where = where.Replace("() and", "");
            }

            Debug.WriteLine(where);

            this.dataView.RowFilter = where;
			//���������ѡ�����ѡ��
			//	�������Ϊ��ţ���������ʱͬ��ŵ���������������
			//					����Ӣ��ʱͬ��ŵ�����Ӣ��������
			if(sortColumn.Equals("X"))
				this.dataView.Sort = sortColumn+(Application.CurrentCulture.Name.StartsWith("zh")?",N":",E");
			else
				this.dataView.Sort = sortColumn;

			this.toolStripProgressBar.Visible = true;
            this.toolStripProgressBar.Maximum = this.dataView.Count;
            this.toolStripProgressBar.Value = 0;

            foreach (DataRowView row in this.dataView)
            {
				//��ListView��д����
				this.updateLVI(row.Row, true,false);

				if (this.toolStripProgressBar.Value < this.toolStripProgressBar.Maximum)
					this.toolStripProgressBar.Value++;
            }

			//ѡ����,����ˢ�������ʾ���λ��
			if (nowItem != null) 
			foreach(ListViewItem lvi in this.listView.Items)
				if (lvi.Tag.ToString() == nowItem)
				{
					lvi.Selected = true;
					this.listView.TopItem = lvi;
					break;
				}

            this.listView.EndUpdate();
            this.listView_SelectedIndexChanged(null, null);
			this.toolStripProgressBar.Visible = false;
            this.labelMessage.Text = "Ready.";

            //this.fw.Hide();
            //this.timerHideFw.Start();
            fw.Close();

			//�Զ���֤����ROM
			if (this.autoVerify)
				this.miVerifyROM_Click(null, null);
				//this.miVerifyROM.PerformClick();
		}
		
		public void updateLVI(DataRow row, Boolean insert,Boolean reSUM)
		{
			ListViewItem lvi=null;

			if (!insert)
				foreach (ListViewItem tlvi in this.listView.Items)
					if (tlvi.Tag.ToString() == row["A"].ToString() || tlvi.Tag.ToString()==row["I"].ToString())
					{
						lvi = tlvi;
						Application.DoEvents();
						break;
					}
			
			//���û���ҵ����ʵ�LVI,��Ҳ��Ϊ�����ӵ���Ŀ
			if (lvi == null)
			{
				lvi = new ListViewItem(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
				insert = true;
			}

			lvi.SubItems[0].Text = row["X"].ToString();
			lvi.SubItems[1].Text = row["N"].ToString();
			lvi.SubItems[2].Text = row["E"].ToString();
			lvi.SubItems[3].Text = row["L"].ToString();
			lvi.SubItems[4].Text = row["T"].ToString();
			lvi.SubItems[5].Text = row["Y"].ToString();
			lvi.SubItems[6].Text = row["S"].ToString();
			lvi.SubItems[7].Text = row["C"].ToString();
			lvi.SubItems[8].Text = row["I"].ToString();
			lvi.SubItems[9].Text = row["f"].ToString();

			if (row["A"] == DBNull.Value)
			{
				if (row["f"] == DBNull.Value)
					lvi.Remove();
				else
					lvi.ImageKey = row.IsNull("F") ? this.nowType + "1" : "FAV1";
			}
			else
			{
				if(row.IsNull("F"))
					lvi.ImageKey = (row["f"] == DBNull.Value) ? this.nowType + "3" : this.nowType + "2";
				else
					lvi.ImageKey = (row["f"] == DBNull.Value) ? "FAV3" : "FAV2";
			}

			if (this.miViewGroups.Checked)
			{
				if (this.miViewGroupC.Checked)   //��Ʒ��˾
				{
					lvi.Group = this.listView.Groups[row["C"].ToString()];
				}
				else if (this.miViewGroupYear.Checked)  //��Ʒ���
				{
					string t = (row["Y"].ToString().Length >= 4) ? row["Y"].ToString().Substring(0, 4) : row["Y"].ToString();
					lvi.Group = this.listView.Groups[t];
				}
			}

			lvi.Tag = row.IsNull("A")?row["I"]:row["A"];

			//�жϵ�ǰ��Ϣ�ĵȼ�
			//	Ĭ�Ϸ�Ϊ3���������õ�Ϊ��߼���1Ϊ�м伶��2Ϊ��ͼ�
			//	һ����˵����߼���Ϊ��׼��ϷROM���м伶��Ϊ�����������ƽ�ROM��2Ϊ�ͻ����Ե�ROM�����绵�Ļ������°汾�ģ�
			if (this.UseLevel)
			{
				if (row["H"].ToString() == "2")
					lvi.ForeColor = Color.LightGray;
				else if (row["H"].ToString() == "1")
					lvi.ForeColor = Color.Gray;
			}

			if(insert) this.listView.Items.Add(lvi);
			
		//	Application.DoEvents();

			//�������½ǵ�����
			if (reSUM)
			{
				//��ʾ�ҵ��µ�ROM
			//	if (!row.IsNull("f"))
			//	{
			//		ToolTip tt = new ToolTip();
			//		tt.IsBalloon = true;
			//		tt.Show("ʶ����ROM:" + row["E"].ToString(), this,this.toolStripStatusLabelState.Bounds.Location.X,this.toolStripStatusLabelState.Bounds.Location.Y+this.Height-70, 2000);
			//	}

				this.toolStripStatusLabelState.ToolTipText = row.IsNull("f")?null:row["f"].ToString();
				
				string ss = this.dataTableR.Select("A is not null and f is not null").Length.ToString() + "/" + this.dataTableR.Select("A is not null").Length.ToString();
				switch (this.nowType)
				{
					case "NES": this.labelNES.Text = ss; break;
					case "GBA": this.labelGBA.Text = ss; break;
					case "NDS": this.labelNDS.Text = ss; break;
				}
				this.labelUnknow.Text = this.dataTableR.Select("A is null and f is not null").Length.ToString();
			}
		}

        private void miSetup_Click(object sender, EventArgs e)
        {
			//��ʾ���ô���
            FormSetup f = new FormSetup();
            if (f.ShowDialog() == DialogResult.OK)
            {
                this.autoDownload = f.checkBoxAutoDownload.Checked;
                this.autoDownloadOther = f.checkBoxAutoDownloadOther.Checked;
				this.autoVerify = f.checkBoxAutoVerify.Checked;
				this.UseLevel = f.checkBoxUseLevel.Checked;

				this.miRefresh.PerformClick();
            }
        }

        private void tsbShowVerifyRom_Click(object sender, EventArgs e)
        {
            this.miShowVerifyROM.PerformClick();
        }

        private void tsbShowUnseenROM_Click(object sender, EventArgs e)
        {
            this.miShowUnseenROM.PerformClick();
        }

        private void tsbShowUnknowRom_Click(object sender, EventArgs e)
        {
            this.miShowUnknowROM.PerformClick();
        }

		private void miRomEdit_Click(object sender, EventArgs e)
		{
			if (this.listView.SelectedItems.Count == 0)
				return;

			DataRow[] rows = this.dataTableR.Select("A='" + this.listView.SelectedItems[0].Tag.ToString() + "'");
			if(rows.Length==0)
				rows=this.dataTableR.Select("A is null and I='" + this.listView.SelectedItems[0].Tag.ToString() + "'");
			if (rows.Length == 0)
			{
				Debug.WriteLine("û���ҵ�ָ����ROM����");
				return;
			}

			FormEdit f = new FormEdit(this.nowType, this.listView.SelectedItems[0].Tag.ToString(), rows[0]["E"].ToString() , rows[0]["S"].ToString() );
			if (f.ShowDialog() == DialogResult.OK)
			{
				//���浱ǰROM����
				this.dataSet.WriteXml(Application.StartupPath + @"\" + this.nowType + @"\ROM.xml");
				//����ROM����
				this.update(this.nowType);

				//this.miRefresh.PerformClick();
				rows=this.dataTableR.Select("A='"+this.listView.SelectedItems[0].Tag+"'");
				this.updateLVI(rows[0], false, true);
			}
		}

		/// <summary>
		/// ��ʼ������֤���е�ǰ�����ROM
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void miVerifyROM_Click(object sender, EventArgs e)
        {
			//��ʼ�߳�֮ǰ����ȷ��ԭ�����߳��Ѿ�������
			this.waitThreadStop();

			//����ROM��֤�߳�
			//	˵����ԭ��û�е������߳�����ROM��֤���������֤��ʱ��û�취������Ӧ��������������������߳��Ժ�ROMʶ��������ʹ�þͿ�����Է����ˡ�
			VerifyROM verifyROM = new VerifyROM(this);
			this.verifyThread = new Thread(new ThreadStart(verifyROM.Start));
			this.verifyThread.IsBackground = true;
			this.verifyThread.Priority = ThreadPriority.Lowest;
			this.verifyThread.Start();
        }//miVerifyROM_Click

		/// <summary>
		/// �ȴ�ROM��֤�߳̽���
		/// </summary>
		private void waitThreadStop()
		{
			if (this.verifyThread == null) return;

			//todo �����Ƿ�Ӧ��дһ������ֹͣ�̣߳�
			this.toolStripStatusLabelState.Text = "����ֹͣ��̨��֤...";

			this.verifyThread.Abort();
			while (this.verifyThread.ThreadState != System.Threading.ThreadState.Stopped)
				Application.DoEvents();
		}

		private void miOpen_Click(object sender, EventArgs e)
		{
			//���û��ROM��ѡ����ֱ�ӷ���
			if (this.listView.SelectedItems.Count == 0)
				return;

			string sha1 = this.listView.SelectedItems[0].Tag.ToString();
			DataRow row;
			if (this.dataTableR.Select("A='" + sha1 + "'").Length > 0)
				row = this.dataTableR.Select("A='" + sha1 + "'")[0];
			else
				row = this.dataTableR.Select("E='" + this.listView.SelectedItems[0].SubItems[2].Text.Replace("'","''") + "'")[0];

			//�жϵ�ǰ��Ϸ�Ƿ���ڣ������������ֱ���˳�
			if (row["f"].ToString() == "")
				return;

            FormWait fw = new FormWait();
			fw.Show();
			Application.DoEvents();

			Process proc = new Process();

			//ѡ��ģ����
			string emu = ((ToolStripMenuItem)sender).Tag.ToString().Replace("<swift.rom>", Application.StartupPath);
			if(!File.Exists(emu))
			{
				MessageBox.Show("ģ����·�����ô���", "��ʾ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			proc.StartInfo.FileName = emu;

			//���Ϊѹ���ļ�������Ҫ���н�ѹ
			string[] fs = row["f"].ToString().Split('?');
			if (fs.Length == 1)
				proc.StartInfo.Arguments = "\"" + row["f"].ToString() + "\"";
			else//Ϊѹ���ļ�
			{
				Tools.unZIP(fs[0], Application.StartupPath + @"\TEMP\PlayROM\");
				proc.StartInfo.Arguments = "\"" + Application.StartupPath+@"\TEMP\PlayROM\"+fs[1] + "\"";
			}

			try
			{
				proc.Start();
			}
			catch
			{
				MessageBox.Show("ģ��������", "��ʾ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

			//fw.Hide();
            fw.Close();
		}

		private void listView_SelectedIndexChanged(object sender, EventArgs e)
		{
			//���û��ROM��ѡ�����ͼ����Ϊ�գ�������
			if (this.listView.SelectedItems.Count == 0)
			{
				if(this.pictureBoxTitle.Image==null || this.pictureBoxTitle.ImageLocation!=null)
					this.pictureBoxTitle.ImageLocation = "http://sflogo.sourceforge.net/sflogo.php?group_id=192786&type=7";
				if(this.pictureBoxPIC.Image==null || this.pictureBoxPIC.ImageLocation!=null)
					this.pictureBoxPIC.ImageLocation = "http://images.sourceforge.net/images/project-support.jpg";

				this.pictureBoxTitle.Image = null;
				this.pictureBoxPIC.Image = null;
				this.labelInfo.Text = null;

				return;
			}

			ListViewItem lvi = this.listView.SelectedItems[0];

			if (lvi.Tag.ToString().Length == 0)
			{
				this.labelMessage.Text = "Ready.";
				return;
			}

			DataRow[] rows = this.dataTableR.Select("A='" + lvi.Tag.ToString() + "'");
			if (rows.Length == 0)
				rows = this.dataTableR.Select("I='" + lvi.Tag.ToString() + "'");

			this.labelMessage.Text = Path.GetFileName(rows[0]["f"].ToString());

			try
			{
				String fs = Application.StartupPath + @"\" + this.nowType + @"\" + lvi.Tag.ToString();
				
				if (File.Exists(fs + "_01.png"))
				{
					Image image = Tools.GetImage(fs + "_01.png");
					this.pictureBoxTitle.ImageLocation = null;
					this.pictureBoxTitle.Image = (Image)image.Clone();
					this.pictureBoxTitle.Tag = fs + "_01.png";
					this.groupBoxTitle.Width = this.pictureBoxTitle.Image.Width + 6;
					this.groupBoxTitle.Height = this.pictureBoxTitle.Image.Height + 20;
				}
				else
				{
					this.pictureBoxTitle.Image = null;
					this.pictureBoxTitle.ImageLocation = null;
					this.pictureBoxTitle.Tag = null;
					if (this.autoDownload && !rows[0].IsNull("A"))
						fd.download(this.nowType + "/" + lvi.Tag.ToString() + "_01.png");
				}

				if (File.Exists(fs + "_02.png"))
				{
					this.pictureBoxPIC.ImageLocation = null;
					this.pictureBoxPIC.Image =Tools.GetImage(fs + "_02.png");
					this.pictureBoxPIC.Tag = fs + "_02.png";
					this.groupBoxPIC.Width = this.pictureBoxPIC.Image.Width + 6;
					this.groupBoxPIC.Height = this.pictureBoxPIC.Image.Height + 20;
				}
				else
				{
					this.pictureBoxPIC.Image = null;
					this.pictureBoxPIC.ImageLocation = null;
					this.pictureBoxPIC.Tag = null;
					if (this.autoDownload && !rows[0].IsNull("A"))
						fd.download(this.nowType + "/" + lvi.Tag.ToString() + "_02.png");
				}

				//fill Other Image
				this.listViewOther.Items.Clear();
				this.imageListOther.Images.Clear();
				int m=2;
				if(!rows[0].IsNull("m"))
					m = (int)this.dataTableR.Select("A='" + lvi.Tag.ToString() + "'")[0]["m"];
				for (int i = 3; i <= m; i++)
				{
					if (File.Exists(fs + "_" + i.ToString("00") + ".png"))
					{
						try
						{
							this.imageListOther.Images.Add("k" + i, Tools.GetImage(fs + "_" + i.ToString("00") + ".png"));
							this.listViewOther.Items.Add("", "k" + i).Tag = fs + "_" + i.ToString("00") + ".png";
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex.Message);
						}
					}
					else
					{
                        //��� �����Զ����� && ��ʼ�Զ�������չͼƬ && ѡ���ROMΪ��¼ROM,��������չͼƬ
						if (this.autoDownload && this.autoDownloadOther && !rows[0].IsNull("A"))
							fd.download(this.nowType + "/" + lvi.Tag.ToString() + "_" + i.ToString("00") + ".png");
					}
				}
				//�ж��Ƿ�������ͼ�����û�����Other��ǩ�ص�
				if (listViewOther.Items.Count == 0)
				{
					this.tabControl.SelectTab("Info");
					this.tabControl.TabPages["Other"].Text = "Other";
				}
				else
				{
					this.tabControl.TabPages["Other"].Text = "Other(" + (m - 2) + ")";
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			//�ж���ʾ��ͼƬ��ʲô���ӵģ��������������в��ֵ���
			if (this.groupBoxPIC.Height < 300)
			{
				this.tabControl.Width = this.groupBoxPIC.Width + 20;
				this.groupBoxInfo.Width = this.groupBoxPIC.Width;
			}
			else
			{
				this.tabControl.Width = this.groupBoxPIC.Width + this.groupBoxTitle.Width + 26;
				this.groupBoxInfo.Width = this.groupBoxPIC.Width + this.groupBoxTitle.Width + 6;
			}

			this.miFavorite.Checked =!rows[0].IsNull("F");
            this.miSaveAsOriginal.Enabled = rows[0]["f"].ToString().Split('?').Length > 1;
			this.miReDownload.Enabled =!rows[0].IsNull("A");
			this.labelInfo.Text = rows[0]["I"].ToString().Replace('|', '\n');

		}

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
			//���浱ǰROM����
			this.dataSet.WriteXml(Application.StartupPath + "/"+this.nowType+"/ROM.xml");

            //������Ի�����
            XmlDocument xd = new XmlDocument();
            XmlElement xe;
            //�ж������ļ��Ƿ���ڣ������ڵĻ�����һ�����Ի������ļ�
            if (File.Exists(Application.StartupPath + "/Swift.ROM.xml"))
            {
                xd.Load(Application.StartupPath + "/Swift.ROM.xml");
            }
            else
            {
                xe = xd.CreateElement("ROM");
                xd.SelectSingleNode("/").AppendChild(xe);
            }
            xe = (XmlElement)xd.SelectSingleNode("/ROM");
            //������ͼ���п���Ϣ
            foreach (ColumnHeader ch in this.listView.Columns)
            {
                xe.SetAttribute("lvcw" + ch.Index, ch.Width.ToString());
            }
			
            xe.SetAttribute("sort", this.sortColumn);
            xe.SetAttribute("nowType", this.nowType);
			xe.SetAttribute("miShowT0", this.miShowT0.Checked ? "1" : "0");
			xe.SetAttribute("miShowT1", this.miShowT1.Checked ? "1" : "0");
			xe.SetAttribute("miShowT2", this.miShowT2.Checked ? "1" : "0");
			xe.SetAttribute("dataVersion", this.nowVersion);
            //xe.SetAttribute("miShowUnseenROM", this.miShowUnseenROM.Checked.ToString());
            //xe.SetAttribute("miShowUnknowROM", this.miShowUnknowROM.Checked.ToString());
            //xe.SetAttribute("miShowNDS", this.miShowNDS.Checked.ToString());
            //xe.SetAttribute("miShowNES", this.miShowNES.Checked.ToString());
            xd.Save(Application.StartupPath + "/Swift.ROM.xml");

			//ɾ����ʱ�ļ���
			try//�п�����Щ�ļ�����ʹ�ã���ʱ����ɾ���ͻᷢ�������������������Щ����
			{
				if (Directory.Exists(Application.StartupPath + @"\TEMP"))
					Directory.Delete(Application.StartupPath + @"\TEMP", true);
			}
			catch { }
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column.ToString() != sortColumn)
            {
                sortColumn = this.listView.Columns[e.Column].Tag.ToString();
                this.miRefresh.PerformClick();
            }

        }

        private void miViewGroupYear_Click(object sender, EventArgs e)
        {
            this.tsbViewGroupYear.Checked=this.miViewGroups.Checked=this.miViewGroupYear.Checked = !this.miViewGroupYear.Checked;
            this.tsbViewGroupC.Checked=this.miViewGroupC.Checked = false;
            this.miRefresh.PerformClick();
        }

        private void miViewGroupC_Click(object sender, EventArgs e)
        {
            this.tsbViewGroupC.Checked=this.miViewGroups.Checked = this.miViewGroupC.Checked = !this.miViewGroupC.Checked;
            this.tsbViewGroupYear.Checked=this.miViewGroupYear.Checked = false;
            this.miRefresh.PerformClick();
        }

        private void miViewList_Click(object sender, EventArgs e)
        {
            this.listView.View = View.List;
        }

        private void miViewDetails_Click(object sender, EventArgs e)
        {
            this.listView.View = View.Details;
        }

        private void miFavorite_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in this.listView.SelectedItems)
            {
				if (lvi.Tag.ToString() != "")
				{
					//ȡ��ָ��SHA1������
					DataRow[] rows=	this.dataTableR.Select("A='" + lvi.Tag.ToString() + "'");
					//�������û��ȡ�أ��ж��Ƿ��Ƿ�ʶ��ROM������
					if(rows.Length==0)
						rows=this.dataTableR.Select("I='" + lvi.Tag.ToString() + "'");
					//ȷ��������ȡ�غ�����ղؼд���
					if (rows.Length > 0)
					{
						if (this.miFavorite.Checked)
							rows[0]["F"] = 1;
						else
							rows[0]["F"] = DBNull.Value;
						this.updateLVI(rows[0], false, false);
					}
				}
            }
        }

        private void tsbShowFavorite_Click(object sender, EventArgs e)
        {
            this.miShowFavorite.PerformClick();
        }

        private void miFormDownload_Click(object sender, EventArgs e)
        {
            fd.Show();
        }

        private void miReDownload_Click(object sender, EventArgs e)
        {
			fd.download(this.nowType + "/" + this.listView.SelectedItems[0].Tag.ToString() + "_01.png");
            fd.download(this.nowType + "/" + this.listView.SelectedItems[0].Tag.ToString() + "_02.png");
        }

        private void miFind_Click(object sender, EventArgs e)
        {
            FormFind f = new FormFind();
            if (f.ShowDialog() == DialogResult.OK)
            {
                FormWait fw = new FormWait();
                fw.Show();
                foreach (ListViewItem lvi in this.listView.Items)
                {
                    if (lvi.SubItems[0].Text.IndexOf(f.textBoxKeyword.Text) != -1 || lvi.SubItems[1].Text.IndexOf(f.textBoxKeyword.Text) != -1 || lvi.SubItems[2].Text.IndexOf(f.textBoxKeyword.Text) != -1)
                    {
                        lvi.Selected = true;
                        this.listView.TopItem = lvi;
                    }
                }
                //fw.Hide();
                fw.Close();
            }
        }

        private void miShow_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;

            //����Ƿ��Ѿ�ѡ���������һ��ѡ���������ֱ�ӷ���
			if (mi.Checked) return;

			this.waitThreadStop();	//������������߳��ڹ�������ȴ�����߳�ֹͣ���ټ���

			//���浱ǰ��ROM��Ϣ
			//	�ڻ�����ʱ����һ����һ�����͵�����
			if(this.nowType!=null)
				this.dataSet.WriteXml(Application.StartupPath + "/" + this.nowType + "/ROM.xml");

			this.miShowNES.Checked = this.tsbShowNES.Checked = false;
			this.miShowGBA.Checked = this.tsbShowGBA.Checked = false;
            this.miShowNDS.Checked = this.tsbShowNDS.Checked = false;

            mi.Checked = true;

            this.nowType = mi.Tag.ToString();

			//��ģ������ַȡ��
			this.miOpen.Tag = null;
			this.miPlayBy.DropDownItems.Clear();
			XmlDocument xd = new XmlDocument();
			xd.Load(Application.StartupPath + "/Swift.ROM.xml");
			foreach (XmlNode node in xd.SelectSingleNode("/ROM/" + this.nowType))
				if (node.Name == "EMU")
				{
					XmlElement xe = (XmlElement)node;
					ToolStripMenuItem tsi = new ToolStripMenuItem(xe.GetAttribute("Name"), null, new EventHandler(miOpen_Click));
					tsi.Tag = xe.GetAttribute("File");
					if (xe.GetAttribute("Default") == "Default")
					{
						this.miOpen.Tag = xe.GetAttribute("File");
						tsi.Font = new Font(tsi.Font, FontStyle.Bold);
					}
					this.miPlayBy.DropDownItems.Add(tsi);
				}
			this.miOpen.Enabled = this.miOpen.Tag != null;
			this.miPlayBy.Enabled = this.miPlayBy.DropDownItems.Count>0;

            switch(this.nowType)
            {
				case "NES": this.tsbShowNES.Checked = true; break;
				case "GBA": this.tsbShowGBA.Checked = true; break;
				case "NDS": this.tsbShowNDS.Checked = true; break;
            }
            this.dataTableR.Rows.Clear();
            if (File.Exists(Application.StartupPath + "/"+this.nowType+"/ROM.xml"))
                this.dataSet.ReadXml(Application.StartupPath + "/"+this.nowType+"/ROM.xml");

            this.labelUnknow.Text = this.dataTableR.Select("A is null and f is not null").Length.ToString();

            //ˢ���б�
            this.miRefresh.PerformClick();
        }

        private void tsbShowGBA_Click(object sender, EventArgs e)
        {
			//Ϊ�˼�ⰴť״̬��������ô����
            this.miShowGBA.PerformClick();
        }

        private void tsbShowNDS_Click(object sender, EventArgs e)
        {
            this.miShowNDS.PerformClick();
        }

        private void miSaveAsOriginal_Click(object sender, EventArgs e)
        {
            //�ж�һ����û��ѡ�����Ŀ�����û����ֱ�ӷ���
            if (this.listView.SelectedItems.Count == 0)
                return;

            //ȡ���ļ�����
            string sha1 = this.listView.SelectedItems[0].Tag.ToString();
            DataRow[] rows=this.dataTableR.Select("A='" + sha1 + "'");
			if(rows.Length == 0)
				rows=this.dataTableR.Select("I='" + sha1 + "'");

			if (rows.Length == 0)
				return;

			string fp = rows[0]["f"].ToString().Split('?')[0];

			this.saveFileDialog1.FileName = Path.GetFileName(fp);
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy(fp, this.saveFileDialog1.FileName);
                }
                catch { }
            }
        }

		private void listViewOther_DoubleClick(object sender, EventArgs e)
		{
			if (this.listViewOther.SelectedItems.Count == 0)
				return;

			FormShowImage f = new FormShowImage();
			f.pictureBox1.Image = Image.FromFile(this.listViewOther.SelectedItems[0].Tag.ToString());
			f.Show();
		}

		private void miProjectWeb_Click(object sender, EventArgs e)
		{
			Tools.Start("http://sourceforge.net/projects/swift-rom");
		}

		private void miCopy_Click(object sender, EventArgs e)
		{
			if (this.listView.SelectedItems.Count == 0)
				return;

			try
			{
				Clipboard.SetText(this.listView.SelectedItems[0].SubItems[2].Text);
			}
			catch { }
		}

		private int verifyThreadImg = 0;

		private void timerState_Tick(object sender, EventArgs e)
		{
			if (this.verifyThread == null)
				return;

			this.miVerifyROM.Enabled = (this.verifyThread.ThreadState == System.Threading.ThreadState.Stopped);
			this.miStopVerifyROM.Enabled = !this.miVerifyROM.Enabled;

			if (this.verifyThread.ThreadState == System.Threading.ThreadState.Stopped)
			{
				this.toolStripStatusLabelState.Text = null;
				this.toolStripStatusLabelState.ToolTipText = null;
				this.toolStripStatusLabelState.Image = global::Swift.ROM.Properties.Resources.vr0;
				this.verifyThreadImg = 0;
				this.timerState.Interval = 1000;
			}
			else
			{
				if (this.verifyThreadImg == 0)
				{
					this.toolStripStatusLabelState.Text = "Verifying ROM...";
					this.timerState.Interval = 100;
				}

				switch (verifyThreadImg)
				{
					case 1:this.toolStripStatusLabelState.Image = global::Swift.ROM.Properties.Resources.vr1;break;
					case 2:this.toolStripStatusLabelState.Image = global::Swift.ROM.Properties.Resources.vr2;break;
					case 3:this.toolStripStatusLabelState.Image = global::Swift.ROM.Properties.Resources.vr3;break;
					case 4:this.toolStripStatusLabelState.Image = global::Swift.ROM.Properties.Resources.vr4;break;
					case 5:this.toolStripStatusLabelState.Image = global::Swift.ROM.Properties.Resources.vr5;break;
					case 6:this.toolStripStatusLabelState.Image = global::Swift.ROM.Properties.Resources.vr6;break;
				}
				if (verifyThreadImg++ >= 6) verifyThreadImg = 1;
			}
		}

		private void miStopVerifyROM_Click(object sender, EventArgs e)
		{
			this.waitThreadStop();
		}

		private void miSubmitBug_Click(object sender, EventArgs e)
		{
			Tools.Start("http://sourceforge.net/tracker/?func=add&group_id=192786&atid=942798");
		}

		private void miImageSaveAs_Click(object sender, EventArgs e)
		{
			if(this.pictureBoxTitle.Tag==null)
				return;

			this.saveFileDialog1.FileName = Path.GetFileName( this.pictureBoxTitle.Tag.ToString());
			if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				File.Copy(this.pictureBoxTitle.Tag.ToString(),this.saveFileDialog1.FileName);
			}
		}

		private void miImageSaveAsgame_Click(object sender, EventArgs e)
		{
			if (this.pictureBoxPIC.Tag == null)
				return;

			this.saveFileDialog1.FileName = Path.GetFileName(this.pictureBoxPIC.Tag.ToString());
			if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				File.Copy(this.pictureBoxPIC.Tag.ToString(), this.saveFileDialog1.FileName);
			}
		}

		private void tsbShowNES_Click(object sender, EventArgs e)
		{
			this.miShowNES.PerformClick();
		}

		private void pictureBoxTitle_Click(object sender, EventArgs e)
		{
			if (this.pictureBoxTitle.ImageLocation != null)
				Tools.Start("http://sourceforge.net");
		}

		private void pictureBoxPIC_Click(object sender, EventArgs e)
		{
			if (this.pictureBoxPIC.ImageLocation != null)
				Tools.Start("http://sourceforge.net/donate/index.php?group_id=192786");
		}

		private void miCheckUpdate_Click(object sender, EventArgs e)
		{
			this.toolStripStatusLabelUpdate.Text = "Check Update...";
			this.toolStripStatusLabelUpdate.ForeColor = Color.Gray;
			Application.DoEvents();

			CheckUpdate checkUpdate = new CheckUpdate();
            checkUpdate.nowVersion = this.nowVersion.Substring(0, 9) ;
			Thread t = new Thread(new ThreadStart(checkUpdate.checkUpdate));
			t.IsBackground = true;
			t.Priority = ThreadPriority.Lowest;
			t.Start();

			while (t.ThreadState != System.Threading.ThreadState.Stopped)
			{
				Application.DoEvents();
				//System.Threading.Thread.Sleep(100);
			}

			switch (checkUpdate.state)
			{
				case 0:	//����Ҫ����
					this.toolStripStatusLabelUpdate.Text = "";// "No update.";
					break;
				case 1:	//������
                    this.toolStripStatusLabelUpdate.Text = Application.CurrentCulture.Name.StartsWith("zh") ? "��һ�����õĸ���,����������..." : "A new version is available,click on to download ...";
					this.toolStripStatusLabelUpdate.Image = global::Swift.ROM.Properties.Resources.update1;
					this.toolStripStatusLabelUpdate.IsLink = true;
					break;
				case 2:	//����
					this.toolStripStatusLabelUpdate.Text = Application.CurrentCulture.Name.StartsWith("zh")?"�������ӵ�����������...":"Can't connect to the Update Service.";
					break;
			}//switch

		}

		private void toolStripStatusLabelUpdate_Click(object sender, EventArgs e)
		{
			if (this.toolStripStatusLabelUpdate.IsLink)
				Tools.Start("http://sourceforge.net/project/showfiles.php?group_id=192786");
		}

		private void tsbShowT0_Click(object sender, EventArgs e)
		{
			this.miShowT0.PerformClick();
		}

		private void tsbShowT1_Click(object sender, EventArgs e)
		{
			this.miShowT1.PerformClick();
		}

		private void tsbShowT2_Click(object sender, EventArgs e)
		{
			this.miShowT2.PerformClick();
		}

		private void listView_DoubleClick(object sender, EventArgs e)
		{
			this.miOpen.PerformClick();
		}

		private void miPrint_Click(object sender, EventArgs e)
		{
			FormPrintPreview f = new FormPrintPreview(this.dataView);
			f.ShowDialog();
		}

        private void miSaveAs_Click(object sender, EventArgs e)
        {
            //�ж�һ����û��ѡ�����Ŀ�����û����ֱ�ӷ���
            if (this.listView.SelectedItems.Count == 0)
                return;

            //ȡ���ļ�����
            string sha1 = this.listView.SelectedItems[0].Tag.ToString();
            DataRow[] rows = this.dataTableR.Select("A='" + sha1 + "'");
            if (rows.Length == 0)
                rows = this.dataTableR.Select("I='" + sha1 + "'");

            if (rows.Length == 0)
                return;

            //ɾ����ǰ��ʱĿ¼�е��ļ�
            try
            {
                if (Directory.Exists(Application.StartupPath + @"\TEMP\CopyROM"))
                    Directory.Delete(Application.StartupPath + @"\TEMP\CopyROM", true);
            }
            catch { Debug.WriteLine("��ɾ����ǰ��ʱĿ¼ʱ��������"); }

            string fp = rows[0]["f"].ToString().Split('?')[0];
            //string fn = null;
            if (rows[0]["f"].ToString().Split('?').Length > 1)
            {
                Tools.unZIP(fp, Application.StartupPath + @"\TEMP\CopyROM\");
                fp = Application.StartupPath + @"\TEMP\CopyROM\" + rows[0]["f"].ToString().Split('?')[1];
            }

            this.saveFileDialog1.FileName = Path.GetFileName(fp);
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy(fp, this.saveFileDialog1.FileName);
                }
                catch { }
            }

            //ɾ����ʱ�ļ�
            try
            {
                Directory.Delete(Application.StartupPath + @"\TEMP\VerifyROM\", true);
            }
            catch { Debug.WriteLine("��ʶ����ѹ���ļ���ɾ����ѹ��ʱ�ļ�ʱ��������"); }

        }

        private void miReDownloadOther_Click(object sender, EventArgs e)
        {
            //���û��ROM��ѡ���򷵻�
            if (this.listView.SelectedItems.Count == 0) return;
            
            ListViewItem lvi = this.listView.SelectedItems[0];

            if (lvi.Tag.ToString().Length == 0) return;

            DataRow[] rows = this.dataTableR.Select("A='" + lvi.Tag.ToString() + "'");
            if (rows.Length == 0) return;

            int m = 2;
            if (!rows[0].IsNull("m"))
                m = (int)rows[0]["m"];
            for (int i = 3; i <= m; i++)
                fd.download(this.nowType + "/" + lvi.Tag.ToString() + "_" + i.ToString("00") + ".png");
        }

    }
}