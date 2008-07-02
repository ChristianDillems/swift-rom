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

            //初始化等待窗口
         //   this.fw = new FormWait();
        }

        /// <summary>
        /// 等待窗口
        /// </summary>
      //  private FormWait fw;

        private String sortColumn;

		/// <summary>
		/// 当前版本号，用于区分数据版本的
		/// </summary>
		private string nowVersion;

        public String nowType=null;

        /// <summary>
        /// 是否自动下载图片的标记
        /// </summary>
        private Boolean autoDownload;
        /// <summary>
        /// 自动下载图片时，是否下载其他扩展图片
        /// </summary>
        private Boolean autoDownloadOther;
		private Boolean autoVerify;
		private Boolean UseLevel;

        private FormDownload fd;

		private Thread verifyThread;

        #region 从升级文件中升级
        private void update(string type)
        {
            //判断升级文件是否存在，如果不存在则直接退出
            if (!File.Exists(Application.StartupPath + "/Update-" + type + ".xml"))
                return;

			this.waitThreadStop();

            XmlDocument xd = new XmlDocument();
            XmlElement xe;
            DataRow row;

            //记录本次最新的更新时间
            DateTime ndt=new DateTime(2000,1,1);

            //读取最新的更新日期
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

            this.labelMessage.Text = "发现" + type + "升级文件，正在升级......";
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
					Debug.WriteLine("读出的版本号:" + xe.GetAttribute("Version"));

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
               
                //判断更新日期是否是新的，如果不是新的则直接跳过更新
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

            //重新计算未识别的ROM
			//this.labelMessage.Text = "正在识别以前未识别的ROM";
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

            //删除升级文件
            File.Delete(Application.StartupPath + "/Update-" + type + ".xml");

            //存储更新时间
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
			//删除临时文件
			try
			{
				if (Directory.Exists(Application.StartupPath + @"\TEMP"))
					Directory.Delete(Application.StartupPath + @"\TEMP", true);
			}
			catch 
			{
				Debug.WriteLine("删除/temp目录时出错!");
			}

			//打开下载窗口
            fd = new FormDownload(this.toolStripStatusLabelDownload);
#if !DEBUG
            this.miRom.Visible = false;
			Application.DoEvents();
#endif
            FormWait fw = new FormWait();
            fw.Show();
            this.labelMessage.Text = "正在进行初始化操作......";
            this.Show();
            Application.DoEvents();

            //判断所需目录是否存在，如果不存在则建立
			if (!Directory.Exists(Application.StartupPath + "/NES")) Directory.CreateDirectory(Application.StartupPath + "/NES");
            if (!Directory.Exists(Application.StartupPath + "/GBA")) Directory.CreateDirectory(Application.StartupPath + "/GBA");
            if (!Directory.Exists(Application.StartupPath + "/NDS")) Directory.CreateDirectory(Application.StartupPath + "/NDS");

            //读取设置文件
            XmlDocument xd = new XmlDocument();
            XmlElement xe;

			//如果配置文件不存在，则重新创建一个
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

			//读取个性化信息文件
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

				//判断版本
				Debug.WriteLine("读出的版本号:" + this.nowVersion);
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
				

				//读取排序列
				this.sortColumn = xe.GetAttribute("sort");
				if (this.sortColumn == null) this.sortColumn = "X";

				nType = xe.GetAttribute("nowType");

				//设置视图的列宽
				foreach (ColumnHeader ch in this.listView.Columns)
				{
					try
					{
						ch.Width = int.Parse(xe.GetAttribute("lvcw" + ch.Index));
					}
					catch { }
				}

				// TODO 这段代码2009年1月删除
				//查看当前是否使用的旧版配置文件，如果是则进行升级
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
				//2009.1删除到这

				//判断设置文件是否完整，不完整的话补充完整
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

            //升级ROM信息
			this.update("NES"); Application.DoEvents();
			this.update("GBA"); Application.DoEvents();
			this.update("NDS"); Application.DoEvents();

			//显示程序版本
			this.Text += this.nowVersion;

            //显示ROM数量
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

			//判断语言区域，如果不是简体中文 zh_CN 区域的(限制中文区域)，则不显示中文列
			if (!Application.CurrentCulture.Name.StartsWith("zh"))
				this.listView.Columns[1].Width = 0;

            this.labelMessage.Text = "Ready.";

			//读取默认显示的ROM机型
			switch (nType)
			{
				case "NES": this.miShowNES.PerformClick(); break;
				case "GBA": this.miShowGBA.PerformClick(); break;
				case "NDS": this.miShowNDS.PerformClick(); break;
				default: this.miShowNDS.PerformClick(); break;
			}

			this.timerState.Start();

			//检测是否有新的版本，如果有进行提示
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

		#region 关于窗口
        private void miAbout_Click(object sender, EventArgs e)
        {
            FormAbout f = new FormAbout();
            f.ShowDialog();
		}
		#endregion

		private void miRefresh_Click(object sender, EventArgs e)
        {
            //同步菜单和工具栏
            this.tsbShowVerifyRom.Checked = this.miShowVerifyROM.Checked;
            this.tsbShowUnseenROM.Checked = this.miShowUnseenROM.Checked;
            this.tsbShowUnknowRom.Checked = this.miShowUnknowROM.Checked;
            this.tsbShowFavorite.Checked = this.miShowFavorite.Checked;
			this.tsbShowT0.Checked = this.miShowT0.Checked;
			this.tsbShowT1.Checked = this.miShowT1.Checked;
			this.tsbShowT2.Checked = this.miShowT2.Checked;

            //显示等待窗口
            FormWait fw = new FormWait();
            fw.Show();
            this.labelMessage.Text = "正在刷新列表...";
            Application.DoEvents();

			//等待ROM验证线程停止，因为不停止的话可能会出现数据冲突
			this.waitThreadStop();

            //读取当前选择项,用于刷新完后还显示这个位置
			string nowItem = (this.listView.SelectedItems.Count > 0) ? this.listView.SelectedItems[0].Tag.ToString() : null;

            this.listView.BeginUpdate();

            //清除现有信息
            this.listView.Items.Clear();
            this.listView.Groups.Clear();

            //创建分组
            this.listView.ShowGroups = this.miViewGroups.Checked;
            if (this.miViewGroups.Checked)
            {
                if (this.miViewGroupC.Checked)  //出品公司
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
                else if (this.miViewGroupYear.Checked)  //发布年代
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

            //构建填充数据条件
            string where = "";

            if (this.miShowVerifyROM.Checked) where += " or A is not null and f is not null";
            if (this.miShowUnseenROM.Checked) where += " or A is not null and f is null";
            if (this.miShowUnknowROM.Checked) where += " or A is null and f is not null";

			where = (where == "") ? "" : where.Substring(3);

			//处理非重要、可舍弃ROM的显示
			string tw = "";
			if (this.miShowT0.Checked) tw += " or H is null";// or f is not null";
			if (this.miShowT1.Checked) tw += " or H=1";
			if (this.miShowT2.Checked) tw += " or H=2";
			where = (tw == "") ? where : (where=="")?tw.Substring(3):"(" + where + ") and (" + tw.Substring(3) + ")";

			//处理收藏夹显示
            if (this.miShowFavorite.Checked)
            {
                where += "(" + where + ") and F=1";
                where = where.Replace("() and", "");
            }

            Debug.WriteLine(where);

            this.dataView.RowFilter = where;
			//排序根据所选项进行选择
			//	如果排序为编号，则在中文时同编号的再以中文名排序
			//					则在英文时同编号的再以英文名排序
			if(sortColumn.Equals("X"))
				this.dataView.Sort = sortColumn+(Application.CurrentCulture.Name.StartsWith("zh")?",N":",E");
			else
				this.dataView.Sort = sortColumn;

			this.toolStripProgressBar.Visible = true;
            this.toolStripProgressBar.Maximum = this.dataView.Count;
            this.toolStripProgressBar.Value = 0;

            foreach (DataRowView row in this.dataView)
            {
				//往ListView中写数据
				this.updateLVI(row.Row, true,false);

				if (this.toolStripProgressBar.Value < this.toolStripProgressBar.Maximum)
					this.toolStripProgressBar.Value++;
            }

			//选择项,用于刷新完后还显示这个位置
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

			//自动验证所有ROM
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
			
			//如果没有找到合适的LVI,则也认为是增加的条目
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
				if (this.miViewGroupC.Checked)   //出品公司
				{
					lvi.Group = this.listView.Groups[row["C"].ToString()];
				}
				else if (this.miViewGroupYear.Checked)  //出品年代
				{
					string t = (row["Y"].ToString().Length >= 4) ? row["Y"].ToString().Substring(0, 4) : row["Y"].ToString();
					lvi.Group = this.listView.Groups[t];
				}
			}

			lvi.Tag = row.IsNull("A")?row["I"]:row["A"];

			//判断当前信息的等级
			//	默认分为3级，不设置的为最高级，1为中间级，2为最低级
			//	一般来说，最高级的为标准游戏ROM，中间级的为汉化破其他破解ROM，2为客户忽略的ROM（例如坏的或者有新版本的）
			if (this.UseLevel)
			{
				if (row["H"].ToString() == "2")
					lvi.ForeColor = Color.LightGray;
				else if (row["H"].ToString() == "1")
					lvi.ForeColor = Color.Gray;
			}

			if(insert) this.listView.Items.Add(lvi);
			
		//	Application.DoEvents();

			//更新右下角的数量
			if (reSUM)
			{
				//提示找到新的ROM
			//	if (!row.IsNull("f"))
			//	{
			//		ToolTip tt = new ToolTip();
			//		tt.IsBalloon = true;
			//		tt.Show("识别到新ROM:" + row["E"].ToString(), this,this.toolStripStatusLabelState.Bounds.Location.X,this.toolStripStatusLabelState.Bounds.Location.Y+this.Height-70, 2000);
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
			//显示设置窗口
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
				Debug.WriteLine("没有找到指定的ROM数据");
				return;
			}

			FormEdit f = new FormEdit(this.nowType, this.listView.SelectedItems[0].Tag.ToString(), rows[0]["E"].ToString() , rows[0]["S"].ToString() );
			if (f.ShowDialog() == DialogResult.OK)
			{
				//保存当前ROM数据
				this.dataSet.WriteXml(Application.StartupPath + @"\" + this.nowType + @"\ROM.xml");
				//升级ROM数据
				this.update(this.nowType);

				//this.miRefresh.PerformClick();
				rows=this.dataTableR.Select("A='"+this.listView.SelectedItems[0].Tag+"'");
				this.updateLVI(rows[0], false, true);
			}
		}

		/// <summary>
		/// 开始重新验证所有当前分类的ROM
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void miVerifyROM_Click(object sender, EventArgs e)
        {
			//开始线程之前，先确认原来的线程已经结束。
			this.waitThreadStop();

			//声明ROM验证线程
			//	说明：原来没有单独的线程用于ROM验证，造成在验证的时候没办法进行相应的其他操作，加上这个线程以后ROM识别和软件的使用就可以相对分离了。
			VerifyROM verifyROM = new VerifyROM(this);
			this.verifyThread = new Thread(new ThreadStart(verifyROM.Start));
			this.verifyThread.IsBackground = true;
			this.verifyThread.Priority = ThreadPriority.Lowest;
			this.verifyThread.Start();
        }//miVerifyROM_Click

		/// <summary>
		/// 等待ROM验证线程结束
		/// </summary>
		private void waitThreadStop()
		{
			if (this.verifyThread == null) return;

			//todo 这里是否应该写一下正在停止线程？
			this.toolStripStatusLabelState.Text = "正在停止后台验证...";

			this.verifyThread.Abort();
			while (this.verifyThread.ThreadState != System.Threading.ThreadState.Stopped)
				Application.DoEvents();
		}

		private void miOpen_Click(object sender, EventArgs e)
		{
			//如果没有ROM被选择，则直接返回
			if (this.listView.SelectedItems.Count == 0)
				return;

			string sha1 = this.listView.SelectedItems[0].Tag.ToString();
			DataRow row;
			if (this.dataTableR.Select("A='" + sha1 + "'").Length > 0)
				row = this.dataTableR.Select("A='" + sha1 + "'")[0];
			else
				row = this.dataTableR.Select("E='" + this.listView.SelectedItems[0].SubItems[2].Text.Replace("'","''") + "'")[0];

			//判断当前游戏是否存在，如果不存在则直接退出
			if (row["f"].ToString() == "")
				return;

            FormWait fw = new FormWait();
			fw.Show();
			Application.DoEvents();

			Process proc = new Process();

			//选择模拟器
			string emu = ((ToolStripMenuItem)sender).Tag.ToString().Replace("<swift.rom>", Application.StartupPath);
			if(!File.Exists(emu))
			{
				MessageBox.Show("模拟器路径设置错误！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			proc.StartInfo.FileName = emu;

			//如果为压缩文件，则需要进行解压
			string[] fs = row["f"].ToString().Split('?');
			if (fs.Length == 1)
				proc.StartInfo.Arguments = "\"" + row["f"].ToString() + "\"";
			else//为压缩文件
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
				MessageBox.Show("模拟器错误！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

			//fw.Hide();
            fw.Close();
		}

		private void listView_SelectedIndexChanged(object sender, EventArgs e)
		{
			//如果没有ROM被选择，则把图像置为空，并返回
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
                        //如果 开启自动下载 && 开始自动下载扩展图片 && 选择的ROM为收录ROM,则下载扩展图片
						if (this.autoDownload && this.autoDownloadOther && !rows[0].IsNull("A"))
							fd.download(this.nowType + "/" + lvi.Tag.ToString() + "_" + i.ToString("00") + ".png");
					}
				}
				//判断是否有其它图像，如果没有则把Other标签关掉
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

			//判断显示的图片是什么样子的，并根据条件进行布局调整
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
			//保存当前ROM数据
			this.dataSet.WriteXml(Application.StartupPath + "/"+this.nowType+"/ROM.xml");

            //保存个性化设置
            XmlDocument xd = new XmlDocument();
            XmlElement xe;
            //判断设置文件是否存在，不存在的话创建一个个性化设置文件
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
            //保存视图的列宽信息
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

			//删除临时文件夹
			try//有可能这些文件正在使用，这时候作删除就会发生错误，所以这里忽略这些错误
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
					//取出指定SHA1的数据
					DataRow[] rows=	this.dataTableR.Select("A='" + lvi.Tag.ToString() + "'");
					//如果数据没有取回，判断是否是非识别ROM的数据
					if(rows.Length==0)
						rows=this.dataTableR.Select("I='" + lvi.Tag.ToString() + "'");
					//确认有数据取回后进行收藏夹处理
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

            //检测是否已经选择的类型又一次选择，如果是则直接返回
			if (mi.Checked) return;

			this.waitThreadStop();	//如果还有其他线程在工作，则等待这个线程停止后再继续

			//保存当前的ROM信息
			//	在换机型时保存一下上一个机型的数据
			if(this.nowType!=null)
				this.dataSet.WriteXml(Application.StartupPath + "/" + this.nowType + "/ROM.xml");

			this.miShowNES.Checked = this.tsbShowNES.Checked = false;
			this.miShowGBA.Checked = this.tsbShowGBA.Checked = false;
            this.miShowNDS.Checked = this.tsbShowNDS.Checked = false;

            mi.Checked = true;

            this.nowType = mi.Tag.ToString();

			//把模拟器地址取出
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

            //刷新列表
            this.miRefresh.PerformClick();
        }

        private void tsbShowGBA_Click(object sender, EventArgs e)
        {
			//为了检测按钮状态，所以这么调用
            this.miShowGBA.PerformClick();
        }

        private void tsbShowNDS_Click(object sender, EventArgs e)
        {
            this.miShowNDS.PerformClick();
        }

        private void miSaveAsOriginal_Click(object sender, EventArgs e)
        {
            //判断一下有没有选择的项目，如果没有则直接返回
            if (this.listView.SelectedItems.Count == 0)
                return;

            //取出文件名称
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
				case 0:	//不需要升级
					this.toolStripStatusLabelUpdate.Text = "";// "No update.";
					break;
				case 1:	//有升级
                    this.toolStripStatusLabelUpdate.Text = Application.CurrentCulture.Name.StartsWith("zh") ? "有一个可用的更新,点这里下载..." : "A new version is available,click on to download ...";
					this.toolStripStatusLabelUpdate.Image = global::Swift.ROM.Properties.Resources.update1;
					this.toolStripStatusLabelUpdate.IsLink = true;
					break;
				case 2:	//错误
					this.toolStripStatusLabelUpdate.Text = Application.CurrentCulture.Name.StartsWith("zh")?"不能连接到升级服务器...":"Can't connect to the Update Service.";
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
            //判断一下有没有选择的项目，如果没有则直接返回
            if (this.listView.SelectedItems.Count == 0)
                return;

            //取出文件名称
            string sha1 = this.listView.SelectedItems[0].Tag.ToString();
            DataRow[] rows = this.dataTableR.Select("A='" + sha1 + "'");
            if (rows.Length == 0)
                rows = this.dataTableR.Select("I='" + sha1 + "'");

            if (rows.Length == 0)
                return;

            //删除以前临时目录中的文件
            try
            {
                if (Directory.Exists(Application.StartupPath + @"\TEMP\CopyROM"))
                    Directory.Delete(Application.StartupPath + @"\TEMP\CopyROM", true);
            }
            catch { Debug.WriteLine("在删除以前临时目录时发生错误"); }

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

            //删除临时文件
            try
            {
                Directory.Delete(Application.StartupPath + @"\TEMP\VerifyROM\", true);
            }
            catch { Debug.WriteLine("在识别完压缩文件，删除解压临时文件时发生错误"); }

        }

        private void miReDownloadOther_Click(object sender, EventArgs e)
        {
            //如果没有ROM被选择，则返回
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