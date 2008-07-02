using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Swift.ROM
{
    public partial class FormSetup : Form
    {
        public FormSetup()
        {
            InitializeComponent();
        }

        private void FormSetup_Load(object sender, EventArgs e)
        {
            try
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(Application.StartupPath + @"\Swift.ROM.xml");

                if (xd.SelectSingleNode("/ROM") != null)
                {
                    if (((XmlElement)xd.SelectSingleNode("/ROM")).GetAttribute("AutoDownload") == "0")
                        this.checkBoxAutoDownload.Checked = false;

                    if (((XmlElement)xd.SelectSingleNode("/ROM")).GetAttribute("AutoDownloadOther") == "0")
                        this.checkBoxAutoDownloadOther.Checked = false;

					if (((XmlElement)xd.SelectSingleNode("/ROM")).GetAttribute("AutoVerify") == "0")
						this.checkBoxAutoVerify.Checked = false;

					if (((XmlElement)xd.SelectSingleNode("/ROM")).GetAttribute("UseLevel") == "0")
						this.checkBoxUseLevel.Checked = false;

                    if (((XmlElement)xd.SelectSingleNode("/ROM")).GetAttribute("AutoUpdate") == "0")
                        this.checkBoxAutoUpdate.Checked = false;
                }

				//取NES相关信息
				foreach (XmlNode node in xd.SelectSingleNode("/ROM/NES").ChildNodes)
				{
					if (node.Name == "Path")
						this.listBoxRomNES.Items.Add(node.Attributes["path"].Value);
					if (node.Name == "EMU")	//读出所有模拟器
						this.listViewEmuNES.Items.Add(new ListViewItem(new string[] { node.Attributes["Name"].Value, node.Attributes["File"].Value, node.Attributes["Default"].Value }));
				}
				//取GBA相关信息
                foreach (XmlNode node in xd.SelectSingleNode("/ROM/GBA").ChildNodes)
                {
                    if (node.Name == "Path")
						this.listBoxRomGBA.Items.Add(node.Attributes["path"].Value);
					if (node.Name == "EMU")	//读出所有模拟器
						this.listViewEmuGBA.Items.Add(new ListViewItem(new string[] { node.Attributes["Name"].Value, node.Attributes["File"].Value, node.Attributes["Default"].Value }));
				}
                //取NDS信息
                foreach (XmlNode node in xd.SelectSingleNode("/ROM/NDS").ChildNodes)
                {
                    if (node.Name == "Path")
                        this.listBoxRomNDS.Items.Add(node.Attributes["path"].Value);
					if (node.Name == "EMU")	//读出所有模拟器
						this.listViewEmuNDS.Items.Add(new ListViewItem(new string[] { node.Attributes["Name"].Value, node.Attributes["File"].Value, node.Attributes["Default"].Value }));
				}
            }
            catch
            { }
        }

        private void buttonDeleteGbaRomPath_Click(object sender, EventArgs e)
        {
            if(this.listBoxRomGBA.SelectedItem!=null)
                this.listBoxRomGBA.Items.Remove(this.listBoxRomGBA.SelectedItem);
        }

        private void buttonAddGbaRomPath_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
                this.listBoxRomGBA.Items.Add(this.folderBrowserDialog.SelectedPath);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            XmlDocument xd = new XmlDocument();
            XmlElement xe;
            try
            {
                xd.Load(Application.StartupPath + @"\Swift.ROM.xml");
            }
            catch
            {
                xe = xd.CreateElement("ROM");
                xd.SelectSingleNode("/").AppendChild(xe);
            }

            //保存选项
            ((XmlElement)xd.SelectSingleNode("/ROM")).SetAttribute("AutoDownload", this.checkBoxAutoDownload.Checked ? "1" : "0");
            ((XmlElement)xd.SelectSingleNode("/ROM")).SetAttribute("AutoDownloadOther", this.checkBoxAutoDownloadOther.Checked ? "1" : "0");
			((XmlElement)xd.SelectSingleNode("/ROM")).SetAttribute("AutoVerify", this.checkBoxAutoVerify.Checked ? "1" : "0");
			((XmlElement)xd.SelectSingleNode("/ROM")).SetAttribute("UseLevel", this.checkBoxUseLevel.Checked ? "1" : "0");
            ((XmlElement)xd.SelectSingleNode("/ROM")).SetAttribute("AutoUpdate", this.checkBoxAutoUpdate.Checked ? "1" : "0");
			//保存NES相关信息
			if (xd.SelectSingleNode("/ROM/NES") == null)
				xd.SelectSingleNode("/ROM").AppendChild(xd.CreateElement("NES"));
			xd.SelectSingleNode("/ROM/NES").RemoveAll();
			foreach (ListViewItem lvi in this.listViewEmuNES.Items)	//保存所有NES模拟器的信息
			{
				xe = xd.CreateElement("EMU");
				xe.SetAttribute("Name", lvi.SubItems[0].Text);
				xe.SetAttribute("File", lvi.SubItems[1].Text);
				xe.SetAttribute("Default", lvi.SubItems[2].Text);
				xd.SelectSingleNode("/ROM/NES").AppendChild(xe);
			}
			foreach (string s in this.listBoxRomNES.Items)
			{
				xe = xd.CreateElement("Path");
				xe.SetAttribute("path", s);
				xd.SelectSingleNode("/ROM/NES").AppendChild(xe);
			}
			//保存GBA相关信息
            if (xd.SelectSingleNode("/ROM/GBA") == null)
                xd.SelectSingleNode("/ROM").AppendChild(xd.CreateElement("GBA"));
            xd.SelectSingleNode("/ROM/GBA").RemoveAll();
			foreach (ListViewItem lvi in this.listViewEmuGBA.Items)	//保存所有NES模拟器的信息
			{
				xe = xd.CreateElement("EMU");
				xe.SetAttribute("Name", lvi.SubItems[0].Text);
				xe.SetAttribute("File", lvi.SubItems[1].Text);
				xe.SetAttribute("Default", lvi.SubItems[2].Text);
				xd.SelectSingleNode("/ROM/GBA").AppendChild(xe);
			}
            foreach (string s in this.listBoxRomGBA.Items)
            {
                xe = xd.CreateElement("Path");
                xe.SetAttribute("path", s);
                xd.SelectSingleNode("/ROM/GBA").AppendChild(xe);
            }
            //保存NDS项目
            if (xd.SelectSingleNode("/ROM/NDS") == null)
                xd.SelectSingleNode("/ROM").AppendChild(xd.CreateElement("NDS"));
            xd.SelectSingleNode("/ROM/NDS").RemoveAll();
			foreach (ListViewItem lvi in this.listViewEmuNDS.Items)	//保存所有NES模拟器的信息
			{
				xe = xd.CreateElement("EMU");
				xe.SetAttribute("Name", lvi.SubItems[0].Text);
				xe.SetAttribute("File", lvi.SubItems[1].Text);
				xe.SetAttribute("Default", lvi.SubItems[2].Text);
				xd.SelectSingleNode("/ROM/NDS").AppendChild(xe);
			}
            foreach (string s in this.listBoxRomNDS.Items)
            {
                xe = xd.CreateElement("Path");
                xe.SetAttribute("path", s);
                xd.SelectSingleNode("/ROM/NDS").AppendChild(xe);
            }

            xd.Save(Application.StartupPath + @"\Swift.ROM.xml");

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonDeleteNdsRomPath_Click(object sender, EventArgs e)
        {
            if (this.listBoxRomNDS.SelectedItem != null)
                this.listBoxRomNDS.Items.Remove(this.listBoxRomNDS.SelectedItem);
        }

        private void buttonAddNdsRomPath_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
                this.listBoxRomNDS.Items.Add(this.folderBrowserDialog.SelectedPath);
        }

		private void buttonDeleteNesRomPath_Click(object sender, EventArgs e)
		{
			if (this.listBoxRomNES.SelectedItem != null)
				this.listBoxRomNES.Items.Remove(this.listBoxRomNES.SelectedItem);
		}

		private void buttonAddNesRomPath_Click(object sender, EventArgs e)
		{
			if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
				this.listBoxRomNES.Items.Add(this.folderBrowserDialog.SelectedPath);
		}

		private void listViewEmu_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListView lv = (ListView)sender;
			Button bDelete, bDefault;
			switch (lv.Tag.ToString())
			{
				case "NES": bDefault = this.buttonDefaultEmuNES; bDelete = this.buttonDeleteEmuNES; break;
				case "GBA": bDefault = this.buttonDefaultEmuGBA; bDelete = this.buttonDeleteEmuGBA; break;
				case "NDS": bDefault = this.buttonDefaultEmuNDS; bDelete = this.buttonDeleteEmuNDS; break;
				default: return;
			}

			bDefault.Enabled = bDelete.Enabled = lv.SelectedItems.Count > 0;
		}

		private void buttonAddEmu_Click(object sender, EventArgs e)
		{
			ListView lv;
			switch (((Button)sender).Tag.ToString())
			{
				case "NES": lv = this.listViewEmuNES; break;
				case "GBA": lv = this.listViewEmuGBA; break;
				case "NDS": lv = this.listViewEmuNDS; break;
				default: return;
			}

			if (this.openFileDialog.ShowDialog() == DialogResult.OK)
				lv.Items.Add(new ListViewItem(new string[] { System.IO.Path.GetFileName(this.openFileDialog.FileName).Split('.')[0], this.openFileDialog.FileName.Replace(Application.StartupPath,"<swift.rom>"), lv.Items.Count == 0 ? "Default" : "" }));
		}

		private void buttonDeleteEmu_Click(object sender, EventArgs e)
		{
			ListView lv;
			switch (((Button)sender).Tag.ToString())
			{
				case "NES": lv = this.listViewEmuNES; break;
				case "GBA": lv = this.listViewEmuGBA; break;
				case "NDS": lv = this.listViewEmuNDS; break;
				default: return;
			}

			ListViewItem lvi=lv.SelectedItems[0];
			//判断选择要删除的项是否为默认模拟器，如果是则找一个其他的模拟器作为默认模拟器
			if (lvi.SubItems[2].Text != "" && lv.Items.Count > 1)
				foreach (ListViewItem i in lv.Items)
					if (i.SubItems[2].Text == "")
						i.SubItems[2].Text = "Default";
			//删除选择的项
			lv.SelectedItems[0].Remove();
		}

		private void buttonDefaultEmu_Click(object sender, EventArgs e)
		{
			ListView lv;
			switch (((Button)sender).Tag.ToString())
			{
				case "NES": lv = this.listViewEmuNES; break;
				case "GBA": lv = this.listViewEmuGBA; break;
				case "NDS": lv = this.listViewEmuNDS; break;
				default: return;
			}

			foreach (ListViewItem lvi in lv.Items)
				lvi.SubItems[2].Text = "";
			lv.SelectedItems[0].SubItems[2].Text = "Default";
		}

        private void checkBoxAutoDownload_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBoxAutoDownloadOther.Enabled = this.checkBoxAutoDownload.Checked;
        }
    }
}