using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Swift.ROM
{
	class VerifyROM
	{
		public VerifyROM(FormMain form)
		{
			this.formMain = form;
		}

		delegate void cbUpdateLVI(DataRow row,Boolean insert,Boolean reSUM);

		private FormMain formMain;

		public void Start()
		{
			Debug.WriteLine("验证线程开始工作..."+formMain.nowType);

		    //验证所有现有的文件
		    //	例如有的文件已经被移除等
		    foreach (DataRow row in this.formMain.dataTableR.Select("f is not null"))
		    {
				string fn=row["f"].ToString().Split('?')[0];
				//判断文件是否存在，如果不存在则把ROM.xml文件中的f置NULL
                fn = fn.Replace("<rom>", Application.StartupPath);
				if (!File.Exists(fn))
				{
					row["f"] = row["ft"] = DBNull.Value;

					cbUpdateLVI cb = new cbUpdateLVI(formMain.updateLVI);
					formMain.Invoke(cb, new object[] { row, false, true });

					if (row.IsNull("A")) row.Delete();
				}
				else//如果文件存在，则判断文件是否被改动过。改动的标准是文件修改时间的变动，以文件UTC时间为准
				{
					long ft = File.GetLastWriteTimeUtc(fn).ToFileTimeUtc();
					if (row.IsNull("ft"))
						row["ft"] = ft;
					else
						if ((long)row["ft"] != ft)
						{
							row["f"] = row["ft"] = DBNull.Value;

							cbUpdateLVI cb = new cbUpdateLVI(formMain.updateLVI);
							formMain.Invoke(cb, new object[] { row, false, true });
						}
					//Application.DoEvents();
				}
		    }//foreach

		    if (!File.Exists(Application.StartupPath + "/Swift.ROM.xml"))	return;
			
		    //验证所有文件
			Debug.WriteLine("验证线程开始验证所有文件...");
		    XmlDocument xd = new XmlDocument();
		    xd.Load(Application.StartupPath + "/Swift.ROM.xml");
		    foreach (XmlNode node in xd.SelectSingleNode("/ROM/" + formMain.nowType).ChildNodes)
			{
				if (node.Name != "Path") continue;
		        XmlElement xe = (XmlElement)node;

		        try
		        {
		            foreach (string f in Directory.GetFiles(xe.GetAttribute("path").Replace("<rom>",Application.StartupPath)))
		            {
						//判断文件是否已经被引用
						string t_ext = Path.GetExtension(f).Substring(1).ToUpper();
						if (t_ext == "RAR" || t_ext == "ZIP" || t_ext == "7Z")
						{
							if (formMain.dataTableR.Select("f like '" + f.Replace("'", "''").Replace("[", "[[]").Replace("_", "[_]") + "?*'").Length > 0)
								continue;
						}
						else
						{
							if (formMain.dataTableR.Select("f = '" + f.Replace("'", "''").Replace("[", "[[]").Replace("_", "[_]") + "'").Length > 0)
								continue;
						}

						bool flag = false;

						//CRC32验证
						string[][] crc32 = Tools.GetCRC32(f);
						foreach (string[] crc in crc32)
						{
							string ext = crc[0].Substring(crc[0].Length - 3).ToUpper();
							//判断文件是否是需要的文件
							if (formMain.nowType == "NDS")
							{
								if (ext != "NDS" && ext != "IDS") continue;
							}
							else
							{
								if (ext != formMain.nowType) continue;
							}

							DataRow[] rows = formMain.dataTableR.Select("CRC32='" + crc[2] + "'");
							if (rows.Length > 1)
							{
								Debug.WriteLine("CRC32碰撞!!!进行SHA1验证!");
								flag = false;
								break;
							}
							if (rows.Length == 1)
							{
								rows[0]["f"] = crc[0];
								//更新显示
								cbUpdateLVI cb = new cbUpdateLVI(formMain.updateLVI);
								formMain.Invoke(cb, new object[] { rows[0], false, true });
								flag = true;
							}
						}
						if (flag) continue;

						//对压缩文件的支持
						string ext1 = Path.GetExtension(f).Substring(1).ToUpper();
						//如果是支持的压缩格式，则进行相应的解压
						if (ext1 == "RAR" || ext1 == "ZIP" || ext1=="7Z")
						{
							if (formMain.dataTableR.Select("f like '" + f.Replace("'", "''").Replace("[","[[]").Replace("_","[_]") + "?*'").Length > 0)
								continue;

							//测试压缩文件的内容是否是需要的
							Process proc = new Process();
							proc.StartInfo.FileName = Application.StartupPath + @"\"+(ext1=="RAR"?"UnRAR.exe":"7za.exe");
							proc.StartInfo.Arguments = "l \"" + f + "\"";
							proc.StartInfo.UseShellExecute = false;
							proc.StartInfo.RedirectStandardOutput = true;
							proc.StartInfo.RedirectStandardError = false;
							proc.StartInfo.CreateNoWindow = true;
							proc.Start();
							string read=proc.StandardOutput.ReadToEnd();
							Debug.WriteLine("压缩文件中的文件:" + read);
                            if (formMain.nowType == "NDS")
                            {
                                if (read.ToUpper().IndexOf("." + formMain.nowType) == -1 && read.ToUpper().IndexOf(".IDS") == -1)
                                    continue;
                            }
                            else
                            {
                                if (read.ToUpper().IndexOf("." + formMain.nowType) == -1)
                                    continue;
                            }

							Debug.WriteLine("测试当前文件是压缩文件,进行解压...");

							//

							//删除以前临时目录中的文件
							try
							{
								if (Directory.Exists(Application.StartupPath + @"\TEMP\VerifyROM"))
									Directory.Delete(Application.StartupPath + @"\TEMP\VerifyROM", true);
							}
							catch { Debug.WriteLine("在删除以前临时目录时发生错误"); }

							//解压文件
						 	Tools.unZIP(f, Application.StartupPath + @"\TEMP\VerifyROM\");

							foreach (string ff in Directory.GetFiles(Application.StartupPath + @"\TEMP\VerifyROM\"))
								verifyFile(ff,f);

							//删除临时文件
							try
							{
								Directory.Delete(Application.StartupPath + @"\TEMP\VerifyROM\", true);
							}
							catch { Debug.WriteLine("在识别完压缩文件，删除解压临时文件时发生错误"); }
						}//压缩格式
						else
						{
							verifyFile(f,null);
						}
						
		            }//foreach
		        }
		        catch(Exception ex)
		        {
		            Debug.WriteLine(ex.Message);
					Debug.WriteLine(ex.StackTrace);
		        }
		    }//foreach

		}

		/// <summary>
		/// 验证指定文件
		/// </summary>
		/// <param name="f">文件名称（完整文件路径）</param>
		/// <param name="zipFile">压缩文件名称</param>
		private void verifyFile(string f, string zipFile)
		{
			string ext = Path.GetExtension(f).ToUpper().Substring(1);

			//忽略一些明显不是ROM的文件
            //  有一个例外,NDS游戏中,IDS游戏的扩展名为IDS,这里单独处理一下。
            if (formMain.nowType == "NDS")
            {
                if (ext != "NDS" && ext != "IDS") return;
            }
            else
            {
                if (ext != formMain.nowType) return;
            }

			//如果当前文件已经存在并不是压缩文件，则直接返回
			if (zipFile == null)//非压缩文件的
			{
                if (formMain.dataTableR.Select("f='" + Tools.car(f).Replace("'", "''") + "'").Length > 0)
					return;
			}
			else//压缩文件的
			{
                if (formMain.dataTableR.Select("f='" + Tools.car(zipFile).Replace("'", "''") + "?" + Path.GetFileName(f).Replace("'", "''") + "'").Length > 0)
					return;
			}

			Debug.WriteLine("VerifyROM=" + f);

			string sha1 =Tools.GetFileSHA1(f);
			if (sha1 == null) return;

			DataRow[] rows = formMain.dataTableR.Select("A='" + sha1 + "'");
			//这里如果查到A=sha1则是一个可识别的ROM,如果查找不到则认为是一个不可识别得ROM
			if (rows.Length > 0)
			{
				if (rows[0]["f"] == DBNull.Value)
				{
					rows[0]["f"] = (zipFile == null) ? Tools.car(f) : Tools.car(zipFile) + "?" + Path.GetFileName(f);
				}
				else//判断一下重复的是原来的错了,还是新的重复
				{
					//判断一下原来的文件是否是压缩文件
					string[] fs = Tools.cra(rows[0]["f"].ToString()).Split('?');
					if (fs.Length == 1)//不是压缩文件
					{
						sha1 = Tools.GetFileSHA1(Tools.cra(rows[0]["f"].ToString()));
					}
					else //是压缩文件
					{
						//对原来的文件进行解压
						Tools.unZIP(fs[0], Application.StartupPath + @"\TEMP\VerifyROM\");
						sha1 = Tools.GetFileSHA1(Application.StartupPath + @"\TEMP\VerifyROM\" + Path.GetFileName(fs[1]));
					}

					if (sha1 == null) return;

					if (sha1 == rows[0]["A"].ToString())
					{
						if (MessageBox.Show("发现一个重复的ROM,是否删除这个ROM文件？\n文件1=" + rows[0]["f"] + "\n文件2=" + (zipFile==null? f:zipFile), "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
							File.Delete(fs[0]);
					}
					else
					{
						rows[0]["f"] = (zipFile == null) ? Tools.car(f) : Tools.car(zipFile) + "?" + Path.GetFileName(f);
					}

				}

				//更新显示
				cbUpdateLVI cb = new cbUpdateLVI(formMain.updateLVI);
				formMain.Invoke(cb, new object[] { rows[0], false, true });
			}
			else
			{
				//计算ROM容量
				FileStream fs = new FileStream(Tools.cra(f), FileMode.Open, FileAccess.Read, FileShare.Read);
				long fz = fs.Length;
				string fzs = null;

				fz /= 1024;
				if (fz > 1000)
				{
					fz /= 1024;
					fzs= fz.ToString() + "M";
				}
				else
					fzs = fz.ToString() + "K";
				fs.Close();

				DataRow row = formMain.dataTableR.NewRow();
				row["N"] = "未知ROM";
				row["E"] = (zipFile == null) ? Path.GetFileName(f) : zipFile + "?" + Path.GetFileName(f);
				row["S"] = fzs;
				row["f"] = (zipFile == null) ? Tools.car(f) : Tools.car(zipFile) + "?" + Path.GetFileName(f);
				row["I"] = sha1;
				formMain.dataTableR.Rows.Add(row);

				//把新增的ROM添加到ListView中
				cbUpdateLVI cb = new cbUpdateLVI(formMain.updateLVI);
				formMain.Invoke(cb, new object[] { row, true, true });
			}

		}

	}
}
