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
			Debug.WriteLine("��֤�߳̿�ʼ����..."+formMain.nowType);

		    //��֤�������е��ļ�
		    //	�����е��ļ��Ѿ����Ƴ���
		    foreach (DataRow row in this.formMain.dataTableR.Select("f is not null"))
		    {
				string fn=row["f"].ToString().Split('?')[0];
				//�ж��ļ��Ƿ���ڣ�������������ROM.xml�ļ��е�f��NULL
                fn = fn.Replace("<rom>", Application.StartupPath);
				if (!File.Exists(fn))
				{
					row["f"] = row["ft"] = DBNull.Value;

					cbUpdateLVI cb = new cbUpdateLVI(formMain.updateLVI);
					formMain.Invoke(cb, new object[] { row, false, true });

					if (row.IsNull("A")) row.Delete();
				}
				else//����ļ����ڣ����ж��ļ��Ƿ񱻸Ķ������Ķ��ı�׼���ļ��޸�ʱ��ı䶯�����ļ�UTCʱ��Ϊ׼
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
			
		    //��֤�����ļ�
			Debug.WriteLine("��֤�߳̿�ʼ��֤�����ļ�...");
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
						//�ж��ļ��Ƿ��Ѿ�������
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

						//CRC32��֤
						string[][] crc32 = Tools.GetCRC32(f);
						foreach (string[] crc in crc32)
						{
							string ext = crc[0].Substring(crc[0].Length - 3).ToUpper();
							//�ж��ļ��Ƿ�����Ҫ���ļ�
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
								Debug.WriteLine("CRC32��ײ!!!����SHA1��֤!");
								flag = false;
								break;
							}
							if (rows.Length == 1)
							{
								rows[0]["f"] = crc[0];
								//������ʾ
								cbUpdateLVI cb = new cbUpdateLVI(formMain.updateLVI);
								formMain.Invoke(cb, new object[] { rows[0], false, true });
								flag = true;
							}
						}
						if (flag) continue;

						//��ѹ���ļ���֧��
						string ext1 = Path.GetExtension(f).Substring(1).ToUpper();
						//�����֧�ֵ�ѹ����ʽ���������Ӧ�Ľ�ѹ
						if (ext1 == "RAR" || ext1 == "ZIP" || ext1=="7Z")
						{
							if (formMain.dataTableR.Select("f like '" + f.Replace("'", "''").Replace("[","[[]").Replace("_","[_]") + "?*'").Length > 0)
								continue;

							//����ѹ���ļ��������Ƿ�����Ҫ��
							Process proc = new Process();
							proc.StartInfo.FileName = Application.StartupPath + @"\"+(ext1=="RAR"?"UnRAR.exe":"7za.exe");
							proc.StartInfo.Arguments = "l \"" + f + "\"";
							proc.StartInfo.UseShellExecute = false;
							proc.StartInfo.RedirectStandardOutput = true;
							proc.StartInfo.RedirectStandardError = false;
							proc.StartInfo.CreateNoWindow = true;
							proc.Start();
							string read=proc.StandardOutput.ReadToEnd();
							Debug.WriteLine("ѹ���ļ��е��ļ�:" + read);
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

							Debug.WriteLine("���Ե�ǰ�ļ���ѹ���ļ�,���н�ѹ...");

							//

							//ɾ����ǰ��ʱĿ¼�е��ļ�
							try
							{
								if (Directory.Exists(Application.StartupPath + @"\TEMP\VerifyROM"))
									Directory.Delete(Application.StartupPath + @"\TEMP\VerifyROM", true);
							}
							catch { Debug.WriteLine("��ɾ����ǰ��ʱĿ¼ʱ��������"); }

							//��ѹ�ļ�
						 	Tools.unZIP(f, Application.StartupPath + @"\TEMP\VerifyROM\");

							foreach (string ff in Directory.GetFiles(Application.StartupPath + @"\TEMP\VerifyROM\"))
								verifyFile(ff,f);

							//ɾ����ʱ�ļ�
							try
							{
								Directory.Delete(Application.StartupPath + @"\TEMP\VerifyROM\", true);
							}
							catch { Debug.WriteLine("��ʶ����ѹ���ļ���ɾ����ѹ��ʱ�ļ�ʱ��������"); }
						}//ѹ����ʽ
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
		/// ��ָ֤���ļ�
		/// </summary>
		/// <param name="f">�ļ����ƣ������ļ�·����</param>
		/// <param name="zipFile">ѹ���ļ�����</param>
		private void verifyFile(string f, string zipFile)
		{
			string ext = Path.GetExtension(f).ToUpper().Substring(1);

			//����һЩ���Բ���ROM���ļ�
            //  ��һ������,NDS��Ϸ��,IDS��Ϸ����չ��ΪIDS,���ﵥ������һ�¡�
            if (formMain.nowType == "NDS")
            {
                if (ext != "NDS" && ext != "IDS") return;
            }
            else
            {
                if (ext != formMain.nowType) return;
            }

			//�����ǰ�ļ��Ѿ����ڲ�����ѹ���ļ�����ֱ�ӷ���
			if (zipFile == null)//��ѹ���ļ���
			{
                if (formMain.dataTableR.Select("f='" + Tools.car(f).Replace("'", "''") + "'").Length > 0)
					return;
			}
			else//ѹ���ļ���
			{
                if (formMain.dataTableR.Select("f='" + Tools.car(zipFile).Replace("'", "''") + "?" + Path.GetFileName(f).Replace("'", "''") + "'").Length > 0)
					return;
			}

			Debug.WriteLine("VerifyROM=" + f);

			string sha1 =Tools.GetFileSHA1(f);
			if (sha1 == null) return;

			DataRow[] rows = formMain.dataTableR.Select("A='" + sha1 + "'");
			//��������鵽A=sha1����һ����ʶ���ROM,������Ҳ�������Ϊ��һ������ʶ���ROM
			if (rows.Length > 0)
			{
				if (rows[0]["f"] == DBNull.Value)
				{
					rows[0]["f"] = (zipFile == null) ? Tools.car(f) : Tools.car(zipFile) + "?" + Path.GetFileName(f);
				}
				else//�ж�һ���ظ�����ԭ���Ĵ���,�����µ��ظ�
				{
					//�ж�һ��ԭ�����ļ��Ƿ���ѹ���ļ�
					string[] fs = Tools.cra(rows[0]["f"].ToString()).Split('?');
					if (fs.Length == 1)//����ѹ���ļ�
					{
						sha1 = Tools.GetFileSHA1(Tools.cra(rows[0]["f"].ToString()));
					}
					else //��ѹ���ļ�
					{
						//��ԭ�����ļ����н�ѹ
						Tools.unZIP(fs[0], Application.StartupPath + @"\TEMP\VerifyROM\");
						sha1 = Tools.GetFileSHA1(Application.StartupPath + @"\TEMP\VerifyROM\" + Path.GetFileName(fs[1]));
					}

					if (sha1 == null) return;

					if (sha1 == rows[0]["A"].ToString())
					{
						if (MessageBox.Show("����һ���ظ���ROM,�Ƿ�ɾ�����ROM�ļ���\n�ļ�1=" + rows[0]["f"] + "\n�ļ�2=" + (zipFile==null? f:zipFile), "��ʾ��Ϣ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
							File.Delete(fs[0]);
					}
					else
					{
						rows[0]["f"] = (zipFile == null) ? Tools.car(f) : Tools.car(zipFile) + "?" + Path.GetFileName(f);
					}

				}

				//������ʾ
				cbUpdateLVI cb = new cbUpdateLVI(formMain.updateLVI);
				formMain.Invoke(cb, new object[] { rows[0], false, true });
			}
			else
			{
				//����ROM����
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
				row["N"] = "δ֪ROM";
				row["E"] = (zipFile == null) ? Path.GetFileName(f) : zipFile + "?" + Path.GetFileName(f);
				row["S"] = fzs;
				row["f"] = (zipFile == null) ? Tools.car(f) : Tools.car(zipFile) + "?" + Path.GetFileName(f);
				row["I"] = sha1;
				formMain.dataTableR.Rows.Add(row);

				//��������ROM��ӵ�ListView��
				cbUpdateLVI cb = new cbUpdateLVI(formMain.updateLVI);
				formMain.Invoke(cb, new object[] { row, true, true });
			}

		}

	}
}
